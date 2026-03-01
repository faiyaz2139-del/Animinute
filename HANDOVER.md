# AnyMinute — Client Handover Document

**Date:** March 2026  
**Version:** 1.0.0  
**Status:** Production Ready (TEST MODE for payments)

---

## What Was Built

### AnyMinute Timesheet Application

A complete multi-tenant timesheet tracking system with the following features:

#### Core Features
- **Authentication & Authorization** — JWT-based auth with role-based access control
- **Multi-tenant Architecture** — Complete data isolation per organization
- **Business Management** — Create, edit, and manage multiple business locations
- **User Management** — Admin, Manager, Accountant, and Employee roles
- **User Status Lifecycle** — Active, Not Active, Terminated with history tracking

#### Timesheet Management
- **Weekly Timesheet Grid** — Saturday-start week with daily entries
- **Time Entry** — Start/end times, breaks, automatic net hours calculation
- **Entry Status** — Pending, Approved, Rejected, Absent with color coding
- **Bulk Actions** — Approve or reject entire week with one click
- **Schedule Management** — Weekly calendar with shift assignments

#### Billing & Payments
- **Plan Management** — Free, Basic (25 seats), Pro (999 seats) plans
- **Seat Limit Enforcement** — Cannot add users beyond plan limit
- **Stripe Integration** — Subscription webhooks ready (TEST mode)
  - Subscription created → Activate account
  - Subscription updated → Update plan tier
  - Subscription deleted → Deactivate account with login block

#### Reporting & Analytics
- **Business Reports** — Hours breakdown by employee and business
- **Compare to Prior Period** — Side-by-side period comparison with trends
- **CSV Export** — Download report data

#### Support & Audit
- **Support Tickets** — Internal ticketing with priority levels and status workflow
- **Audit Logs** — Complete trail of all actions with old/new value tracking
- **Pay Rate Management** — Multiple rates per employee with effective dates

### Payroll Canada Application

Canadian payroll processing integrated with AnyMinute:

- **Canadian Tax Compliance** — CPP, EI, federal/provincial tax calculations
- **Employee Management** — Salary and hourly classifications
- **Pay Period Management** — Multiple period types supported
- **Payroll Calculation** — Automatic deductions and net pay
- **PDF Payslips** — Professional payslip generation
- **Analytics Dashboard** — Trends, breakdowns, and forecasts
- **AnyMinute Integration** — Import approved hours directly

---

## How to Access the App

### Local Development
- **AnyMinute**: http://localhost:3000/anyminute/login
- **Payroll Canada**: http://localhost:3000/login
- **API Docs**: http://localhost:8001/docs

### Default Admin Login

| Application | Email | Password |
|-------------|-------|----------|
| AnyMinute | `admin@anyminute.com` | `test123` |
| Payroll Canada | `admin@test.com` | `test123` |

---

## What to Do Before Going Live

### Critical Checklist

- [ ] **Change default admin passwords** — All test credentials must be changed
- [ ] **Generate new JWT secrets** — Run `openssl rand -hex 32` for each secret
- [ ] **Set up MongoDB backups** — Daily backups recommended (mongodump or Atlas)
- [ ] **Configure real SMTP credentials** — Replace mock email service
- [ ] **Switch Stripe to LIVE mode** — Replace test keys with production keys
- [ ] **Set `APP_ENV=production`** — Disable debug mode
- [ ] **Set `DEBUG=false`** — Disable detailed error messages
- [ ] **Set up SSL/HTTPS** — Required for production security
- [ ] **Configure a domain name** — Set up DNS and update CORS_ORIGINS
- [ ] **Set `SEED_ON_STARTUP=false`** — Disable demo data seeding
- [ ] **Review CORS settings** — Replace `*` with specific allowed domains

### Security Recommendations

1. **Database Security**
   - Enable MongoDB authentication
   - Use a strong password for database user
   - Restrict network access to database port

2. **API Security**
   - Rate limiting on authentication endpoints
   - Input validation on all user inputs
   - Regular security audits

3. **Infrastructure**
   - Use a reverse proxy (nginx) in front of the API
   - Enable firewall rules
   - Set up monitoring and alerting

---

## How to Add the First Real Business and Admin User

### Step 1: Access the Admin Account

1. Navigate to `/anyminute/login`
2. Log in with default admin credentials
3. **Immediately change the password** in Settings

### Step 2: Update Tenant Information

1. Go to **Settings** in the sidebar
2. Update your organization name
3. Note your **Payroll API Key** (needed for Payroll Canada integration)

### Step 3: Create Your First Business

1. Click **Add Business** in the sidebar
2. Fill in:
   - Business name
   - Address
   - Contact email
   - Contact phone
3. Click **Create Business**

### Step 4: Add Users

1. Click **Add User** in the sidebar
2. Fill in user details:
   - Email address
   - First/Last name
   - Role (Admin/Manager/Employee)
