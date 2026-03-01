from fastapi import APIRouter, Depends, HTTPException, status, Header, Query
from fastapi.security import HTTPBearer, HTTPAuthorizationCredentials
from motor.motor_asyncio import AsyncIOMotorClient
import os
from pydantic import BaseModel, ConfigDict, EmailStr
from typing import List, Optional, Dict, Any
import uuid
from datetime import datetime, timezone, date, timedelta
import jwt
import bcrypt
import secrets
import logging

# Import email service
from any_minute.email_service import (
    send_ticket_created_notification,
    send_ticket_reply_notification,
    send_timesheet_approved_notification
)

# Import Stripe service
from any_minute.stripe_service import (
    get_config as get_stripe_config,
    create_checkout_session,
    create_customer_portal_session,
    handle_webhook_event,
    is_stripe_configured,
    is_live_mode
)

logger = logging.getLogger("any_minute")

# Create router with /api/am prefix (to work with existing proxy)
am_router = APIRouter(prefix="/api/am")
am_security = HTTPBearer()

# JWT Config for Any Minute (separate from Payroll)
AM_JWT_SECRET = os.environ.get('AM_JWT_SECRET', 'any-minute-secret-key-2024')
AM_JWT_ALGORITHM = 'HS256'
AM_JWT_EXPIRATION_HOURS = 24

# Database reference - will be set by main server
am_db = None

def init_am_db(database):
    global am_db
    am_db = database

# ===================== PYDANTIC MODELS =====================

class AMBillingUpdate(BaseModel):
    plan: Optional[str] = None  # free, basic, pro
    seat_limit: Optional[int] = None
    account_status: Optional[str] = None  # active, blocked

class AMUserCreate(BaseModel):
    email: EmailStr
    password: str
    first_name: str
    last_name: str
    tenant_id: Optional[str] = None

class AMUserLogin(BaseModel):
    email: EmailStr
    password: str

class AMBusinessCreate(BaseModel):
    name: str
    address: Optional[str] = None
    contact_email: Optional[EmailStr] = None
    contact_phone: Optional[str] = None

class AMBusinessUpdate(BaseModel):
    name: Optional[str] = None
    address: Optional[str] = None
    contact_email: Optional[EmailStr] = None
    contact_phone: Optional[str] = None
    active: Optional[bool] = None

class AMUserManageCreate(BaseModel):
    email: EmailStr
    password: str
    first_name: str
    last_name: str
    role: str = "employee"
    employee_mapping_key: Optional[str] = None

class AMUserManageUpdate(BaseModel):
    first_name: Optional[str] = None
    last_name: Optional[str] = None
    role: Optional[str] = None
    active: Optional[bool] = None
    employee_mapping_key: Optional[str] = None

class AMUserBusinessRoleCreate(BaseModel):
    user_id: str
    business_id: str
    role: str = "employee"

class AMTimesheetEntryCreate(BaseModel):
    business_id: str
    work_date: str
    start_time: Optional[str] = None
    end_time: Optional[str] = None
    break_minutes: int = 0
    notes: Optional[str] = None

class AMTimesheetEntryUpdate(BaseModel):
    start_time: Optional[str] = None
    end_time: Optional[str] = None
    break_minutes: Optional[int] = None
    notes: Optional[str] = None

class AMTimesheetWeekCreate(BaseModel):
    user_id: str
    business_id: str
    week_start_date: str

class AMTimesheetApproveReject(BaseModel):
    status: str
    rejection_reason: Optional[str] = None

class AMScheduleEntryCreate(BaseModel):
    user_id: str
    business_id: str
    scheduled_date: str
    start_time: str
    end_time: str
    notes: Optional[str] = None

class AMScheduleEntryUpdate(BaseModel):
    start_time: Optional[str] = None
    end_time: Optional[str] = None
    notes: Optional[str] = None

# User Status Models
class AMUserStatusUpdate(BaseModel):
    status: str  # active, not_active, terminated
    effective_date: str
    reason: Optional[str] = None

# Pay Rate Models
class AMPayRateCreate(BaseModel):
    user_id: str
    business_id: str
    rate_type: str  # hourly, salary, overtime
    rate_amount: float
    effective_from: str
    effective_to: Optional[str] = None

class AMPayRateUpdate(BaseModel):
    rate_type: Optional[str] = None
    rate_amount: Optional[float] = None
    effective_from: Optional[str] = None
    effective_to: Optional[str] = None
    active: Optional[bool] = None

# Timesheet Entry Status Update
class AMTimesheetEntryStatusUpdate(BaseModel):
    status: str  # pending, approved, rejected, absent

# Ticket Models
class AMTicketCreate(BaseModel):
    subject: str
    description: str
    priority: str = "medium"  # low, medium, high, urgent

class AMTicketReply(BaseModel):
    message: str

class AMTicketStatusUpdate(BaseModel):
    status: str  # open, in_progress, resolved, closed

# ===================== AUTH HELPERS =====================

def am_hash_password(password: str) -> str:
    return bcrypt.hashpw(password.encode('utf-8'), bcrypt.gensalt()).decode('utf-8')

def am_verify_password(password: str, hashed: str) -> bool:
    return bcrypt.checkpw(password.encode('utf-8'), hashed.encode('utf-8'))

def am_create_token(user_id: str, tenant_id: str, role: str) -> str:
    payload = {
        'user_id': user_id,
        'tenant_id': tenant_id,
        'role': role,
        'exp': datetime.now(timezone.utc) + timedelta(hours=AM_JWT_EXPIRATION_HOURS)
    }
    return jwt.encode(payload, AM_JWT_SECRET, algorithm=AM_JWT_ALGORITHM)

async def am_get_current_user(credentials: HTTPAuthorizationCredentials = Depends(am_security)):
    try:
        payload = jwt.decode(credentials.credentials, AM_JWT_SECRET, algorithms=[AM_JWT_ALGORITHM])
        user = await am_db.am_users.find_one({"id": payload['user_id']}, {"_id": 0})
        if not user:
            raise HTTPException(status_code=401, detail="User not found")
        if not user.get('active', True):
            raise HTTPException(status_code=401, detail="User account is disabled")
        return user
    except jwt.ExpiredSignatureError:
        raise HTTPException(status_code=401, detail="Token expired")
    except jwt.InvalidTokenError:
        raise HTTPException(status_code=401, detail="Invalid token")

async def am_require_admin(user: dict = Depends(am_get_current_user)):
    if user['role'] != 'admin':
        raise HTTPException(status_code=403, detail="Admin access required")
    return user

async def am_require_manager_or_admin(user: dict = Depends(am_get_current_user)):
    if user['role'] not in ['admin', 'manager']:
        raise HTTPException(status_code=403, detail="Manager or Admin access required")
    return user

async def am_require_accountant_or_above(user: dict = Depends(am_get_current_user)):
    """Accountant, Manager, or Admin can access"""
    if user['role'] not in ['admin', 'manager', 'accountant']:
        raise HTTPException(status_code=403, detail="Accountant, Manager, or Admin access required")
    return user

# ===================== UTILITY FUNCTIONS =====================

def am_get_week_start(date_str: str) -> str:
    d = datetime.strptime(date_str, "%Y-%m-%d").date()
    days_since_saturday = (d.weekday() + 2) % 7
    saturday = d - timedelta(days=days_since_saturday)
    return saturday.strftime("%Y-%m-%d")

def am_calculate_net_hours(start_time: str, end_time: str, break_minutes: int) -> float:
    if not start_time or not end_time:
        return 0.0
    try:
        start = datetime.strptime(start_time, "%H:%M")
        end = datetime.strptime(end_time, "%H:%M")
        if end < start:
            end += timedelta(days=1)
        total_minutes = (end - start).total_seconds() / 60
        net_minutes = total_minutes - break_minutes
        return round(max(0, net_minutes) / 60, 2)
    except:
        return 0.0

# ===================== AUDIT LOGGING =====================

async def am_log_audit(
    tenant_id: str,
    actor_id: str,
    actor_name: str,
    action: str,  # CREATE, UPDATE, DELETE
    entity_type: str,  # user, business, timesheet, pay_rate, payroll_run, ticket
    entity_id: str,
    entity_name: str = None,
    old_value: dict = None,
    new_value: dict = None,
    metadata: dict = None
):
    """
    Log an audit entry for Any Minute.
    Tracks who did what, when, and the before/after values.
    """
    # Build changes summary
    changes = {}
    if old_value and new_value:
        for key in set(list(old_value.keys()) + list(new_value.keys())):
            old_v = old_value.get(key)
            new_v = new_value.get(key)
            if old_v != new_v and key not in ['_id', 'password_hash', 'updated_at', 'created_at']:
                changes[key] = {"old": old_v, "new": new_v}
    
    audit_entry = {
        "id": str(uuid.uuid4()),
        "tenant_id": tenant_id,
        "actor_id": actor_id,
        "actor_name": actor_name,
        "action": action,
        "entity_type": entity_type,
        "entity_id": entity_id,
        "entity_name": entity_name,
        "changes": changes if changes else None,
        "old_value": old_value,
        "new_value": new_value,
        "metadata": metadata,
        "timestamp": datetime.now(timezone.utc).isoformat()
    }
    await am_db.am_audit_logs.insert_one(audit_entry)
    logger.info(f"AUDIT: {action} {entity_type} '{entity_name or entity_id}' by {actor_name}")

