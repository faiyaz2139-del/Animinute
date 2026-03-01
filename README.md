# AnyMinute — Timesheet & Payroll Management

## Overview

AnyMinute is a comprehensive timesheet tracking and payroll management platform designed for Canadian SMBs. It consists of two integrated applications:

1. **AnyMinute Timesheet** — Employee time tracking, scheduling, and approval workflows
2. **Payroll Canada** — Payroll processing with Canadian tax compliance (CPP, EI, federal/provincial taxes)

The platform supports multi-tenant architecture with role-based access control, Stripe billing integration, and real-time analytics.

---

## Tech Stack

| Layer | Technology |
|-------|------------|
| Frontend | React 18, Tailwind CSS, Recharts, Lucide Icons |
| Backend | FastAPI (Python 3.11), Pydantic v2 |
| Database | MongoDB 6.0+ (Motor async driver) |
| Payments | Stripe (Subscriptions, Webhooks) |
| Auth | JWT (JSON Web Tokens) |
| PDF Generation | ReportLab |
| Email | Mock service (ready for SMTP integration) |

---

## Folder Structure

```
/app
├── backend/                    # FastAPI backend application
│   ├── any_minute/             # AnyMinute-specific modules
│   │   ├── routes.py           # All AnyMinute API endpoints
│   │   ├── stripe_service.py   # Stripe webhook handlers
│   │   └── email_service.py    # Email service (mock mode)
│   ├── modules/                # Payroll Canada modules
│   │   ├── payroll_engine.py   # Tax calculations (CPP, EI, etc.)
│   │   ├── pdf_service.py      # Payslip PDF generation
│   │   ├── csv_service.py      # CSV export functionality
│   │   └── timesheet_connector.py # AnyMinute integration
│   ├── tests/                  # Backend test files
│   ├── server.py               # Main FastAPI application
│   ├── requirements.txt        # Python dependencies
│   └── .env                    # Environment variables
│
├── frontend/                   # React frontend application
│   ├── src/
│   │   ├── anyminute/          # AnyMinute app components
│   │   │   ├── pages/          # AnyMinute page components
│   │   │   ├── components/     # Shared UI components
│   │   │   └── context/        # React context (auth, etc.)
│   │   ├── pages/              # Payroll Canada pages
│   │   ├── components/         # Shared components
│   │   │   └── ui/             # Shadcn UI components
│   │   ├── context/            # Auth & theme contexts
│   │   └── App.js              # Main app with routing
│   ├── public/                 # Static assets
│   ├── package.json            # Node dependencies
│   └── .env                    # Frontend environment variables
│
├── memory/                     # Documentation
│   └── PRD.md                  # Product Requirements Document
│
├── test_reports/               # Test execution reports
├── docker-compose.yml          # Docker orchestration
├── Dockerfile.backend          # Backend container config
├── Dockerfile.frontend         # Frontend container config
├── README.md                   # This file
└── HANDOVER.md                 # Client handover checklist
```

---

## Prerequisites

Before setup, ensure you have installed:

| Requirement | Version | Notes |
|-------------|---------|-------|
| Node.js | 18.x or 20.x | LTS recommended |
| Python | 3.11+ | Required for FastAPI |
| MongoDB | 6.0+ | Community or Atlas |
| Yarn | 1.22+ | Package manager |
| Docker | 24.0+ | Optional, for containerized deployment |
| Stripe Account | - | For payment processing |
| SMTP Provider | - | SendGrid, Mailgun, or similar (optional) |

---

## Local Development Setup

### 1. Clone the Repository

```bash
git clone https://github.com/your-org/anyminute-app.git
cd anyminute-app
```

### 2. Backend Setup

```bash
# Navigate to backend
cd backend

# Create virtual environment
python -m venv venv
source venv/bin/activate  # On Windows: venv\Scripts\activate

# Install dependencies
pip install -r requirements.txt

# Copy environment template
cp .env.example .env

# Edit .env with your values (see Environment Variables section)
nano .env
```

### 3. Frontend Setup

```bash
# Navigate to frontend
cd ../frontend

# Install dependencies
yarn install

# Copy environment template
cp .env.example .env

# Edit .env with your values
nano .env
```

### 4. Start MongoDB

```bash
# Using Docker (recommended)
docker run -d -p 27017:27017 --name mongodb mongo:6.0

# Or start your local MongoDB service
mongod --dbpath /data/db
```

### 5. Run Backend

```bash
cd backend
source venv/bin/activate
uvicorn server:app --host 0.0.0.0 --port 8001 --reload
```

### 6. Run Frontend

```bash
cd frontend
yarn start
```

### 7. Access the Application

- **AnyMinute Timesheet**: http://localhost:3000/anyminute/login
- **Payroll Canada**: http://localhost:3000/login
- **API Documentation**: http://localhost:8001/docs

---

## Environment Variables

### Backend (.env)

