#!/usr/bin/env python3

import requests
import sys
import json
from datetime import datetime, date, timedelta

class PayrollAnalyticsAPITester:
    def __init__(self, base_url="https://smb-timesheet-hub.preview.emergentagent.com/api"):
        self.base_url = base_url
        self.token = None
        self.tests_run = 0
        self.tests_passed = 0
        self.failed_tests = []

    def run_test(self, name, method, endpoint, expected_status, data=None, description=""):
        """Run a single API test"""
        url = f"{self.base_url}/{endpoint.lstrip('/')}"
        headers = {'Content-Type': 'application/json'}
        if self.token:
            headers['Authorization'] = f'Bearer {self.token}'

        self.tests_run += 1
        print(f"\n🔍 Test {self.tests_run}: {name}")
        if description:
            print(f"   {description}")
        
        try:
            if method == 'GET':
                response = requests.get(url, headers=headers, timeout=30)
            elif method == 'POST':
                response = requests.post(url, json=data, headers=headers, timeout=30)
            elif method == 'PUT':
                response = requests.put(url, json=data, headers=headers, timeout=30)
            elif method == 'DELETE':
                response = requests.delete(url, headers=headers, timeout=30)

            success = response.status_code == expected_status
            
            if success:
                self.tests_passed += 1
                print(f"✅ PASSED - Status: {response.status_code}")
                try:
                    return True, response.json() if response.text.strip() else {}
                except:
                    return True, {}
            else:
                print(f"❌ FAILED - Expected {expected_status}, got {response.status_code}")
                if response.text:
                    print(f"   Response: {response.text[:200]}")
                return False, {}

        except Exception as e:
            print(f"❌ FAILED - Error: {str(e)}")
            return False, {}

    def test_health_check(self):
        """Test API health"""
        return self.run_test(
            "API Health Check",
            "GET", 
            "/", 
            200,
            description="Verify API is accessible"
        )

    def test_register_admin(self):
        """Test admin registration (creates company)"""
        timestamp = datetime.now().strftime('%Y%m%d_%H%M%S')
        test_data = {
            "email": f"test_admin_{timestamp}@example.com",
            "password": "TestPass123!",
            "first_name": "Test",
            "last_name": "Admin",
            "role": "admin"
        }
        
        success, response = self.run_test(
            "Admin Registration",
            "POST",
            "/auth/register",
            200,
            data=test_data,
            description="Register admin user - should create company"
        )
        
        if success and 'token' in response:
            self.token = response['token']
            if 'user' in response:
                self.company_id = response['user'].get('company_id')
                self.user_id = response['user'].get('id')
                print(f"   🔑 Token acquired, Company ID: {self.company_id}")
            return True
        return False

    def test_login_existing_admin(self):
        """Test login with existing admin account"""
        test_data = {
            "email": "admin@test.com", 
            "password": "test123"
        }
        
        success, response = self.run_test(
            "Admin Login",
            "POST",
            "/auth/login",
            200,
            data=test_data,
            description="Login with test admin credentials"
        )
        
        if success and 'token' in response:
            self.token = response['token']
            if 'user' in response:
                self.company_id = response['user'].get('company_id')
                self.user_id = response['user'].get('id')
                print(f"   🔑 Token acquired, Company ID: {self.company_id}")
            return True
        return False

    def test_get_current_user(self):
        """Test getting current user info"""
        return self.run_test(
            "Get Current User",
            "GET",
            "/auth/me",
            200,
            description="Verify JWT token validation"
        )

    def test_dashboard_stats(self):
        """Test dashboard stats endpoint"""
        success, response = self.run_test(
            "Dashboard Stats",
            "GET",
            "/dashboard/stats",
            200,
            description="Get dashboard statistics"
        )
        
        if success:
            required_keys = ['employee_count', 'open_pay_periods', 'pending_imports']
            for key in required_keys:
                if key not in response:
                    print(f"   ⚠️ Warning: Missing key '{key}' in response")
                    
        return success

    def test_generate_demo_employees(self):
        """Test demo employee generation"""
        success, response = self.run_test(
            "Generate Demo Employees",
            "POST",
            "/demo/generate", 
            200,
            description="Create 8 sample employees"
        )
        
        if success and response.get('employees_created') == 8:
            print(f"   📝 Created {response['employees_created']} demo employees")
        
        return success

    def test_get_employees(self):
        """Test retrieving employees list"""
        success, response = self.run_test(
            "Get Employees",
            "GET",
            "/employees",
            200,
            description="Retrieve all company employees"
        )
        
        if success and isinstance(response, list):
            print(f"   👥 Found {len(response)} employees")
            if len(response) > 0:
                # Store first employee for later tests
                self.created_entities['employees'] = response
                
        return success

    def test_create_pay_period(self):
        """Test creating a pay period"""
        start_date = date.today().strftime('%Y-%m-%d')
        end_date = (date.today() + timedelta(days=13)).strftime('%Y-%m-%d')  # 2 week period
        pay_date = (date.today() + timedelta(days=16)).strftime('%Y-%m-%d')  # 3 days after end
        
        test_data = {
            "start_date": start_date,
            "end_date": end_date,
            "pay_date": pay_date
        }
        
        success, response = self.run_test(
            "Create Pay Period",
            "POST",
            "/pay-periods",
            200,
            data=test_data,
            description=f"Create pay period {start_date} to {end_date}"
        )
        
        if success and 'id' in response:
            self.created_entities['pay_periods'].append(response)
            print(f"   📅 Created pay period: {response['id']}")
            
        return success

    def test_get_pay_periods(self):
        """Test retrieving pay periods"""
        return self.run_test(
            "Get Pay Periods",
            "GET", 
            "/pay-periods",
            200,
            description="Retrieve all pay periods"
        )

    def test_timesheet_preview(self):
        """Test timesheet import preview"""
        if not self.created_entities['pay_periods']:
            print("   ⚠️ Skipping - No pay periods available")
            return False
            
        period_id = self.created_entities['pay_periods'][0]['id']
        test_data = {"pay_period_id": period_id}
        
        success, response = self.run_test(
            "Timesheet Import Preview",
            "POST",
            "/timesheets/preview",
            200,
            data=test_data,
            description="Preview mock timesheet entries"
        )
        
        if success:
            matched = response.get('matched_entries', [])
            unmatched = response.get('unmatched_entries', [])
            total = response.get('total_entries', 0)
            print(f"   📊 Preview: {len(matched)} matched, {len(unmatched)} unmatched, {total} total")
            
        return success

    def test_import_timesheets(self):
        """Test importing timesheets"""
        if not self.created_entities['pay_periods']:
            print("   ⚠️ Skipping - No pay periods available")
            return False
            
        period_id = self.created_entities['pay_periods'][0]['id']
        test_data = {"pay_period_id": period_id}
        
        success, response = self.run_test(
            "Import Timesheets",
            "POST",
            "/timesheets/import",
            200,
            data=test_data,
            description="Import timesheet entries to database"
        )
        
        if success:
            imported = response.get('imported_count', 0)
            print(f"   📥 Imported {imported} timesheet entries")
            
        return success

    def test_create_payroll_run(self):
        """Test creating a payroll run"""
        if not self.created_entities['pay_periods']:
            print("   ⚠️ Skipping - No pay periods available")
            return False
            
        period_id = self.created_entities['pay_periods'][0]['id']
        test_data = {"pay_period_id": period_id}
        
        success, response = self.run_test(
            "Create Payroll Run",
            "POST",
            "/payroll-runs",
            200,
            data=test_data,
            description="Create new payroll run"
        )
        
        if success and 'id' in response:
            self.created_entities['payroll_runs'].append(response)
            print(f"   🧮 Created payroll run: {response['id']}")
            
        return success

    def test_calculate_payroll(self):
        """Test payroll calculation"""
        if not self.created_entities['payroll_runs']:
            print("   ⚠️ Skipping - No payroll runs available")
            return False
            
        run_id = self.created_entities['payroll_runs'][0]['id']
        
        success, response = self.run_test(
            "Calculate Payroll",
            "POST", 
            f"/payroll-runs/{run_id}/calculate",
            200,
            description="Calculate payroll for all employees"
        )
        
        if success:
            lines = response.get('lines_count', 0)
            print(f"   💰 Calculated {lines} payroll lines")
            
        return success

    def test_get_payroll_lines(self):
        """Test retrieving payroll lines"""
        if not self.created_entities['payroll_runs']:
            print("   ⚠️ Skipping - No payroll runs available")
            return False
            
        run_id = self.created_entities['payroll_runs'][0]['id']
        
        success, response = self.run_test(
            "Get Payroll Lines",
            "GET",
            f"/payroll-runs/{run_id}/lines", 
            200,
            description="Retrieve calculated payroll lines"
        )
        
        if success and isinstance(response, list):
            print(f"   📋 Found {len(response)} payroll lines")
            # Check structure of first line
            if len(response) > 0:
                line = response[0]
                required_fields = ['gross_pay', 'cpp', 'ei', 'federal_tax', 'provincial_tax', 'net_pay']
                for field in required_fields:
                    if field not in line:
                        print(f"   ⚠️ Warning: Missing payroll field '{field}'")
                        
        return success

    def test_lock_payroll_run(self):
        """Test locking a payroll run"""
        if not self.created_entities['payroll_runs']:
            print("   ⚠️ Skipping - No payroll runs available")
            return False
            
        run_id = self.created_entities['payroll_runs'][0]['id']
        
        success, response = self.run_test(
            "Lock Payroll Run",
            "POST",
            f"/payroll-runs/{run_id}/lock",
            200,
            description="Lock payroll to prevent changes"
        )
        
        if success:
            print("   🔒 Payroll run locked successfully")
            
        return success

    def test_generate_payslips(self):
        """Test payslip generation"""
        if not self.created_entities['payroll_runs']:
            print("   ⚠️ Skipping - No payroll runs available")
            return False
            
        run_id = self.created_entities['payroll_runs'][0]['id']
        
        success, response = self.run_test(
            "Generate Payslips",
            "POST",
            f"/payroll-runs/{run_id}/generate-payslips",
            200, 
            description="Generate payslips after locking"
        )
        
        if success:
            count = response.get('count', 0)
            print(f"   📄 Generated {count} payslips")
            
        return success

    def test_get_payslips(self):
        """Test retrieving payslips"""
        return self.run_test(
            "Get Payslips",
            "GET",
            "/payslips",
            200,
            description="Retrieve generated payslips"
        )

    def test_export_payroll_summary(self):
        """Test CSV export functionality"""
        if not self.created_entities['payroll_runs']:
            print("   ⚠️ Skipping - No payroll runs available")
            return False
            
        run_id = self.created_entities['payroll_runs'][0]['id']
        
        # Test payroll summary export
        success, _ = self.run_test(
            "Export Payroll Summary CSV",
            "GET",
            f"/reports/payroll-summary/{run_id}",
            200,
            description="Export payroll summary as CSV"
        )
        
        return success

    def test_export_deductions_summary(self):
        """Test deductions CSV export"""
        if not self.created_entities['payroll_runs']:
            print("   ⚠️ Skipping - No payroll runs available")
            return False
            
        run_id = self.created_entities['payroll_runs'][0]['id']
        
        success, _ = self.run_test(
            "Export Deductions Summary CSV", 
            "GET",
            f"/reports/deductions-summary/{run_id}",
            200,
            description="Export deductions summary as CSV"
        )
        
        return success

    def test_audit_logs(self):
        """Test audit log retrieval"""
        success, response = self.run_test(
            "Get Audit Logs",
            "GET",
            "/audit-logs",
            200,
            description="Retrieve audit trail"
        )
        
        if success and isinstance(response, list):
            print(f"   📝 Found {len(response)} audit log entries")
            
        return success

