"""
Test Payroll Canada and Any Minute Timesheet Integration
Tests the integration flow:
1. Any Minute payroll API endpoints (employees, approved-entries, lock)
2. Payroll Canada timesheet preview/import using real Any Minute API
3. Employee matching by external_employee_key (EMP001 -> Sarah Johnson)
4. Full flow: login -> fetch timesheets -> import -> calculate payroll
"""
import pytest
import requests
import os

BASE_URL = os.environ.get('REACT_APP_BACKEND_URL').rstrip('/')
PAYROLL_API_KEY = "dZPrWlX5SWIAVk_cD1ac-fYxt97avVDjMS0dj2-NxPw"

# Test credentials
PAYROLL_ADMIN = {"email": "admin@test.com", "password": "test123"}
ANYMINUTE_ADMIN = {"email": "admin@anyminute.com", "password": "test123"}
ANYMINUTE_EMPLOYEE = {"email": "employee_test@anyminute.com", "password": "test123"}

class TestAPIHealth:
    """Basic API health checks"""
    
    def test_payroll_canada_api_health(self):
        """Verify Payroll Canada API is healthy"""
        response = requests.get(f"{BASE_URL}/api/")
        assert response.status_code == 200
        data = response.json()
        assert data.get("message") == "Payroll Canada API"
        assert data.get("status") == "healthy"
        print("Payroll Canada API health: PASSED")
    
    def test_anyminute_api_health(self):
        """Verify Any Minute API is healthy"""
        response = requests.get(f"{BASE_URL}/api/am/")
        assert response.status_code == 200
        data = response.json()
        assert data.get("message") == "Any Minute Timesheet API"
        assert data.get("status") == "healthy"
        print("Any Minute API health: PASSED")


class TestAnyMinutePayrollAPI:
    """Test Any Minute payroll integration API endpoints using X-PAYROLL-KEY"""
    
    def test_payroll_employees_endpoint(self):
        """GET /api/am/payroll/employees with valid API key"""
        headers = {"X-PAYROLL-KEY": PAYROLL_API_KEY}
        response = requests.get(f"{BASE_URL}/api/am/payroll/employees", headers=headers)
        assert response.status_code == 200
        data = response.json()
        assert "employees" in data
        employees = data["employees"]
        print(f"Found {len(employees)} employees via payroll API")
        
        # Verify employee structure
        if employees:
            emp = employees[0]
            assert "external_employee_key" in emp
            assert "first_name" in emp
            assert "last_name" in emp
            assert "email" in emp
        print("Payroll employees endpoint: PASSED")
        return employees
    
    def test_payroll_approved_entries_endpoint(self):
        """GET /api/am/payroll/approved-entries with date range"""
        headers = {"X-PAYROLL-KEY": PAYROLL_API_KEY}
        params = {"start": "2026-02-01", "end": "2026-02-15"}
        response = requests.get(
            f"{BASE_URL}/api/am/payroll/approved-entries",
            headers=headers,
            params=params
        )
        assert response.status_code == 200
        data = response.json()
        assert "entries" in data
        entries = data["entries"]
        print(f"Found {len(entries)} approved entries for Feb 1-15 2026")
        
        # Verify entry structure
        if entries:
            entry = entries[0]
            assert "employee_key" in entry
            assert "work_date" in entry
            assert "regular_hours" in entry
            print(f"Sample entry: employee_key={entry['employee_key']}, date={entry['work_date']}, hours={entry['regular_hours']}")
        print("Payroll approved-entries endpoint: PASSED")
        return entries
    
    def test_payroll_invalid_api_key(self):
        """Invalid API key returns 401"""
        headers = {"X-PAYROLL-KEY": "invalid-key"}
        response = requests.get(f"{BASE_URL}/api/am/payroll/employees", headers=headers)
        assert response.status_code == 401
        print("Invalid API key rejection: PASSED")
    
    def test_payroll_missing_api_key(self):
        """Missing API key returns 401/422"""
        response = requests.get(f"{BASE_URL}/api/am/payroll/employees")
        # Should be 401 or 422 (validation error for missing header)
        assert response.status_code in [401, 422]
        print("Missing API key rejection: PASSED")