# ===================== ROOT ENDPOINT =====================

@am_router.get("/")
async def am_root():
    return {"message": "Any Minute Timesheet API", "status": "healthy"}

# ===================== AUTH ROUTES =====================

@am_router.post("/auth/register")
async def am_register(data: AMUserCreate):
    existing = await am_db.am_users.find_one({"email": data.email.lower()})
    if existing:
        raise HTTPException(status_code=400, detail="Email already registered")
    
    user_id = str(uuid.uuid4())
    tenant_id = data.tenant_id
    role = "employee"
    
    if not tenant_id:
        demo_tenant = await am_db.am_tenants.find_one({"name": "Demo Tenant"}, {"_id": 0})
        if demo_tenant:
            tenant_id = demo_tenant['id']
        else:
            tenant_id = str(uuid.uuid4())
            payroll_key = secrets.token_urlsafe(32)
            tenant = {
                "id": tenant_id,
                "name": "Demo Tenant",
                "contact_email": data.email.lower(),
                "payroll_api_key": payroll_key,
                "created_at": datetime.now(timezone.utc).isoformat()
            }
            await am_db.am_tenants.insert_one(tenant)
            role = "admin"
    
    existing_users = await am_db.am_users.count_documents({"tenant_id": tenant_id})
    if existing_users == 0:
        role = "admin"
    
    user = {
        "id": user_id,
        "tenant_id": tenant_id,
        "email": data.email.lower(),
        "password_hash": am_hash_password(data.password),
        "first_name": data.first_name,
        "last_name": data.last_name,
        "role": role,
        "active": True,
        "employee_mapping_key": None,
        "created_at": datetime.now(timezone.utc).isoformat()
    }
    await am_db.am_users.insert_one(user)
    
    token = am_create_token(user_id, tenant_id, role)
    user_response = {k: v for k, v in user.items() if k not in ['password_hash', '_id']}
    return {"token": token, "user": user_response}

@am_router.post("/auth/login")
async def am_login(data: AMUserLogin):
    user = await am_db.am_users.find_one({"email": data.email.lower()}, {"_id": 0})
    if not user or not am_verify_password(data.password, user['password_hash']):
        raise HTTPException(status_code=401, detail="Invalid credentials")
    
    # Check if account is disabled or terminated
    if not user.get('active', True):
        raise HTTPException(status_code=401, detail="Account is disabled")
    
    user_status = user.get('status', 'active')
    if user_status == 'terminated':
        raise HTTPException(status_code=401, detail="Account is terminated. Please contact your administrator.")
    if user_status == 'not_active':
        raise HTTPException(status_code=401, detail="Account is not active. Please contact your administrator.")
    
    token = am_create_token(user['id'], user['tenant_id'], user['role'])
    user_response = {k: v for k, v in user.items() if k != 'password_hash'}
    return {"token": token, "user": user_response}

@am_router.get("/auth/me")
async def am_get_me(user: dict = Depends(am_get_current_user)):
    return {k: v for k, v in user.items() if k != 'password_hash'}

# ===================== TENANT ROUTES =====================

@am_router.get("/tenant")
async def am_get_tenant(user: dict = Depends(am_get_current_user)):
    tenant = await am_db.am_tenants.find_one({"id": user['tenant_id']}, {"_id": 0})
    if not tenant:
        raise HTTPException(status_code=404, detail="Tenant not found")
    return tenant

@am_router.get("/tenant/settings")
async def am_get_tenant_settings(user: dict = Depends(am_require_admin)):
    tenant = await am_db.am_tenants.find_one({"id": user['tenant_id']}, {"_id": 0})
    if not tenant:
        raise HTTPException(status_code=404, detail="Tenant not found")
    
    if not tenant.get('payroll_api_key'):
        payroll_key = secrets.token_urlsafe(32)
        await am_db.am_tenants.update_one(
            {"id": user['tenant_id']},
            {"$set": {"payroll_api_key": payroll_key}}
        )
        tenant['payroll_api_key'] = payroll_key
    
    return {
        "payroll_api_key": tenant.get('payroll_api_key', ''),
        "tenant_name": tenant.get('name', ''),
        "contact_email": tenant.get('contact_email', '')
    }

@am_router.post("/tenant/settings/regenerate-key")
async def am_regenerate_payroll_key(user: dict = Depends(am_require_admin)):
    new_key = secrets.token_urlsafe(32)
    await am_db.am_tenants.update_one(
        {"id": user['tenant_id']},
        {"$set": {"payroll_api_key": new_key}}
    )
    return {"payroll_api_key": new_key}

# ===================== BILLING ROUTES =====================

@am_router.get("/billing")
async def am_get_billing(user: dict = Depends(am_get_current_user)):
    """Get billing info - all users can view, only admin can modify"""
    tenant = await am_db.am_tenants.find_one({"id": user['tenant_id']}, {"_id": 0})
    if not tenant:
        raise HTTPException(status_code=404, detail="Tenant not found")
    
    # Count active users for seat usage
    active_users = await am_db.am_users.count_documents({"tenant_id": user['tenant_id'], "active": True})
    
    return {
        "plan": tenant.get('plan', 'free'),
        "seat_limit": tenant.get('seat_limit', 5),
        "seat_usage": active_users,
        "status": tenant.get('status', 'active'),
        "tenant_name": tenant.get('name', ''),
        "is_admin": user['role'] == 'admin'
    }

@am_router.put("/billing")
async def am_update_billing(data: AMBillingUpdate, user: dict = Depends(am_require_admin)):
    """Update billing info - admin only"""
    update_data = {}
    
    if data.plan is not None:
        if data.plan not in ['free', 'basic', 'pro']:
            raise HTTPException(status_code=400, detail="Invalid plan. Must be: free, basic, or pro")
        update_data['plan'] = data.plan
    
    if data.seat_limit is not None:
        if data.seat_limit < 1:
            raise HTTPException(status_code=400, detail="Seat limit must be at least 1")
        update_data['seat_limit'] = data.seat_limit
    
    if data.account_status is not None:
        if data.account_status not in ['active', 'blocked']:
            raise HTTPException(status_code=400, detail="Invalid status. Must be: active or blocked")
        update_data['status'] = data.account_status
    
    if not update_data:
        raise HTTPException(status_code=400, detail="No data to update")
    
    await am_db.am_tenants.update_one(
        {"id": user['tenant_id']},
        {"$set": update_data}
    )
    
    # Return updated billing info
    tenant = await am_db.am_tenants.find_one({"id": user['tenant_id']}, {"_id": 0})
    active_users = await am_db.am_users.count_documents({"tenant_id": user['tenant_id'], "active": True})
    
    return {
        "plan": tenant.get('plan', 'free'),
        "seat_limit": tenant.get('seat_limit', 5),
        "seat_usage": active_users,
        "status": tenant.get('status', 'active'),
        "tenant_name": tenant.get('name', ''),
        "is_admin": True
    }

# ===================== BUSINESS ROUTES =====================

@am_router.get("/businesses")
async def am_get_businesses(user: dict = Depends(am_get_current_user)):
    businesses = await am_db.am_businesses.find(
        {"tenant_id": user['tenant_id'], "active": True},
        {"_id": 0}
    ).to_list(1000)
    return businesses

@am_router.get("/businesses/all")
async def am_get_all_businesses(user: dict = Depends(am_require_admin)):
    businesses = await am_db.am_businesses.find(
        {"tenant_id": user['tenant_id']},
        {"_id": 0}
    ).to_list(1000)
    return businesses

@am_router.post("/businesses")
async def am_create_business(data: AMBusinessCreate, user: dict = Depends(am_require_admin)):
    business_id = str(uuid.uuid4())
    business = {
        "id": business_id,
        "tenant_id": user['tenant_id'],
        "name": data.name,
        "address": data.address,
        "contact_email": data.contact_email,
        "contact_phone": data.contact_phone,
        "active": True,
        "created_at": datetime.now(timezone.utc).isoformat()
    }
    await am_db.am_businesses.insert_one(business)
    
    # Audit log - business created
    await am_log_audit(
        tenant_id=user['tenant_id'],
        actor_id=user['id'],
        actor_name=f"{user['first_name']} {user['last_name']}",
        action="CREATE",
        entity_type="business",
        entity_id=business_id,
        entity_name=data.name,
        new_value={"name": data.name, "address": data.address, "contact_email": data.contact_email}
    )
    
    return {k: v for k, v in business.items() if k != '_id'}

