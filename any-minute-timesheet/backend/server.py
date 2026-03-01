from fastapi import FastAPI, APIRouter, Depends, HTTPException, status, Header, Query
from fastapi.security import HTTPBearer, HTTPAuthorizationCredentials
from dotenv import load_dotenv
from starlette.middleware.cors import CORSMiddleware
from motor.motor_asyncio import AsyncIOMotorClient
import os
import logging
from pathlib import Path
from pydantic import BaseModel, Field, ConfigDict, EmailStr
from typing import List, Optional, Dict, Any
import uuid
from datetime import datetime, timezone, date, timedelta
import jwt
import bcrypt
import secrets

ROOT_DIR = Path(__file__).parent
load_dotenv(ROOT_DIR / '.env')

# MongoDB connection
mongo_url = os.environ['MONGO_URL']
client = AsyncIOMotorClient(mongo_url)
db = client[os.environ['DB_NAME']]

# JWT Config
JWT_SECRET = os.environ.get('JWT_SECRET', 'any-minute-secret-key-2024')
JWT_ALGORITHM = 'HS256'
JWT_EXPIRATION_HOURS = 24

# Create the main app
app = FastAPI(title="Any Minute Timesheet API")
api_router = APIRouter(prefix="/api")
security = HTTPBearer()

# Configure logging
logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(name)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

# ===================== PYDANTIC MODELS =====================

# --- Auth Models ---
class UserCreate(BaseModel):
    email: EmailStr
    password: str
    first_name: str
    last_name: str
    tenant_id: Optional[str] = None

class UserLogin(BaseModel):
    email: EmailStr
    password: str

class UserResponse(BaseModel):
    model_config = ConfigDict(extra="ignore")
    id: str
    tenant_id: str
    email: str
    first_name: str
    last_name: str
    role: str
    active: bool
    employee_mapping_key: Optional[str] = None
    created_at: str

# --- Tenant Models ---
class TenantCreate(BaseModel):
    name: str
    contact_email: EmailStr

# --- Business Models ---
class BusinessCreate(BaseModel):
    name: str
    address: Optional[str] = None
    contact_email: Optional[EmailStr] = None
    contact_phone: Optional[str] = None

class BusinessUpdate(BaseModel):
    name: Optional[str] = None
    address: Optional[str] = None
    contact_email: Optional[EmailStr] = None
    contact_phone: Optional[str] = None
    active: Optional[bool] = None

# --- User Management Models ---
class UserManageCreate(BaseModel):
    email: EmailStr
    password: str
    first_name: str
    last_name: str
    role: str = "employee"
    employee_mapping_key: Optional[str] = None

class UserManageUpdate(BaseModel):
    first_name: Optional[str] = None
    last_name: Optional[str] = None
    role: Optional[str] = None
    active: Optional[bool] = None
    employee_mapping_key: Optional[str] = None

class UserBusinessRoleCreate(BaseModel):
    user_id: str
    business_id: str
    role: str = "employee"

# --- Timesheet Models ---
class TimesheetEntryCreate(BaseModel):
    business_id: str
    work_date: str
    start_time: Optional[str] = None
    end_time: Optional[str] = None
    break_minutes: int = 0
    notes: Optional[str] = None

class TimesheetEntryUpdate(BaseModel):
    start_time: Optional[str] = None
    end_time: Optional[str] = None
    break_minutes: Optional[int] = None
    notes: Optional[str] = None

class TimesheetWeekCreate(BaseModel):
    user_id: str
    business_id: str
    week_start_date: str

class TimesheetApproveReject(BaseModel):
    status: str
    rejection_reason: Optional[str] = None

# --- Schedule Models ---
class ScheduleEntryCreate(BaseModel):
    user_id: str
    business_id: str
    scheduled_date: str
    start_time: str
    end_time: str
    notes: Optional[str] = None

class ScheduleEntryUpdate(BaseModel):
    start_time: Optional[str] = None
    end_time: Optional[str] = None
    notes: Optional[str] = None

# --- Settings Models ---
class SettingsUpdate(BaseModel):
    payroll_api_key: Optional[str] = None

# ===================== AUTH HELPERS =====================

def hash_password(password: str) -> str:
    return bcrypt.hashpw(password.encode('utf-8'), bcrypt.gensalt()).decode('utf-8')

