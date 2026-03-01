"""
Any Minute - Time Tracking Backend Server
Runs on port 8002 alongside Payroll Canada (port 8001)
"""
from fastapi import FastAPI, APIRouter, Depends, HTTPException, status, Header
from fastapi.responses import StreamingResponse, JSONResponse
from fastapi.security import HTTPBearer, HTTPAuthorizationCredentials
from dotenv import load_dotenv
from starlette.middleware.cors import CORSMiddleware
from motor.motor_asyncio import AsyncIOMotorClient
import os
import logging
from pathlib import Path
from pydantic import BaseModel, Field, EmailStr
from typing import List, Optional, Dict, Any
import uuid
from datetime import datetime, timezone, date, timedelta
import jwt
import bcrypt
import secrets
from io import BytesIO

ROOT_DIR = Path(__file__).parent
load_dotenv(ROOT_DIR / '.env')

# MongoDB connection (same instance, different collections with am_ prefix)
mongo_url = os.environ['MONGO_URL']
client = AsyncIOMotorClient(mongo_url)
db = client[os.environ['DB_NAME']]

# JWT Config
JWT_SECRET = os.environ.get('AM_JWT_SECRET', 'anyminute-secret-key-2024')
JWT_ALGORITHM = 'HS256'
JWT_EXPIRATION_HOURS = 24

# Payroll Integration Key
X_PAYROLL_KEY = os.environ.get('X_PAYROLL_KEY', secrets.token_urlsafe(32))

# Create the main app
app = FastAPI(title="Any Minute - Time Tracking API")
api_router = APIRouter(prefix="/api")
security = HTTPBearer()

# Configure logging
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

# ===================== MODELS =====================

class UserLogin(BaseModel):
    email: EmailStr
    password: str

class SubscriberRegister(BaseModel):
    first_name: str
    last_name: str
    email: EmailStr
    phone: Optional[str] = None
    ext: Optional[str] = None
    mobile: Optional[str] = None
    postal_code: Optional[str] = None
    plan: str = "basic"
    password: str

class ForgotPassword(BaseModel):
    email: EmailStr

class BusinessCreate(BaseModel):
    company_name: str
    operating_name: Optional[str] = None
    primary_contact_name: str
    business_number: Optional[str] = None
    legal_entity_type: str = "Incorporation"
    website: Optional[str] = None
    street_number: Optional[str] = None
    address_line1: Optional[str] = None
    address_line2: Optional[str] = None
    suite_number: Optional[str] = None
    suite_type: Optional[str] = None
    city: Optional[str] = None
    province: Optional[str] = None
    postal_code: Optional[str] = None
    logo_url: Optional[str] = None

class UserCreate(BaseModel):
    business_id: str
    role: str = "employee"
    first_name: str
    last_name: str
    email: Optional[EmailStr] = None
    password: Optional[str] = None
    phone: Optional[str] = None
    mobile: Optional[str] = None
    dob: Optional[str] = None
    gender: Optional[str] = None
    hire_date: Optional[str] = None
    postal_code: Optional[str] = None

class ProjectCreate(BaseModel):
    business_id: str
    project_name: str
    start_date: str
    end_date: Optional[str] = None

class TimesheetEntryCreate(BaseModel):
    employee_id: str
    business_id: str
    work_date: str
    worked: bool = True
    start_time: Optional[str] = None
    end_time: Optional[str] = None
    break_minutes: int = 0
    project_id: Optional[str] = None
    notes: Optional[str] = None

class TimesheetStatusUpdate(BaseModel):
    entry_ids: List[str]
    status: str  # approved, rejected

class ScheduleCreate(BaseModel):
    employee_id: str
    business_id: str
    project_id: Optional[str] = None
    date: str
    scheduled_minutes: int

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
        user['tenant_id'] = payload.get('tenant_id')
        return user
    except jwt.ExpiredSignatureError:
        raise HTTPException(status_code=401, detail="Token expired")
    except jwt.InvalidTokenError:
        raise HTTPException(status_code=401, detail="Invalid token")