@am_router.get("/businesses/{business_id}")
async def am_get_business(business_id: str, user: dict = Depends(am_get_current_user)):
    business = await am_db.am_businesses.find_one(
        {"id": business_id, "tenant_id": user['tenant_id']},
        {"_id": 0}
    )
    if not business:
        raise HTTPException(status_code=404, detail="Business not found")
    return business

@am_router.put("/businesses/{business_id}")
async def am_update_business(business_id: str, data: AMBusinessUpdate, user: dict = Depends(am_require_admin)):
    # Get old value for audit
    old_business = await am_db.am_businesses.find_one(
        {"id": business_id, "tenant_id": user['tenant_id']},
        {"_id": 0}
    )
    if not old_business:
        raise HTTPException(status_code=404, detail="Business not found")
    
    update_data = {k: v for k, v in data.model_dump().items() if v is not None}
    if not update_data:
        raise HTTPException(status_code=400, detail="No data to update")
    
    await am_db.am_businesses.update_one(
        {"id": business_id, "tenant_id": user['tenant_id']},
        {"$set": update_data}
    )
    
    business = await am_db.am_businesses.find_one({"id": business_id}, {"_id": 0})
    
    # Audit log - business updated
    await am_log_audit(
        tenant_id=user['tenant_id'],
        actor_id=user['id'],
        actor_name=f"{user['first_name']} {user['last_name']}",
        action="UPDATE",
        entity_type="business",
        entity_id=business_id,
        entity_name=business.get('name', ''),
        old_value=old_business,
        new_value=business
    )
    
    return business

@am_router.delete("/businesses/{business_id}")
async def am_delete_business(business_id: str, user: dict = Depends(am_require_admin)):
    # Get business info for audit
    business = await am_db.am_businesses.find_one(
        {"id": business_id, "tenant_id": user['tenant_id']},
        {"_id": 0}
    )
    if not business:
        raise HTTPException(status_code=404, detail="Business not found")
    
    await am_db.am_businesses.update_one(
        {"id": business_id, "tenant_id": user['tenant_id']},
        {"$set": {"active": False}}
    )
    
    # Audit log - business deleted (soft delete)
    await am_log_audit(
        tenant_id=user['tenant_id'],
        actor_id=user['id'],
        actor_name=f"{user['first_name']} {user['last_name']}",
        action="DELETE",
        entity_type="business",
        entity_id=business_id,
        entity_name=business.get('name', ''),
        old_value={"active": True},
        new_value={"active": False}
    )
    
    return {"success": True}

# ===================== USER MANAGEMENT ROUTES =====================

@am_router.get("/users")
async def am_get_users(user: dict = Depends(am_require_manager_or_admin)):
    users = await am_db.am_users.find(
        {"tenant_id": user['tenant_id']},
        {"_id": 0, "password_hash": 0}
    ).to_list(1000)
    return users

@am_router.post("/users")
async def am_create_user(data: AMUserManageCreate, user: dict = Depends(am_require_admin)):
    existing = await am_db.am_users.find_one({"email": data.email.lower()})
    if existing:
        raise HTTPException(status_code=400, detail="Email already registered")
    
    # Enforce seat limit
    tenant = await am_db.am_tenants.find_one({"id": user['tenant_id']}, {"_id": 0})
    if tenant:
        seat_limit = tenant.get('seat_limit', 999)
        current_users = await am_db.am_users.count_documents({"tenant_id": user['tenant_id'], "active": True})
        if current_users >= seat_limit:
            raise HTTPException(
                status_code=403, 
                detail=f"Seat limit reached ({seat_limit}). Please upgrade your plan to add more users."
            )
        # Check tenant status
        if tenant.get('status') == 'blocked':
            raise HTTPException(status_code=403, detail="Account is blocked. Please contact support.")
    
    user_id = str(uuid.uuid4())
    new_user = {
        "id": user_id,
        "tenant_id": user['tenant_id'],
        "email": data.email.lower(),
        "password_hash": am_hash_password(data.password),
        "first_name": data.first_name,
        "last_name": data.last_name,
        "role": data.role,
        "active": True,
        "employee_mapping_key": data.employee_mapping_key,
        "created_at": datetime.now(timezone.utc).isoformat()
    }
    await am_db.am_users.insert_one(new_user)
    
    # Audit log - user created
    await am_log_audit(
        tenant_id=user['tenant_id'],
        actor_id=user['id'],
        actor_name=f"{user['first_name']} {user['last_name']}",
        action="CREATE",
        entity_type="user",
        entity_id=user_id,
        entity_name=f"{data.first_name} {data.last_name}",
        new_value={"email": data.email.lower(), "role": data.role, "first_name": data.first_name, "last_name": data.last_name}
    )
    
    return {k: v for k, v in new_user.items() if k not in ['_id', 'password_hash']}

@am_router.get("/users/{user_id}")
async def am_get_user(user_id: str, user: dict = Depends(am_require_manager_or_admin)):
    target_user = await am_db.am_users.find_one(
        {"id": user_id, "tenant_id": user['tenant_id']},
        {"_id": 0, "password_hash": 0}
    )
    if not target_user:
        raise HTTPException(status_code=404, detail="User not found")
    return target_user

@am_router.put("/users/{user_id}")
async def am_update_user(user_id: str, data: AMUserManageUpdate, user: dict = Depends(am_require_admin)):
    # Get old value for audit
    old_user = await am_db.am_users.find_one(
        {"id": user_id, "tenant_id": user['tenant_id']},
        {"_id": 0, "password_hash": 0}
    )
    if not old_user:
        raise HTTPException(status_code=404, detail="User not found")
    
    update_data = {k: v for k, v in data.model_dump().items() if v is not None}
    if not update_data:
        raise HTTPException(status_code=400, detail="No data to update")
    
    result = await am_db.am_users.update_one(
        {"id": user_id, "tenant_id": user['tenant_id']},
        {"$set": update_data}
    )
    
    updated_user = await am_db.am_users.find_one({"id": user_id}, {"_id": 0, "password_hash": 0})
    
    # Audit log - user updated
    await am_log_audit(
        tenant_id=user['tenant_id'],
        actor_id=user['id'],
        actor_name=f"{user['first_name']} {user['last_name']}",
        action="UPDATE",
        entity_type="user",
        entity_id=user_id,
        entity_name=f"{updated_user.get('first_name', '')} {updated_user.get('last_name', '')}",
        old_value=old_user,
        new_value=updated_user
    )
    
    return updated_user

@am_router.delete("/users/{user_id}")
async def am_delete_user(user_id: str, user: dict = Depends(am_require_admin)):
    if user_id == user['id']:
        raise HTTPException(status_code=400, detail="Cannot delete your own account")
    
    # Get user info before delete for audit
    target_user = await am_db.am_users.find_one(
        {"id": user_id, "tenant_id": user['tenant_id']},
        {"_id": 0, "password_hash": 0}
    )
    if not target_user:
        raise HTTPException(status_code=404, detail="User not found")
    
    result = await am_db.am_users.update_one(
        {"id": user_id, "tenant_id": user['tenant_id']},
        {"$set": {"active": False}}
    )
    
    # Audit log - user deleted (soft delete)
    await am_log_audit(
        tenant_id=user['tenant_id'],
        actor_id=user['id'],
        actor_name=f"{user['first_name']} {user['last_name']}",
        action="DELETE",
        entity_type="user",
        entity_id=user_id,
        entity_name=f"{target_user.get('first_name', '')} {target_user.get('last_name', '')}",
        old_value={"active": True},
        new_value={"active": False}
    )
    
    return {"success": True}

# ===================== USER-BUSINESS ROLE ROUTES =====================

@am_router.get("/user-business-roles")
async def am_get_user_business_roles(user: dict = Depends(am_get_current_user)):
    roles = await am_db.am_user_business_roles.find(
        {"tenant_id": user['tenant_id']},
        {"_id": 0}
    ).to_list(1000)
    return roles

@am_router.post("/user-business-roles")
async def am_assign_user_to_business(data: AMUserBusinessRoleCreate, user: dict = Depends(am_require_admin)):
    target_user = await am_db.am_users.find_one({"id": data.user_id, "tenant_id": user['tenant_id']})
    if not target_user:
        raise HTTPException(status_code=404, detail="User not found")
    
    business = await am_db.am_businesses.find_one({"id": data.business_id, "tenant_id": user['tenant_id']})
    if not business:
        raise HTTPException(status_code=404, detail="Business not found")
    
    existing = await am_db.am_user_business_roles.find_one({
        "user_id": data.user_id,
        "business_id": data.business_id
    })
    if existing:
        await am_db.am_user_business_roles.update_one(
            {"user_id": data.user_id, "business_id": data.business_id},
            {"$set": {"role": data.role}}
        )
        return {"success": True, "message": "Role updated"}
    
    role_id = str(uuid.uuid4())
    role_record = {
        "id": role_id,
        "tenant_id": user['tenant_id'],
        "user_id": data.user_id,
        "business_id": data.business_id,
        "role": data.role,
        "created_at": datetime.now(timezone.utc).isoformat()
    }
    await am_db.am_user_business_roles.insert_one(role_record)
    return {k: v for k, v in role_record.items() if k != '_id'}

