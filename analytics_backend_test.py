#!/usr/bin/env python3

import requests
import sys
import json
from datetime import datetime

class PayrollAnalyticsAPITester:
    def __init__(self, base_url="https://smb-timesheet-hub.preview.emergentagent.com/api"):
        self.base_url = base_url
        self.token = None
        self.tests_run = 0
        self.tests_passed = 0
        self.failed_tests = []

    def run_test(self, name, method, endpoint, expected_status, data=None):
        """Run a single API test"""
        url = f"{self.base_url}/{endpoint}"
        headers = {'Content-Type': 'application/json'}
        if self.token:
            headers['Authorization'] = f'Bearer {self.token}'

        self.tests_run += 1
        print(f"\n🔍 Testing {name}...")
        print(f"   URL: {url}")
        
        try:
            if method == 'GET':
                response = requests.get(url, headers=headers, timeout=10)
            elif method == 'POST':
                response = requests.post(url, json=data, headers=headers, timeout=10)

            print(f"   Status: {response.status_code}")
            
            success = response.status_code == expected_status
            if success:
                self.tests_passed += 1
                print(f"✅ PASSED - Status: {response.status_code}")
                try:
                    response_data = response.json()
                    print(f"   Response keys: {list(response_data.keys()) if isinstance(response_data, dict) else type(response_data)}")
                    return True, response_data
                except:
                    return True, {}
            else:
                print(f"❌ FAILED - Expected {expected_status}, got {response.status_code}")
                try:
                    error_detail = response.json()
                    print(f"   Error: {error_detail}")
                except:
                    print(f"   Error text: {response.text}")
                self.failed_tests.append(f"{name}: Expected {expected_status}, got {response.status_code}")
                return False, {}

        except requests.exceptions.Timeout:
            print(f"❌ FAILED - Request timeout")
            self.failed_tests.append(f"{name}: Request timeout")
            return False, {}
        except Exception as e:
            print(f"❌ FAILED - Error: {str(e)}")
            self.failed_tests.append(f"{name}: {str(e)}")
            return False, {}

    def test_login(self, email, password):
        """Test login and get token"""
        success, response = self.run_test(
            "Admin Login",
            "POST",
            "auth/login",
            200,
            data={"email": email, "password": password}
        )
        if success and 'token' in response:
            self.token = response['token']
            print(f"   ✅ Token obtained")
            return True
        return False

    def test_analytics_summary(self):
        """Test analytics summary endpoint"""
        success, response = self.run_test(
            "Analytics Summary",
            "GET",
            "analytics/summary",
            200
        )
        if success:
            # Check expected fields
            expected_fields = ['total_payroll_runs', 'total_gross_paid', 'total_net_paid', 'total_deductions', 'active_employees', 'avg_payroll_per_run']
            missing_fields = [f for f in expected_fields if f not in response]
            if missing_fields:
                print(f"   ⚠️  Missing fields: {missing_fields}")
            else:
                print(f"   ✅ All expected fields present")
        return success

    def test_payroll_trends(self):
        """Test payroll trends endpoint"""
        success, response = self.run_test(
            "Payroll Trends",
            "GET",
            "analytics/payroll-trends",
            200
        )
        if success:
            print(f"   Data count: {len(response)}")
            if response and len(response) > 0:
                sample = response[0]
                expected_fields = ['period', 'pay_date', 'gross', 'deductions', 'net', 'employee_count']
                missing_fields = [f for f in expected_fields if f not in sample]
                if missing_fields:
                    print(f"   ⚠️  Missing fields in trend data: {missing_fields}")
                else:
                    print(f"   ✅ Trend data structure valid")
        return success

    def test_deductions_breakdown(self):
        """Test deductions breakdown endpoint"""
        success, response = self.run_test(
            "Deductions Breakdown",
            "GET",
            "analytics/deductions-breakdown",
            200
        )
        if success:
            print(f"   Breakdown items: {len(response)}")
            expected_names = {'CPP', 'EI', 'Federal Tax', 'Provincial Tax'}
            if response:
                actual_names = {item.get('name') for item in response}
                missing_names = expected_names - actual_names
                if missing_names:
                    print(f"   ⚠️  Missing deduction types: {missing_names}")
                else:
                    print(f"   ✅ All deduction types present: {actual_names}")
        return success

    def test_employee_costs(self):
        """Test employee costs endpoint"""
        success, response = self.run_test(
            "Employee Costs",
            "GET",
            "analytics/employee-costs",
            200
        )
        if success:
            print(f"   Employee records: {len(response)}")
            if response and len(response) > 0:
                sample = response[0]
                expected_fields = ['name', 'gross', 'deductions', 'net']
                missing_fields = [f for f in expected_fields if f not in sample]
                if missing_fields:
                    print(f"   ⚠️  Missing fields in employee cost data: {missing_fields}")
                else:
                    print(f"   ✅ Employee cost data structure valid")
        return success

    def test_cost_forecast(self):
        """Test cost forecast endpoint"""
        success, response = self.run_test(
            "Cost Forecast",
            "GET",
            "analytics/cost-forecast",
            200
        )
        if success:
            expected_top_level = ['monthly', 'annual_projection']
            missing_top = [f for f in expected_top_level if f not in response]
            if missing_top:
                print(f"   ⚠️  Missing top-level fields: {missing_top}")
            else:
                print(f"   ✅ Top-level structure valid")
                
                # Check annual projection fields
                if 'annual_projection' in response:
                    annual = response['annual_projection']
                    expected_annual = ['gross', 'cpp', 'ei', 'tax', 'net', 'employer_cost']
                    missing_annual = [f for f in expected_annual if f not in annual]
                    if missing_annual:
                        print(f"   ⚠️  Missing annual projection fields: {missing_annual}")
                    else:
                        print(f"   ✅ Annual projection fields complete")
                
                # Check monthly data
                if 'monthly' in response:
                    monthly = response['monthly']
                    print(f"   Monthly data points: {len(monthly)}")
                    if monthly and len(monthly) > 0:
                        sample_month = monthly[0]
                        expected_monthly = ['month', 'projected', 'actual']
                        missing_monthly = [f for f in expected_monthly if f not in sample_month]
                        if missing_monthly:
                            print(f"   ⚠️  Missing monthly fields: {missing_monthly}")
                        else:
                            print(f"   ✅ Monthly data structure valid")
        return success