def verify_password(password: str, hashed: str) -> bool:
    return bcrypt.checkpw(password.encode('utf-8'), hashed.encode('utf-8'))

def create_token(user_id: str, tenant_id: str, role: str) -> str:
    payload = {
        'user_id': user_id,
        'tenant_id': tenant_id,
        'role': role,
        'exp': datetime.now(timezone.utc) + timedelta(hours=JWT_EXPIRATION_HOURS)
    }
    return jwt.encode(payload, JWT_SECRET, algorithm=JWT_ALGORITHM)

async def get_current_user(credentials: HTTPAuthorizationCredentials = Depends(security)):
    try:
        payload = jwt.decode(credentials.credentials, JWT_SECRET, algorithms=[JWT_ALGORITHM])
        user = await db.am_users.find_one({"id": payload['user_id']}, {"_id": 0})
        if not user:
            raise HTTPException(status_code=401, detail="User not found")
        if not user.get('active', True):
            raise HTTPException(status_code=401, detail="User account is disabled")
        return user
    except jwt.ExpiredSignatureError:
        raise HTTPException(status_code=401, detail="Token expired")
    except jwt.InvalidTokenError:
        raise HTTPException(status_code=401, detail="Invalid token")

async def require_admin(user: dict = Depends(get_current_user)):
    if user['role'] not in ['admin', 'manager']:
        raise HTTPException(status_code=403, detail="Admin access required")
    return user

async def require_manager_or_admin(user: dict = Depends(get_current_user)):
    if user['role'] not in ['admin', 'manager']:
        raise HTTPException(status_code=403, detail="Manager or Admin access required")
    return user

# ===================== UTILITY FUNCTIONS =====================

def get_week_start(date_str: str) -> str:
    """Get the Saturday that starts the week containing the given date"""
    d = datetime.strptime(date_str, "%Y-%m-%d").date()
    days_since_saturday = (d.weekday() + 2) % 7
    saturday = d - timedelta(days=days_since_saturday)
    return saturday.strftime("%Y-%m-%d")

def calculate_net_hours(start_time: str, end_time: str, break_minutes: int) -> float:
    """Calculate net hours from start/end times minus break"""
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

# ===================== ROOT ENDPOINT =====================

@api_router.get("/")
async def root():
    return {"message": "Any Minute Timesheet API", "status": "healthy"}

# ===================== AUTH ROUTES =====================

@api_router.post("/auth/register")
async def register(data: UserCreate):
    existing = await db.am_users.find_one({"email": data.email.lower()})
    if existing:
        raise HTTPException(status_code=400, detail="Email already registered")
    
    user_id = str(uuid.uuid4())
    tenant_id = data.tenant_id
    role = "employee"
    
    # If no tenant specified, create "Demo Tenant" or find existing demo
    if not tenant_id:
        demo_tenant = await db.am_tenants.find_one({"name": "Demo Tenant"}, {"_id": 0})
        if demo_tenant:
            tenant_id = demo_tenant['id']
        else:
            # Create Demo Tenant
            tenant_id = str(uuid.uuid4())
            payroll_key = secrets.token_urlsafe(32)
            tenant = {
                "id": tenant_id,
                "name": "Demo Tenant",
                "contact_email": data.email.lower(),
                "payroll_api_key": payroll_key,
                "created_at": datetime.now(timezone.utc).isoformat()
            }
            await db.am_tenants.insert_one(tenant)
            role = "admin"
    
    # Check if first user in tenant - make them admin
    existing_users = await db.am_users.count_documents({"tenant_id": tenant_id})
    if existing_users == 0:
        role = "admin"
    
    user = {
        "id": user_id,
        "tenant_id": tenant_id,
        "email": data.email.lower(),
        "password_hash": hash_password(data.password),
        "first_name": data.first_name,
        "last_name": data.last_name,
        "role": role,
        "active": True,
        "employee_mapping_key": None,
        "created_at": datetime.now(timezone.utc).isoformat()
    }
    await db.am_users.insert_one(user)
    
    token = create_token(user_id, tenant_id, role)
    user_response = {k: v for k, v in user.items() if k != 'password_hash'}
    return {"token": token, "user": user_response}

