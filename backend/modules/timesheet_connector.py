"""
Timesheet Connector Module
Integrates with Any Minute Timesheet API to fetch approved time entries for payroll.
Falls back to mock data only when USE_MOCK=true is explicitly set.
"""
import httpx
import os
from datetime import datetime, timedelta
import random
from typing import List, Dict, Any
import uuid


class TimesheetConnectorError(Exception):
    """Custom exception for timesheet connector errors"""
    pass


class TimesheetConnector:
    def __init__(self, config: Dict[str, Any] = None):
        config = config or {}
        
        # Any Minute API configuration from environment
        self.base_url = os.environ.get('ANYMINUTE_BASE_URL', config.get('api_base_url', ''))
        self.payroll_key = os.environ.get('ANYMINUTE_PAYROLL_KEY', config.get('api_key_value', ''))
        self.timeout = int(os.environ.get('ANYMINUTE_TIMEOUT_SECONDS', '15'))
        
        # Only use mock if USE_MOCK=true is explicitly set
        use_mock_env = os.environ.get('USE_MOCK', 'false').lower()
        self.use_mock = use_mock_env == 'true'
    
    def _get_headers(self) -> Dict[str, str]:
        """Get authentication headers for Any Minute API"""
        return {
            'Content-Type': 'application/json',
            'X-PAYROLL-KEY': self.payroll_key
        }
    
    async def fetch_approved_entries(self, start_date: str, end_date: str) -> List[Dict[str, Any]]:
        """Fetch approved timesheet entries from Any Minute API"""
        if self.use_mock:
            return self._generate_mock_entries(start_date, end_date)
        
        if not self.payroll_key:
            raise TimesheetConnectorError(
                "Timesheet import failed: ANYMINUTE_PAYROLL_KEY not configured. "
                "Please set the payroll API key from Any Minute Settings."
            )
        
        if not self.base_url:
            raise TimesheetConnectorError(
                "Timesheet import failed: ANYMINUTE_BASE_URL not configured."
            )
        
        return await self._fetch_from_anyminute(start_date, end_date)
    
    async def _fetch_from_anyminute(self, start_date: str, end_date: str) -> List[Dict[str, Any]]:
        """Fetch approved entries from Any Minute payroll API"""
        url = f"{self.base_url}/api/am/payroll/approved-entries"
        params = {'start': start_date, 'end': end_date}
        
        try:
            async with httpx.AsyncClient(timeout=float(self.timeout)) as client:
                response = await client.get(url, headers=self._get_headers(), params=params)
                
                if response.status_code == 401:
                    raise TimesheetConnectorError(
                        "Timesheet import failed: Invalid or expired payroll API key. "
                        "Please check the X-PAYROLL-KEY in Any Minute Settings."
                    )
                
                if response.status_code != 200:
                    raise TimesheetConnectorError(
                        f"Timesheet import failed: Any Minute API returned status {response.status_code}. "
                        f"Response: {response.text[:200]}"
                    )
                
                data = response.json()
                entries = data.get('entries', [])
                
                # Transform entries to match expected format for Payroll Canada
                result = []
                for entry in entries:
                    result.append({
                        "source_ref": entry.get('source_ref', ''),
                        "employee_key": entry.get('employee_key', ''),
                        "employee_email": entry.get('employee_email', ''),
                        "work_date": entry.get('work_date', ''),
                        "regular_hours": entry.get('regular_hours', 0),
                        "overtime_hours": entry.get('overtime_hours', 0),
                        "status": "approved",
                        "notes": entry.get('notes', '')
                    })
                
                return result
                
        except httpx.TimeoutException:
            raise TimesheetConnectorError(
                f"Timesheet import failed: Connection to Any Minute API timed out after {self.timeout}s."
            )
        except httpx.RequestError as e:
            raise TimesheetConnectorError(
                f"Timesheet import failed: Unable to connect to Any Minute API. Error: {str(e)}"
            )
    
    async def lock_timesheets(self, start_date: str, end_date: str, employee_keys: List[str] = None, reason: str = None) -> bool:
        """Lock timesheets in Any Minute system after payroll export"""
        if self.use_mock:
            return True
        
        if not self.payroll_key or not self.base_url:
            raise TimesheetConnectorError(
                "Timesheet lock failed: API configuration missing."
            )
        
        url = f"{self.base_url}/api/am/payroll/lock"
        params = {'start': start_date, 'end': end_date}
        
        try:
            async with httpx.AsyncClient(timeout=float(self.timeout)) as client:
                response = await client.post(url, headers=self._get_headers(), params=params)
                
                if response.status_code == 401:
                    raise TimesheetConnectorError(
                        "Timesheet lock failed: Invalid or expired payroll API key."
                    )
                
                if response.status_code != 200:
                    raise TimesheetConnectorError(
                        f"Timesheet lock failed: Any Minute API returned status {response.status_code}."
                    )
                
                data = response.json()
                return data.get('success', False)
                
        except httpx.TimeoutException:
            raise TimesheetConnectorError(
                f"Timesheet lock failed: Connection to Any Minute API timed out."
            )
        except httpx.RequestError as e:
            raise TimesheetConnectorError(
                f"Timesheet lock failed: Unable to connect to Any Minute API. Error: {str(e)}"
            )
    
    async def fetch_employees(self) -> List[Dict[str, Any]]:
        """Fetch employee list from Any Minute system"""
        if self.use_mock:
            return self._generate_mock_employees()
        
        if not self.payroll_key or not self.base_url:
            raise TimesheetConnectorError(
                "Employee fetch failed: API configuration missing."
            )
        
        url = f"{self.base_url}/api/am/payroll/employees"
        
        try:
            async with httpx.AsyncClient(timeout=float(self.timeout)) as client:
                response = await client.get(url, headers=self._get_headers())
                
                if response.status_code == 401:
                    raise TimesheetConnectorError(
                        "Employee fetch failed: Invalid or expired payroll API key."
                    )
                
                if response.status_code != 200:
                    raise TimesheetConnectorError(
                        f"Employee fetch failed: Any Minute API returned status {response.status_code}."
                    )
                
                data = response.json()
                employees = data.get('employees', [])
                
                # Transform to expected format
                result = []
                for emp in employees:
                    result.append({
                        "key": emp.get('external_employee_key', ''),
                        "name": f"{emp.get('first_name', '')} {emp.get('last_name', '')}".strip(),
                        "email": emp.get('email', '')
                    })
                
                return result
                
        except httpx.TimeoutException:
            raise TimesheetConnectorError(
                f"Employee fetch failed: Connection to Any Minute API timed out."
            )
        except httpx.RequestError as e:
            raise TimesheetConnectorError(
                f"Employee fetch failed: Unable to connect to Any Minute API. Error: {str(e)}"
            )
    
    def _generate_mock_employees(self) -> List[Dict[str, Any]]:
        """Generate mock employee list for testing"""
        return [
            {"key": "EMP001", "name": "Sarah Johnson", "email": "sarah.johnson@example.com"},
            {"key": "EMP002", "name": "Michael Chen", "email": "michael.chen@example.com"},
            {"key": "EMP003", "name": "Emily Williams", "email": "emily.williams@example.com"},
            {"key": "EMP004", "name": "David Brown", "email": "david.brown@example.com"},
            {"key": "EMP005", "name": "Jessica Martinez", "email": "jessica.martinez@example.com"},
            {"key": "EMP006", "name": "James Wilson", "email": "james.wilson@example.com"},
            {"key": "EMP007", "name": "Amanda Taylor", "email": "amanda.taylor@example.com"},
            {"key": "EMP008", "name": "Robert Anderson", "email": "robert.anderson@example.com"},
        ]
    
    def _generate_mock_entries(self, start_date: str, end_date: str) -> List[Dict[str, Any]]:
        """Generate realistic mock timesheet entries for testing"""
        mock_employees = self._generate_mock_employees()
        
        entries = []
        start = datetime.strptime(start_date, '%Y-%m-%d')
        end = datetime.strptime(end_date, '%Y-%m-%d')
        
        current = start
        while current <= end:
            # Skip weekends
            if current.weekday() < 5:  # Monday = 0, Friday = 4
                for emp in mock_employees:
                    # Randomly skip some days for realism (about 5% absence)
                    if random.random() > 0.05:
                        regular_hours = 8.0
                        
                        # Add some overtime randomly (about 20% of days)
                        overtime_hours = 0.0
                        if random.random() < 0.2:
                            overtime_hours = random.choice([1.0, 1.5, 2.0, 2.5, 3.0])
                        
                        entries.append({
                            "source_ref": f"TS-{str(uuid.uuid4())[:8]}",
                            "employee_key": emp["key"],
                            "employee_name": emp["name"],
                            "employee_email": emp["email"],
                            "work_date": current.strftime('%Y-%m-%d'),
                            "regular_hours": regular_hours,
                            "overtime_hours": overtime_hours,
                            "status": "approved",
                            "notes": ""
                        })
            
            current += timedelta(days=1)
        
        return entries