async def require_subscriber(user: dict = Depends(get_current_user)):
    if user.get('role') != 'subscriber':
        raise HTTPException(status_code=403, detail="Subscriber access required")
    return user

async def require_manager_or_above(user: dict = Depends(get_current_user)):
    if user.get('role') not in ['subscriber', 'manager']:
        raise HTTPException(status_code=403, detail="Manager or Subscriber access required")
    return user

def verify_payroll_key(x_payroll_key: str = Header(None, alias="X-PAYROLL-KEY")):
    if not x_payroll_key or x_payroll_key != X_PAYROLL_KEY:
        raise HTTPException(status_code=401, detail="Invalid or missing X-PAYROLL-KEY")
    return True

# ===================== AUTH ROUTES =====================

@api_router.get("/")
async def root():
    return {"message": "Any Minute Time Tracking API", "status": "healthy"}

@api_router.post("/auth/login")
async def login(data: UserLogin):
    user = await db.am_users.find_one({"email": data.email.lower()}, {"_id": 0})
    if not user or not verify_password(data.password, user.get('password_hash', '')):
        raise HTTPException(status_code=401, detail="Invalid credentials")
    
    token = create_token(user['id'], user.get('tenant_id'), user.get('role', 'employee'))
    return {"token": token, "user": {k: v for k, v in user.items() if k != 'password_hash'}}

@api_router.post("/auth/register")
async def register_subscriber(data: SubscriberRegister):
    existing = await db.am_users.find_one({"email": data.email.lower()})
    if existing:
        raise HTTPException(status_code=400, detail="Email already registered")
    
    tenant_id = str(uuid.uuid4())
    user_id = str(uuid.uuid4())
    
    # Create tenant (subscriber account)
    tenant = {
        "id": tenant_id,
        "owner_user_id": user_id,
        "plan": data.plan,
        "created_at": datetime.now(timezone.utc).isoformat(),
        "active": True
    }
    await db.am_tenants.insert_one(tenant)
    
    # Create subscriber user
    user = {
        "id": user_id,
        "tenant_id": tenant_id,
        "email": data.email.lower(),
        "password_hash": hash_password(data.password),
        "first_name": data.first_name,
        "last_name": data.last_name,
        "phone": data.phone,
        "ext": data.ext,
        "mobile": data.mobile,
        "postal_code": data.postal_code,
        "role": "subscriber",
        "active": True,
        "created_at": datetime.now(timezone.utc).isoformat()
    }
    await db.am_users.insert_one(user)
    
    token = create_token(user_id, tenant_id, "subscriber")
    return {
        "token": token,
        "user": {k: v for k, v in user.items() if k != 'password_hash'},
        "message": "Registration successful. Please check your email for next steps."
    }

@api_router.post("/auth/forgot-password")
async def forgot_password(data: ForgotPassword):
    user = await db.am_users.find_one({"email": data.email.lower()}, {"_id": 0})
    if not user:
        # Don't reveal if email exists
        return {"message": "If the email exists, a reset link will be sent."}
    
    # Mock email sending - in production would send real email
    reset_token = secrets.token_urlsafe(32)
    await db.am_password_resets.insert_one({
        "id": str(uuid.uuid4()),
        "user_id": user['id'],
        "token": reset_token,
        "expires_at": (datetime.now(timezone.utc) + timedelta(hours=1)).isoformat(),
        "used": False
    })
    
    logger.info(f"Password reset token generated for {data.email}: {reset_token}")
    return {"message": "If the email exists, a reset link will be sent."}

@api_router.get("/auth/me")
async def get_me(user: dict = Depends(get_current_user)):
    return {k: v for k, v in user.items() if k != 'password_hash'}

# ===================== BUSINESS ROUTES =====================

@api_router.get("/businesses")
async def get_businesses(user: dict = Depends(get_current_user)):
    businesses = await db.am_businesses.find(
        {"tenant_id": user['tenant_id']}, {"_id": 0}
    ).to_list(1000)
    return businesses