def main():
    print("🚀 Starting Payroll Canada MVP API Tests")
    print("=" * 60)
    
    tester = PayrollAPITester()
    
    # Test sequence - order matters for dependent tests
    tests = [
        # Basic connectivity and auth
        ("API Health", tester.test_health_check),
        ("Admin Login", tester.test_login_existing_admin),
        ("Get Current User", tester.test_get_current_user),
        ("Dashboard Stats", tester.test_dashboard_stats),
        
        # Employee management
        ("Generate Demo Employees", tester.test_generate_demo_employees),
        ("Get Employees", tester.test_get_employees),
        
        # Pay period flow
        ("Create Pay Period", tester.test_create_pay_period),
        ("Get Pay Periods", tester.test_get_pay_periods),
        
        # Timesheet import flow
        ("Timesheet Preview", tester.test_timesheet_preview),
        ("Import Timesheets", tester.test_import_timesheets),
        
        # Payroll processing flow
        ("Create Payroll Run", tester.test_create_payroll_run),
        ("Calculate Payroll", tester.test_calculate_payroll),
        ("Get Payroll Lines", tester.test_get_payroll_lines),
        ("Lock Payroll Run", tester.test_lock_payroll_run),
        
        # Payslip generation
        ("Generate Payslips", tester.test_generate_payslips),
        ("Get Payslips", tester.test_get_payslips),
        
        # Reporting
        ("Export Payroll Summary", tester.test_export_payroll_summary),
        ("Export Deductions Summary", tester.test_export_deductions_summary),
        ("Audit Logs", tester.test_audit_logs),
    ]
    
    failed_tests = []
    
    for test_name, test_func in tests:
        try:
            success = test_func()
            if not success:
                failed_tests.append(test_name)
        except Exception as e:
            print(f"❌ FAILED - {test_name}: {str(e)}")
            failed_tests.append(test_name)
    
    # Summary
    print("\n" + "=" * 60)
    print("📊 TEST SUMMARY")
    print(f"Tests run: {tester.tests_run}")
    print(f"Passed: {tester.tests_passed}")
    print(f"Failed: {tester.tests_run - tester.tests_passed}")
    print(f"Success rate: {(tester.tests_passed / tester.tests_run * 100):.1f}%")
    
    if failed_tests:
        print("\n❌ Failed tests:")
        for test in failed_tests:
            print(f"  - {test}")
    else:
        print("\n✅ All tests passed!")
    
    return 0 if len(failed_tests) == 0 else 1

if __name__ == "__main__":
    sys.exit(main())