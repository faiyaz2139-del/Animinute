from fastapi import FastAPI, APIRouter, Depends, HTTPException, status
from fastapi.responses import StreamingResponse, JSONResponse
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
import json
from io import BytesIO
import hashlib

# Import modules
from modules.timesheet_connector import TimesheetConnector, TimesheetConnectorError
from modules.payroll_engine import PayrollEngine
from modules.pdf_service import PDFService
from modules.csv_service import CSVService

# Import Any Minute routes
from any_minute.routes import am_router, init_am_db

ROOT_DIR = Path(__file__).parent
load_dotenv(ROOT_DIR / '.env')

# MongoDB connection
mongo_url = os.environ['MONGO_URL']
client = AsyncIOMotorClient(mongo_url)
db = client[os.environ['DB_NAME']]

# Initialize Any Minute database reference
init_am_db(db)

# JWT Config
JWT_SECRET = os.environ.get('JWT_SECRET', 'payroll-canada-secret-key-2024')
JWT_ALGORITHM = 'HS256'
JWT_EXPIRATION_HOURS = 24

# Create the main app
app = FastAPI(title="Payroll Canada API")
api_router = APIRouter(prefix="/api")
security = HTTPBearer()

# Root API endpoint
@api_router.get("/")
async def root():
    return {"message": "Payroll Canada API", "status": "healthy"}

# Configure logging
logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(name)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

# ===================== MODELS =====================

class UserCreate(BaseModel):
    email: EmailStr
    password: str
    role: str = "employee"
    first_name: str
    last_name: str

class UserLogin(BaseModel):
    email: EmailStr
    password: str

class UserResponse(BaseModel):
    model_config = ConfigDict(extra="ignore")
    id: str
    company_id: Optional[str] = None
    employee_id: Optional[str] = None
    email: str
    role: str
    first_name: str
    last_name: str
    created_at: str

class CompanyCreate(BaseModel):
    name: str
    province: str = "ON"
    pay_frequency: str = "biweekly"
    pay_day_rule: str = "friday"
    default_hourly_rate: float = 20.0
    vacation_pay_percent_default: float = 4.0

class CompanyUpdate(BaseModel):
    name: Optional[str] = None
    province: Optional[str] = None
    pay_frequency: Optional[str] = None
    pay_day_rule: Optional[str] = None
    default_hourly_rate: Optional[float] = None
    vacation_pay_percent_default: Optional[float] = None
    settings_json: Optional[Dict[str, Any]] = None

class EmployeeCreate(BaseModel):
    external_employee_key: Optional[str] = None
    first_name: str
    last_name: str
    email: EmailStr
    employment_type: str = "hourly"
    hourly_rate: Optional[float] = None
    annual_salary: Optional[float] = None

class EmployeeUpdate(BaseModel):
    external_employee_key: Optional[str] = None
    first_name: Optional[str] = None
    last_name: Optional[str] = None
    email: Optional[EmailStr] = None
    employment_type: Optional[str] = None
    hourly_rate: Optional[float] = None
    annual_salary: Optional[float] = None
    active: Optional[bool] = None

class PayPeriodCreate(BaseModel):
    start_date: str
    end_date: str
    pay_date: str

class TimesheetImportRequest(BaseModel):
    pay_period_id: str

class PayrollRunCreate(BaseModel):
    pay_period_id: str

class EmployeeMappingUpdate(BaseModel):
    external_employee_key: str
    employee_id: str

class TimesheetConfigUpdate(BaseModel):
    api_base_url: Optional[str] = None
    auth_method: Optional[str] = None
    api_key_header_name: Optional[str] = None
    api_key_value: Optional[str] = None
    bearer_token: Optional[str] = None
    use_mock: Optional[bool] = None

# ===================== AUTH HELPERS =====================

def hash_password(password: str) -> str:
    return bcrypt.hashpw(password.encode('utf-8'), bcrypt.gensalt()).decode('utf-8')

def verify_password(password: str, hashed: str) -> bool:
    return bcrypt.checkpw(password.encode('utf-8'), hashed.encode('utf-8'))

def create_token(user_id: str, company_id: str, role: str) -> str:
    payload = {
        'user_id': user_id,
        'company_id': company_id,
        'role': role,
        'exp': datetime.now(timezone.utc) + timedelta(hours=JWT_EXPIRATION_HOURS)
    }
    return jwt.encode(payload, JWT_SECRET, algorithm=JWT_ALGORITHM)

async def get_current_user(credentials: HTTPAuthorizationCredentials = Depends(security)):
    try:
        payload = jwt.decode(credentials.credentials, JWT_SECRET, algorithms=[JWT_ALGORITHM])
        user = await db.users.find_one({"id": payload['user_id']}, {"_id": 0})
        if not user:
            raise HTTPException(status_code=401, detail="User not found")
        return user
    except jwt.ExpiredSignatureError:
        raise HTTPException(status_code=401, detail="Token expired")
    except jwt.InvalidTokenError:
        raise HTTPException(status_code=401, detail="Invalid token")

async def require_admin(user: dict = Depends(get_current_user)):
    if user['role'] not in ['admin', 'manager']:
        raise HTTPException(status_code=403, detail="Admin access required")
    return user

async def log_audit(company_id: str, user_id: str, action: str, entity_type: str, entity_id: str, metadata: dict = None):
    audit_entry = {
        "id": str(uuid.uuid4()),
        "company_id": company_id,
        "actor_user_id": user_id,
        "action": action,
        "entity_type": entity_type,
        "entity_id": entity_id,
        "timestamp": datetime.now(timezone.utc).isoformat(),
        "metadata_json": metadata or {}
    }
    await db.audit_logs.insert_one(audit_entry)

# ===================== AUTH ROUTES =====================