@am_router.delete("/user-business-roles/{user_id}/{business_id}")
async def am_remove_user_from_business(user_id: str, business_id: str, user: dict = Depends(am_require_admin)):
    result = await am_db.am_user_business_roles.delete_one({
        "user_id": user_id,
        "business_id": business_id,
        "tenant_id": user['tenant_id']
    })
    if result.deleted_count == 0:
        raise HTTPException(status_code=404, detail="Role assignment not found")
    return {"success": True}

# ===================== USER STATUS MANAGEMENT =====================

@am_router.put("/users/{user_id}/status")
async def am_update_user_status(user_id: str, data: AMUserStatusUpdate, user: dict = Depends(am_require_admin)):
    """Update user status with history tracking"""
    if data.status not in ['active', 'not_active', 'terminated']:
        raise HTTPException(status_code=400, detail="Invalid status. Must be: active, not_active, or terminated")
    
    target_user = await am_db.am_users.find_one({"id": user_id, "tenant_id": user['tenant_id']})
    if not target_user:
        raise HTTPException(status_code=404, detail="User not found")
    
    old_status = target_user.get('status', 'active')
    
    # Update user status
    await am_db.am_users.update_one(
        {"id": user_id},
        {"$set": {
            "status": data.status,
            "status_effective_date": data.effective_date,
            "active": data.status == 'active'
        }}
    )
    
    # Record in status history
    history_record = {
        "id": str(uuid.uuid4()),
        "tenant_id": user['tenant_id'],
        "user_id": user_id,
        "old_status": old_status,
        "new_status": data.status,
        "effective_date": data.effective_date,
        "reason": data.reason,
        "changed_by": user['id'],
        "changed_by_name": f"{user['first_name']} {user['last_name']}",
        "changed_at": datetime.now(timezone.utc).isoformat()
    }
    await am_db.am_user_status_history.insert_one(history_record)
    
    return {"success": True, "status": data.status}

@am_router.get("/users/{user_id}/status-history")
async def am_get_user_status_history(user_id: str, user: dict = Depends(am_require_manager_or_admin)):
    """Get user status change history"""
    history = await am_db.am_user_status_history.find(
        {"user_id": user_id, "tenant_id": user['tenant_id']},
        {"_id": 0}
    ).sort("changed_at", -1).to_list(100)
    return history

# ===================== PAY RATE MANAGEMENT =====================

@am_router.get("/pay-rates")
async def am_get_pay_rates(
    user: dict = Depends(am_require_manager_or_admin),
    user_id: Optional[str] = Query(None),
    business_id: Optional[str] = Query(None)
):
    """Get pay rates with optional filters"""
    query = {"tenant_id": user['tenant_id']}
    if user_id:
        query['user_id'] = user_id
    if business_id:
        query['business_id'] = business_id
    
    rates = await am_db.am_pay_rates.find(query, {"_id": 0}).to_list(1000)
    return rates

@am_router.post("/pay-rates")
async def am_create_pay_rate(data: AMPayRateCreate, user: dict = Depends(am_require_admin)):
    """Create a new pay rate for a user"""
    if data.rate_type not in ['hourly', 'salary', 'overtime']:
        raise HTTPException(status_code=400, detail="Invalid rate type. Must be: hourly, salary, or overtime")
    
    target_user = await am_db.am_users.find_one({"id": data.user_id, "tenant_id": user['tenant_id']})
    if not target_user:
        raise HTTPException(status_code=404, detail="User not found")
    
    business = await am_db.am_businesses.find_one({"id": data.business_id, "tenant_id": user['tenant_id']})
    if not business:
        raise HTTPException(status_code=404, detail="Business not found")
    
    rate_id = str(uuid.uuid4())
    rate = {
        "id": rate_id,
        "tenant_id": user['tenant_id'],
        "user_id": data.user_id,
        "business_id": data.business_id,
        "rate_type": data.rate_type,
        "rate_amount": data.rate_amount,
        "effective_from": data.effective_from,
        "effective_to": data.effective_to,
        "active": True,
        "created_by": user['id'],
        "created_at": datetime.now(timezone.utc).isoformat()
    }
    await am_db.am_pay_rates.insert_one(rate)
    
    # Audit log - pay rate created
    await am_log_audit(
        tenant_id=user['tenant_id'],
        actor_id=user['id'],
        actor_name=f"{user['first_name']} {user['last_name']}",
        action="CREATE",
        entity_type="pay_rate",
        entity_id=rate_id,
        entity_name=f"{target_user.get('first_name', '')} {target_user.get('last_name', '')} - {data.rate_type}",
        new_value={"user_id": data.user_id, "rate_type": data.rate_type, "rate_amount": data.rate_amount, "effective_from": data.effective_from}
    )
    
    return {k: v for k, v in rate.items() if k != '_id'}

@am_router.put("/pay-rates/{rate_id}")
async def am_update_pay_rate(rate_id: str, data: AMPayRateUpdate, user: dict = Depends(am_require_admin)):
    """Update a pay rate"""
    # Get old value for audit
    old_rate = await am_db.am_pay_rates.find_one(
        {"id": rate_id, "tenant_id": user['tenant_id']},
        {"_id": 0}
    )
    if not old_rate:
        raise HTTPException(status_code=404, detail="Pay rate not found")
    
    update_data = {k: v for k, v in data.model_dump().items() if v is not None}
    if not update_data:
        raise HTTPException(status_code=400, detail="No data to update")
    
    await am_db.am_pay_rates.update_one(
        {"id": rate_id, "tenant_id": user['tenant_id']},
        {"$set": update_data}
    )
    
    new_rate = await am_db.am_pay_rates.find_one({"id": rate_id}, {"_id": 0})
    
    # Audit log - pay rate updated
    await am_log_audit(
        tenant_id=user['tenant_id'],
        actor_id=user['id'],
        actor_name=f"{user['first_name']} {user['last_name']}",
        action="UPDATE",
        entity_type="pay_rate",
        entity_id=rate_id,
        entity_name=f"Pay Rate {old_rate.get('rate_type', '')}",
        old_value=old_rate,
        new_value=new_rate
    )
    
    return {"success": True}

@am_router.delete("/pay-rates/{rate_id}")
async def am_delete_pay_rate(rate_id: str, user: dict = Depends(am_require_admin)):
    """Soft delete a pay rate"""
    # Get rate info for audit
    rate = await am_db.am_pay_rates.find_one(
        {"id": rate_id, "tenant_id": user['tenant_id']},
        {"_id": 0}
    )
    if not rate:
        raise HTTPException(status_code=404, detail="Pay rate not found")
    
    await am_db.am_pay_rates.update_one(
        {"id": rate_id, "tenant_id": user['tenant_id']},
        {"$set": {"active": False}}
    )
    
    # Audit log - pay rate deleted
    await am_log_audit(
        tenant_id=user['tenant_id'],
        actor_id=user['id'],
        actor_name=f"{user['first_name']} {user['last_name']}",
        action="DELETE",
        entity_type="pay_rate",
        entity_id=rate_id,
        entity_name=f"Pay Rate {rate.get('rate_type', '')}",
        old_value={"active": True, "rate_amount": rate.get('rate_amount')},
        new_value={"active": False}
    )
    
    return {"success": True}

@am_router.get("/pay-rates/effective/{user_id}")
async def am_get_effective_pay_rate(
    user_id: str, 
    business_id: str = Query(...),
    as_of_date: Optional[str] = Query(None),
    user: dict = Depends(am_require_manager_or_admin)
):
    """Get the effective pay rate for a user as of a specific date"""
    check_date = as_of_date or datetime.now(timezone.utc).strftime('%Y-%m-%d')
    
    rate = await am_db.am_pay_rates.find_one({
        "tenant_id": user['tenant_id'],
        "user_id": user_id,
        "business_id": business_id,
        "active": True,
        "effective_from": {"$lte": check_date},
        "$or": [
            {"effective_to": None},
            {"effective_to": {"$gte": check_date}}
        ]
    }, {"_id": 0})
    
    if not rate:
        return {"pay_rate": None, "message": "No effective pay rate found"}
    
    return {"pay_rate": rate}

# ===================== TIMESHEET WEEK ROUTES =====================

@am_router.get("/timesheet-weeks")
async def am_get_timesheet_weeks(
    user: dict = Depends(am_get_current_user),
    user_id: Optional[str] = Query(None),
    business_id: Optional[str] = Query(None),
    status: Optional[str] = Query(None)
):
    query = {"tenant_id": user['tenant_id']}
    
    if user['role'] == 'employee':
        query['user_id'] = user['id']
    elif user_id:
        query['user_id'] = user_id
    
    if business_id:
        query['business_id'] = business_id
    if status:
        query['status'] = status
    
    weeks = await am_db.am_timesheet_weeks.find(query, {"_id": 0}).sort("week_start_date", -1).to_list(100)
    return weeks