3. Click **Create User**
4. Share login credentials securely with the new user

### Step 5: Configure Billing (If Using Paid Plans)

1. Go to **Billing** in the sidebar
2. Select appropriate plan
3. Set seat limit
4. (When Stripe is live) Complete payment flow

---

## How to Upgrade Plans (Stripe)

### Current State: TEST MODE

The application is configured with Stripe test keys. In this mode:
- No real charges are made
- Use test card numbers (e.g., `4242 4242 4242 4242`)
- Webhooks simulate subscription events

### Transitioning to LIVE Mode

1. **Get Live Stripe Keys**
   - Log in to [Stripe Dashboard](https://dashboard.stripe.com)
   - Go to Developers > API Keys
   - Copy your live publishable and secret keys

2. **Update Environment Variables**
   ```bash
   # Backend .env
   STRIPE_SECRET_KEY=sk_live_your_live_key
   STRIPE_PUBLISHABLE_KEY=pk_live_your_live_key
   STRIPE_MODE=live
   
   # Frontend .env
   REACT_APP_STRIPE_PUBLISHABLE_KEY=pk_live_your_live_key
   ```

3. **Configure Live Webhook**
   - In Stripe Dashboard, go to Developers > Webhooks
   - Add endpoint: `https://yourdomain.com/api/am/stripe/webhook`
   - Select events:
     - `customer.subscription.created`
     - `customer.subscription.updated`
     - `customer.subscription.deleted`
   - Copy the signing secret to `STRIPE_WEBHOOK_SECRET`

4. **Create Products and Prices**
   - Create products for Basic and Pro plans in Stripe
   - Update price IDs in `stripe_service.py` if needed

5. **Test with a Real Card**
   - Make a small test purchase
   - Verify webhook delivery in Stripe Dashboard
   - Confirm database updates correctly

---

## Ongoing Maintenance

### How to View Audit Logs

1. Log in as an Admin user
2. Navigate to **Audit Logs** in the sidebar
3. Use filters to search by:
   - Entity type (User, Business, Timesheet, etc.)
   - Action (Create, Update, Delete)
   - Date range
4. Click on entries to see detailed change history

### How to Handle Support Tickets

1. Navigate to **Tickets** in the sidebar
2. View all tickets or filter by status
3. Click a ticket to view conversation
4. Add replies and update status:
   - Open → In Progress → Resolved → Closed
5. Set priority levels: Low, Medium, High, Critical

### How to Run Payroll Each Period

1. Log in to **Payroll Canada** (`/login`)
2. Go to **Pay Periods** and create new period
3. Click **Import Timesheets** to pull approved hours from AnyMinute
4. Review imported hours and make adjustments
5. Run payroll calculation
6. Generate and distribute payslips
7. Export CSV for accounting system

### How to Add New Employees

1. In AnyMinute: **Add User** → Create employee account
2. Assign to a business
3. Set pay rate in **Pay Rates** section
4. In Payroll Canada: **Employees** → Link to AnyMinute user
5. Set `external_employee_key` to match AnyMinute `employee_mapping_key`

---

## Support & Next Steps

### Recommended Next Development Items

| Priority | Item | Description |
|----------|------|-------------|
| 1 | **Live Email Service** | Replace mock email with real SMTP (SendGrid recommended) |
| 2 | **Production Stripe** | Switch from test to live payment processing |
| 3 | **Reseller Dashboard** | Build out partner registration and management features |

### Future Enhancements

- Mobile-responsive design improvements
- Employee self-service portal
- Advanced reporting and analytics
- Multi-language support
- Integration with accounting software (QuickBooks, Xero)

### Technical Support

For technical issues or questions:

1. Check the API documentation at `/docs`
2. Review audit logs for recent changes
3. Check server logs for error messages
4. Contact development team with:
   - Error message/screenshot
   - Steps to reproduce
   - Expected vs actual behavior

---

## Quick Reference

### Key URLs

| Resource | URL |
|----------|-----|
| AnyMinute App | `/anyminute/login` |
| Payroll Canada | `/login` |
| API Documentation | `/docs` |
| Billing Management | `/anyminute/billing` |
| Audit Logs | `/anyminute/audit-logs` |

### Key Files

| File | Purpose |
|------|---------|
| `backend/.env` | Backend configuration |
| `frontend/.env` | Frontend configuration |
| `memory/PRD.md` | Full product requirements |
| `docker-compose.yml` | Container orchestration |

### Database Collections

| Collection | Purpose |
|------------|---------|
| `am_tenants` | Organizations/companies |
| `am_users` | User accounts |
| `am_businesses` | Business locations |
| `am_timesheet_weeks` | Weekly timesheets |
| `am_timesheet_entries` | Daily time entries |
| `am_audit_logs` | Audit trail |
| `am_tickets` | Support tickets |
| `am_pay_rates` | Employee pay rates |

---

*Document generated: March 2026*  
*AnyMinute v1.0.0*