@api_router.post("/auth/register")
async def register(data: UserCreate):
    existing = await db.users.find_one({"email": data.email.lower()})
    if existing:
        raise HTTPException(status_code=400, detail="Email already registered")
    
    user_id = str(uuid.uuid4())
    company_id = None
    
    # If first admin, create company
    if data.role == "admin":
        company_id = str(uuid.uuid4())
        company = {
            "id": company_id,
            "name": "My Company",
            "province": "ON",
            "pay_frequency": "biweekly",
            "pay_day_rule": "friday",
            "default_hourly_rate": 20.0,
            "vacation_pay_percent_default": 4.0,
            "settings_json": get_default_settings(),
            "timesheet_config": {
                "api_base_url": "https://YOUR-TIMESHEET-DOMAIN.COM",
                "auth_method": "API_KEY",
                "api_key_header_name": "X-API-KEY",
                "api_key_value": "",
                "bearer_token": "",
                "use_mock": True
            },
            "created_at": datetime.now(timezone.utc).isoformat()
        }
        await db.companies.insert_one(company)
    
    user = {
        "id": user_id,
        "company_id": company_id,
        "employee_id": None,
        "email": data.email.lower(),
        "password_hash": hash_password(data.password),
        "role": data.role,
        "first_name": data.first_name,
        "last_name": data.last_name,
        "created_at": datetime.now(timezone.utc).isoformat()
    }
    await db.users.insert_one(user)
    
    token = create_token(user_id, company_id, data.role)
    return {"token": token, "user": {k: v for k, v in user.items() if k != 'password_hash'}}

@api_router.post("/auth/login")
async def login(data: UserLogin):
    user = await db.users.find_one({"email": data.email.lower()}, {"_id": 0})
    if not user or not verify_password(data.password, user['password_hash']):
        raise HTTPException(status_code=401, detail="Invalid credentials")
    
    token = create_token(user['id'], user.get('company_id'), user['role'])
    return {"token": token, "user": {k: v for k, v in user.items() if k != 'password_hash'}}

@api_router.get("/auth/me")
async def get_me(user: dict = Depends(get_current_user)):
    return {k: v for k, v in user.items() if k != 'password_hash'}

# ===================== COMPANY ROUTES =====================

def get_default_settings():
    return {
        "cpp": {
            "rate": 5.95,
            "basic_exemption_annual": 3500,
            "max_annual": 3867.50
        },
        "ei": {
            "rate": 1.63,
            "max_annual": 1049.12
        },
        "federal_tax_brackets": [
            {"min": 0, "max": 55867, "rate": 15},
            {"min": 55867, "max": 111733, "rate": 20.5},
            {"min": 111733, "max": 173205, "rate": 26},
            {"min": 173205, "max": 246752, "rate": 29},
            {"min": 246752, "max": 999999999, "rate": 33}
        ],
        "ontario_tax_brackets": [
            {"min": 0, "max": 51446, "rate": 5.05},
            {"min": 51446, "max": 102894, "rate": 9.15},
            {"min": 102894, "max": 150000, "rate": 11.16},
            {"min": 150000, "max": 220000, "rate": 12.16},
            {"min": 220000, "max": 999999999, "rate": 13.16}
        ]
    }

@api_router.get("/company")
async def get_company(user: dict = Depends(get_current_user)):
    company = await db.companies.find_one({"id": user['company_id']}, {"_id": 0})
    if not company:
        raise HTTPException(status_code=404, detail="Company not found")
    return company

@api_router.put("/company")
async def update_company(data: CompanyUpdate, user: dict = Depends(require_admin)):
    update_data = {k: v for k, v in data.model_dump().items() if v is not None}
    if not update_data:
        raise HTTPException(status_code=400, detail="No data to update")
    
    await db.companies.update_one({"id": user['company_id']}, {"$set": update_data})
    await log_audit(user['company_id'], user['id'], "UPDATE", "company", user['company_id'], update_data)
    
    company = await db.companies.find_one({"id": user['company_id']}, {"_id": 0})
    return company

@api_router.put("/company/timesheet-config")
async def update_timesheet_config(data: TimesheetConfigUpdate, user: dict = Depends(require_admin)):
    update_data = {f"timesheet_config.{k}": v for k, v in data.model_dump().items() if v is not None}
    if not update_data:
        raise HTTPException(status_code=400, detail="No data to update")
    
    await db.companies.update_one({"id": user['company_id']}, {"$set": update_data})
    await log_audit(user['company_id'], user['id'], "UPDATE", "timesheet_config", user['company_id'])
    
    company = await db.companies.find_one({"id": user['company_id']}, {"_id": 0})
    return company

# ===================== EMPLOYEE ROUTES =====================

@api_router.get("/employees")
async def get_employees(user: dict = Depends(get_current_user)):
    employees = await db.employees.find({"company_id": user['company_id']}, {"_id": 0}).to_list(1000)
    return employees

@api_router.post("/employees")
async def create_employee(data: EmployeeCreate, user: dict = Depends(require_admin)):
    company = await db.companies.find_one({"id": user['company_id']}, {"_id": 0})
    
    employee_id = str(uuid.uuid4())
    employee = {
        "id": employee_id,
        "company_id": user['company_id'],
        "external_employee_key": data.external_employee_key,
        "first_name": data.first_name,
        "last_name": data.last_name,
        "email": data.email.lower(),
        "employment_type": data.employment_type,
        "hourly_rate": data.hourly_rate or company.get('default_hourly_rate', 20.0),
        "annual_salary": data.annual_salary,
        "active": True,
        "created_at": datetime.now(timezone.utc).isoformat()
    }
    await db.employees.insert_one(employee)
    await log_audit(user['company_id'], user['id'], "CREATE", "employee", employee_id)
    return {k: v for k, v in employee.items() if k != '_id'}

@api_router.put("/employees/{employee_id}")
async def update_employee(employee_id: str, data: EmployeeUpdate, user: dict = Depends(require_admin)):
    update_data = {k: v for k, v in data.model_dump().items() if v is not None}
    if not update_data:
        raise HTTPException(status_code=400, detail="No data to update")
    
    result = await db.employees.update_one(
        {"id": employee_id, "company_id": user['company_id']},
        {"$set": update_data}
    )
    if result.matched_count == 0:
        raise HTTPException(status_code=404, detail="Employee not found")
    
    await log_audit(user['company_id'], user['id'], "UPDATE", "employee", employee_id, update_data)
    employee = await db.employees.find_one({"id": employee_id}, {"_id": 0})
    return employee