@api_router.post("/businesses")
async def create_business(data: BusinessCreate, user: dict = Depends(require_subscriber)):
    business_id = str(uuid.uuid4())
    business = {
        "id": business_id,
        "tenant_id": user['tenant_id'],
        "company_name": data.company_name,
        "operating_name": data.operating_name,
        "primary_contact_name": data.primary_contact_name,
        "business_number": data.business_number,
        "legal_entity_type": data.legal_entity_type,
        "website": data.website,
        "street_number": data.street_number,
        "address_line1": data.address_line1,
        "address_line2": data.address_line2,
        "suite_number": data.suite_number,
        "suite_type": data.suite_type,
        "city": data.city,
        "province": data.province,
        "postal_code": data.postal_code,
        "logo_url": data.logo_url,
        "active": True,
        "created_at": datetime.now(timezone.utc).isoformat()
    }
    await db.am_businesses.insert_one(business)
    return {k: v for k, v in business.items() if k != '_id'}

@api_router.put("/businesses/{business_id}")
async def update_business(business_id: str, data: BusinessCreate, user: dict = Depends(require_subscriber)):
    update_data = {k: v for k, v in data.model_dump().items() if v is not None}
    result = await db.am_businesses.update_one(
        {"id": business_id, "tenant_id": user['tenant_id']},
        {"$set": update_data}
    )
    if result.matched_count == 0:
        raise HTTPException(status_code=404, detail="Business not found")
    business = await db.am_businesses.find_one({"id": business_id}, {"_id": 0})
    return business

@api_router.get("/businesses/{business_id}")
async def get_business(business_id: str, user: dict = Depends(get_current_user)):
    business = await db.am_businesses.find_one(
        {"id": business_id, "tenant_id": user['tenant_id']}, {"_id": 0}
    )
    if not business:
        raise HTTPException(status_code=404, detail="Business not found")
    return business

# ===================== USER ROUTES =====================

@api_router.get("/users")
async def get_users(business_id: Optional[str] = None, user: dict = Depends(get_current_user)):
    query = {"tenant_id": user['tenant_id']}
    if business_id:
        query["business_id"] = business_id
    users = await db.am_users.find(query, {"_id": 0, "password_hash": 0}).to_list(1000)
    return users

@api_router.post("/users")
async def create_user(data: UserCreate, user: dict = Depends(require_subscriber)):
    user_id = str(uuid.uuid4())
    new_user = {
        "id": user_id,
        "tenant_id": user['tenant_id'],
        "business_id": data.business_id,
        "role": data.role,
        "first_name": data.first_name,
        "last_name": data.last_name,
        "email": data.email.lower() if data.email else None,
        "password_hash": hash_password(data.password) if data.password else None,
        "phone": data.phone,
        "mobile": data.mobile,
        "dob": data.dob,
        "gender": data.gender,
        "hire_date": data.hire_date,
        "postal_code": data.postal_code,
        "active": True,
        "created_at": datetime.now(timezone.utc).isoformat()
    }
    await db.am_users.insert_one(new_user)
    return {k: v for k, v in new_user.items() if k not in ['_id', 'password_hash']}

@api_router.put("/users/{user_id}")
async def update_user(user_id: str, data: UserCreate, user: dict = Depends(require_subscriber)):
    update_data = {k: v for k, v in data.model_dump().items() if v is not None and k != 'password'}
    if data.password:
        update_data['password_hash'] = hash_password(data.password)
    if data.email:
        update_data['email'] = data.email.lower()
    
    result = await db.am_users.update_one(
        {"id": user_id, "tenant_id": user['tenant_id']},
        {"$set": update_data}
    )
    if result.matched_count == 0:
        raise HTTPException(status_code=404, detail="User not found")
    updated_user = await db.am_users.find_one({"id": user_id}, {"_id": 0, "password_hash": 0})
    return updated_user