class TestPayrollCanadaAuth:
    """Test Payroll Canada authentication"""
    
    @pytest.fixture
    def payroll_token(self):
        """Get authentication token for Payroll Canada admin"""
        response = requests.post(
            f"{BASE_URL}/api/auth/login",
            json=PAYROLL_ADMIN
        )
        if response.status_code == 200:
            return response.json().get("token")
        # Try registering if login fails
        response = requests.post(
            f"{BASE_URL}/api/auth/register",
            json={
                **PAYROLL_ADMIN,
                "role": "admin",
                "first_name": "Admin",
                "last_name": "User"
            }
        )
        if response.status_code == 200:
            return response.json().get("token")
        pytest.skip("Could not authenticate to Payroll Canada")
    
    def test_payroll_login(self, payroll_token):
        """Verify Payroll Canada login works"""
        assert payroll_token is not None
        print("Payroll Canada auth: PASSED")
        return payroll_token


class TestTimesheetPreviewIntegration:
    """Test Payroll Canada timesheet preview using Any Minute data"""
    
    @pytest.fixture
    def auth_headers(self):
        """Get auth headers for Payroll Canada"""
        response = requests.post(
            f"{BASE_URL}/api/auth/login",
            json=PAYROLL_ADMIN
        )
        if response.status_code != 200:
            # Try registering
            response = requests.post(
                f"{BASE_URL}/api/auth/register",
                json={
                    **PAYROLL_ADMIN,
                    "role": "admin",
                    "first_name": "Admin",
                    "last_name": "User"
                }
            )
        if response.status_code != 200:
            pytest.skip("Could not authenticate")
        token = response.json().get("token")
        return {"Authorization": f"Bearer {token}"}
    
    def test_get_pay_periods(self, auth_headers):
        """Get existing pay periods"""
        response = requests.get(
            f"{BASE_URL}/api/pay-periods",
            headers=auth_headers
        )
        assert response.status_code == 200
        periods = response.json()
        print(f"Found {len(periods)} pay periods")
        return periods
    
    def test_create_or_get_pay_period(self, auth_headers):
        """Create or get Feb 1-15 2026 pay period for testing"""
        # First check existing periods
        response = requests.get(
            f"{BASE_URL}/api/pay-periods",
            headers=auth_headers
        )
        assert response.status_code == 200
        periods = response.json()
        
        # Check if Feb 1-15 period exists
        target_period = None
        for p in periods:
            if p.get("start_date") == "2026-02-01" and p.get("end_date") == "2026-02-15":
                target_period = p
                break
        
        if not target_period:
            # Create it
            response = requests.post(
                f"{BASE_URL}/api/pay-periods",
                headers=auth_headers,
                json={
                    "start_date": "2026-02-01",
                    "end_date": "2026-02-15",
                    "pay_date": "2026-02-20"
                }
            )
            if response.status_code in [200, 201]:
                target_period = response.json()
                print(f"Created pay period: {target_period.get('id')}")
            else:
                print(f"Could not create period: {response.status_code} - {response.text}")
                pytest.skip("Could not create pay period")
        else:
            print(f"Using existing pay period: {target_period.get('id')}")
        
        return target_period
    
    def test_timesheet_preview(self, auth_headers):
        """Test timesheet preview fetches data from Any Minute"""
        # Get/create pay period
        response = requests.get(
            f"{BASE_URL}/api/pay-periods",
            headers=auth_headers
        )
        periods = response.json()
        
        # Find open period
        target_period = None
        for p in periods:
            if p.get("status") == "open":
                target_period = p
                break
        
        if not target_period:
            # Create one
            response = requests.post(
                f"{BASE_URL}/api/pay-periods",
                headers=auth_headers,
                json={
                    "start_date": "2026-02-01",
                    "end_date": "2026-02-15",
                    "pay_date": "2026-02-20"
                }
            )
            if response.status_code in [200, 201]:
                target_period = response.json()
        
        if not target_period:
            pytest.skip("No open pay period available")
        
        # Preview timesheets
        response = requests.post(
            f"{BASE_URL}/api/timesheets/preview",
            headers=auth_headers,
            json={"pay_period_id": target_period["id"]}
        )
        
        assert response.status_code == 200
        data = response.json()
        
        # Verify response structure
        assert "matched_entries" in data
        assert "unmatched_entries" in data
        assert "total_entries" in data
        
        print(f"Preview results:")
        print(f"  - Total entries: {data['total_entries']}")
        print(f"  - Matched: {len(data['matched_entries'])}")
        print(f"  - Unmatched: {len(data['unmatched_entries'])}")
        
        # If entries exist, verify they have expected fields
        if data['matched_entries']:
            entry = data['matched_entries'][0]
            assert "employee_key" in entry
            assert "work_date" in entry
            assert "regular_hours" in entry
            print(f"  - Sample matched entry: key={entry.get('employee_key')}, hours={entry.get('regular_hours')}")
        
        print("Timesheet preview: PASSED")
        return data