@api_router.delete("/employees/{employee_id}")
async def delete_employee(employee_id: str, user: dict = Depends(require_admin)):
    result = await db.employees.delete_one({"id": employee_id, "company_id": user['company_id']})
    if result.deleted_count == 0:
        raise HTTPException(status_code=404, detail="Employee not found")
    await log_audit(user['company_id'], user['id'], "DELETE", "employee", employee_id)
    return {"success": True}

@api_router.put("/employees/mapping")
async def update_employee_mapping(data: EmployeeMappingUpdate, user: dict = Depends(require_admin)):
    result = await db.employees.update_one(
        {"id": data.employee_id, "company_id": user['company_id']},
        {"$set": {"external_employee_key": data.external_employee_key}}
    )
    if result.matched_count == 0:
        raise HTTPException(status_code=404, detail="Employee not found")
    await log_audit(user['company_id'], user['id'], "MAP", "employee", data.employee_id, {"external_key": data.external_employee_key})
    return {"success": True}

# ===================== PAY PERIOD ROUTES =====================

@api_router.get("/pay-periods")
async def get_pay_periods(user: dict = Depends(get_current_user)):
    periods = await db.pay_periods.find({"company_id": user['company_id']}, {"_id": 0}).sort("start_date", -1).to_list(100)
    return periods

@api_router.post("/pay-periods")
async def create_pay_period(data: PayPeriodCreate, user: dict = Depends(require_admin)):
    period_id = str(uuid.uuid4())
    period = {
        "id": period_id,
        "company_id": user['company_id'],
        "start_date": data.start_date,
        "end_date": data.end_date,
        "pay_date": data.pay_date,
        "status": "open",
        "created_at": datetime.now(timezone.utc).isoformat()
    }
    await db.pay_periods.insert_one(period)
    await log_audit(user['company_id'], user['id'], "CREATE", "pay_period", period_id)
    # Return without _id
    return {k: v for k, v in period.items() if k != '_id'}

@api_router.get("/pay-periods/{period_id}")
async def get_pay_period(period_id: str, user: dict = Depends(get_current_user)):
    period = await db.pay_periods.find_one({"id": period_id, "company_id": user['company_id']}, {"_id": 0})
    if not period:
        raise HTTPException(status_code=404, detail="Pay period not found")
    return period

# ===================== TIMESHEET IMPORT ROUTES =====================

@api_router.post("/timesheets/preview")
async def preview_timesheets(data: TimesheetImportRequest, user: dict = Depends(require_admin)):
    company = await db.companies.find_one({"id": user['company_id']}, {"_id": 0})
    period = await db.pay_periods.find_one({"id": data.pay_period_id, "company_id": user['company_id']}, {"_id": 0})
    
    if not period:
        raise HTTPException(status_code=404, detail="Pay period not found")
    
    try:
        connector = TimesheetConnector(company.get('timesheet_config', {}))
        entries = await connector.fetch_approved_entries(period['start_date'], period['end_date'])
    except TimesheetConnectorError as e:
        raise HTTPException(status_code=400, detail=str(e))
    
    # Get employees for matching
    employees = await db.employees.find({"company_id": user['company_id']}, {"_id": 0}).to_list(1000)
    
    # Match entries to employees
    matched_entries = []
    unmatched_entries = []
    
    for entry in entries:
        matched_employee = None
        ext_key = entry.get('employee_key')
        ext_email = entry.get('employee_email', '').lower()
        
        for emp in employees:
            if emp.get('external_employee_key') == ext_key:
                matched_employee = emp
                break
            if emp.get('email', '').lower() == ext_email:
                matched_employee = emp
                break
        
        if matched_employee:
            entry['matched_employee'] = matched_employee
            matched_entries.append(entry)
        else:
            unmatched_entries.append(entry)
    
    return {
        "matched_entries": matched_entries,
        "unmatched_entries": unmatched_entries,
        "total_entries": len(entries)
    }

@api_router.post("/timesheets/import")
async def import_timesheets(data: TimesheetImportRequest, user: dict = Depends(require_admin)):
    company = await db.companies.find_one({"id": user['company_id']}, {"_id": 0})
    period = await db.pay_periods.find_one({"id": data.pay_period_id, "company_id": user['company_id']}, {"_id": 0})
    
    if not period:
        raise HTTPException(status_code=404, detail="Pay period not found")
    
    if period['status'] == 'locked':
        raise HTTPException(status_code=400, detail="Pay period is locked")
    
    try:
        connector = TimesheetConnector(company.get('timesheet_config', {}))
        entries = await connector.fetch_approved_entries(period['start_date'], period['end_date'])
    except TimesheetConnectorError as e:
        raise HTTPException(status_code=400, detail=str(e))
    
    # Get employees for matching
    employees = await db.employees.find({"company_id": user['company_id']}, {"_id": 0}).to_list(1000)
    employees_map = {}
    for emp in employees:
        if emp.get('external_employee_key'):
            employees_map[emp['external_employee_key']] = emp
        if emp.get('email'):
            employees_map[emp['email'].lower()] = emp
    
    # Create import record - determine source from environment
    use_mock = os.environ.get('USE_MOCK', 'false').lower() == 'true'
    source_hash = hashlib.md5(json.dumps(entries, sort_keys=True, default=str).encode()).hexdigest()
    import_record = {
        "id": str(uuid.uuid4()),
        "company_id": user['company_id'],
        "pay_period_id": data.pay_period_id,
        "imported_at": datetime.now(timezone.utc).isoformat(),
        "source": "mock" if use_mock else "anyminute",
        "source_hash": source_hash,
        "status": "imported",
        "notes": f"Imported {len(entries)} entries"
    }
    await db.timesheet_imports.insert_one(import_record)
    
    # Delete existing entries for this period
    await db.time_entries.delete_many({"pay_period_id": data.pay_period_id, "company_id": user['company_id']})
    
    # Create time entries
    imported_count = 0
    for entry in entries:
        ext_key = entry.get('employee_key')
        ext_email = entry.get('employee_email', '').lower()
        
        matched_emp = employees_map.get(ext_key) or employees_map.get(ext_email)
        if matched_emp:
            time_entry = {
                "id": str(uuid.uuid4()),
                "company_id": user['company_id'],
                "pay_period_id": data.pay_period_id,
                "employee_id": matched_emp['id'],
                "work_date": entry.get('work_date'),
                "regular_hours": entry.get('regular_hours', 0),
                "overtime_hours": entry.get('overtime_hours', 0),
                "source_ref": entry.get('source_ref'),
                "notes": entry.get('notes', '')
            }
            await db.time_entries.insert_one(time_entry)
            imported_count += 1
    
    await log_audit(user['company_id'], user['id'], "IMPORT", "timesheet", import_record['id'], {"count": imported_count})
    
    return {"success": True, "imported_count": imported_count, "import_id": import_record['id']}

