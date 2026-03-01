"""
Payroll Engine Module
Handles all payroll calculations for Canadian SMBs (Ontario focus for Phase 1)
"""
from typing import Dict, List, Any, Optional
from decimal import Decimal, ROUND_HALF_UP

class PayrollEngine:
    def __init__(self, company: Dict[str, Any]):
        self.company = company
        self.settings = company.get('settings_json', {})
        self.pay_frequency = company.get('pay_frequency', 'biweekly')
        self.vacation_percent = company.get('vacation_pay_percent_default', 4.0)
        
        # Pay periods per year
        self.periods_per_year = 26 if self.pay_frequency == 'biweekly' else 52
    
    def calculate_employee_pay(self, employee: Dict[str, Any], time_entries: List[Dict[str, Any]], ytd: Dict[str, Any]) -> Dict[str, Any]:
        """Calculate pay for a single employee"""
        
        # Calculate gross pay
        gross_pay = self._calculate_gross(employee, time_entries)
        
        # Calculate vacation pay
        vacation_pay = self._calculate_vacation_pay(gross_pay, employee)
        
        # Total gross including vacation
        total_gross = gross_pay + vacation_pay
        
        # Calculate deductions
        cpp = self._calculate_cpp(total_gross, ytd.get('ytd_cpp', 0))
        ei = self._calculate_ei(total_gross, ytd.get('ytd_ei', 0))
        federal_tax = self._calculate_federal_tax(total_gross)
        provincial_tax = self._calculate_ontario_tax(total_gross)
        
        # Calculate net pay
        total_deductions = cpp + ei + federal_tax + provincial_tax
        net_pay = total_gross - total_deductions
        
        return {
            "gross_pay": round(gross_pay, 2),
            "vacation_pay": round(vacation_pay, 2),
            "cpp": round(cpp, 2),
            "ei": round(ei, 2),
            "federal_tax": round(federal_tax, 2),
            "provincial_tax": round(provincial_tax, 2),
            "other_deductions": 0,
            "net_pay": round(net_pay, 2),
            "ytd_gross": round(ytd.get('ytd_gross', 0) + total_gross, 2),
            "ytd_cpp": round(ytd.get('ytd_cpp', 0) + cpp, 2),
            "ytd_ei": round(ytd.get('ytd_ei', 0) + ei, 2),
            "ytd_tax": round(ytd.get('ytd_tax', 0) + federal_tax + provincial_tax, 2),
            "regular_hours": sum(e.get('regular_hours', 0) for e in time_entries),
            "overtime_hours": sum(e.get('overtime_hours', 0) for e in time_entries)
        }
    
    def _calculate_gross(self, employee: Dict[str, Any], time_entries: List[Dict[str, Any]]) -> float:
        """Calculate gross pay based on employment type"""
        employment_type = employee.get('employment_type', 'hourly')
        
        if employment_type == 'salary':
            # Salary employees get fixed amount per period
            annual_salary = employee.get('annual_salary', 0)
            return annual_salary / self.periods_per_year
        else:
            # Hourly employees
            hourly_rate = employee.get('hourly_rate', self.company.get('default_hourly_rate', 20.0))
            
            regular_hours = sum(e.get('regular_hours', 0) for e in time_entries)
            overtime_hours = sum(e.get('overtime_hours', 0) for e in time_entries)
            
            regular_pay = regular_hours * hourly_rate
            overtime_pay = overtime_hours * hourly_rate * 1.5  # OT at 1.5x
            
            return regular_pay + overtime_pay
    
    def _calculate_vacation_pay(self, gross: float, employee: Dict[str, Any]) -> float:
        """Calculate vacation pay as percentage of gross"""
        # Only for hourly employees - salary typically includes vacation
        if employee.get('employment_type') == 'salary':
            return 0
        
        vacation_percent = self.vacation_percent / 100
        return gross * vacation_percent
    
    def _calculate_cpp(self, gross: float, ytd_cpp: float) -> float:
        """Calculate Canada Pension Plan contribution"""
        cpp_settings = self.settings.get('cpp', {
            'rate': 5.95,
            'basic_exemption_annual': 3500,
            'max_annual': 3867.50
        })
        
        rate = cpp_settings['rate'] / 100
        basic_exemption_per_period = cpp_settings['basic_exemption_annual'] / self.periods_per_year
        max_annual = cpp_settings['max_annual']
        
        # Check if already at max
        if ytd_cpp >= max_annual:
            return 0
        
        # Pensionable earnings
        pensionable = max(0, gross - basic_exemption_per_period)
        cpp = pensionable * rate
        
        # Cap at remaining annual max
        remaining = max_annual - ytd_cpp
        return min(cpp, remaining)
    
    def _calculate_ei(self, gross: float, ytd_ei: float) -> float:
        """Calculate Employment Insurance premium"""
        ei_settings = self.settings.get('ei', {
            'rate': 1.63,
            'max_annual': 1049.12
        })
        
        rate = ei_settings['rate'] / 100
        max_annual = ei_settings['max_annual']
        
        # Check if already at max
        if ytd_ei >= max_annual:
            return 0
        
        ei = gross * rate
        
        # Cap at remaining annual max
        remaining = max_annual - ytd_ei
        return min(ei, remaining)
    
    def _calculate_federal_tax(self, gross: float) -> float:
        """Calculate federal income tax using simplified brackets"""
        annual_income = gross * self.periods_per_year
        
        brackets = self.settings.get('federal_tax_brackets', [
            {"min": 0, "max": 55867, "rate": 15},
            {"min": 55867, "max": 111733, "rate": 20.5},
            {"min": 111733, "max": 173205, "rate": 26},
            {"min": 173205, "max": 246752, "rate": 29},
            {"min": 246752, "max": 999999999, "rate": 33}
        ])
        
        annual_tax = self._calculate_bracket_tax(annual_income, brackets)
        return annual_tax / self.periods_per_year
    
    def _calculate_ontario_tax(self, gross: float) -> float:
        """Calculate Ontario provincial income tax using simplified brackets"""
        annual_income = gross * self.periods_per_year
        
        brackets = self.settings.get('ontario_tax_brackets', [
            {"min": 0, "max": 51446, "rate": 5.05},
            {"min": 51446, "max": 102894, "rate": 9.15},
            {"min": 102894, "max": 150000, "rate": 11.16},
            {"min": 150000, "max": 220000, "rate": 12.16},
            {"min": 220000, "max": 999999999, "rate": 13.16}
        ])
        
        annual_tax = self._calculate_bracket_tax(annual_income, brackets)
        return annual_tax / self.periods_per_year
    
    def _calculate_bracket_tax(self, annual_income: float, brackets: List[Dict[str, Any]]) -> float:
        """Calculate tax based on progressive brackets"""
        tax = 0.0
        
        for bracket in brackets:
            min_amt = bracket['min']
            max_amt = bracket['max']
            rate = bracket['rate'] / 100
            
            if annual_income > min_amt:
                taxable_in_bracket = min(annual_income, max_amt) - min_amt
                tax += taxable_in_bracket * rate
        
        return tax
