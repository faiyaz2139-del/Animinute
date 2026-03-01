# Payroll Canada + Any Minute Timesheet - Product Requirements Document

## Original Problem Statement
Build a production-ready MVP web app called "Payroll Canada" for Canadian SMBs. The app imports approved hours from an existing ASP.NET timesheet system (supporting both real API connection and mock mode). Key features include company setup, employee management, pay period creation, payroll calculation, PDF payslip generation, and CSV reports.

**Extended Scope:**
1. **Payroll Analytics Dashboard** - Successfully implemented
2. **Any Minute Timesheet App** - Phase 1 MVP complete (Feb 2026)
3. **Payroll Canada ↔ Any Minute Integration** - Complete (Feb 14, 2026)

---

## User Personas
1. **HR Manager/Admin** - Manages payroll, employees, businesses
2. **Manager** - Approves timesheets, views reports
3. **Employee** - Enters timesheet hours, views schedule

---

## Core Requirements

### Payroll Canada (Complete)
- [x] Company setup with Canadian tax compliance
- [x] Employee management with salary/hourly classification
- [x] Pay period creation and management
- [x] Payroll calculation with CPP, EI, tax deductions
- [x] PDF payslip generation
- [x] CSV reports
- [x] Analytics Dashboard with trends, breakdowns, forecasts
- [x] JWT authentication
- [x] Audit logging
- [x] **Any Minute Integration** - Live timesheet import from Any Minute API

### Any Minute Timesheet App - Phase 1 MVP (Complete Feb 2026)
- [x] **Auth + Tenant + Roles (JWT)** - Demo Tenant auto-creation
- [x] **Business CRUD** - Create, read, update, soft delete businesses
- [x] **Users CRUD** - Role assignment (admin/manager/employee), employee mapping key
- [x] **Timesheet Core** - Weekly grid (Saturday start), start/end times, breaks, net hours, approve/reject workflow
- [x] **Payroll Integration APIs**:
  - `GET /api/am/payroll/employees` - Returns employees with mapping keys
  - `GET /api/am/payroll/approved-entries?start&end` - Approved entries only
  - `POST /api/am/payroll/lock?start&end` - Lock entries after payroll
- [x] **Scheduling** - Basic weekly calendar for shift management
- [x] **Custom Report by Business Name** - Hours breakdown by employee

---

## Integration Architecture

### Payroll Canada ↔ Any Minute Integration (Complete)
```
┌─────────────────┐                    ┌──────────────────────┐
│  Payroll Canada │────API Call────────│  Any Minute Timesheet│
│   (Main App)    │  X-PAYROLL-KEY     │    (Phase 1 MVP)     │
│                 │◄───JSON Response───│                      │
└─────────────────┘                    └──────────────────────┘
```

**Environment Variables (backend/.env):**
- `ANYMINUTE_BASE_URL` - Base URL for Any Minute API
- `ANYMINUTE_PAYROLL_KEY` - X-PAYROLL-KEY for authentication
- `ANYMINUTE_TIMEOUT_SECONDS` - API timeout (default: 15)
- `USE_MOCK` - Set to "true" to use mock data (default: false)

**Employee Matching Strategy:**
1. Match by `external_employee_key` in Payroll Canada ↔ `employee_mapping_key` in Any Minute
2. Fallback to email matching

**Flow:**
1. In Any Minute: Employee creates timesheet → Manager approves
2. In Payroll Canada: Create pay period → Preview timesheets → Import → Calculate payroll → Lock

---

## Architecture

### Two-Service Architecture
```
┌─────────────────┐         ┌──────────────────────┐
│  Payroll Canada │◄───────►│  Any Minute Timesheet│
│   (Main App)    │  API    │    (Phase 1 MVP)     │
└─────────────────┘         └──────────────────────┘
        │                           │
        ▼                           ▼
┌─────────────────────────────────────────────────┐
│              MongoDB (Shared Instance)           │
│  Collections: companies, users, employees...     │
│  AM Collections: am_tenants, am_users,           │
│  am_businesses, am_timesheet_weeks...            │
└─────────────────────────────────────────────────┘
```