@api_router.get("/timesheets/entries/{period_id}")
async def get_time_entries(period_id: str, user: dict = Depends(get_current_user)):
    entries = await db.time_entries.find(
        {"pay_period_id": period_id, "company_id": user['company_id']},
        {"_id": 0}
    ).to_list(1000)
    return entries

# ===================== PAYROLL RUN ROUTES =====================

@api_router.get("/payroll-runs")
async def get_payroll_runs(user: dict = Depends(get_current_user)):
    runs = await db.payroll_runs.find({"company_id": user['company_id']}, {"_id": 0}).sort("created_at", -1).to_list(100)
    return runs

@api_router.post("/payroll-runs")
async def create_payroll_run(data: PayrollRunCreate, user: dict = Depends(require_admin)):
    period = await db.pay_periods.find_one({"id": data.pay_period_id, "company_id": user['company_id']}, {"_id": 0})
    if not period:
        raise HTTPException(status_code=404, detail="Pay period not found")
    
    # Count existing runs for this period
    existing_runs = await db.payroll_runs.count_documents({"pay_period_id": data.pay_period_id})
    
    run_id = str(uuid.uuid4())
    run = {
        "id": run_id,
        "company_id": user['company_id'],
        "pay_period_id": data.pay_period_id,
        "run_number": existing_runs + 1,
        "status": "draft",
        "calculated_at": None,
        "locked_at": None,
        "created_at": datetime.now(timezone.utc).isoformat()
    }
    await db.payroll_runs.insert_one(run)
    await log_audit(user['company_id'], user['id'], "CREATE", "payroll_run", run_id)
    return {k: v for k, v in run.items() if k != '_id'}

@api_router.post("/payroll-runs/{run_id}/calculate")
async def calculate_payroll(run_id: str, user: dict = Depends(require_admin)):
    run = await db.payroll_runs.find_one({"id": run_id, "company_id": user['company_id']}, {"_id": 0})
    if not run:
        raise HTTPException(status_code=404, detail="Payroll run not found")
    
    if run['status'] == 'locked':
        raise HTTPException(status_code=400, detail="Payroll run is locked")
    
    company = await db.companies.find_one({"id": user['company_id']}, {"_id": 0})
    period = await db.pay_periods.find_one({"id": run['pay_period_id']}, {"_id": 0})
    
    # Get employees and time entries
    employees = await db.employees.find({"company_id": user['company_id'], "active": True}, {"_id": 0}).to_list(1000)
    time_entries = await db.time_entries.find({"pay_period_id": run['pay_period_id'], "company_id": user['company_id']}, {"_id": 0}).to_list(1000)
    
    # Group time entries by employee
    entries_by_employee = {}
    for entry in time_entries:
        emp_id = entry['employee_id']
        if emp_id not in entries_by_employee:
            entries_by_employee[emp_id] = []
        entries_by_employee[emp_id].append(entry)
    
    # Calculate payroll lines
    engine = PayrollEngine(company)
    
    # Delete existing lines for this run
    await db.payroll_lines.delete_many({"payroll_run_id": run_id})
    
    payroll_lines = []
    for employee in employees:
        entries = entries_by_employee.get(employee['id'], [])
        
        # Get YTD totals
        ytd = await get_ytd_totals(user['company_id'], employee['id'], period['start_date'])
        
        line = engine.calculate_employee_pay(employee, entries, ytd)
        line['id'] = str(uuid.uuid4())
        line['payroll_run_id'] = run_id
        line['employee_id'] = employee['id']
        
        await db.payroll_lines.insert_one(line)
        payroll_lines.append(line)
    
    # Update run status
    await db.payroll_runs.update_one(
        {"id": run_id},
        {"$set": {"status": "calculated", "calculated_at": datetime.now(timezone.utc).isoformat()}}
    )
    
    await log_audit(user['company_id'], user['id'], "CALCULATE", "payroll_run", run_id, {"lines": len(payroll_lines)})
    
    return {"success": True, "lines_count": len(payroll_lines)}

async def get_ytd_totals(company_id: str, employee_id: str, before_date: str) -> dict:
    """Get year-to-date totals for an employee"""
    year_start = f"{before_date[:4]}-01-01"
    
    pipeline = [
        {"$match": {"company_id": company_id}},
        {"$lookup": {
            "from": "payroll_runs",
            "localField": "pay_period_id",
            "foreignField": "id",
            "as": "run"
        }},
        {"$unwind": "$run"},
        {"$match": {
            "run.status": "locked",
            "start_date": {"$gte": year_start, "$lt": before_date}
        }},
        {"$lookup": {
            "from": "payroll_lines",
            "localField": "run.id",
            "foreignField": "payroll_run_id",
            "as": "lines"
        }},
        {"$unwind": "$lines"},
        {"$match": {"lines.employee_id": employee_id}},
        {"$group": {
            "_id": None,
            "ytd_gross": {"$sum": "$lines.gross_pay"},
            "ytd_cpp": {"$sum": "$lines.cpp"},
            "ytd_ei": {"$sum": "$lines.ei"},
            "ytd_tax": {"$sum": {"$add": ["$lines.federal_tax", "$lines.provincial_tax"]}}
        }}
    ]
    
    result = await db.pay_periods.aggregate(pipeline).to_list(1)
    if result:
        return result[0]
    return {"ytd_gross": 0, "ytd_cpp": 0, "ytd_ei": 0, "ytd_tax": 0}