@api_router.get("/users/{user_id}")
async def get_user(user_id: str, user: dict = Depends(get_current_user)):
    found_user = await db.am_users.find_one(
        {"id": user_id, "tenant_id": user['tenant_id']}, {"_id": 0, "password_hash": 0}
    )
    if not found_user:
        raise HTTPException(status_code=404, detail="User not found")
    return found_user

# ===================== PROJECT ROUTES =====================

@api_router.get("/projects")
async def get_projects(business_id: Optional[str] = None, user: dict = Depends(get_current_user)):
    query = {"tenant_id": user['tenant_id']}
    if business_id:
        query["business_id"] = business_id
    projects = await db.am_projects.find(query, {"_id": 0}).to_list(1000)
    return projects

@api_router.post("/projects")
async def create_project(data: ProjectCreate, user: dict = Depends(require_subscriber)):
    project_id = str(uuid.uuid4())
    project = {
        "id": project_id,
        "tenant_id": user['tenant_id'],
        "business_id": data.business_id,
        "project_name": data.project_name,
        "start_date": data.start_date,
        "end_date": data.end_date,
        "active": True,
        "created_at": datetime.now(timezone.utc).isoformat()
    }
    await db.am_projects.insert_one(project)
    return {k: v for k, v in project.items() if k != '_id'}

@api_router.put("/projects/{project_id}")
async def update_project(project_id: str, data: ProjectCreate, user: dict = Depends(require_subscriber)):
    update_data = {k: v for k, v in data.model_dump().items() if v is not None}
    result = await db.am_projects.update_one(
        {"id": project_id, "tenant_id": user['tenant_id']},
        {"$set": update_data}
    )
    if result.matched_count == 0:
        raise HTTPException(status_code=404, detail="Project not found")
    project = await db.am_projects.find_one({"id": project_id}, {"_id": 0})
    return project

@api_router.delete("/projects/{project_id}")
async def delete_project(project_id: str, user: dict = Depends(require_subscriber)):
    result = await db.am_projects.delete_one(
        {"id": project_id, "tenant_id": user['tenant_id']}
    )
    if result.deleted_count == 0:
        raise HTTPException(status_code=404, detail="Project not found")
    return {"success": True}

# ===================== TIMESHEET ROUTES =====================

def get_week_start(d: date) -> date:
    """Get Saturday of the week containing the given date"""
    days_since_saturday = (d.weekday() + 2) % 7
    return d - timedelta(days=days_since_saturday)

def calculate_minutes(start_time: str, end_time: str) -> int:
    """Calculate minutes between start and end time (HH:MM format)"""
    if not start_time or not end_time:
        return 0
    try:
        start_parts = start_time.split(':')
        end_parts = end_time.split(':')
        start_minutes = int(start_parts[0]) * 60 + int(start_parts[1])
        end_minutes = int(end_parts[0]) * 60 + int(end_parts[1])
        return max(0, end_minutes - start_minutes)
    except:
        return 0

@api_router.get("/timesheets")
async def get_timesheets(
    employee_id: Optional[str] = None,
    business_id: Optional[str] = None,
    week_start: Optional[str] = None,
    user: dict = Depends(get_current_user)
):
    query = {"tenant_id": user['tenant_id']}
    if employee_id:
        query["employee_id"] = employee_id
    if business_id:
        query["business_id"] = business_id
    if week_start:
        # Get entries for the week (Saturday to Friday)
        start = datetime.strptime(week_start, '%Y-%m-%d').date()
        end = start + timedelta(days=6)
        query["work_date"] = {"$gte": week_start, "$lte": end.isoformat()}
    
    entries = await db.am_timesheet_entries.find(query, {"_id": 0}).sort("work_date", 1).to_list(1000)
    return entries