class TestTimesheetImportIntegration:
    """Test importing timesheets from Any Minute to Payroll Canada"""
    
    @pytest.fixture
    def auth_headers(self):
        """Get auth headers for Payroll Canada"""
        response = requests.post(
            f"{BASE_URL}/api/auth/login",
            json=PAYROLL_ADMIN
        )
        if response.status_code != 200:
            response = requests.post(
                f"{BASE_URL}/api/auth/register",
                json={
                    **PAYROLL_ADMIN,
                    "role": "admin",
                    "first_name": "Admin",
                    "last_name": "User"
                }
            )
        if response.status_code != 200:
            pytest.skip("Could not authenticate")
        token = response.json().get("token")
        return {"Authorization": f"Bearer {token}"}
    
    def test_timesheet_import(self, auth_headers):
        """Test importing timesheets from Any Minute"""
        # Get open pay period
        response = requests.get(
            f"{BASE_URL}/api/pay-periods",
            headers=auth_headers
        )
        periods = response.json()
        
        target_period = None
        for p in periods:
            if p.get("status") == "open":
                target_period = p
                break
        
        if not target_period:
            pytest.skip("No open pay period available")
        
        # Import timesheets
        response = requests.post(
            f"{BASE_URL}/api/timesheets/import",
            headers=auth_headers,
            json={"pay_period_id": target_period["id"]}
        )
        
        assert response.status_code == 200
        data = response.json()
        
        assert "success" in data
        assert "imported_count" in data
        assert "import_id" in data
        
        print(f"Import results:")
        print(f"  - Success: {data['success']}")
        print(f"  - Imported count: {data['imported_count']}")
        print(f"  - Import ID: {data['import_id']}")
        
        print("Timesheet import: PASSED")
        return data
    
    def test_get_time_entries_after_import(self, auth_headers):
        """Verify time entries exist after import"""
        # Get open pay period
        response = requests.get(
            f"{BASE_URL}/api/pay-periods",
            headers=auth_headers
        )
        periods = response.json()
        
        target_period = None
        for p in periods:
            if p.get("status") == "open":
                target_period = p
                break
        
        if not target_period:
            pytest.skip("No open pay period available")
        
        # Get time entries
        response = requests.get(
            f"{BASE_URL}/api/timesheets/entries/{target_period['id']}",
            headers=auth_headers
        )
        
        assert response.status_code == 200
        entries = response.json()
        
        print(f"Time entries after import: {len(entries)}")
        
        if entries:
            entry = entries[0]
            assert "employee_id" in entry
            assert "work_date" in entry
            assert "regular_hours" in entry
            print(f"  - Sample entry: emp={entry.get('employee_id')[:8]}..., date={entry.get('work_date')}, hours={entry.get('regular_hours')}")
        
        print("Get time entries: PASSED")
        return entries