@api_router.get("/payroll-runs/{run_id}/lines")
async def get_payroll_lines(run_id: str, user: dict = Depends(get_current_user)):
    lines = await db.payroll_lines.find({"payroll_run_id": run_id}, {"_id": 0}).to_list(1000)
    
    # Add employee info
    employees = await db.employees.find({"company_id": user['company_id']}, {"_id": 0}).to_list(1000)
    emp_map = {e['id']: e for e in employees}
    
    for line in lines:
        emp = emp_map.get(line['employee_id'], {})
        line['employee_name'] = f"{emp.get('first_name', '')} {emp.get('last_name', '')}".strip()
        line['employee_email'] = emp.get('email', '')
    
    return lines

@api_router.post("/payroll-runs/{run_id}/lock")
async def lock_payroll_run(run_id: str, user: dict = Depends(require_admin)):
    run = await db.payroll_runs.find_one({"id": run_id, "company_id": user['company_id']}, {"_id": 0})
    if not run:
        raise HTTPException(status_code=404, detail="Payroll run not found")
    
    if run['status'] != 'calculated':
        raise HTTPException(status_code=400, detail="Payroll must be calculated before locking")
    
    # Lock the run
    await db.payroll_runs.update_one(
        {"id": run_id},
        {"$set": {"status": "locked", "locked_at": datetime.now(timezone.utc).isoformat()}}
    )
    
    # Lock the pay period
    await db.pay_periods.update_one(
        {"id": run['pay_period_id']},
        {"$set": {"status": "locked"}}
    )
    
    await log_audit(user['company_id'], user['id'], "LOCK", "payroll_run", run_id)
    
    return {"success": True}

# ===================== PAYSLIP ROUTES =====================

@api_router.get("/payslips")
async def get_payslips(user: dict = Depends(get_current_user)):
    if user['role'] == 'employee':
        # Employee can only see their own
        payslips = await db.payslips.find(
            {"company_id": user['company_id'], "employee_id": user.get('employee_id')},
            {"_id": 0}
        ).to_list(100)
    else:
        payslips = await db.payslips.find({"company_id": user['company_id']}, {"_id": 0}).to_list(1000)
    return payslips

@api_router.post("/payroll-runs/{run_id}/generate-payslips")
async def generate_payslips(run_id: str, user: dict = Depends(require_admin)):
    run = await db.payroll_runs.find_one({"id": run_id, "company_id": user['company_id']}, {"_id": 0})
    if not run:
        raise HTTPException(status_code=404, detail="Payroll run not found")
    
    if run['status'] != 'locked':
        raise HTTPException(status_code=400, detail="Payroll must be locked before generating payslips")
    
    company = await db.companies.find_one({"id": user['company_id']}, {"_id": 0})
    period = await db.pay_periods.find_one({"id": run['pay_period_id']}, {"_id": 0})
    lines = await db.payroll_lines.find({"payroll_run_id": run_id}, {"_id": 0}).to_list(1000)
    employees = await db.employees.find({"company_id": user['company_id']}, {"_id": 0}).to_list(1000)
    emp_map = {e['id']: e for e in employees}
    
    # Delete existing payslips for this run
    await db.payslips.delete_many({"payroll_run_id": run_id})
    
    payslips = []
    for line in lines:
        employee = emp_map.get(line['employee_id'], {})
        payslip = {
            "id": str(uuid.uuid4()),
            "company_id": user['company_id'],
            "payroll_run_id": run_id,
            "employee_id": line['employee_id'],
            "employee_name": f"{employee.get('first_name', '')} {employee.get('last_name', '')}".strip(),
            "created_at": datetime.now(timezone.utc).isoformat()
        }
        await db.payslips.insert_one(payslip)
        payslips.append(payslip)
    
    await log_audit(user['company_id'], user['id'], "GENERATE", "payslips", run_id, {"count": len(payslips)})
    
    return {"success": True, "count": len(payslips)}

@api_router.get("/payslips/{payslip_id}/pdf")
async def download_payslip_pdf(payslip_id: str, user: dict = Depends(get_current_user)):
    payslip = await db.payslips.find_one({"id": payslip_id, "company_id": user['company_id']}, {"_id": 0})
    if not payslip:
        raise HTTPException(status_code=404, detail="Payslip not found")
    
    # Check access for employees
    if user['role'] == 'employee' and payslip['employee_id'] != user.get('employee_id'):
        raise HTTPException(status_code=403, detail="Access denied")
    
    company = await db.companies.find_one({"id": user['company_id']}, {"_id": 0})
    run = await db.payroll_runs.find_one({"id": payslip['payroll_run_id']}, {"_id": 0})
    period = await db.pay_periods.find_one({"id": run['pay_period_id']}, {"_id": 0})
    line = await db.payroll_lines.find_one({"payroll_run_id": payslip['payroll_run_id'], "employee_id": payslip['employee_id']}, {"_id": 0})
    employee = await db.employees.find_one({"id": payslip['employee_id']}, {"_id": 0})
    
    # Validate all required data exists
    if not company:
        raise HTTPException(status_code=404, detail="Company not found")
    if not run:
        raise HTTPException(status_code=404, detail="Payroll run not found")
    if not period:
        raise HTTPException(status_code=404, detail="Pay period not found")
    if not line:
        raise HTTPException(status_code=404, detail="Payroll line not found")
    if not employee:
        raise HTTPException(status_code=404, detail="Employee not found")
    
    pdf_service = PDFService()
    pdf_bytes = pdf_service.generate_payslip(company, employee, period, line)
    
    return StreamingResponse(
        BytesIO(pdf_bytes),
        media_type="application/pdf",
        headers={"Content-Disposition": f"attachment; filename=payslip_{payslip_id}.pdf"}
    )

# ===================== REPORTS ROUTES =====================

@api_router.get("/reports/payroll-summary/{run_id}")
async def get_payroll_summary_csv(run_id: str, user: dict = Depends(require_admin)):
    run = await db.payroll_runs.find_one({"id": run_id, "company_id": user['company_id']}, {"_id": 0})
    if not run:
        raise HTTPException(status_code=404, detail="Payroll run not found")
    
    lines = await db.payroll_lines.find({"payroll_run_id": run_id}, {"_id": 0}).to_list(1000)
    employees = await db.employees.find({"company_id": user['company_id']}, {"_id": 0}).to_list(1000)
    emp_map = {e['id']: e for e in employees}
    
    for line in lines:
        emp = emp_map.get(line['employee_id'], {})
        line['employee_name'] = f"{emp.get('first_name', '')} {emp.get('last_name', '')}".strip()
        line['employee_email'] = emp.get('email', '')
    
    csv_service = CSVService()
    csv_content = csv_service.generate_payroll_summary(lines)
    
    return StreamingResponse(
        BytesIO(csv_content.encode()),
        media_type="text/csv",
        headers={"Content-Disposition": f"attachment; filename=payroll_summary_{run_id}.csv"}
    )