@api_router.post("/auth/login")
async def login(data: UserLogin):
    user = await db.am_users.find_one({"email": data.email.lower()}, {"_id": 0})
    if not user or not verify_password(data.password, user['password_hash']):
        raise HTTPException(status_code=401, detail="Invalid credentials")
    
    if not user.get('active', True):
        raise HTTPException(status_code=401, detail="Account is disabled")
    
    token = create_token(user['id'], user['tenant_id'], user['role'])
    user_response = {k: v for k, v in user.items() if k != 'password_hash'}
    return {"token": token, "user": user_response}

@api_router.get("/auth/me")
async def get_me(user: dict = Depends(get_current_user)):
    return {k: v for k, v in user.items() if k != 'password_hash'}

# ===================== TENANT ROUTES =====================

@api_router.get("/tenant")
async def get_tenant(user: dict = Depends(get_current_user)):
    tenant = await db.am_tenants.find_one({"id": user['tenant_id']}, {"_id": 0})
    if not tenant:
        raise HTTPException(status_code=404, detail="Tenant not found")
    return tenant

@api_router.get("/tenant/settings")
async def get_tenant_settings(user: dict = Depends(require_admin)):
    tenant = await db.am_tenants.find_one({"id": user['tenant_id']}, {"_id": 0})
    if not tenant:
        raise HTTPException(status_code=404, detail="Tenant not found")
    
    # Generate payroll key if not exists
    if not tenant.get('payroll_api_key'):
        payroll_key = secrets.token_urlsafe(32)
        await db.am_tenants.update_one(
            {"id": user['tenant_id']},
            {"$set": {"payroll_api_key": payroll_key}}
        )
        tenant['payroll_api_key'] = payroll_key
    
    return {
        "payroll_api_key": tenant.get('payroll_api_key', ''),
        "tenant_name": tenant.get('name', ''),
        "contact_email": tenant.get('contact_email', '')
    }

@api_router.post("/tenant/settings/regenerate-key")
async def regenerate_payroll_key(user: dict = Depends(require_admin)):
    new_key = secrets.token_urlsafe(32)
    await db.am_tenants.update_one(
        {"id": user['tenant_id']},
        {"$set": {"payroll_api_key": new_key}}
    )
    return {"payroll_api_key": new_key}

# ===================== BUSINESS ROUTES =====================

@api_router.get("/businesses")
async def get_businesses(user: dict = Depends(get_current_user)):
    businesses = await db.am_businesses.find(
        {"tenant_id": user['tenant_id'], "active": True},
        {"_id": 0}
    ).to_list(1000)
    return businesses

@api_router.get("/businesses/all")
async def get_all_businesses(user: dict = Depends(require_admin)):
    businesses = await db.am_businesses.find(
        {"tenant_id": user['tenant_id']},
        {"_id": 0}
    ).to_list(1000)
    return businesses

@api_router.post("/businesses")
async def create_business(data: BusinessCreate, user: dict = Depends(require_admin)):
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
    await db.am_businesses.insert_one(business)
    return {k: v for k, v in business.items() if k != '_id'}

@api_router.get("/businesses/{business_id}")
async def get_business(business_id: str, user: dict = Depends(get_current_user)):
    business = await db.am_businesses.find_one(
        {"id": business_id, "tenant_id": user['tenant_id']},
        {"_id": 0}
    )
    if not business:
        raise HTTPException(status_code=404, detail="Business not found")
    return business

@api_router.put("/businesses/{business_id}")
async def update_business(business_id: str, data: BusinessUpdate, user: dict = Depends(require_admin)):
    update_data = {k: v for k, v in data.model_dump().items() if v is not None}
    if not update_data:
        raise HTTPException(status_code=400, detail="No data to update")
    
    result = await db.am_businesses.update_one(
        {"id": business_id, "tenant_id": user['tenant_id']},
        {"$set": update_data}
    )
    if result.matched_count == 0:
        raise HTTPException(status_code=404, detail="Business not found")
    
    business = await db.am_businesses.find_one({"id": business_id}, {"_id": 0})
    return business

@api_router.delete("/businesses/{business_id}")
async def delete_business(business_id: str, user: dict = Depends(require_admin)):
    result = await db.am_businesses.update_one(
        {"id": business_id, "tenant_id": user['tenant_id']},
        {"$set": {"active": False}}
    )
    if result.matched_count == 0:
        raise HTTPException(status_code=404, detail="Business not found")
    return {"success": True}

# ===================== USER MANAGEMENT ROUTES =====================