@api_router.post("/timesheets")
async def create_timesheet_entry(data: TimesheetEntryCreate, user: dict = Depends(get_current_user)):
    # Check if entry exists for this employee/date
    existing = await db.am_timesheet_entries.find_one({
        "tenant_id": user['tenant_id'],
        "employee_id": data.employee_id,
        "work_date": data.work_date
    })
    
    total_minutes = calculate_minutes(data.start_time, data.end_time) if data.worked else 0
    net_minutes = max(0, total_minutes - data.break_minutes)
    
    entry_data = {
        "tenant_id": user['tenant_id'],
        "employee_id": data.employee_id,
        "business_id": data.business_id,
        "work_date": data.work_date,
        "worked": data.worked,
        "start_time": data.start_time if data.worked else None,
        "end_time": data.end_time if data.worked else None,
        "total_minutes": total_minutes,
        "break_minutes": data.break_minutes if data.worked else 0,
        "net_minutes": net_minutes,
        "project_id": data.project_id,
        "notes": data.notes,
        "status": "pending",
        "approved_by": None,
        "approved_at": None,
        "exported_to_payroll": False,
        "exported_at": None,
        "exported_payroll_run_id": None,
        "updated_at": datetime.now(timezone.utc).isoformat()
    }
    
    if existing:
        # Update existing entry
        await db.am_timesheet_entries.update_one(
            {"id": existing['id']},
            {"$set": entry_data}
        )
        entry_data['id'] = existing['id']
    else:
        # Create new entry
        entry_data['id'] = str(uuid.uuid4())
        entry_data['created_at'] = datetime.now(timezone.utc).isoformat()
        await db.am_timesheet_entries.insert_one(entry_data)
    
    return {k: v for k, v in entry_data.items() if k != '_id'}

@api_router.put("/timesheets/status")
async def update_timesheet_status(data: TimesheetStatusUpdate, user: dict = Depends(require_manager_or_above)):
    if data.status not in ['approved', 'rejected', 'pending']:
        raise HTTPException(status_code=400, detail="Invalid status")
    
    update_data = {
        "status": data.status,
        "updated_at": datetime.now(timezone.utc).isoformat()
    }
    
    if data.status == 'approved':
        update_data['approved_by'] = user['id']
        update_data['approved_at'] = datetime.now(timezone.utc).isoformat()
    else:
        update_data['approved_by'] = None
        update_data['approved_at'] = None
    
    result = await db.am_timesheet_entries.update_many(
        {"id": {"$in": data.entry_ids}, "tenant_id": user['tenant_id']},
        {"$set": update_data}
    )
    
    return {"updated": result.modified_count}

@api_router.put("/timesheets/{entry_id}/break")
async def update_break(entry_id: str, break_minutes: int, user: dict = Depends(get_current_user)):
    entry = await db.am_timesheet_entries.find_one(
        {"id": entry_id, "tenant_id": user['tenant_id']}, {"_id": 0}
    )
    if not entry:
        raise HTTPException(status_code=404, detail="Entry not found")
    
    net_minutes = max(0, entry.get('total_minutes', 0) - break_minutes)
    
    await db.am_timesheet_entries.update_one(
        {"id": entry_id},
        {"$set": {
            "break_minutes": break_minutes,
            "net_minutes": net_minutes,
            "updated_at": datetime.now(timezone.utc).isoformat()
        }}
    )
    
    return {"success": True, "net_minutes": net_minutes}

# ===================== SCHEDULE ROUTES =====================

@api_router.get("/schedules")
async def get_schedules(
    employee_id: Optional[str] = None,
    business_id: Optional[str] = None,
    start_date: Optional[str] = None,
    end_date: Optional[str] = None,
    user: dict = Depends(get_current_user)
):
    query = {"tenant_id": user['tenant_id']}
    if employee_id:
        query["employee_id"] = employee_id
    if business_id:
        query["business_id"] = business_id
    if start_date and end_date:
        query["date"] = {"$gte": start_date, "$lte": end_date}
    
    schedules = await db.am_schedules.find(query, {"_id": 0}).sort("date", 1).to_list(1000)
    return schedules