class TestEmployeeMapping:
    """Test employee matching between Payroll Canada and Any Minute"""
    
    @pytest.fixture
    def auth_headers(self):
        """Get auth headers for Payroll Canada"""
        response = requests.post(
            f"{BASE_URL}/api/auth/login",
            json=PAYROLL_ADMIN
        )
        if response.status_code != 200:
            response = requests.post(
                f"{BASE_URL}/api/auth/register",
                json={
                    **PAYROLL_ADMIN,
                    "role": "admin",
                    "first_name": "Admin",
                    "last_name": "User"
                }
            )
        if response.status_code != 200:
            pytest.skip("Could not authenticate")
        token = response.json().get("token")
        return {"Authorization": f"Bearer {token}"}
    
    def test_get_employees(self, auth_headers):
        """Get Payroll Canada employees"""
        response = requests.get(
            f"{BASE_URL}/api/employees",
            headers=auth_headers
        )
        assert response.status_code == 200
        employees = response.json()
        print(f"Found {len(employees)} employees in Payroll Canada")
        
        # Check for Sarah Johnson with EMP001 key
        sarah = None
        for emp in employees:
            if emp.get("external_employee_key") == "EMP001":
                sarah = emp
                break
        
        if sarah:
            print(f"  - Found Sarah Johnson: key={sarah.get('external_employee_key')}, name={sarah.get('first_name')} {sarah.get('last_name')}")
        else:
            print("  - Sarah Johnson (EMP001) not found - may need to run demo/generate")
        
        return employees
    
    def test_generate_demo_employees(self, auth_headers):
        """Generate demo employees if needed"""
        response = requests.post(
            f"{BASE_URL}/api/demo/generate",
            headers=auth_headers
        )
        # May return error if employees already exist, that's OK
        if response.status_code == 200:
            data = response.json()
            print(f"Demo data generated: {data.get('employees_created', 0)} employees created")
        else:
            print(f"Demo generation: {response.status_code} - may already have employees")
        
        # Verify employees exist now
        response = requests.get(
            f"{BASE_URL}/api/employees",
            headers=auth_headers
        )
        employees = response.json()
        assert len(employees) > 0, "Should have employees after demo generation"
        
        # Check for EMP001
        has_emp001 = any(e.get("external_employee_key") == "EMP001" for e in employees)
        print(f"EMP001 mapping exists: {has_emp001}")
        
        return employees


class TestPayrollLockIntegration:
    """Test locking timesheets via Payroll lock endpoint"""
    
    def test_payroll_lock_endpoint(self):
        """POST /api/am/payroll/lock to mark timesheets as exported"""
        headers = {"X-PAYROLL-KEY": PAYROLL_API_KEY}
        params = {"start": "2026-02-01", "end": "2026-02-15"}
        
        response = requests.post(
            f"{BASE_URL}/api/am/payroll/lock",
            headers=headers,
            params=params
        )
        
        assert response.status_code == 200
        data = response.json()
        
        assert "success" in data
        assert "locked_count" in data
        
        print(f"Lock results:")
        print(f"  - Success: {data['success']}")
        print(f"  - Locked count: {data['locked_count']}")
        
        print("Payroll lock: PASSED")
        return data