@api_router.get("/users")
async def get_users(user: dict = Depends(require_manager_or_admin)):
    users = await db.am_users.find(
        {"tenant_id": user['tenant_id']},
        {"_id": 0, "password_hash": 0}
    ).to_list(1000)
    return users

@api_router.post("/users")
async def create_user(data: UserManageCreate, user: dict = Depends(require_admin)):
    existing = await db.am_users.find_one({"email": data.email.lower()})
    if existing:
        raise HTTPException(status_code=400, detail="Email already registered")
    
    user_id = str(uuid.uuid4())
    new_user = {
        "id": user_id,
        "tenant_id": user['tenant_id'],
        "email": data.email.lower(),
        "password_hash": hash_password(data.password),
        "first_name": data.first_name,
        "last_name": data.last_name,
        "role": data.role,
        "active": True,
        "employee_mapping_key": data.employee_mapping_key,
        "created_at": datetime.now(timezone.utc).isoformat()
    }
    await db.am_users.insert_one(new_user)
    return {k: v for k, v in new_user.items() if k not in ['_id', 'password_hash']}

@api_router.get("/users/{user_id}")
async def get_user(user_id: str, user: dict = Depends(require_manager_or_admin)):
    target_user = await db.am_users.find_one(
        {"id": user_id, "tenant_id": user['tenant_id']},
        {"_id": 0, "password_hash": 0}
    )
    if not target_user:
        raise HTTPException(status_code=404, detail="User not found")
    return target_user

@api_router.put("/users/{user_id}")
async def update_user(user_id: str, data: UserManageUpdate, user: dict = Depends(require_admin)):
    update_data = {k: v for k, v in data.model_dump().items() if v is not None}
    if not update_data:
        raise HTTPException(status_code=400, detail="No data to update")
    
    result = await db.am_users.update_one(
        {"id": user_id, "tenant_id": user['tenant_id']},
        {"$set": update_data}
    )
    if result.matched_count == 0:
        raise HTTPException(status_code=404, detail="User not found")
    
    updated_user = await db.am_users.find_one({"id": user_id}, {"_id": 0, "password_hash": 0})
    return updated_user

@api_router.delete("/users/{user_id}")
async def delete_user(user_id: str, user: dict = Depends(require_admin)):
    if user_id == user['id']:
        raise HTTPException(status_code=400, detail="Cannot delete your own account")
    
    result = await db.am_users.update_one(
        {"id": user_id, "tenant_id": user['tenant_id']},
        {"$set": {"active": False}}
    )
    if result.matched_count == 0:
        raise HTTPException(status_code=404, detail="User not found")
    return {"success": True}

# ===================== USER-BUSINESS ROLE ROUTES =====================

@api_router.get("/user-business-roles")
async def get_user_business_roles(user: dict = Depends(get_current_user)):
    roles = await db.am_user_business_roles.find(
        {"tenant_id": user['tenant_id']},
        {"_id": 0}
    ).to_list(1000)
    return roles