@am_router.post("/timesheet-weeks")
async def am_create_timesheet_week(data: AMTimesheetWeekCreate, user: dict = Depends(am_get_current_user)):
    week_start = am_get_week_start(data.week_start_date)
    
    target_user_id = data.user_id
    if user['role'] == 'employee' and target_user_id != user['id']:
        raise HTTPException(status_code=403, detail="Cannot create timesheet for another user")
    
    business = await am_db.am_businesses.find_one({"id": data.business_id, "tenant_id": user['tenant_id']})
    if not business:
        raise HTTPException(status_code=404, detail="Business not found")
    
    existing = await am_db.am_timesheet_weeks.find_one({
        "user_id": target_user_id,
        "business_id": data.business_id,
        "week_start_date": week_start
    })
    if existing:
        return {k: v for k, v in existing.items() if k != '_id'}
    
    week_id = str(uuid.uuid4())
    week = {
        "id": week_id,
        "tenant_id": user['tenant_id'],
        "user_id": target_user_id,
        "business_id": data.business_id,
        "week_start_date": week_start,
        "status": "draft",
        "total_hours": 0,
        "submitted_at": None,
        "approved_at": None,
        "approved_by": None,
        "rejection_reason": None,
        "locked": False,
        "created_at": datetime.now(timezone.utc).isoformat()
    }
    await am_db.am_timesheet_weeks.insert_one(week)
    return {k: v for k, v in week.items() if k != '_id'}

@am_router.get("/timesheet-weeks/{week_id}")
async def am_get_timesheet_week(week_id: str, user: dict = Depends(am_get_current_user)):
    week = await am_db.am_timesheet_weeks.find_one(
        {"id": week_id, "tenant_id": user['tenant_id']},
        {"_id": 0}
    )
    if not week:
        raise HTTPException(status_code=404, detail="Timesheet week not found")
    
    if user['role'] == 'employee' and week['user_id'] != user['id']:
        raise HTTPException(status_code=403, detail="Access denied")
    
    return week

@am_router.post("/timesheet-weeks/{week_id}/submit")
async def am_submit_timesheet_week(week_id: str, user: dict = Depends(am_get_current_user)):
    week = await am_db.am_timesheet_weeks.find_one(
        {"id": week_id, "tenant_id": user['tenant_id']},
        {"_id": 0}
    )
    if not week:
        raise HTTPException(status_code=404, detail="Timesheet week not found")
    
    if user['role'] == 'employee' and week['user_id'] != user['id']:
        raise HTTPException(status_code=403, detail="Access denied")
    
    if week['status'] not in ['draft', 'rejected']:
        raise HTTPException(status_code=400, detail="Can only submit draft or rejected timesheets")
    
    if week.get('locked'):
        raise HTTPException(status_code=400, detail="Timesheet is locked")
    
    entries = await am_db.am_timesheet_entries.find({"week_id": week_id}, {"_id": 0}).to_list(100)
    total_hours = sum(e.get('net_hours', 0) for e in entries)
    
    await am_db.am_timesheet_weeks.update_one(
        {"id": week_id},
        {"$set": {
            "status": "submitted",
            "total_hours": total_hours,
            "submitted_at": datetime.now(timezone.utc).isoformat(),
            "rejection_reason": None
        }}
    )
    return {"success": True, "status": "submitted"}

@am_router.post("/timesheet-weeks/{week_id}/approve")
async def am_approve_reject_timesheet(week_id: str, data: AMTimesheetApproveReject, user: dict = Depends(am_require_manager_or_admin)):
    week = await am_db.am_timesheet_weeks.find_one(
        {"id": week_id, "tenant_id": user['tenant_id']},
        {"_id": 0}
    )
    if not week:
        raise HTTPException(status_code=404, detail="Timesheet week not found")
    
    if week['status'] != 'submitted':
        raise HTTPException(status_code=400, detail="Can only approve/reject submitted timesheets")
    
    if data.status == 'approved':
        await am_db.am_timesheet_weeks.update_one(
            {"id": week_id},
            {"$set": {
                "status": "approved",
                "approved_at": datetime.now(timezone.utc).isoformat(),
                "approved_by": user['id']
            }}
        )
    elif data.status == 'rejected':
        await am_db.am_timesheet_weeks.update_one(
            {"id": week_id},
            {"$set": {
                "status": "rejected",
                "rejection_reason": data.rejection_reason
            }}
        )
    else:
        raise HTTPException(status_code=400, detail="Invalid status. Use 'approved' or 'rejected'")
    
    return {"success": True, "status": data.status}

# ===================== TIMESHEET ENTRY ROUTES =====================

@am_router.get("/timesheet-entries")
async def am_get_timesheet_entries(
    user: dict = Depends(am_get_current_user),
    week_id: Optional[str] = Query(None),
    user_id: Optional[str] = Query(None),
    business_id: Optional[str] = Query(None)
):
    query = {"tenant_id": user['tenant_id']}
    
    if user['role'] == 'employee':
        query['user_id'] = user['id']
    elif user_id:
        query['user_id'] = user_id
    
    if week_id:
        query['week_id'] = week_id
    if business_id:
        query['business_id'] = business_id
    
    entries = await am_db.am_timesheet_entries.find(query, {"_id": 0}).sort("work_date", 1).to_list(1000)
    return entries

@am_router.post("/timesheet-entries")
async def am_create_timesheet_entry(data: AMTimesheetEntryCreate, user: dict = Depends(am_get_current_user)):
    week_start = am_get_week_start(data.work_date)
    
    week = await am_db.am_timesheet_weeks.find_one({
        "user_id": user['id'],
        "business_id": data.business_id,
        "week_start_date": week_start
    })
    
    if not week:
        week_id = str(uuid.uuid4())
        week = {
            "id": week_id,
            "tenant_id": user['tenant_id'],
            "user_id": user['id'],
            "business_id": data.business_id,
            "week_start_date": week_start,
            "status": "draft",
            "total_hours": 0,
            "submitted_at": None,
            "approved_at": None,
            "approved_by": None,
            "rejection_reason": None,
            "locked": False,
            "created_at": datetime.now(timezone.utc).isoformat()
        }
        await am_db.am_timesheet_weeks.insert_one(week)
    else:
        week_id = week['id']
        if week.get('locked'):
            raise HTTPException(status_code=400, detail="Timesheet week is locked")
        if week['status'] == 'approved':
            raise HTTPException(status_code=400, detail="Cannot modify approved timesheet")
    
    existing = await am_db.am_timesheet_entries.find_one({
        "week_id": week_id,
        "work_date": data.work_date
    })
    if existing:
        raise HTTPException(status_code=400, detail="Entry already exists for this date. Use PUT to update.")
    
    net_hours = am_calculate_net_hours(data.start_time, data.end_time, data.break_minutes)
    
    entry_id = str(uuid.uuid4())
    entry = {
        "id": entry_id,
        "tenant_id": user['tenant_id'],
        "user_id": user['id'],
        "business_id": data.business_id,
        "week_id": week_id,
        "work_date": data.work_date,
        "start_time": data.start_time,
        "end_time": data.end_time,
        "break_minutes": data.break_minutes,
        "net_hours": net_hours,
        "notes": data.notes,
        "created_at": datetime.now(timezone.utc).isoformat()
    }
    await am_db.am_timesheet_entries.insert_one(entry)
    
    await am_update_week_total_hours(week_id)
    
    return {k: v for k, v in entry.items() if k != '_id'}

@am_router.put("/timesheet-entries/{entry_id}")
async def am_update_timesheet_entry(entry_id: str, data: AMTimesheetEntryUpdate, user: dict = Depends(am_get_current_user)):
    entry = await am_db.am_timesheet_entries.find_one(
        {"id": entry_id, "tenant_id": user['tenant_id']},
        {"_id": 0}
    )
    if not entry:
        raise HTTPException(status_code=404, detail="Entry not found")
    
    if user['role'] == 'employee' and entry['user_id'] != user['id']:
        raise HTTPException(status_code=403, detail="Access denied")
    
    week = await am_db.am_timesheet_weeks.find_one({"id": entry['week_id']}, {"_id": 0})
    if week and week.get('locked'):
        raise HTTPException(status_code=400, detail="Timesheet week is locked")
    if week and week['status'] == 'approved':
        raise HTTPException(status_code=400, detail="Cannot modify approved timesheet")
    
    update_data = {k: v for k, v in data.model_dump().items() if v is not None}
    
    start_time = update_data.get('start_time', entry.get('start_time'))
    end_time = update_data.get('end_time', entry.get('end_time'))
    break_minutes = update_data.get('break_minutes', entry.get('break_minutes', 0))
    update_data['net_hours'] = am_calculate_net_hours(start_time, end_time, break_minutes)
    
    await am_db.am_timesheet_entries.update_one({"id": entry_id}, {"$set": update_data})
    
    await am_update_week_total_hours(entry['week_id'])
    
    updated_entry = await am_db.am_timesheet_entries.find_one({"id": entry_id}, {"_id": 0})
    return updated_entry

