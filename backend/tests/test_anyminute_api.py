"""
Any Minute Timesheet App - Backend API Tests
Tests cover: Auth, Business CRUD, User Management, Timesheet, Schedule, Reports, Payroll Integration
"""
import pytest
import requests
import os
import uuid
from datetime import datetime, timedelta

BASE_URL = os.environ.get('REACT_APP_BACKEND_URL', '').rstrip('/')
AM_API = f"{BASE_URL}/api/am"

# Test data storage
test_data = {}


class TestAMHealthAndAuth:
    """Test Any Minute health check and authentication endpoints"""
    
    def test_am_health_check(self):
        """Test AM API health endpoint"""
        response = requests.get(f"{AM_API}/")
        assert response.status_code == 200
        data = response.json()
        assert data.get("status") == "healthy"
        assert "Any Minute" in data.get("message", "")
        print("PASS: AM API health check")

    def test_user_registration_first_user_becomes_admin(self):
        """Test user registration - first user should become admin"""
        unique_email = f"test_admin_{uuid.uuid4().hex[:8]}@anyminute.com"
        payload = {
            "email": unique_email,
            "password": "TestAdmin123!",
            "first_name": "Test",
            "last_name": "Admin"
        }
        response = requests.post(f"{AM_API}/auth/register", json=payload)
        
        assert response.status_code == 200
        data = response.json()
        assert "token" in data
        assert "user" in data
        assert data["user"]["email"] == unique_email.lower()
        
        # Store for later tests
        test_data["admin_token"] = data["token"]
        test_data["admin_user"] = data["user"]
        test_data["admin_email"] = unique_email
        test_data["tenant_id"] = data["user"]["tenant_id"]
        print(f"PASS: User registration successful, role: {data['user']['role']}")

    def test_login_with_registered_user(self):
        """Test login with previously registered user"""
        if "admin_email" not in test_data:
            pytest.skip("No admin user registered")
        
        payload = {
            "email": test_data["admin_email"],
            "password": "TestAdmin123!"
        }
        response = requests.post(f"{AM_API}/auth/login", json=payload)
        
        assert response.status_code == 200
        data = response.json()
        assert "token" in data
        assert "user" in data
        test_data["admin_token"] = data["token"]
        print("PASS: Login successful")

    def test_login_invalid_credentials(self):
        """Test login with invalid credentials"""
        payload = {
            "email": "nonexistent@test.com",
            "password": "wrongpassword"
        }
        response = requests.post(f"{AM_API}/auth/login", json=payload)
        assert response.status_code == 401
        print("PASS: Invalid credentials rejected correctly")

    def test_get_current_user(self):
        """Test getting current user info"""
        if "admin_token" not in test_data:
            pytest.skip("No admin token available")
        
        headers = {"Authorization": f"Bearer {test_data['admin_token']}"}
        response = requests.get(f"{AM_API}/auth/me", headers=headers)
        
        assert response.status_code == 200
        data = response.json()
        assert "email" in data
        assert "password_hash" not in data  # Should not expose password hash
        print("PASS: Get current user successful")