@api_router.post("/schedules")
async def create_schedule(data: ScheduleCreate, user: dict = Depends(require_manager_or_above)):
    # Can't create schedule for today or past
    schedule_date = datetime.strptime(data.date, '%Y-%m-%d').date()
    if schedule_date <= date.today():
        raise HTTPException(status_code=400, detail="Cannot create schedule for today or past dates")
    
    # Check if schedule exists
    existing = await db.am_schedules.find_one({
        "tenant_id": user['tenant_id'],
        "employee_id": data.employee_id,
        "date": data.date
    })
    
    schedule_data = {
        "tenant_id": user['tenant_id'],
        "employee_id": data.employee_id,
        "business_id": data.business_id,
        "project_id": data.project_id,
        "date": data.date,
        "scheduled_minutes": data.scheduled_minutes,
        "updated_at": datetime.now(timezone.utc).isoformat()
    }
    
    if existing:
        await db.am_schedules.update_one({"id": existing['id']}, {"$set": schedule_data})
        schedule_data['id'] = existing['id']
    else:
        schedule_data['id'] = str(uuid.uuid4())
        schedule_data['created_at'] = datetime.now(timezone.utc).isoformat()
        await db.am_schedules.insert_one(schedule_data)
    
    return {k: v for k, v in schedule_data.items() if k != '_id'}

@api_router.delete("/schedules/{schedule_id}")
async def delete_schedule(schedule_id: str, user: dict = Depends(require_manager_or_above)):
    schedule = await db.am_schedules.find_one({"id": schedule_id, "tenant_id": user['tenant_id']})
    if not schedule:
        raise HTTPException(status_code=404, detail="Schedule not found")
    
    # Can't delete schedule for today or past
    schedule_date = datetime.strptime(schedule['date'], '%Y-%m-%d').date()
    if schedule_date <= date.today():
        raise HTTPException(status_code=400, detail="Cannot delete schedule for today or past dates")
    
    await db.am_schedules.delete_one({"id": schedule_id})
    return {"success": True}

# ===================== REPORTS ROUTES =====================

@api_router.get("/reports/business-hours")
async def get_business_hours_report(
    business_id: str,
    start_date: str,
    end_date: str,
    user: dict = Depends(get_current_user)
):
    # Verify business belongs to tenant
    business = await db.am_businesses.find_one(
        {"id": business_id, "tenant_id": user['tenant_id']}, {"_id": 0}
    )
    if not business:
        raise HTTPException(status_code=404, detail="Business not found")
    
    # Get all approved entries for the date range
    pipeline = [
        {
            "$match": {
                "tenant_id": user['tenant_id'],
                "business_id": business_id,
                "work_date": {"$gte": start_date, "$lte": end_date}
            }
        },
        {
            "$group": {
                "_id": "$employee_id",
                "gross_minutes": {"$sum": "$total_minutes"},
                "break_minutes": {"$sum": "$break_minutes"},
                "net_minutes": {"$sum": "$net_minutes"}
            }
        }
    ]
    
    results = await db.am_timesheet_entries.aggregate(pipeline).to_list(1000)
    
    # Get employee names
    report_data = []
    for r in results:
        emp = await db.am_users.find_one({"id": r['_id']}, {"_id": 0})
        if emp:
            report_data.append({
                "employee_id": r['_id'],
                "employee_name": f"{emp.get('first_name', '')} {emp.get('last_name', '')}".strip(),
                "gross_hours": round(r['gross_minutes'] / 60, 2),
                "break_hours": round(r['break_minutes'] / 60, 2),
                "net_hours": round(r['net_minutes'] / 60, 2)
            })
    
    return {
        "business_name": business.get('company_name'),
        "start_date": start_date,
        "end_date": end_date,
        "data": report_data
    }

@api_router.get("/reports/export/{format}")
async def export_report(
    format: str,
    business_id: str,
    start_date: str,
    end_date: str,
    user: dict = Depends(get_current_user)
):
    report = await get_business_hours_report(business_id, start_date, end_date, user)
    
    if format == 'csv':
        import csv
        from io import StringIO
        output = StringIO()
        writer = csv.writer(output)
        writer.writerow(['Employee Name', 'Gross Hours', 'Break Hours', 'Net Hours'])
        for row in report['data']:
            writer.writerow([row['employee_name'], row['gross_hours'], row['break_hours'], row['net_hours']])
        
        return StreamingResponse(
            iter([output.getvalue()]),
            media_type="text/csv",
            headers={"Content-Disposition": f"attachment; filename=report_{start_date}_{end_date}.csv"}
        )
    
    raise HTTPException(status_code=400, detail="Unsupported format. Use 'csv'")