@am_router.delete("/timesheet-entries/{entry_id}")
async def am_delete_timesheet_entry(entry_id: str, user: dict = Depends(am_get_current_user)):
    entry = await am_db.am_timesheet_entries.find_one(
        {"id": entry_id, "tenant_id": user['tenant_id']},
        {"_id": 0}
    )
    if not entry:
        raise HTTPException(status_code=404, detail="Entry not found")
    
    if user['role'] == 'employee' and entry['user_id'] != user['id']:
        raise HTTPException(status_code=403, detail="Access denied")
    
    week = await am_db.am_timesheet_weeks.find_one({"id": entry['week_id']}, {"_id": 0})
    if week and week.get('locked'):
        raise HTTPException(status_code=400, detail="Timesheet week is locked")
    if week and week['status'] == 'approved':
        raise HTTPException(status_code=400, detail="Cannot modify approved timesheet")
    
    week_id = entry['week_id']
    await am_db.am_timesheet_entries.delete_one({"id": entry_id})
    
    await am_update_week_total_hours(week_id)
    
    return {"success": True}

@am_router.put("/timesheet-entries/{entry_id}/status")
async def am_update_entry_status(entry_id: str, data: AMTimesheetEntryStatusUpdate, user: dict = Depends(am_require_manager_or_admin)):
    """Update individual timesheet entry status (Pending/Approved/Rejected/Absent)"""
    if data.status not in ['pending', 'approved', 'rejected', 'absent']:
        raise HTTPException(status_code=400, detail="Invalid status. Must be: pending, approved, rejected, or absent")
    
    entry = await am_db.am_timesheet_entries.find_one(
        {"id": entry_id, "tenant_id": user['tenant_id']},
        {"_id": 0}
    )
    if not entry:
        raise HTTPException(status_code=404, detail="Entry not found")
    
    week = await am_db.am_timesheet_weeks.find_one({"id": entry['week_id']}, {"_id": 0})
    if week and week.get('locked'):
        raise HTTPException(status_code=400, detail="Timesheet week is locked")
    
    await am_db.am_timesheet_entries.update_one(
        {"id": entry_id},
        {"$set": {
            "entry_status": data.status,
            "status_updated_by": user['id'],
            "status_updated_at": datetime.now(timezone.utc).isoformat()
        }}
    )
    
    return {"success": True, "status": data.status}

@am_router.post("/timesheet-weeks/{week_id}/bulk-approve")
async def am_bulk_approve_week(week_id: str, user: dict = Depends(am_require_manager_or_admin)):
    """Approve all entries in a week at once"""
    week = await am_db.am_timesheet_weeks.find_one(
        {"id": week_id, "tenant_id": user['tenant_id']},
        {"_id": 0}
    )
    if not week:
        raise HTTPException(status_code=404, detail="Timesheet week not found")
    
    if week.get('locked'):
        raise HTTPException(status_code=400, detail="Timesheet week is locked")
    
    # Update all entries in the week to approved
    result = await am_db.am_timesheet_entries.update_many(
        {"week_id": week_id, "tenant_id": user['tenant_id']},
        {"$set": {
            "entry_status": "approved",
            "status_updated_by": user['id'],
            "status_updated_at": datetime.now(timezone.utc).isoformat()
        }}
    )
    
    # Update week status to approved
    await am_db.am_timesheet_weeks.update_one(
        {"id": week_id},
        {"$set": {
            "status": "approved",
            "approved_by": user['id'],
            "approved_at": datetime.now(timezone.utc).isoformat()
        }}
    )
    
    # Send email notification to employee
    try:
        emp = await am_db.am_users.find_one({"id": week['user_id']}, {"_id": 0})
        if emp and emp.get('email'):
            await send_timesheet_approved_notification(
                to_email=emp['email'],
                employee_name=f"{emp['first_name']} {emp['last_name']}",
                week_start=week['week_start_date'],
                total_hours=week.get('total_hours', 0),
                approved_by=f"{user['first_name']} {user['last_name']}"
            )
    except Exception as e:
        logger.warning(f"Failed to send timesheet approval notification: {e}")
    
    return {"success": True, "entries_updated": result.modified_count}

@am_router.post("/timesheet-weeks/{week_id}/bulk-reject")
async def am_bulk_reject_week(week_id: str, reason: Optional[str] = Query(None), user: dict = Depends(am_require_manager_or_admin)):
    """Reject all entries in a week at once"""
    week = await am_db.am_timesheet_weeks.find_one(
        {"id": week_id, "tenant_id": user['tenant_id']},
        {"_id": 0}
    )
    if not week:
        raise HTTPException(status_code=404, detail="Timesheet week not found")
    
    if week.get('locked'):
        raise HTTPException(status_code=400, detail="Timesheet week is locked")
    
    # Update all entries in the week to rejected
    result = await am_db.am_timesheet_entries.update_many(
        {"week_id": week_id, "tenant_id": user['tenant_id']},
        {"$set": {
            "entry_status": "rejected",
            "status_updated_by": user['id'],
            "status_updated_at": datetime.now(timezone.utc).isoformat()
        }}
    )
    
    # Update week status to rejected
    await am_db.am_timesheet_weeks.update_one(
        {"id": week_id},
        {"$set": {
            "status": "rejected",
            "rejection_reason": reason,
            "rejected_by": user['id'],
            "rejected_at": datetime.now(timezone.utc).isoformat()
        }}
    )
    
    return {"success": True, "entries_updated": result.modified_count}

async def am_update_week_total_hours(week_id: str):
    entries = await am_db.am_timesheet_entries.find({"week_id": week_id}, {"_id": 0}).to_list(100)
    total_hours = sum(e.get('net_hours', 0) for e in entries)
    await am_db.am_timesheet_weeks.update_one(
        {"id": week_id},
        {"$set": {"total_hours": round(total_hours, 2)}}
    )

# ===================== SCHEDULE ROUTES =====================

@am_router.get("/schedules")
async def am_get_schedules(
    user: dict = Depends(am_get_current_user),
    user_id: Optional[str] = Query(None),
    business_id: Optional[str] = Query(None),
    start_date: Optional[str] = Query(None),
    end_date: Optional[str] = Query(None)
):
    query = {"tenant_id": user['tenant_id']}
    
    if user['role'] == 'employee':
        query['user_id'] = user['id']
    elif user_id:
        query['user_id'] = user_id
    
    if business_id:
        query['business_id'] = business_id
    
    if start_date and end_date:
        query['scheduled_date'] = {"$gte": start_date, "$lte": end_date}
    elif start_date:
        query['scheduled_date'] = {"$gte": start_date}
    elif end_date:
        query['scheduled_date'] = {"$lte": end_date}
    
    schedules = await am_db.am_schedule_entries.find(query, {"_id": 0}).sort("scheduled_date", 1).to_list(1000)
    return schedules

@am_router.post("/schedules")
async def am_create_schedule(data: AMScheduleEntryCreate, user: dict = Depends(am_require_manager_or_admin)):
    target_user = await am_db.am_users.find_one({"id": data.user_id, "tenant_id": user['tenant_id']})
    if not target_user:
        raise HTTPException(status_code=404, detail="User not found")
    
    business = await am_db.am_businesses.find_one({"id": data.business_id, "tenant_id": user['tenant_id']})
    if not business:
        raise HTTPException(status_code=404, detail="Business not found")
    
    schedule_id = str(uuid.uuid4())
    schedule = {
        "id": schedule_id,
        "tenant_id": user['tenant_id'],
        "user_id": data.user_id,
        "business_id": data.business_id,
        "scheduled_date": data.scheduled_date,
        "start_time": data.start_time,
        "end_time": data.end_time,
        "notes": data.notes,
        "created_by": user['id'],
        "created_at": datetime.now(timezone.utc).isoformat()
    }
    await am_db.am_schedule_entries.insert_one(schedule)
    return {k: v for k, v in schedule.items() if k != '_id'}

@am_router.put("/schedules/{schedule_id}")
async def am_update_schedule(schedule_id: str, data: AMScheduleEntryUpdate, user: dict = Depends(am_require_manager_or_admin)):
    update_data = {k: v for k, v in data.model_dump().items() if v is not None}
    if not update_data:
        raise HTTPException(status_code=400, detail="No data to update")
    
    result = await am_db.am_schedule_entries.update_one(
        {"id": schedule_id, "tenant_id": user['tenant_id']},
        {"$set": update_data}
    )
    if result.matched_count == 0:
        raise HTTPException(status_code=404, detail="Schedule not found")
    
    schedule = await am_db.am_schedule_entries.find_one({"id": schedule_id}, {"_id": 0})
    return schedule

@am_router.delete("/schedules/{schedule_id}")
async def am_delete_schedule(schedule_id: str, user: dict = Depends(am_require_manager_or_admin)):
    result = await am_db.am_schedule_entries.delete_one(
        {"id": schedule_id, "tenant_id": user['tenant_id']}
    )
    if result.deleted_count == 0:
        raise HTTPException(status_code=404, detail="Schedule not found")
    return {"success": True}