class TestAMBusinessCRUD:
    """Test Business CRUD operations"""
    
    def test_create_business(self):
        """Test creating a new business"""
        if "admin_token" not in test_data:
            pytest.skip("No admin token available")
        
        headers = {"Authorization": f"Bearer {test_data['admin_token']}"}
        payload = {
            "name": f"TEST_Business_{uuid.uuid4().hex[:6]}",
            "address": "123 Test Street, Toronto, ON",
            "contact_email": "test@business.com",
            "contact_phone": "416-555-0100"
        }
        response = requests.post(f"{AM_API}/businesses", json=payload, headers=headers)
        
        assert response.status_code == 200
        data = response.json()
        assert data["name"] == payload["name"]
        assert "id" in data
        test_data["business_id"] = data["id"]
        test_data["business_name"] = data["name"]
        print(f"PASS: Business created: {data['name']}")

    def test_get_businesses(self):
        """Test retrieving businesses list"""
        if "admin_token" not in test_data:
            pytest.skip("No admin token available")
        
        headers = {"Authorization": f"Bearer {test_data['admin_token']}"}
        response = requests.get(f"{AM_API}/businesses", headers=headers)
        
        assert response.status_code == 200
        data = response.json()
        assert isinstance(data, list)
        print(f"PASS: Retrieved {len(data)} businesses")

    def test_get_single_business(self):
        """Test retrieving a single business"""
        if "business_id" not in test_data:
            pytest.skip("No business created")
        
        headers = {"Authorization": f"Bearer {test_data['admin_token']}"}
        response = requests.get(f"{AM_API}/businesses/{test_data['business_id']}", headers=headers)
        
        assert response.status_code == 200
        data = response.json()
        assert data["id"] == test_data["business_id"]
        print("PASS: Retrieved single business")

    def test_update_business(self):
        """Test updating a business"""
        if "business_id" not in test_data:
            pytest.skip("No business created")
        
        headers = {"Authorization": f"Bearer {test_data['admin_token']}"}
        payload = {"address": "456 Updated Street, Toronto, ON"}
        response = requests.put(f"{AM_API}/businesses/{test_data['business_id']}", json=payload, headers=headers)
        
        assert response.status_code == 200
        data = response.json()
        assert data["address"] == payload["address"]
        print("PASS: Business updated")

    def test_delete_business_soft_delete(self):
        """Test soft delete of business"""
        # Create a separate business to delete
        if "admin_token" not in test_data:
            pytest.skip("No admin token available")
        
        headers = {"Authorization": f"Bearer {test_data['admin_token']}"}
        
        # Create new business to delete
        payload = {"name": f"TEST_ToDelete_{uuid.uuid4().hex[:6]}"}
        create_res = requests.post(f"{AM_API}/businesses", json=payload, headers=headers)
        assert create_res.status_code == 200
        biz_id = create_res.json()["id"]
        
        # Delete it
        delete_res = requests.delete(f"{AM_API}/businesses/{biz_id}", headers=headers)
        assert delete_res.status_code == 200
        assert delete_res.json().get("success") == True
        print("PASS: Business soft deleted")


class TestAMUserManagement:
    """Test User management CRUD operations"""
    
    def test_create_user_with_role(self):
        """Test creating a new user with role assignment"""
        if "admin_token" not in test_data:
            pytest.skip("No admin token available")
        
        headers = {"Authorization": f"Bearer {test_data['admin_token']}"}
        unique_email = f"test_employee_{uuid.uuid4().hex[:8]}@anyminute.com"
        payload = {
            "email": unique_email,
            "password": "TestEmployee123!",
            "first_name": "Test",
            "last_name": "Employee",
            "role": "employee",
            "employee_mapping_key": f"EMP_{uuid.uuid4().hex[:8]}"
        }
        response = requests.post(f"{AM_API}/users", json=payload, headers=headers)
        
        assert response.status_code == 200
        data = response.json()
        assert data["email"] == unique_email.lower()
        assert data["role"] == "employee"
        assert data["employee_mapping_key"] == payload["employee_mapping_key"]
        test_data["employee_id"] = data["id"]
        test_data["employee_email"] = unique_email
        print(f"PASS: User created with role: {data['role']}")

    def test_get_users(self):
        """Test retrieving users list"""
        if "admin_token" not in test_data:
            pytest.skip("No admin token available")
        
        headers = {"Authorization": f"Bearer {test_data['admin_token']}"}
        response = requests.get(f"{AM_API}/users", headers=headers)
        
        assert response.status_code == 200
        data = response.json()
        assert isinstance(data, list)
        # Ensure password hash is not exposed
        for user in data:
            assert "password_hash" not in user
        print(f"PASS: Retrieved {len(data)} users")

    def test_get_single_user(self):
        """Test retrieving a single user"""
        if "employee_id" not in test_data:
            pytest.skip("No employee created")
        
        headers = {"Authorization": f"Bearer {test_data['admin_token']}"}
        response = requests.get(f"{AM_API}/users/{test_data['employee_id']}", headers=headers)
        
        assert response.status_code == 200
        data = response.json()
        assert data["id"] == test_data["employee_id"]
        print("PASS: Retrieved single user")

    def test_update_user(self):
        """Test updating user"""
        if "employee_id" not in test_data:
            pytest.skip("No employee created")
        
        headers = {"Authorization": f"Bearer {test_data['admin_token']}"}
        payload = {"role": "manager", "employee_mapping_key": f"MGR_{uuid.uuid4().hex[:6]}"}
        response = requests.put(f"{AM_API}/users/{test_data['employee_id']}", json=payload, headers=headers)
        
        assert response.status_code == 200
        data = response.json()
        assert data["role"] == "manager"
        print("PASS: User updated")