@api_router.get("/reports/deductions-summary/{run_id}")
async def get_deductions_summary_csv(run_id: str, user: dict = Depends(require_admin)):
    run = await db.payroll_runs.find_one({"id": run_id, "company_id": user['company_id']}, {"_id": 0})
    if not run:
        raise HTTPException(status_code=404, detail="Payroll run not found")
    
    lines = await db.payroll_lines.find({"payroll_run_id": run_id}, {"_id": 0}).to_list(1000)
    employees = await db.employees.find({"company_id": user['company_id']}, {"_id": 0}).to_list(1000)
    emp_map = {e['id']: e for e in employees}
    
    for line in lines:
        emp = emp_map.get(line['employee_id'], {})
        line['employee_name'] = f"{emp.get('first_name', '')} {emp.get('last_name', '')}".strip()
    
    csv_service = CSVService()
    csv_content = csv_service.generate_deductions_summary(lines)
    
    return StreamingResponse(
        BytesIO(csv_content.encode()),
        media_type="text/csv",
        headers={"Content-Disposition": f"attachment; filename=deductions_summary_{run_id}.csv"}
    )

# ===================== AUDIT LOG ROUTES =====================

@api_router.get("/audit-logs")
async def get_audit_logs(user: dict = Depends(require_admin)):
    logs = await db.audit_logs.find({"company_id": user['company_id']}, {"_id": 0}).sort("timestamp", -1).to_list(500)
    
    # Add user names
    users = await db.users.find({"company_id": user['company_id']}, {"_id": 0, "password_hash": 0}).to_list(100)
    user_map = {u['id']: u for u in users}
    
    for log in logs:
        actor = user_map.get(log['actor_user_id'], {})
        log['actor_name'] = f"{actor.get('first_name', '')} {actor.get('last_name', '')}".strip() or actor.get('email', 'Unknown')
    
    return logs

# ===================== DEMO DATA =====================

@api_router.post("/demo/generate")
async def generate_demo_data(user: dict = Depends(require_admin)):
    """Generate demo employees and data for testing"""
    company = await db.companies.find_one({"id": user['company_id']}, {"_id": 0})
    if not company:
        raise HTTPException(status_code=404, detail="Company not found")
    
    # Demo employees
    demo_employees = [
        {"first_name": "Sarah", "last_name": "Johnson", "email": "sarah.johnson@example.com", "employment_type": "hourly", "hourly_rate": 25.0, "external_employee_key": "EMP001"},
        {"first_name": "Michael", "last_name": "Chen", "email": "michael.chen@example.com", "employment_type": "hourly", "hourly_rate": 28.0, "external_employee_key": "EMP002"},
        {"first_name": "Emily", "last_name": "Williams", "email": "emily.williams@example.com", "employment_type": "salary", "annual_salary": 75000, "external_employee_key": "EMP003"},
        {"first_name": "David", "last_name": "Brown", "email": "david.brown@example.com", "employment_type": "hourly", "hourly_rate": 22.0, "external_employee_key": "EMP004"},
        {"first_name": "Jessica", "last_name": "Martinez", "email": "jessica.martinez@example.com", "employment_type": "salary", "annual_salary": 65000, "external_employee_key": "EMP005"},
        {"first_name": "James", "last_name": "Wilson", "email": "james.wilson@example.com", "employment_type": "hourly", "hourly_rate": 30.0, "external_employee_key": "EMP006"},
        {"first_name": "Amanda", "last_name": "Taylor", "email": "amanda.taylor@example.com", "employment_type": "hourly", "hourly_rate": 24.0, "external_employee_key": "EMP007"},
        {"first_name": "Robert", "last_name": "Anderson", "email": "robert.anderson@example.com", "employment_type": "salary", "annual_salary": 85000, "external_employee_key": "EMP008"},
    ]
    
    # Clear existing demo data completely
    await db.employees.delete_many({"company_id": user['company_id']})
    await db.payslips.delete_many({"company_id": user['company_id']})
    await db.payroll_lines.delete_many({"company_id": user['company_id']})
    await db.payroll_runs.delete_many({"company_id": user['company_id']})
    await db.time_entries.delete_many({"company_id": user['company_id']})
    await db.timesheet_imports.delete_many({"company_id": user['company_id']})
    await db.pay_periods.delete_many({"company_id": user['company_id']})
    
    created_employees = []
    for emp_data in demo_employees:
        employee = {
            "id": str(uuid.uuid4()),
            "company_id": user['company_id'],
            "external_employee_key": emp_data.get('external_employee_key'),
            "first_name": emp_data['first_name'],
            "last_name": emp_data['last_name'],
            "email": emp_data['email'],
            "employment_type": emp_data['employment_type'],
            "hourly_rate": emp_data.get('hourly_rate'),
            "annual_salary": emp_data.get('annual_salary'),
            "active": True,
            "created_at": datetime.now(timezone.utc).isoformat()
        }
        await db.employees.insert_one(employee)
        created_employees.append(employee)
    
    await log_audit(user['company_id'], user['id'], "GENERATE_DEMO", "employees", "", {"count": len(created_employees)})
    
    return {"success": True, "employees_created": len(created_employees)}

# ===================== DASHBOARD STATS =====================

@api_router.get("/dashboard/stats")
async def get_dashboard_stats(user: dict = Depends(get_current_user)):
    company_id = user['company_id']
    
    # Count employees
    employee_count = await db.employees.count_documents({"company_id": company_id, "active": True})
    
    # Count open pay periods
    open_periods = await db.pay_periods.count_documents({"company_id": company_id, "status": "open"})
    
    # Get last payroll run
    last_run = await db.payroll_runs.find_one(
        {"company_id": company_id, "status": "locked"},
        {"_id": 0},
        sort=[("locked_at", -1)]
    )
    
    # Count pending imports (periods without imports)
    periods = await db.pay_periods.find({"company_id": company_id, "status": "open"}, {"_id": 0}).to_list(100)
    pending_imports = 0
    for period in periods:
        import_exists = await db.timesheet_imports.find_one({"pay_period_id": period['id']})
        if not import_exists:
            pending_imports += 1
    
    return {
        "employee_count": employee_count,
        "open_pay_periods": open_periods,
        "pending_imports": pending_imports,
        "last_payroll_run": last_run
    }