### Key Technical Details
- **Backend**: FastAPI, Python, MongoDB (pymongo/motor)
- **Frontend**: React, JavaScript, Tailwind CSS, Recharts
- **Authentication**: JWT (separate tokens for each app)
- **Inter-service Security**: `X-PAYROLL-KEY` header for payroll integration

---

## API Endpoints

### Any Minute API (/api/am/*)
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /auth/register | Register new user (creates Demo Tenant if first) |
| POST | /auth/login | Login and get JWT token |
| GET | /auth/me | Get current user profile |
| GET/POST/PUT/DELETE | /businesses/* | Business CRUD |
| GET/POST/PUT/DELETE | /users/* | User management |
| **PUT** | **/users/{id}/status** | **Update user status (Active/Not Active/Terminated)** |
| **GET** | **/users/{id}/status-history** | **Get user status change history** |
| GET/POST | /timesheet-weeks/* | Timesheet week management |
| POST | /timesheet-weeks/{id}/submit | Submit for approval |
| POST | /timesheet-weeks/{id}/approve | Approve/reject timesheet |
| **POST** | **/timesheet-weeks/{id}/bulk-approve** | **Approve all entries in week** |
| **POST** | **/timesheet-weeks/{id}/bulk-reject** | **Reject all entries in week** |
| GET/POST/PUT/DELETE | /timesheet-entries/* | Daily time entries |
| **PUT** | **/timesheet-entries/{id}/status** | **Update entry status (Pending/Approved/Rejected/Absent)** |
| GET/POST/PUT/DELETE | /schedules/* | Schedule management |
| GET | /reports/by-business | Hours by business report |
| GET | /tenant/settings | Get payroll API key |
| POST | /tenant/settings/regenerate-key | Regenerate API key |
| **GET** | **/billing** | **Get billing info (plan, seats, status)** |
| **PUT** | **/billing** | **Update billing (admin only)** |
| **GET/POST/PUT/DELETE** | **/pay-rates/*** | **Pay rate management** |
| **GET** | **/pay-rates/effective/{user_id}** | **Get effective pay rate for user** |
| GET | /payroll/employees | Payroll integration - employees |
| GET | /payroll/approved-entries | Payroll integration - approved hours |
| POST | /payroll/lock | Payroll integration - lock entries |

---

## Database Schema

### Any Minute Collections (am_* prefix)
- **am_tenants**: Multi-tenant isolation, payroll API key, **plan, seat_limit, status (billing fields)**
- **am_users**: Users with role (admin/manager/employee), employee_mapping_key, **status, status_effective_date**
- **am_user_status_history**: **NEW - Track status changes with effective dates**
- **am_businesses**: Business locations per tenant
- **am_timesheet_weeks**: Weekly timesheet containers with status
- **am_timesheet_entries**: Daily time entries with net hours, **entry_status (pending/approved/rejected/absent)**
- **am_schedule_entries**: Shift schedules
- **am_user_business_roles**: User-business role assignments
- **am_pay_rates**: **NEW - Pay rates per user per business with effective dates**

---

## Test Credentials

### Any Minute App
- **Email**: admin@anyminute.com
- **Password**: test123
- **Role**: Admin
- **Access**: `/anyminute/login`

### Payroll Canada
- **Email**: admin@test.com
- **Password**: test123
- **Access**: `/login`

### Payroll API Key
- **Key**: `dZPrWlX5SWIAVk_cD1ac-fYxt97avVDjMS0dj2-NxPw`

---

## Completed Work Log

### March 1, 2026 (Phase 2 Bug Fixes - 8 Priority Items + Tickets)
All bugs from AnyMinute_BugFix_StatusReport.docx now FIXED:

**#18 Tickets (Support) System** ✅ COMPLETE
- Backend APIs: `/tickets/*` with CRUD, replies, status updates
- Frontend: Full ticket list, create modal, detail view with conversation
- Features: Priority levels, status management (Open/In Progress/Resolved/Closed)
- Admin can update status, view all tickets; users see own tickets only

**Previous 8 Priority Items** ✅ All Complete:
1. User Status Lifecycle - Active/Not Active/Terminated with history
2. Pay Rate Management - CRUD with effective dates
3. Timesheet Entry Status - Color-coded badges per row
4. Bulk Approve/Reject Week - One-click actions
5. Payroll Run Guard - Block before pay date
6. Fix $0 Employees - Skip 0-hour entries
7. CSV Export Fix - Correct run filtering
8. Import Timesheets - Enhanced error handling