# ===================== DASHBOARD ROUTES =====================

@api_router.get("/dashboard/stats")
async def get_dashboard_stats(business_id: Optional[str] = None, user: dict = Depends(get_current_user)):
    query = {"tenant_id": user['tenant_id']}
    if business_id:
        query["business_id"] = business_id
    
    # Count employees
    emp_query = {"tenant_id": user['tenant_id'], "role": {"$in": ["employee", "manager"]}}
    if business_id:
        emp_query["business_id"] = business_id
    employee_count = await db.am_users.count_documents(emp_query)
    
    # Get total hours this week
    today = date.today()
    week_start = get_week_start(today)
    week_end = week_start + timedelta(days=6)
    
    ts_query = {
        "tenant_id": user['tenant_id'],
        "work_date": {"$gte": week_start.isoformat(), "$lte": week_end.isoformat()}
    }
    if business_id:
        ts_query["business_id"] = business_id
    
    pipeline = [
        {"$match": ts_query},
        {"$group": {"_id": None, "total_minutes": {"$sum": "$net_minutes"}}}
    ]
    
    result = await db.am_timesheet_entries.aggregate(pipeline).to_list(1)
    total_hours = round(result[0]['total_minutes'] / 60, 2) if result else 0
    
    return {
        "employee_count": employee_count,
        "total_working_hours": total_hours,
        "week_start": week_start.isoformat(),
        "week_end": week_end.isoformat()
    }

# ===================== SETTINGS ROUTES =====================

@api_router.get("/settings")
async def get_settings(user: dict = Depends(require_subscriber)):
    tenant = await db.am_tenants.find_one({"id": user['tenant_id']}, {"_id": 0})
    return {
        "tenant": tenant,
        "payroll_api_key": X_PAYROLL_KEY[:8] + "..." + X_PAYROLL_KEY[-4:],  # Masked
        "payroll_api_key_full": X_PAYROLL_KEY  # For copying
    }

# ===================== PAYROLL INTEGRATION API =====================
# These endpoints are called by Payroll Canada

@api_router.get("/payroll/employees")
async def payroll_get_employees(
    tenantId: str,
    businessId: str,
    _: bool = Depends(verify_payroll_key)
):
    """Get employees for payroll integration"""
    employees = await db.am_users.find(
        {
            "tenant_id": tenantId,
            "business_id": businessId,
            "role": {"$in": ["employee", "manager"]},
            "active": True
        },
        {"_id": 0, "password_hash": 0}
    ).to_list(1000)
    
    return [
        {
            "employeeKey": emp['id'],
            "fullName": f"{emp.get('first_name', '')} {emp.get('last_name', '')}".strip(),
            "email": emp.get('email'),
            "active": emp.get('active', True),
            "role": emp.get('role')
        }
        for emp in employees
    ]