class TestPayrollRunCalculation:
    """Test payroll calculation with imported timesheet data"""
    
    @pytest.fixture
    def auth_headers(self):
        """Get auth headers for Payroll Canada"""
        response = requests.post(
            f"{BASE_URL}/api/auth/login",
            json=PAYROLL_ADMIN
        )
        if response.status_code != 200:
            response = requests.post(
                f"{BASE_URL}/api/auth/register",
                json={
                    **PAYROLL_ADMIN,
                    "role": "admin",
                    "first_name": "Admin",
                    "last_name": "User"
                }
            )
        if response.status_code != 200:
            pytest.skip("Could not authenticate")
        token = response.json().get("token")
        return {"Authorization": f"Bearer {token}"}
    
    def test_create_payroll_run(self, auth_headers):
        """Create a payroll run for testing"""
        # Get open pay period
        response = requests.get(
            f"{BASE_URL}/api/pay-periods",
            headers=auth_headers
        )
        periods = response.json()
        
        target_period = None
        for p in periods:
            if p.get("status") == "open":
                target_period = p
                break
        
        if not target_period:
            pytest.skip("No open pay period available")
        
        # Create payroll run
        response = requests.post(
            f"{BASE_URL}/api/payroll-runs",
            headers=auth_headers,
            json={"pay_period_id": target_period["id"]}
        )
        
        assert response.status_code == 200
        run = response.json()
        
        assert "id" in run
        assert "status" in run
        assert run["status"] == "draft"
        
        print(f"Created payroll run: {run['id']}")
        return run
    
    def test_calculate_payroll(self, auth_headers):
        """Calculate payroll for the run"""
        # Get open pay period
        response = requests.get(
            f"{BASE_URL}/api/pay-periods",
            headers=auth_headers
        )
        periods = response.json()
        
        target_period = None
        for p in periods:
            if p.get("status") == "open":
                target_period = p
                break
        
        if not target_period:
            pytest.skip("No open pay period available")
        
        # Get or create payroll run
        response = requests.get(
            f"{BASE_URL}/api/payroll-runs",
            headers=auth_headers
        )
        runs = response.json()
        
        # Find draft run for this period
        target_run = None
        for run in runs:
            if run.get("pay_period_id") == target_period["id"] and run.get("status") == "draft":
                target_run = run
                break
        
        if not target_run:
            # Create one
            response = requests.post(
                f"{BASE_URL}/api/payroll-runs",
                headers=auth_headers,
                json={"pay_period_id": target_period["id"]}
            )
            if response.status_code == 200:
                target_run = response.json()
        
        if not target_run:
            pytest.skip("No payroll run available")
        
        # Calculate
        response = requests.post(
            f"{BASE_URL}/api/payroll-runs/{target_run['id']}/calculate",
            headers=auth_headers
        )
        
        assert response.status_code == 200
        data = response.json()
        
        assert "success" in data
        assert "lines_count" in data
        
        print(f"Calculation results:")
        print(f"  - Success: {data['success']}")
        print(f"  - Lines calculated: {data['lines_count']}")
        
        print("Payroll calculation: PASSED")
        return data
    
    def test_get_payroll_lines(self, auth_headers):
        """Get payroll lines after calculation"""
        # Get payroll runs
        response = requests.get(
            f"{BASE_URL}/api/payroll-runs",
            headers=auth_headers
        )
        runs = response.json()
        
        if not runs:
            pytest.skip("No payroll runs available")
        
        # Find calculated run
        target_run = None
        for run in runs:
            if run.get("status") == "calculated":
                target_run = run
                break
        
        if not target_run:
            # Try first run
            target_run = runs[0] if runs else None
        
        if not target_run:
            pytest.skip("No payroll run to get lines from")
        
        # Get lines
        response = requests.get(
            f"{BASE_URL}/api/payroll-runs/{target_run['id']}/lines",
            headers=auth_headers
        )
        
        assert response.status_code == 200
        lines = response.json()
        
        print(f"Payroll lines: {len(lines)}")
        
        if lines:
            line = lines[0]
            print(f"  - Sample line: emp={line.get('employee_name', 'N/A')}, gross={line.get('gross_pay', 0)}")
        
        return lines


if __name__ == "__main__":
    pytest.main([__file__, "-v", "--tb=short"])