# ===================== REPORTS ROUTES =====================

@am_router.get("/reports/by-business")
async def am_get_report_by_business(
    user: dict = Depends(am_require_manager_or_admin),
    business_id: Optional[str] = Query(None),
    start_date: Optional[str] = Query(None),
    end_date: Optional[str] = Query(None)
):
    query = {"tenant_id": user['tenant_id']}
    
    if business_id:
        query['business_id'] = business_id
    
    week_query = {**query, "status": "approved"}
    if start_date:
        week_query['week_start_date'] = {"$gte": start_date}
    if end_date:
        if 'week_start_date' in week_query:
            week_query['week_start_date']['$lte'] = end_date
        else:
            week_query['week_start_date'] = {"$lte": end_date}
    
    weeks = await am_db.am_timesheet_weeks.find(week_query, {"_id": 0}).to_list(1000)
    
    businesses = await am_db.am_businesses.find({"tenant_id": user['tenant_id']}, {"_id": 0}).to_list(100)
    users = await am_db.am_users.find({"tenant_id": user['tenant_id']}, {"_id": 0, "password_hash": 0}).to_list(1000)
    
    business_map = {b['id']: b for b in businesses}
    user_map = {u['id']: u for u in users}
    
    report = {}
    for week in weeks:
        biz_id = week['business_id']
        if biz_id not in report:
            biz = business_map.get(biz_id, {})
            report[biz_id] = {
                "business_id": biz_id,
                "business_name": biz.get('name', 'Unknown'),
                "total_hours": 0,
                "user_breakdown": {}
            }
        
        report[biz_id]['total_hours'] += week.get('total_hours', 0)
        
        uid = week['user_id']
        if uid not in report[biz_id]['user_breakdown']:
            u = user_map.get(uid, {})
            report[biz_id]['user_breakdown'][uid] = {
                "user_id": uid,
                "user_name": f"{u.get('first_name', '')} {u.get('last_name', '')}".strip(),
                "hours": 0
            }
        report[biz_id]['user_breakdown'][uid]['hours'] += week.get('total_hours', 0)
    
    result = []
    for biz_id, data in report.items():
        data['user_breakdown'] = list(data['user_breakdown'].values())
        data['total_hours'] = round(data['total_hours'], 2)
        for ub in data['user_breakdown']:
            ub['hours'] = round(ub['hours'], 2)
        result.append(data)
    
    return result

# ===================== PAYROLL INTEGRATION API =====================

async def am_verify_payroll_key(x_payroll_key: str = Header(...)):
    if not x_payroll_key:
        raise HTTPException(status_code=401, detail="Missing X-PAYROLL-KEY header")
    
    tenant = await am_db.am_tenants.find_one({"payroll_api_key": x_payroll_key}, {"_id": 0})
    if not tenant:
        raise HTTPException(status_code=401, detail="Invalid payroll API key")
    
    return tenant

@am_router.get("/payroll/employees")
async def am_payroll_get_employees(tenant: dict = Depends(am_verify_payroll_key)):
    users = await am_db.am_users.find(
        {"tenant_id": tenant['id'], "active": True},
        {"_id": 0, "password_hash": 0}
    ).to_list(1000)
    
    employees = []
    for user in users:
        employees.append({
            "external_employee_key": user.get('employee_mapping_key') or user['id'],
            "first_name": user['first_name'],
            "last_name": user['last_name'],
            "email": user['email'],
            "role": user['role']
        })
    
    return {"employees": employees}

@am_router.get("/payroll/approved-entries")
async def am_payroll_get_approved_entries(
    start: str = Query(..., description="Start date YYYY-MM-DD"),
    end: str = Query(..., description="End date YYYY-MM-DD"),
    tenant: dict = Depends(am_verify_payroll_key)
):
    weeks = await am_db.am_timesheet_weeks.find({
        "tenant_id": tenant['id'],
        "status": "approved",
        "week_start_date": {"$gte": start, "$lte": end}
    }, {"_id": 0}).to_list(1000)
    
    week_ids = [w['id'] for w in weeks]
    
    # Get entries - filter by approved entry status if available
    entries = await am_db.am_timesheet_entries.find({
        "week_id": {"$in": week_ids},
        "work_date": {"$gte": start, "$lte": end},
        "$or": [
            {"entry_status": {"$in": ["approved", None]}},  # Include approved or entries without status
            {"entry_status": {"$exists": False}}
        ]
    }, {"_id": 0}).to_list(10000)
    
    users = await am_db.am_users.find({"tenant_id": tenant['id']}, {"_id": 0}).to_list(1000)
    user_map = {u['id']: u for u in users}
    
    result = []
    for entry in entries:
        # Skip entries with no hours
        hours = entry.get('net_hours', 0) or 0
        if hours <= 0:
            continue
            
        user = user_map.get(entry['user_id'], {})
        result.append({
            "employee_key": user.get('employee_mapping_key') or entry['user_id'],
            "employee_email": user.get('email', ''),
            "work_date": entry['work_date'],
            "regular_hours": hours,
            "overtime_hours": 0,
            "source_ref": entry['id'],
            "notes": entry.get('notes', '')
        })
    
    return {"entries": result, "total_count": len(result)}

@am_router.post("/payroll/lock")
async def am_payroll_lock_entries(
    start: str = Query(..., description="Start date YYYY-MM-DD"),
    end: str = Query(..., description="End date YYYY-MM-DD"),
    tenant: dict = Depends(am_verify_payroll_key)
):
    result = await am_db.am_timesheet_weeks.update_many(
        {
            "tenant_id": tenant['id'],
            "status": "approved",
            "week_start_date": {"$gte": start, "$lte": end}
        },
        {"$set": {"locked": True, "locked_at": datetime.now(timezone.utc).isoformat()}}
    )
    
    return {"success": True, "locked_count": result.modified_count}

# ===================== DASHBOARD ROUTES =====================

@am_router.get("/dashboard/stats")
async def am_get_dashboard_stats(user: dict = Depends(am_get_current_user)):
    tenant_id = user['tenant_id']
    
    business_count = await am_db.am_businesses.count_documents({"tenant_id": tenant_id, "active": True})
    user_count = await am_db.am_users.count_documents({"tenant_id": tenant_id, "active": True})
    pending_count = await am_db.am_timesheet_weeks.count_documents({
        "tenant_id": tenant_id,
        "status": "submitted"
    })
    
    today = date.today()
    week_start = am_get_week_start(today.strftime("%Y-%m-%d"))
    
    my_week = await am_db.am_timesheet_weeks.find_one({
        "user_id": user['id'],
        "week_start_date": week_start
    }, {"_id": 0})
    
    my_hours_this_week = my_week.get('total_hours', 0) if my_week else 0
    
    return {
        "business_count": business_count,
        "user_count": user_count,
        "pending_approvals": pending_count,
        "my_hours_this_week": my_hours_this_week,
        "current_week_start": week_start
    }


# ===================== TICKETS (SUPPORT) ROUTES =====================

@am_router.get("/tickets")
async def am_get_tickets(user: dict = Depends(am_get_current_user)):
    """Get tickets - users see their own, admins see all"""
    query = {"tenant_id": user['tenant_id']}
    if user['role'] != 'admin':
        query['created_by'] = user['id']
    
    tickets = await am_db.am_tickets.find(query, {"_id": 0}).sort("created_at", -1).to_list(100)
    return tickets

@am_router.get("/tickets/{ticket_id}")
async def am_get_ticket(ticket_id: str, user: dict = Depends(am_get_current_user)):
    """Get single ticket with replies"""
    ticket = await am_db.am_tickets.find_one(
        {"id": ticket_id, "tenant_id": user['tenant_id']},
        {"_id": 0}
    )
    if not ticket:
        raise HTTPException(status_code=404, detail="Ticket not found")
    
    # Non-admins can only view their own tickets
    if user['role'] != 'admin' and ticket['created_by'] != user['id']:
        raise HTTPException(status_code=403, detail="Access denied")
    
    replies = await am_db.am_ticket_replies.find(
        {"ticket_id": ticket_id},
        {"_id": 0}
    ).sort("created_at", 1).to_list(100)
    
    return {"ticket": ticket, "replies": replies}