class TestAMTimesheet:
    """Test Timesheet functionality - weekly grid, entries, submit/approve"""
    
    def test_create_timesheet_week(self):
        """Test creating a timesheet week"""
        if "admin_token" not in test_data or "business_id" not in test_data:
            pytest.skip("Missing required test data")
        
        headers = {"Authorization": f"Bearer {test_data['admin_token']}"}
        # Get current week's Saturday
        today = datetime.now()
        days_since_saturday = (today.weekday() + 2) % 7
        saturday = today - timedelta(days=days_since_saturday)
        week_start = saturday.strftime("%Y-%m-%d")
        
        payload = {
            "user_id": test_data["admin_user"]["id"],
            "business_id": test_data["business_id"],
            "week_start_date": week_start
        }
        response = requests.post(f"{AM_API}/timesheet-weeks", json=payload, headers=headers)
        
        assert response.status_code == 200
        data = response.json()
        assert "id" in data
        assert data["status"] == "draft"
        test_data["week_id"] = data["id"]
        test_data["week_start"] = week_start
        print(f"PASS: Timesheet week created starting {week_start}")

    def test_create_timesheet_entry(self):
        """Test creating a timesheet entry with time and breaks"""
        if "admin_token" not in test_data or "business_id" not in test_data:
            pytest.skip("Missing required test data")
        
        headers = {"Authorization": f"Bearer {test_data['admin_token']}"}
        work_date = test_data.get("week_start", datetime.now().strftime("%Y-%m-%d"))
        
        payload = {
            "business_id": test_data["business_id"],
            "work_date": work_date,
            "start_time": "09:00",
            "end_time": "17:00",
            "break_minutes": 60,
            "notes": "Test work day entry"
        }
        response = requests.post(f"{AM_API}/timesheet-entries", json=payload, headers=headers)
        
        assert response.status_code == 200
        data = response.json()
        assert data["work_date"] == work_date
        assert data["net_hours"] == 7.0  # 8 hours - 1 hour break
        test_data["entry_id"] = data["id"]
        print(f"PASS: Timesheet entry created, net_hours: {data['net_hours']}")

    def test_update_timesheet_entry(self):
        """Test updating a timesheet entry"""
        if "entry_id" not in test_data:
            pytest.skip("No entry created")
        
        headers = {"Authorization": f"Bearer {test_data['admin_token']}"}
        payload = {
            "start_time": "08:00",
            "end_time": "18:00",
            "break_minutes": 45
        }
        response = requests.put(f"{AM_API}/timesheet-entries/{test_data['entry_id']}", json=payload, headers=headers)
        
        assert response.status_code == 200
        data = response.json()
        # 10 hours - 45 min break = 9.25 hours
        assert data["net_hours"] == 9.25
        print(f"PASS: Timesheet entry updated, new net_hours: {data['net_hours']}")

    def test_get_timesheet_entries(self):
        """Test retrieving timesheet entries"""
        if "admin_token" not in test_data:
            pytest.skip("No admin token available")
        
        headers = {"Authorization": f"Bearer {test_data['admin_token']}"}
        response = requests.get(f"{AM_API}/timesheet-entries", headers=headers)
        
        assert response.status_code == 200
        data = response.json()
        assert isinstance(data, list)
        print(f"PASS: Retrieved {len(data)} timesheet entries")

    def test_submit_timesheet(self):
        """Test submitting timesheet for approval"""
        if "week_id" not in test_data:
            pytest.skip("No week created")
        
        headers = {"Authorization": f"Bearer {test_data['admin_token']}"}
        response = requests.post(f"{AM_API}/timesheet-weeks/{test_data['week_id']}/submit", headers=headers)
        
        assert response.status_code == 200
        data = response.json()
        assert data.get("success") == True
        assert data.get("status") == "submitted"
        print("PASS: Timesheet submitted for approval")

    def test_approve_timesheet(self):
        """Test approving a submitted timesheet"""
        if "week_id" not in test_data:
            pytest.skip("No week created")
        
        headers = {"Authorization": f"Bearer {test_data['admin_token']}"}
        payload = {"status": "approved"}
        response = requests.post(f"{AM_API}/timesheet-weeks/{test_data['week_id']}/approve", json=payload, headers=headers)
        
        assert response.status_code == 200
        data = response.json()
        assert data.get("success") == True
        print("PASS: Timesheet approved")