### February 15, 2026 (Phase 2A - Billing Complete)
- **Plan Upgrade + Billing MVP** implemented without Stripe
- Backend changes (`/app/backend/any_minute/routes.py`):
  - Added `AMBillingUpdate` Pydantic model
  - Added `GET /api/am/billing` - returns plan, seat_limit, seat_usage, status
  - Added `PUT /api/am/billing` - admin-only plan/seat update
  - Modified `POST /api/am/users` to enforce seat limits
- Frontend: Created `/app/frontend/src/anyminute/pages/Billing.js`
  - Shows current plan with color-coded badge
  - Seat usage progress bar with warning at 80%
  - Admin can change plan (Free/Basic/Pro) and custom seat limit
  - Non-admin sees read-only view
- Updated navigation label from "Plan Upgrade" to "Billing"
- Tested: All APIs working, seat limit enforcement verified

### February 14, 2026 (Integration Complete)
- Integrated Payroll Canada with Any Minute API
- Updated `/app/backend/modules/timesheet_connector.py`:
  - Uses `ANYMINUTE_BASE_URL`, `ANYMINUTE_PAYROLL_KEY` environment variables
  - Calls `/api/am/payroll/approved-entries` for timesheet data
  - Calls `/api/am/payroll/employees` for employee list
  - Calls `/api/am/payroll/lock` to mark timesheets exported
- Added `TimesheetConnectorError` exception for proper error handling
- Updated server.py with try/catch blocks for connector errors
- Fixed minor UI bug in ImportTimesheets.js (undefined employee name)
- Tested end-to-end: 15 backend tests passed, all frontend flows working

### February 14, 2026 (Phase 1 MVP)
- Built complete Any Minute Timesheet App Phase 1 MVP
- Implemented all backend APIs at /api/am/*
- Created frontend pages: Login, Signup, Dashboard, Timesheet, Schedule, Reports, Settings, Add Business, Add/Edit User
- Integrated with existing Payroll Canada frontend
- Tested all features - 100% pass rate
- Fixed Dashboard auth headers bug

---

## Backlog / Future Tasks

### P1 - Phase 2 Features
- [x] **Plan Upgrade + Billing (2A)** - Complete (Feb 15, 2026)
  - Billing fields added to tenant (plan, seat_limit, status)
  - Admin-only API: GET/PUT /api/am/billing
  - Seat limit enforcement when adding users
  - Full Billing UI with plan selection
- [ ] Reseller registration (placeholder screen)
- [ ] Tickets system (placeholder screen)
- [ ] RBAC hardening (frontend route guards, backend permission checks)
- [ ] Notifications - event logging for key actions
- [ ] Audit logs for create/update/delete actions
- [ ] "Compare to a prior period" hyperlink in reports

### P2 - Enhancements
- [ ] Mobile responsive improvements
- [ ] Email notifications for timesheet approvals
- [ ] Bulk timesheet operations
- [ ] Export to Excel

---

## Known Issues
- **Platform Branding**: "Made with Emergent" badge is platform-injected at runtime and cannot be removed via application code

---

## Files Reference

### Backend
- `/app/backend/server.py` - Main Payroll Canada server
- `/app/backend/any_minute/routes.py` - Any Minute API routes
- `/app/backend/modules/timesheet_connector.py` - Any Minute integration connector

### Frontend
- `/app/frontend/src/App.js` - Main app with routing
- `/app/frontend/src/anyminute/` - Any Minute frontend components
  - `context/AMAuthContext.js` - Auth context
  - `pages/Timesheet.js` - Weekly timesheet grid
  - `pages/Schedule.js` - Weekly calendar
  - `pages/Reports.js` - Business reports
  - `pages/Settings.js` - Payroll API key management
  - **`pages/Billing.js` - Plan & seat management (Phase 2A)**

### Tests
- `/app/backend/tests/test_payroll_anyminute_integration.py` - Integration tests
- `/app/test_reports/iteration_5.json` - Latest test results