@api_router.get("/payroll/approved-entries")
async def payroll_get_approved_entries(
    tenantId: str,
    businessId: str,
    start: str,
    end: str,
    _: bool = Depends(verify_payroll_key)
):
    """Get approved timesheet entries for payroll integration"""
    entries = await db.am_timesheet_entries.find(
        {
            "tenant_id": tenantId,
            "business_id": businessId,
            "work_date": {"$gte": start, "$lte": end},
            "status": "approved",
            "exported_to_payroll": {"$ne": True}
        },
        {"_id": 0}
    ).to_list(10000)
    
    # Get project names
    project_ids = list(set(e.get('project_id') for e in entries if e.get('project_id')))
    projects = {}
    if project_ids:
        proj_list = await db.am_projects.find({"id": {"$in": project_ids}}, {"_id": 0}).to_list(100)
        projects = {p['id']: p.get('project_name') for p in proj_list}
    
    # Get employee emails
    emp_ids = list(set(e.get('employee_id') for e in entries))
    emp_list = await db.am_users.find({"id": {"$in": emp_ids}}, {"_id": 0}).to_list(1000)
    emp_map = {e['id']: e for e in emp_list}
    
    return [
        {
            "employeeKey": e['employee_id'],
            "email": emp_map.get(e['employee_id'], {}).get('email'),
            "workDate": e['work_date'],
            "regularHours": round(e.get('net_minutes', 0) / 60, 2),
            "overtimeHours": 0,  # Phase 1: always 0
            "breakMinutes": e.get('break_minutes', 0),
            "netHours": round(e.get('net_minutes', 0) / 60, 2),
            "entryId": e['id'],
            "approvedAt": e.get('approved_at'),
            "projectId": e.get('project_id'),
            "projectName": projects.get(e.get('project_id'))
        }
        for e in entries
    ]

@api_router.post("/payroll/lock")
async def payroll_lock_entries(
    tenantId: str = None,
    businessId: str = None,
    start: str = None,
    end: str = None,
    employeeKeys: List[str] = None,
    payrollRunId: str = None,
    _: bool = Depends(verify_payroll_key)
):
    """Lock timesheet entries after payroll export"""
    if not all([tenantId, businessId, start, end, employeeKeys, payrollRunId]):
        raise HTTPException(status_code=400, detail="Missing required fields")
    
    result = await db.am_timesheet_entries.update_many(
        {
            "tenant_id": tenantId,
            "business_id": businessId,
            "work_date": {"$gte": start, "$lte": end},
            "employee_id": {"$in": employeeKeys},
            "status": "approved"
        },
        {
            "$set": {
                "exported_to_payroll": True,
                "exported_at": datetime.now(timezone.utc).isoformat(),
                "exported_payroll_run_id": payrollRunId
            }
        }
    )
    
    return {"locked": result.modified_count, "payrollRunId": payrollRunId}

# ===================== TEST CHECKLIST =====================

@api_router.get("/test-checklist")
async def get_test_checklist(user: dict = Depends(require_subscriber)):
    """Get test checklist status"""
    tenant_id = user['tenant_id']
    
    # Check each step
    has_business = await db.am_businesses.count_documents({"tenant_id": tenant_id}) > 0
    has_employee = await db.am_users.count_documents({"tenant_id": tenant_id, "role": "employee"}) > 0
    has_timesheet = await db.am_timesheet_entries.count_documents({"tenant_id": tenant_id}) > 0
    has_approved = await db.am_timesheet_entries.count_documents({"tenant_id": tenant_id, "status": "approved"}) > 0
    has_exported = await db.am_timesheet_entries.count_documents({"tenant_id": tenant_id, "exported_to_payroll": True}) > 0
    
    return {
        "checklist": [
            {"step": "Create Business", "completed": has_business},
            {"step": "Create Employee", "completed": has_employee},
            {"step": "Enter Timesheet Hours + Break", "completed": has_timesheet},
            {"step": "Approve Timesheet", "completed": has_approved},
            {"step": "Call Payroll API & See Entry", "completed": has_approved, "note": "Use /api/payroll/approved-entries"},
            {"step": "Lock Payroll & Confirm Export", "completed": has_exported}
        ],
        "payroll_api_urls": {
            "employees": "/api/payroll/employees?tenantId={tenantId}&businessId={businessId}",
            "approved_entries": "/api/payroll/approved-entries?tenantId={tenantId}&businessId={businessId}&start=YYYY-MM-DD&end=YYYY-MM-DD",
            "lock": "POST /api/payroll/lock"
        },
        "tenant_id": tenant_id
    }

# Include the router
app.include_router(api_router)

app.add_middleware(
    CORSMiddleware,
    allow_credentials=True,
    allow_origins=["*"],
    allow_methods=["*"],
    allow_headers=["*"],
)

@app.on_event("shutdown")
async def shutdown_db_client():
    client.close()

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8002)