# ===================== ANALYTICS ROUTES =====================

@api_router.get("/analytics/payroll-trends")
async def get_payroll_trends(user: dict = Depends(require_admin)):
    """Get payroll trends over time (last 12 pay periods)"""
    company_id = user['company_id']
    
    # Get all locked payroll runs with their pay periods
    runs = await db.payroll_runs.find(
        {"company_id": company_id, "status": "locked"},
        {"_id": 0}
    ).sort("locked_at", -1).to_list(12)
    
    trends = []
    for run in reversed(runs):  # Oldest first for chart
        period = await db.pay_periods.find_one({"id": run['pay_period_id']}, {"_id": 0})
        lines = await db.payroll_lines.find({"payroll_run_id": run['id']}, {"_id": 0}).to_list(1000)
        
        total_gross = sum(l.get('gross_pay', 0) + l.get('vacation_pay', 0) for l in lines)
        total_deductions = sum(
            l.get('cpp', 0) + l.get('ei', 0) + l.get('federal_tax', 0) + l.get('provincial_tax', 0)
            for l in lines
        )
        total_net = sum(l.get('net_pay', 0) for l in lines)
        
        trends.append({
            "period": period.get('start_date', '')[:10] if period else '',
            "pay_date": period.get('pay_date', '') if period else '',
            "gross": round(total_gross, 2),
            "deductions": round(total_deductions, 2),
            "net": round(total_net, 2),
            "employee_count": len(lines)
        })
    
    return trends

@api_router.get("/analytics/deductions-breakdown")
async def get_deductions_breakdown(user: dict = Depends(require_admin)):
    """Get breakdown of deductions by type for all locked runs"""
    company_id = user['company_id']
    
    # Get all locked payroll runs
    runs = await db.payroll_runs.find(
        {"company_id": company_id, "status": "locked"},
        {"_id": 0}
    ).to_list(100)
    
    totals = {"cpp": 0, "ei": 0, "federal_tax": 0, "provincial_tax": 0}
    
    for run in runs:
        lines = await db.payroll_lines.find({"payroll_run_id": run['id']}, {"_id": 0}).to_list(1000)
        for line in lines:
            totals['cpp'] += line.get('cpp', 0)
            totals['ei'] += line.get('ei', 0)
            totals['federal_tax'] += line.get('federal_tax', 0)
            totals['provincial_tax'] += line.get('provincial_tax', 0)
    
    breakdown = [
        {"name": "CPP", "value": round(totals['cpp'], 2), "color": "#1a237e"},
        {"name": "EI", "value": round(totals['ei'], 2), "color": "#6c8fef"},
        {"name": "Federal Tax", "value": round(totals['federal_tax'], 2), "color": "#10b981"},
        {"name": "Provincial Tax", "value": round(totals['provincial_tax'], 2), "color": "#f59e0b"}
    ]
    
    return breakdown

@api_router.get("/analytics/employee-costs")
async def get_employee_costs(user: dict = Depends(require_admin)):
    """Get cost breakdown per employee for latest payroll run"""
    company_id = user['company_id']
    
    # Get latest locked run
    run = await db.payroll_runs.find_one(
        {"company_id": company_id, "status": "locked"},
        {"_id": 0},
        sort=[("locked_at", -1)]
    )
    
    if not run:
        return []
    
    lines = await db.payroll_lines.find({"payroll_run_id": run['id']}, {"_id": 0}).to_list(1000)
    employees = await db.employees.find({"company_id": company_id}, {"_id": 0}).to_list(1000)
    emp_map = {e['id']: e for e in employees}
    
    costs = []
    for line in lines:
        emp = emp_map.get(line['employee_id'], {})
        costs.append({
            "name": f"{emp.get('first_name', '')} {emp.get('last_name', '')}".strip()[:15],
            "gross": round(line.get('gross_pay', 0) + line.get('vacation_pay', 0), 2),
            "deductions": round(
                line.get('cpp', 0) + line.get('ei', 0) + 
                line.get('federal_tax', 0) + line.get('provincial_tax', 0), 2
            ),
            "net": round(line.get('net_pay', 0), 2)
        })
    
    # Sort by gross descending
    costs.sort(key=lambda x: x['gross'], reverse=True)
    return costs

@api_router.get("/analytics/cost-forecast")
async def get_cost_forecast(user: dict = Depends(require_admin)):
    """Project annual costs based on recent payroll data"""
    company_id = user['company_id']
    company = await db.companies.find_one({"id": company_id}, {"_id": 0})
    
    # Get latest locked run
    run = await db.payroll_runs.find_one(
        {"company_id": company_id, "status": "locked"},
        {"_id": 0},
        sort=[("locked_at", -1)]
    )
    
    if not run:
        return {"monthly": [], "annual_projection": {}}
    
    lines = await db.payroll_lines.find({"payroll_run_id": run['id']}, {"_id": 0}).to_list(1000)
    
    # Calculate per-period totals
    period_gross = sum(l.get('gross_pay', 0) + l.get('vacation_pay', 0) for l in lines)
    period_cpp = sum(l.get('cpp', 0) for l in lines)
    period_ei = sum(l.get('ei', 0) for l in lines)
    period_tax = sum(l.get('federal_tax', 0) + l.get('provincial_tax', 0) for l in lines)
    period_net = sum(l.get('net_pay', 0) for l in lines)
    
    # Get periods per year
    pay_frequency = company.get('pay_frequency', 'biweekly')
    periods_per_year = 52 if pay_frequency == 'weekly' else 26
    
    # Project annual costs
    annual_projection = {
        "gross": round(period_gross * periods_per_year, 2),
        "cpp": round(period_cpp * periods_per_year, 2),
        "ei": round(period_ei * periods_per_year, 2),
        "tax": round(period_tax * periods_per_year, 2),
        "net": round(period_net * periods_per_year, 2),
        "employer_cost": round((period_gross + period_cpp + period_ei) * periods_per_year, 2)  # Including employer portions
    }
    
    # Generate monthly projection
    monthly = []
    months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"]
    periods_per_month = periods_per_year / 12
    
    for i, month in enumerate(months):
        monthly.append({
            "month": month,
            "projected": round(period_gross * periods_per_month, 2),
            "actual": round(period_gross * periods_per_month, 2) if i < 2 else 0  # Simulated actual for first 2 months
        })
    
    return {
        "monthly": monthly,
        "annual_projection": annual_projection,
        "pay_frequency": pay_frequency,
        "periods_per_year": periods_per_year
    }