| Variable | Description | Example |
|----------|-------------|---------|
| `MONGO_URL` | MongoDB connection string | `mongodb://localhost:27017` |
| `DB_NAME` | Database name | `anyminute_db` |
| `CORS_ORIGINS` | Allowed CORS origins | `http://localhost:3000` |
| `AM_JWT_SECRET` | JWT secret for AnyMinute | `your-secret-key-here` |
| `JWT_SECRET` | JWT secret for Payroll Canada | `another-secret-key` |
| `STRIPE_SECRET_KEY` | Stripe secret key | `sk_test_...` |
| `STRIPE_PUBLISHABLE_KEY` | Stripe publishable key | `pk_test_...` |
| `STRIPE_WEBHOOK_SECRET` | Stripe webhook signing secret | `whsec_...` |
| `STRIPE_MODE` | `test` or `live` | `test` |
| `EMAILS_ENABLED` | Enable real email sending | `false` |
| `SMTP_HOST` | SMTP server host | `smtp.sendgrid.net` |
| `SMTP_PORT` | SMTP server port | `587` |
| `SMTP_USER` | SMTP username | `apikey` |
| `SMTP_PASS` | SMTP password/API key | `SG.xxx...` |
| `SMTP_FROM` | Default sender email | `noreply@yourdomain.com` |
| `SEED_ON_STARTUP` | Seed demo data on startup | `true` |

### Frontend (.env)

| Variable | Description | Example |
|----------|-------------|---------|
| `REACT_APP_BACKEND_URL` | Backend API URL | `http://localhost:8001` |
| `REACT_APP_STRIPE_PUBLISHABLE_KEY` | Stripe publishable key | `pk_test_...` |

---

## Default Login Credentials

### AnyMinute Timesheet App

| Role | Email | Password |
|------|-------|----------|
| Admin | `admin@anyminute.com` | `test123` |
| Manager | `manager@anyminute.com` | `test123` |
| Employee | `emp1@anyminute.com` | `test123` |

### Payroll Canada

| Role | Email | Password |
|------|-------|----------|
| Admin | `admin@test.com` | `test123` |

> **Important**: Change all default passwords before going to production!

---

## User Roles

### AnyMinute Roles

| Role | Permissions |
|------|-------------|
| **Admin** | Full access: manage users, businesses, billing, audit logs, all reports |
| **Manager** | Approve timesheets, view reports, manage schedules, edit users |
| **Accountant** | View reports, export data (read-only) |
| **Employee** | Enter own timesheets, view own schedule |

### Payroll Canada Roles

| Role | Permissions |
|------|-------------|
| **Admin** | Full access: payroll runs, employee management, settings |
| **User** | View payslips, limited dashboard access |

---

## Key Features

### AnyMinute Timesheet

- **Multi-tenant Architecture** — Complete data isolation per organization
- **Weekly Timesheet Grid** — Saturday-start week with start/end times, breaks
- **Entry Status Tracking** — Pending, Approved, Rejected, Absent with color coding
- **Bulk Actions** — Approve/reject entire week with one click
- **User Status Lifecycle** — Active, Not Active, Terminated with history tracking
- **Pay Rate Management** — Multiple rates per employee with effective dates
- **Schedule Management** — Weekly calendar with shift assignments
- **Support Tickets** — Internal ticketing system with priority levels
- **Audit Logging** — Complete trail of all create/update/delete actions
- **Reports** — Business breakdown with "Compare to Prior Period" feature
- **Billing & Plans** — Free/Basic/Pro plans with seat limits
- **Stripe Integration** — Subscription webhooks (TEST mode ready)

### Payroll Canada

- **Canadian Tax Compliance** — CPP, EI, federal and provincial tax calculations
- **Employee Management** — Salary and hourly classifications
- **Pay Period Management** — Bi-weekly, semi-monthly, monthly periods
- **Payroll Calculation** — Automatic deductions and net pay
- **PDF Payslips** — Professional payslip generation
- **CSV Export** — Export payroll data for accounting systems
- **Analytics Dashboard** — Trends, breakdowns, and forecasts
- **Any Minute Integration** — Import approved hours directly

---

## Known Limitations & Next Steps

### Current Limitations

| Area | Status | Notes |
|------|--------|-------|
| Email Service | **MOCK MODE** | Logs to console; swap SMTP env vars to enable |
| Stripe Payments | **TEST MODE** | Using test keys; switch to live keys before launch |
| Reseller Dashboard | **PLACEHOLDER** | UI exists but functionality not implemented |
| Mobile Responsiveness | **BASIC** | Desktop-first; mobile improvements pending |
| Platform Branding | **VISIBLE** | "Made with Emergent" badge is platform-injected |

### Recommended Next Steps

1. **Enable Live Email** — Configure SMTP credentials (SendGrid recommended)
2. **Activate Stripe Live Mode** — Replace test keys with production keys
3. **Build Reseller Features** — Complete partner registration and dashboard
4. **Mobile Optimization** — Improve responsive design for mobile devices
5. **Backup Strategy** — Set up automated MongoDB backups

---

## API Documentation

Interactive API documentation is available at:

- **Swagger UI**: `http://localhost:8001/docs`
- **ReDoc**: `http://localhost:8001/redoc`

### Key Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/am/auth/login` | POST | AnyMinute login |
| `/api/am/businesses` | GET/POST | Business management |
| `/api/am/users` | GET/POST | User management |
| `/api/am/timesheet-weeks` | GET/POST | Timesheet management |
| `/api/am/billing` | GET/PUT | Billing management |
| `/api/am/audit-logs` | GET | Audit log retrieval |
| `/api/am/stripe/webhook` | POST | Stripe webhook handler |
| `/api/payroll/*` | Various | Payroll Canada endpoints |

---

## Support

For technical support or questions about this application, please contact:

- **Development Team**: [Your contact info]
- **Documentation**: See `/memory/PRD.md` for detailed requirements
- **Issue Tracking**: [Your issue tracker URL]

---

## License

Proprietary — All rights reserved.

---

*Last Updated: March 2026*