@am_router.post("/tickets")
async def am_create_ticket(data: AMTicketCreate, user: dict = Depends(am_get_current_user)):
    """Create a new support ticket"""
    if data.priority not in ['low', 'medium', 'high', 'urgent']:
        raise HTTPException(status_code=400, detail="Invalid priority")
    
    ticket_id = str(uuid.uuid4())
    ticket_number = await am_db.am_tickets.count_documents({"tenant_id": user['tenant_id']}) + 1
    
    ticket = {
        "id": ticket_id,
        "tenant_id": user['tenant_id'],
        "ticket_number": ticket_number,
        "subject": data.subject,
        "description": data.description,
        "priority": data.priority,
        "status": "open",
        "created_by": user['id'],
        "created_by_name": f"{user['first_name']} {user['last_name']}",
        "created_by_email": user['email'],
        "created_at": datetime.now(timezone.utc).isoformat(),
        "updated_at": datetime.now(timezone.utc).isoformat()
    }
    await am_db.am_tickets.insert_one(ticket)
    
    # Audit log - ticket created
    await am_log_audit(
        tenant_id=user['tenant_id'],
        actor_id=user['id'],
        actor_name=f"{user['first_name']} {user['last_name']}",
        action="CREATE",
        entity_type="ticket",
        entity_id=ticket_id,
        entity_name=f"Ticket #{ticket_number}: {data.subject[:50]}",
        new_value={"subject": data.subject, "priority": data.priority, "status": "open"}
    )
    
    # Send email notification to admins
    try:
        admins = await am_db.am_users.find(
            {"tenant_id": user['tenant_id'], "role": "admin", "active": True},
            {"email": 1, "_id": 0}
        ).to_list(100)
        for admin in admins:
            await send_ticket_created_notification(
                admin_email=admin['email'],
                ticket_number=ticket_number,
                subject=data.subject,
                created_by=ticket['created_by_name'],
                priority=data.priority
            )
    except Exception as e:
        logger.warning(f"Failed to send ticket notification: {e}")
    
    return {k: v for k, v in ticket.items() if k != '_id'}

@am_router.post("/tickets/{ticket_id}/reply")
async def am_reply_ticket(ticket_id: str, data: AMTicketReply, user: dict = Depends(am_get_current_user)):
    """Add a reply to a ticket"""
    ticket = await am_db.am_tickets.find_one(
        {"id": ticket_id, "tenant_id": user['tenant_id']},
        {"_id": 0}
    )
    if not ticket:
        raise HTTPException(status_code=404, detail="Ticket not found")
    
    # Non-admins can only reply to their own tickets
    if user['role'] != 'admin' and ticket['created_by'] != user['id']:
        raise HTTPException(status_code=403, detail="Access denied")
    
    reply = {
        "id": str(uuid.uuid4()),
        "ticket_id": ticket_id,
        "message": data.message,
        "is_admin_reply": user['role'] == 'admin',
        "created_by": user['id'],
        "created_by_name": f"{user['first_name']} {user['last_name']}",
        "created_at": datetime.now(timezone.utc).isoformat()
    }
    await am_db.am_ticket_replies.insert_one(reply)
    
    # Update ticket timestamp
    await am_db.am_tickets.update_one(
        {"id": ticket_id},
        {"$set": {"updated_at": datetime.now(timezone.utc).isoformat()}}
    )
    
    # Send email notification
    try:
        # If admin replies, notify ticket creator
        # If user replies, notify admins
        if user['role'] == 'admin':
            await send_ticket_reply_notification(
                to_email=ticket['created_by_email'],
                ticket_number=ticket['ticket_number'],
                ticket_subject=ticket['subject'],
                reply_by=f"{user['first_name']} {user['last_name']} (Support)",
                reply_preview=data.message[:200]
            )
        else:
            admins = await am_db.am_users.find(
                {"tenant_id": user['tenant_id'], "role": "admin", "active": True},
                {"email": 1, "_id": 0}
            ).to_list(100)
            for admin in admins:
                await send_ticket_reply_notification(
                    to_email=admin['email'],
                    ticket_number=ticket['ticket_number'],
                    ticket_subject=ticket['subject'],
                    reply_by=f"{user['first_name']} {user['last_name']}",
                    reply_preview=data.message[:200]
                )
    except Exception as e:
        logger.warning(f"Failed to send reply notification: {e}")
    
    return {k: v for k, v in reply.items() if k != '_id'}

@am_router.put("/tickets/{ticket_id}/status")
async def am_update_ticket_status(ticket_id: str, data: AMTicketStatusUpdate, user: dict = Depends(am_require_admin)):
    """Update ticket status - admin only"""
    if data.status not in ['open', 'in_progress', 'resolved', 'closed']:
        raise HTTPException(status_code=400, detail="Invalid status")
    
    result = await am_db.am_tickets.update_one(
        {"id": ticket_id, "tenant_id": user['tenant_id']},
        {"$set": {
            "status": data.status,
            "updated_at": datetime.now(timezone.utc).isoformat()
        }}
    )
    
    if result.matched_count == 0:
        raise HTTPException(status_code=404, detail="Ticket not found")
    
    return {"success": True, "status": data.status}

@am_router.get("/tickets/stats/summary")
async def am_get_ticket_stats(user: dict = Depends(am_require_admin)):
    """Get ticket statistics for admin dashboard"""
    tenant_id = user['tenant_id']
    
    total = await am_db.am_tickets.count_documents({"tenant_id": tenant_id})
    open_count = await am_db.am_tickets.count_documents({"tenant_id": tenant_id, "status": "open"})
    in_progress = await am_db.am_tickets.count_documents({"tenant_id": tenant_id, "status": "in_progress"})
    resolved = await am_db.am_tickets.count_documents({"tenant_id": tenant_id, "status": "resolved"})
    
    return {
        "total": total,
        "open": open_count,
        "in_progress": in_progress,
        "resolved": resolved,
        "closed": total - open_count - in_progress - resolved
    }


# ===================== DEMO DATA =====================

@am_router.post("/demo/generate")
async def am_generate_demo_data(user: dict = Depends(am_require_admin)):
    tenant_id = user['tenant_id']
    
    demo_businesses = [
        {"name": "Main Office", "address": "123 Main St, Toronto, ON", "contact_email": "office@example.com"},
        {"name": "Warehouse", "address": "456 Industrial Way, Toronto, ON", "contact_email": "warehouse@example.com"},
        {"name": "Retail Store", "address": "789 Shopping Blvd, Toronto, ON", "contact_email": "retail@example.com"}
    ]
    
    created_businesses = []
    for biz_data in demo_businesses:
        existing = await am_db.am_businesses.find_one({"name": biz_data['name'], "tenant_id": tenant_id})
        if not existing:
            business = {
                "id": str(uuid.uuid4()),
                "tenant_id": tenant_id,
                "name": biz_data['name'],
                "address": biz_data['address'],
                "contact_email": biz_data['contact_email'],
                "contact_phone": None,
                "active": True,
                "created_at": datetime.now(timezone.utc).isoformat()
            }
            await am_db.am_businesses.insert_one(business)
            created_businesses.append(business)
    
    return {"success": True, "businesses_created": len(created_businesses)}


# ===================== STRIPE PAYMENT ROUTES =====================

@am_router.get("/stripe/config")
async def am_get_stripe_config():
    """Get Stripe configuration for frontend"""
    config = get_stripe_config()
    return config

@am_router.post("/stripe/checkout")
async def am_create_checkout(
    plan: str,
    user: dict = Depends(am_require_admin)
):
    """Create a Stripe Checkout session for plan upgrade"""
    if not is_stripe_configured():
        raise HTTPException(status_code=400, detail="Stripe payments not configured. Contact support.")
    
    base_url = os.environ.get("ANYMINUTE_BASE_URL", "https://anyminute.ai")
    
    result = await create_checkout_session(
        tenant_id=user['tenant_id'],
        plan=plan,
        success_url=f"{base_url}/anyminute/plan-upgrade?success=true",
        cancel_url=f"{base_url}/anyminute/plan-upgrade?cancelled=true",
        customer_email=user['email']
    )
    
    if not result.get("success"):
        raise HTTPException(status_code=400, detail=result.get("error", "Failed to create checkout"))
    
    return result

@am_router.post("/stripe/portal")
async def am_create_portal(user: dict = Depends(am_require_admin)):
    """Create a Stripe Customer Portal session for subscription management"""
    if not is_stripe_configured():
        raise HTTPException(status_code=400, detail="Stripe payments not configured")
    
    # Get tenant's Stripe customer ID
    tenant = await am_db.am_tenants.find_one({"id": user['tenant_id']}, {"_id": 0})
    if not tenant or not tenant.get('stripe_customer_id'):
        raise HTTPException(status_code=400, detail="No active subscription found")
    
    base_url = os.environ.get("ANYMINUTE_BASE_URL", "https://anyminute.ai")
    
    result = await create_customer_portal_session(
        customer_id=tenant['stripe_customer_id'],
        return_url=f"{base_url}/anyminute/plan-upgrade"
    )
    
    if not result.get("success"):
        raise HTTPException(status_code=400, detail=result.get("error", "Failed to create portal"))
    
    return result

@am_router.post("/stripe/webhook")
async def am_stripe_webhook(request):
    """Handle Stripe webhook events"""
    from fastapi import Request
    
    payload = await request.body()
    sig_header = request.headers.get("stripe-signature", "")
    
    result = await handle_webhook_event(payload, sig_header)
    
    if not result.get("success"):
        raise HTTPException(status_code=400, detail=result.get("error", "Webhook processing failed"))
    
    return {"received": True}