def main():
    print("🚀 Starting Payroll Analytics API Testing")
    print("=" * 50)
    
    tester = PayrollAnalyticsAPITester()
    
    # Test login first
    if not tester.test_login("admin@test.com", "test123"):
        print("\n❌ Login failed - cannot test analytics endpoints")
        print("This may indicate:")
        print("- Backend server not running")
        print("- Invalid test credentials")
        print("- Database connection issues")
        return 1

    print(f"\n📊 Testing Analytics Endpoints")
    print("-" * 30)
    
    # Test all analytics endpoints
    tester.test_analytics_summary()
    tester.test_payroll_trends() 
    tester.test_deductions_breakdown()
    tester.test_employee_costs()
    tester.test_cost_forecast()

    # Print final results
    print(f"\n📈 TEST RESULTS")
    print("=" * 50)
    print(f"Tests Run: {tester.tests_run}")
    print(f"Tests Passed: {tester.tests_passed}")
    print(f"Tests Failed: {tester.tests_run - tester.tests_passed}")
    print(f"Success Rate: {(tester.tests_passed/tester.tests_run)*100:.1f}%")
    
    if tester.failed_tests:
        print(f"\n❌ Failed Tests:")
        for failure in tester.failed_tests:
            print(f"   - {failure}")
    
    if tester.tests_passed == tester.tests_run:
        print(f"\n🎉 All analytics API tests passed!")
        return 0
    else:
        print(f"\n⚠️  Some analytics API tests failed")
        return 1

if __name__ == "__main__":
    sys.exit(main())