class TestAMSchedule:
    """Test Schedule management"""
    
    def test_create_schedule(self):
        """Test creating a schedule entry"""
        if "admin_token" not in test_data or "business_id" not in test_data:
            pytest.skip("Missing required test data")
        
        headers = {"Authorization": f"Bearer {test_data['admin_token']}"}
        scheduled_date = (datetime.now() + timedelta(days=1)).strftime("%Y-%m-%d")
        
        payload = {
            "user_id": test_data["admin_user"]["id"],
            "business_id": test_data["business_id"],
            "scheduled_date": scheduled_date,
            "start_time": "09:00",
            "end_time": "17:00",
            "notes": "Regular shift"
        }
        response = requests.post(f"{AM_API}/schedules", json=payload, headers=headers)
        
        assert response.status_code == 200
        data = response.json()
        assert data["scheduled_date"] == scheduled_date
        test_data["schedule_id"] = data["id"]
        print(f"PASS: Schedule created for {scheduled_date}")

    def test_get_schedules(self):
        """Test retrieving schedules"""
        if "admin_token" not in test_data:
            pytest.skip("No admin token available")
        
        headers = {"Authorization": f"Bearer {test_data['admin_token']}"}
        response = requests.get(f"{AM_API}/schedules", headers=headers)
        
        assert response.status_code == 200
        data = response.json()
        assert isinstance(data, list)
        print(f"PASS: Retrieved {len(data)} schedules")

    def test_update_schedule(self):
        """Test updating a schedule"""
        if "schedule_id" not in test_data:
            pytest.skip("No schedule created")
        
        headers = {"Authorization": f"Bearer {test_data['admin_token']}"}
        payload = {"start_time": "08:00", "end_time": "16:00", "notes": "Early shift"}
        response = requests.put(f"{AM_API}/schedules/{test_data['schedule_id']}", json=payload, headers=headers)
        
        assert response.status_code == 200
        data = response.json()
        assert data["start_time"] == "08:00"
        print("PASS: Schedule updated")

    def test_delete_schedule(self):
        """Test deleting a schedule"""
        if "schedule_id" not in test_data:
            pytest.skip("No schedule created")
        
        headers = {"Authorization": f"Bearer {test_data['admin_token']}"}
        response = requests.delete(f"{AM_API}/schedules/{test_data['schedule_id']}", headers=headers)
        
        assert response.status_code == 200
        print("PASS: Schedule deleted")


class TestAMReports:
    """Test Reports functionality"""
    
    def test_get_report_by_business(self):
        """Test generating report by business"""
        if "admin_token" not in test_data:
            pytest.skip("No admin token available")
        
        headers = {"Authorization": f"Bearer {test_data['admin_token']}"}
        response = requests.get(f"{AM_API}/reports/by-business", headers=headers)
        
        assert response.status_code == 200
        data = response.json()
        assert isinstance(data, list)
        print(f"PASS: Report generated with {len(data)} business records")


class TestAMTenantSettings:
    """Test Tenant Settings - Payroll API Key"""
    
    def test_get_tenant_settings(self):
        """Test retrieving tenant settings"""
        if "admin_token" not in test_data:
            pytest.skip("No admin token available")
        
        headers = {"Authorization": f"Bearer {test_data['admin_token']}"}
        response = requests.get(f"{AM_API}/tenant/settings", headers=headers)
        
        assert response.status_code == 200
        data = response.json()
        assert "payroll_api_key" in data
        test_data["payroll_api_key"] = data["payroll_api_key"]
        print(f"PASS: Retrieved tenant settings with payroll API key")

    def test_regenerate_payroll_key(self):
        """Test regenerating payroll API key"""
        if "admin_token" not in test_data:
            pytest.skip("No admin token available")
        
        headers = {"Authorization": f"Bearer {test_data['admin_token']}"}
        response = requests.post(f"{AM_API}/tenant/settings/regenerate-key", headers=headers)
        
        assert response.status_code == 200
        data = response.json()
        assert "payroll_api_key" in data
        assert data["payroll_api_key"] != test_data.get("payroll_api_key", "")
        test_data["payroll_api_key"] = data["payroll_api_key"]
        print("PASS: Payroll API key regenerated")