@api_router.get("/analytics/summary")
async def get_analytics_summary(user: dict = Depends(require_admin)):
    """Get summary analytics for dashboard"""
    company_id = user['company_id']
    
    # Count total payroll runs
    total_runs = await db.payroll_runs.count_documents({"company_id": company_id, "status": "locked"})
    
    # Get all locked runs totals
    runs = await db.payroll_runs.find({"company_id": company_id, "status": "locked"}, {"_id": 0}).to_list(100)
    
    total_gross = 0
    total_net = 0
    total_deductions = 0
    
    for run in runs:
        lines = await db.payroll_lines.find({"payroll_run_id": run['id']}, {"_id": 0}).to_list(1000)
        for line in lines:
            total_gross += line.get('gross_pay', 0) + line.get('vacation_pay', 0)
            total_net += line.get('net_pay', 0)
            total_deductions += (
                line.get('cpp', 0) + line.get('ei', 0) + 
                line.get('federal_tax', 0) + line.get('provincial_tax', 0)
            )
    
    # Get employee count
    employee_count = await db.employees.count_documents({"company_id": company_id, "active": True})
    
    return {
        "total_payroll_runs": total_runs,
        "total_gross_paid": round(total_gross, 2),
        "total_net_paid": round(total_net, 2),
        "total_deductions": round(total_deductions, 2),
        "active_employees": employee_count,
        "avg_payroll_per_run": round(total_gross / total_runs, 2) if total_runs > 0 else 0
    }

# Include the router
app.include_router(api_router)

# Include Any Minute router
app.include_router(am_router)

app.add_middleware(
    CORSMiddleware,
    allow_credentials=True,
    allow_origins=os.environ.get('CORS_ORIGINS', '*').split(','),
    allow_methods=["*"],
    allow_headers=["*"],
)

@app.on_event("startup")
async def seed_default_users():
    """Seed default users on startup if they don't exist (for fresh deployments)"""
    # Only run if SEED_ON_STARTUP=true (default: false in production)
    if os.environ.get('SEED_ON_STARTUP', 'false').lower() != 'true':
        logger.info("Database seeding skipped (SEED_ON_STARTUP != true)")
        return
    
    try:
        # Check if Demo Tenant exists for Any Minute
        tenant = await db.am_tenants.find_one({"name": "Demo Tenant"})
        if not tenant:
            import secrets
            tenant_id = str(uuid.uuid4())
            tenant = {
                "id": tenant_id,
                "name": "Demo Tenant",
                "contact_email": "admin@anyminute.com",
                "payroll_api_key": secrets.token_urlsafe(32),
                "created_at": datetime.now(timezone.utc).isoformat()
            }
            await db.am_tenants.insert_one(tenant)
            logger.info(f"Created Demo Tenant: {tenant_id}")
        else:
            tenant_id = tenant['id']
        
        # Seed Any Minute users
        am_users = [
            {"email": "admin@anyminute.com", "first_name": "Admin", "last_name": "User", "role": "admin"},
            {"email": "manager@anyminute.com", "first_name": "Manager", "last_name": "User", "role": "manager"},
            {"email": "emp1@anyminute.com", "first_name": "Employee", "last_name": "One", "role": "employee"},
            {"email": "emp2@anyminute.com", "first_name": "Employee", "last_name": "Two", "role": "employee"},
        ]
        
        for u in am_users:
            existing = await db.am_users.find_one({"email": u['email']})
            if not existing:
                pw_hash = bcrypt.hashpw('test123'.encode('utf-8'), bcrypt.gensalt()).decode('utf-8')
                user = {
                    "id": str(uuid.uuid4()),
                    "tenant_id": tenant_id,
                    "email": u['email'],
                    "password_hash": pw_hash,
                    "first_name": u['first_name'],
                    "last_name": u['last_name'],
                    "role": u['role'],
                    "active": True,
                    "employee_mapping_key": None,
                    "created_at": datetime.now(timezone.utc).isoformat()
                }
                await db.am_users.insert_one(user)
                logger.info(f"Created Any Minute user: {u['email']}")
        
        # Seed Payroll Canada company and admin
        company = await db.companies.find_one({})
        if not company:
            company_id = str(uuid.uuid4())
            company = {
                "id": company_id,
                "name": "Demo Company",
                "address": "123 Main St, Toronto, ON",
                "province": "ON",
                "created_at": datetime.now(timezone.utc).isoformat()
            }
            await db.companies.insert_one(company)
            logger.info(f"Created Demo Company: {company_id}")
        else:
            company_id = company['id']
        
        # Seed Payroll Canada admin user
        pc_admin = await db.users.find_one({"email": "admin@test.com"})
        if not pc_admin:
            pw_hash = bcrypt.hashpw('test123'.encode('utf-8'), bcrypt.gensalt()).decode('utf-8')
            admin = {
                "id": str(uuid.uuid4()),
                "company_id": company_id,
                "email": "admin@test.com",
                "password_hash": pw_hash,
                "first_name": "Admin",
                "last_name": "User",
                "role": "admin",
                "active": True,
                "created_at": datetime.now(timezone.utc).isoformat()
            }
            await db.users.insert_one(admin)
            logger.info("Created Payroll Canada admin: admin@test.com")
        
        logger.info("Database seeding complete")
    except Exception as e:
        logger.error(f"Error seeding database: {e}")

@app.on_event("shutdown")
async def shutdown_db_client():
    client.close()