@api_router.post("/user-business-roles")
async def assign_user_to_business(data: UserBusinessRoleCreate, user: dict = Depends(require_admin)):
    # Verify user and business exist in tenant
    target_user = await db.am_users.find_one({"id": data.user_id, "tenant_id": user['tenant_id']})
    if not target_user:
        raise HTTPException(status_code=404, detail="User not found")
    
    business = await db.am_businesses.find_one({"id": data.business_id, "tenant_id": user['tenant_id']})
    if not business:
        raise HTTPException(status_code=404, detail="Business not found")
    
    # Check if role already exists
    existing = await db.am_user_business_roles.find_one({
        "user_id": data.user_id,
        "business_id": data.business_id
    })
    if existing:
        # Update existing role
        await db.am_user_business_roles.update_one(
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
    await db.am_user_business_roles.insert_one(role_record)
    return {k: v for k, v in role_record.items() if k != '_id'}

@api_router.delete("/user-business-roles/{user_id}/{business_id}")
async def remove_user_from_business(user_id: str, business_id: str, user: dict = Depends(require_admin)):
    result = await db.am_user_business_roles.delete_one({
        "user_id": user_id,
        "business_id": business_id,
        "tenant_id": user['tenant_id']
    })
    if result.deleted_count == 0:
        raise HTTPException(status_code=404, detail="Role assignment not found")
    return {"success": True}

# ===================== TIMESHEET WEEK ROUTES =====================

@api_router.get("/timesheet-weeks")
async def get_timesheet_weeks(
    user: dict = Depends(get_current_user),
    user_id: Optional[str] = Query(None),
    business_id: Optional[str] = Query(None),
    status: Optional[str] = Query(None)
):
    query = {"tenant_id": user['tenant_id']}
    
    # Non-admins can only see their own timesheets
    if user['role'] == 'employee':
        query['user_id'] = user['id']
    elif user_id:
        query['user_id'] = user_id
    
    if business_id:
        query['business_id'] = business_id
    if status:
        query['status'] = status
    
    weeks = await db.am_timesheet_weeks.find(query, {"_id": 0}).sort("week_start_date", -1).to_list(100)
    return weeks

@api_router.post("/timesheet-weeks")
async def create_timesheet_week(data: TimesheetWeekCreate, user: dict = Depends(get_current_user)):
    # Validate week_start_date is a Saturday
    week_start = get_week_start(data.week_start_date)
    
    # Check user can create for target user
    target_user_id = data.user_id
    if user['role'] == 'employee' and target_user_id != user['id']:
        raise HTTPException(status_code=403, detail="Cannot create timesheet for another user")
    
    # Verify business exists
    business = await db.am_businesses.find_one({"id": data.business_id, "tenant_id": user['tenant_id']})
    if not business:
        raise HTTPException(status_code=404, detail="Business not found")
    
    # Check if week already exists
    existing = await db.am_timesheet_weeks.find_one({
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
    await db.am_timesheet_weeks.insert_one(week)
    return {k: v for k, v in week.items() if k != '_id'}

@api_router.get("/timesheet-weeks/{week_id}")
async def get_timesheet_week(week_id: str, user: dict = Depends(get_current_user)):
    week = await db.am_timesheet_weeks.find_one(
        {"id": week_id, "tenant_id": user['tenant_id']},
        {"_id": 0}
    )
    if not week:
        raise HTTPException(status_code=404, detail="Timesheet week not found")
    
    if user['role'] == 'employee' and week['user_id'] != user['id']:
        raise HTTPException(status_code=403, detail="Access denied")
    
    return week

@api_router.post("/timesheet-weeks/{week_id}/submit")
async def submit_timesheet_week(week_id: str, user: dict = Depends(get_current_user)):
    week = await db.am_timesheet_weeks.find_one(
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
    
    # Calculate total hours from entries
    entries = await db.am_timesheet_entries.find({"week_id": week_id}, {"_id": 0}).to_list(100)
    total_hours = sum(e.get('net_hours', 0) for e in entries)
    
    await db.am_timesheet_weeks.update_one(
        {"id": week_id},
        {"$set": {
            "status": "submitted",
            "total_hours": total_hours,
            "submitted_at": datetime.now(timezone.utc).isoformat(),
            "rejection_reason": None
        }}
    )
    return {"success": True, "status": "submitted"}

@api_router.post("/timesheet-weeks/{week_id}/approve")
async def approve_reject_timesheet(week_id: str, data: TimesheetApproveReject, user: dict = Depends(require_manager_or_admin)):
    week = await db.am_timesheet_weeks.find_one(
        {"id": week_id, "tenant_id": user['tenant_id']},
        {"_id": 0}
    )
    if not week:
        raise HTTPException(status_code=404, detail="Timesheet week not found")
    
    if week['status'] != 'submitted':
        raise HTTPException(status_code=400, detail="Can only approve/reject submitted timesheets")
    
    if data.status == 'approved':
        await db.am_timesheet_weeks.update_one(
            {"id": week_id},
            {"$set": {
                "status": "approved",
                "approved_at": datetime.now(timezone.utc).isoformat(),
                "approved_by": user['id']
            }}
        )
    elif data.status == 'rejected':
        await db.am_timesheet_weeks.update_one(
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

@api_router.get("/timesheet-entries")
async def get_timesheet_entries(
    user: dict = Depends(get_current_user),
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
    
    entries = await db.am_timesheet_entries.find(query, {"_id": 0}).sort("work_date", 1).to_list(1000)
    return entries

@api_router.post("/timesheet-entries")
async def create_timesheet_entry(data: TimesheetEntryCreate, user: dict = Depends(get_current_user)):
    # Get or create the week for this entry
    week_start = get_week_start(data.work_date)
    
    week = await db.am_timesheet_weeks.find_one({
        "user_id": user['id'],
        "business_id": data.business_id,
        "week_start_date": week_start
    })
    
    if not week:
        # Create the week
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
        await db.am_timesheet_weeks.insert_one(week)
    else:
        week_id = week['id']
        if week.get('locked'):
            raise HTTPException(status_code=400, detail="Timesheet week is locked")
        if week['status'] == 'approved':
            raise HTTPException(status_code=400, detail="Cannot modify approved timesheet")
    
    # Check if entry exists for this date
    existing = await db.am_timesheet_entries.find_one({
        "week_id": week_id,
        "work_date": data.work_date
    })
    if existing:
        raise HTTPException(status_code=400, detail="Entry already exists for this date. Use PUT to update.")
    
    net_hours = calculate_net_hours(data.start_time, data.end_time, data.break_minutes)
    
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
    await db.am_timesheet_entries.insert_one(entry)
    
    # Update week total hours
    await update_week_total_hours(week_id)
    
    return {k: v for k, v in entry.items() if k != '_id'}

@api_router.put("/timesheet-entries/{entry_id}")
async def update_timesheet_entry(entry_id: str, data: TimesheetEntryUpdate, user: dict = Depends(get_current_user)):
    entry = await db.am_timesheet_entries.find_one(
        {"id": entry_id, "tenant_id": user['tenant_id']},
        {"_id": 0}
    )
    if not entry:
        raise HTTPException(status_code=404, detail="Entry not found")
    
    if user['role'] == 'employee' and entry['user_id'] != user['id']:
        raise HTTPException(status_code=403, detail="Access denied")
    
    # Check week status
    week = await db.am_timesheet_weeks.find_one({"id": entry['week_id']}, {"_id": 0})
    if week and week.get('locked'):
        raise HTTPException(status_code=400, detail="Timesheet week is locked")
    if week and week['status'] == 'approved':
        raise HTTPException(status_code=400, detail="Cannot modify approved timesheet")
    
    update_data = {k: v for k, v in data.model_dump().items() if v is not None}
    
    # Recalculate net hours if time changed
    start_time = update_data.get('start_time', entry.get('start_time'))
    end_time = update_data.get('end_time', entry.get('end_time'))
    break_minutes = update_data.get('break_minutes', entry.get('break_minutes', 0))
    update_data['net_hours'] = calculate_net_hours(start_time, end_time, break_minutes)
    
    await db.am_timesheet_entries.update_one({"id": entry_id}, {"$set": update_data})
    
    # Update week total hours
    await update_week_total_hours(entry['week_id'])
    
    updated_entry = await db.am_timesheet_entries.find_one({"id": entry_id}, {"_id": 0})
    return updated_entry

@api_router.delete("/timesheet-entries/{entry_id}")
async def delete_timesheet_entry(entry_id: str, user: dict = Depends(get_current_user)):
    entry = await db.am_timesheet_entries.find_one(
        {"id": entry_id, "tenant_id": user['tenant_id']},
        {"_id": 0}
    )
    if not entry:
        raise HTTPException(status_code=404, detail="Entry not found")
    
    if user['role'] == 'employee' and entry['user_id'] != user['id']:
        raise HTTPException(status_code=403, detail="Access denied")
    
    # Check week status
    week = await db.am_timesheet_weeks.find_one({"id": entry['week_id']}, {"_id": 0})
    if week and week.get('locked'):
        raise HTTPException(status_code=400, detail="Timesheet week is locked")
    if week and week['status'] == 'approved':
        raise HTTPException(status_code=400, detail="Cannot modify approved timesheet")
    
    week_id = entry['week_id']
    await db.am_timesheet_entries.delete_one({"id": entry_id})
    
    # Update week total hours
    await update_week_total_hours(week_id)
    
    return {"success": True}

async def update_week_total_hours(week_id: str):
    entries = await db.am_timesheet_entries.find({"week_id": week_id}, {"_id": 0}).to_list(100)
    total_hours = sum(e.get('net_hours', 0) for e in entries)
    await db.am_timesheet_weeks.update_one(
        {"id": week_id},
        {"$set": {"total_hours": round(total_hours, 2)}}
    )

# ===================== SCHEDULE ROUTES =====================

@api_router.get("/schedules")
async def get_schedules(
    user: dict = Depends(get_current_user),
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
    
    schedules = await db.am_schedule_entries.find(query, {"_id": 0}).sort("scheduled_date", 1).to_list(1000)
    return schedules

@api_router.post("/schedules")
async def create_schedule(data: ScheduleEntryCreate, user: dict = Depends(require_manager_or_admin)):
    # Verify user exists
    target_user = await db.am_users.find_one({"id": data.user_id, "tenant_id": user['tenant_id']})
    if not target_user:
        raise HTTPException(status_code=404, detail="User not found")
    
    # Verify business exists
    business = await db.am_businesses.find_one({"id": data.business_id, "tenant_id": user['tenant_id']})
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
    await db.am_schedule_entries.insert_one(schedule)
    return {k: v for k, v in schedule.items() if k != '_id'}

@api_router.put("/schedules/{schedule_id}")
async def update_schedule(schedule_id: str, data: ScheduleEntryUpdate, user: dict = Depends(require_manager_or_admin)):
    update_data = {k: v for k, v in data.model_dump().items() if v is not None}
    if not update_data:
        raise HTTPException(status_code=400, detail="No data to update")
    
    result = await db.am_schedule_entries.update_one(
        {"id": schedule_id, "tenant_id": user['tenant_id']},
        {"$set": update_data}
    )
    if result.matched_count == 0:
        raise HTTPException(status_code=404, detail="Schedule not found")
    
    schedule = await db.am_schedule_entries.find_one({"id": schedule_id}, {"_id": 0})
    return schedule

@api_router.delete("/schedules/{schedule_id}")
async def delete_schedule(schedule_id: str, user: dict = Depends(require_manager_or_admin)):
    result = await db.am_schedule_entries.delete_one(
        {"id": schedule_id, "tenant_id": user['tenant_id']}
    )
    if result.deleted_count == 0:
        raise HTTPException(status_code=404, detail="Schedule not found")
    return {"success": True}

# ===================== REPORTS ROUTES =====================

@api_router.get("/reports/by-business")
async def get_report_by_business(
    user: dict = Depends(require_manager_or_admin),
    business_id: Optional[str] = Query(None),
    start_date: Optional[str] = Query(None),
    end_date: Optional[str] = Query(None)
):
    query = {"tenant_id": user['tenant_id']}
    
    if business_id:
        query['business_id'] = business_id
    
    # Get all approved weeks in date range
    week_query = {**query, "status": "approved"}
    if start_date:
        week_query['week_start_date'] = {"$gte": start_date}
    if end_date:
        if 'week_start_date' in week_query:
            week_query['week_start_date']['$lte'] = end_date
        else:
            week_query['week_start_date'] = {"$lte": end_date}
    
    weeks = await db.am_timesheet_weeks.find(week_query, {"_id": 0}).to_list(1000)
    
    # Get businesses and users for names
    businesses = await db.am_businesses.find({"tenant_id": user['tenant_id']}, {"_id": 0}).to_list(100)
    users = await db.am_users.find({"tenant_id": user['tenant_id']}, {"_id": 0, "password_hash": 0}).to_list(1000)
    
    business_map = {b['id']: b for b in businesses}
    user_map = {u['id']: u for u in users}
    
    # Aggregate by business
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
    
    # Convert to list
    result = []
    for biz_id, data in report.items():
        data['user_breakdown'] = list(data['user_breakdown'].values())
        data['total_hours'] = round(data['total_hours'], 2)
        for ub in data['user_breakdown']:
            ub['hours'] = round(ub['hours'], 2)
        result.append(data)
    
    return result

# ===================== PAYROLL INTEGRATION API =====================

async def verify_payroll_key(x_payroll_key: str = Header(...)):
    """Verify the payroll API key"""
    if not x_payroll_key:
        raise HTTPException(status_code=401, detail="Missing X-PAYROLL-KEY header")
    
    tenant = await db.am_tenants.find_one({"payroll_api_key": x_payroll_key}, {"_id": 0})
    if not tenant:
        raise HTTPException(status_code=401, detail="Invalid payroll API key")
    
    return tenant

@api_router.get("/payroll/employees")
async def payroll_get_employees(tenant: dict = Depends(verify_payroll_key)):
    """Get employees for payroll system"""
    users = await db.am_users.find(
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

@api_router.get("/payroll/approved-entries")
async def payroll_get_approved_entries(
    start: str = Query(..., description="Start date YYYY-MM-DD"),
    end: str = Query(..., description="End date YYYY-MM-DD"),
    tenant: dict = Depends(verify_payroll_key)
):
    """Get approved timesheet entries for payroll"""
    # Get all approved weeks in date range
    weeks = await db.am_timesheet_weeks.find({
        "tenant_id": tenant['id'],
        "status": "approved",
        "week_start_date": {"$gte": start, "$lte": end}
    }, {"_id": 0}).to_list(1000)
    
    week_ids = [w['id'] for w in weeks]
    
    # Get all entries for these weeks
    entries = await db.am_timesheet_entries.find({
        "week_id": {"$in": week_ids},
        "work_date": {"$gte": start, "$lte": end}
    }, {"_id": 0}).to_list(10000)
    
    # Get users for mapping keys
    users = await db.am_users.find({"tenant_id": tenant['id']}, {"_id": 0}).to_list(1000)
    user_map = {u['id']: u for u in users}
    
    result = []
    for entry in entries:
        user = user_map.get(entry['user_id'], {})
        result.append({
            "employee_key": user.get('employee_mapping_key') or entry['user_id'],
            "employee_email": user.get('email', ''),
            "work_date": entry['work_date'],
            "regular_hours": entry.get('net_hours', 0),
            "overtime_hours": 0,
            "source_ref": entry['id'],
            "notes": entry.get('notes', '')
        })
    
    return {"entries": result}

@api_router.post("/payroll/lock")
async def payroll_lock_entries(
    start: str = Query(..., description="Start date YYYY-MM-DD"),
    end: str = Query(..., description="End date YYYY-MM-DD"),
    tenant: dict = Depends(verify_payroll_key)
):
    """Lock approved timesheet entries after payroll processing"""
    result = await db.am_timesheet_weeks.update_many(
        {
            "tenant_id": tenant['id'],
            "status": "approved",
            "week_start_date": {"$gte": start, "$lte": end}
        },
        {"$set": {"locked": True, "locked_at": datetime.now(timezone.utc).isoformat()}}
    )
    
    return {"success": True, "locked_count": result.modified_count}

# ===================== DASHBOARD ROUTES =====================

@api_router.get("/dashboard/stats")
async def get_dashboard_stats(user: dict = Depends(get_current_user)):
    tenant_id = user['tenant_id']
    
    # Count businesses
    business_count = await db.am_businesses.count_documents({"tenant_id": tenant_id, "active": True})
    
    # Count users
    user_count = await db.am_users.count_documents({"tenant_id": tenant_id, "active": True})
    
    # Pending approvals (submitted timesheets)
    pending_count = await db.am_timesheet_weeks.count_documents({
        "tenant_id": tenant_id,
        "status": "submitted"
    })
    
    # Get user's own timesheet hours this week
    today = date.today()
    week_start = get_week_start(today.strftime("%Y-%m-%d"))
    
    my_week = await db.am_timesheet_weeks.find_one({
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

# ===================== DEMO DATA =====================

@api_router.post("/demo/generate")
async def generate_demo_data(user: dict = Depends(require_admin)):
    """Generate demo businesses and data for testing"""
    tenant_id = user['tenant_id']
    
    # Create demo businesses
    demo_businesses = [
        {"name": "Main Office", "address": "123 Main St, Toronto, ON", "contact_email": "office@example.com"},
        {"name": "Warehouse", "address": "456 Industrial Way, Toronto, ON", "contact_email": "warehouse@example.com"},
        {"name": "Retail Store", "address": "789 Shopping Blvd, Toronto, ON", "contact_email": "retail@example.com"}
    ]
    
    created_businesses = []
    for biz_data in demo_businesses:
        existing = await db.am_businesses.find_one({"name": biz_data['name'], "tenant_id": tenant_id})
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
            await db.am_businesses.insert_one(business)
            created_businesses.append(business)
    
    return {"success": True, "businesses_created": len(created_businesses)}

# Include the router
app.include_router(api_router)

app.add_middleware(
    CORSMiddleware,
    allow_credentials=True,
    allow_origins=os.environ.get('CORS_ORIGINS', '*').split(','),
    allow_methods=["*"],
    allow_headers=["*"],
)

@app.on_event("shutdown")
async def shutdown_db_client():
    client.close()