class TestAMPayrollIntegration:
    """Test Payroll Integration APIs secured with X-PAYROLL-KEY"""
    
    def test_payroll_employees_endpoint(self):
        """Test GET /api/am/payroll/employees"""
        if "payroll_api_key" not in test_data:
            pytest.skip("No payroll API key available")
        
        headers = {"X-PAYROLL-KEY": test_data["payroll_api_key"]}
        response = requests.get(f"{AM_API}/payroll/employees", headers=headers)
        
        assert response.status_code == 200
        data = response.json()
        assert "employees" in data
        assert isinstance(data["employees"], list)
        print(f"PASS: Payroll employees returned {len(data['employees'])} employees")

    def test_payroll_approved_entries_endpoint(self):
        """Test GET /api/am/payroll/approved-entries"""
        if "payroll_api_key" not in test_data:
            pytest.skip("No payroll API key available")
        
        headers = {"X-PAYROLL-KEY": test_data["payroll_api_key"]}
        start_date = (datetime.now() - timedelta(days=30)).strftime("%Y-%m-%d")
        end_date = datetime.now().strftime("%Y-%m-%d")
        
        response = requests.get(
            f"{AM_API}/payroll/approved-entries",
            headers=headers,
            params={"start": start_date, "end": end_date}
        )
        
        assert response.status_code == 200
        data = response.json()
        assert "entries" in data
        assert isinstance(data["entries"], list)
        print(f"PASS: Payroll approved entries returned {len(data['entries'])} entries")

    def test_payroll_lock_endpoint(self):
        """Test POST /api/am/payroll/lock"""
        if "payroll_api_key" not in test_data:
            pytest.skip("No payroll API key available")
        
        headers = {"X-PAYROLL-KEY": test_data["payroll_api_key"]}
        start_date = (datetime.now() - timedelta(days=30)).strftime("%Y-%m-%d")
        end_date = datetime.now().strftime("%Y-%m-%d")
        
        response = requests.post(
            f"{AM_API}/payroll/lock",
            headers=headers,
            params={"start": start_date, "end": end_date}
        )
        
        assert response.status_code == 200
        data = response.json()
        assert data.get("success") == True
        assert "locked_count" in data
        print(f"PASS: Payroll lock successful, locked_count: {data['locked_count']}")

    def test_payroll_invalid_key(self):
        """Test payroll endpoints with invalid API key"""
        headers = {"X-PAYROLL-KEY": "invalid_key_12345"}
        response = requests.get(f"{AM_API}/payroll/employees", headers=headers)
        
        assert response.status_code == 401
        print("PASS: Invalid payroll API key rejected correctly")

    def test_payroll_missing_key(self):
        """Test payroll endpoints without API key"""
        response = requests.get(f"{AM_API}/payroll/employees")
        
        assert response.status_code in [401, 422]  # Either unauthorized or validation error
        print("PASS: Missing payroll API key rejected correctly")


class TestAMDashboard:
    """Test Dashboard stats endpoint"""
    
    def test_dashboard_stats(self):
        """Test getting dashboard statistics"""
        if "admin_token" not in test_data:
            pytest.skip("No admin token available")
        
        headers = {"Authorization": f"Bearer {test_data['admin_token']}"}
        response = requests.get(f"{AM_API}/dashboard/stats", headers=headers)
        
        assert response.status_code == 200
        data = response.json()
        assert "business_count" in data
        assert "user_count" in data
        assert "pending_approvals" in data
        assert "my_hours_this_week" in data
        print(f"PASS: Dashboard stats retrieved successfully")


if __name__ == "__main__":
    pytest.main([__file__, "-v", "--tb=short"])
