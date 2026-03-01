"""
CSV Service Module
Generates CSV exports for payroll reports
"""
import csv
from io import StringIO
from typing import List, Dict, Any

class CSVService:
    def generate_payroll_summary(self, payroll_lines: List[Dict[str, Any]]) -> str:
        """Generate payroll summary CSV"""
        output = StringIO()
        writer = csv.writer(output)
        
        # Header
        writer.writerow([
            'Employee Name',
            'Email',
            'Regular Hours',
            'Overtime Hours',
            'Gross Pay',
            'Vacation Pay',
            'CPP',
            'EI',
            'Federal Tax',
            'Provincial Tax',
            'Total Deductions',
            'Net Pay',
            'YTD Gross',
            'YTD CPP',
            'YTD EI',
            'YTD Tax'
        ])
        
        # Data rows
        totals = {
            'gross': 0, 'vacation': 0, 'cpp': 0, 'ei': 0,
            'federal': 0, 'provincial': 0, 'net': 0
        }
        
        for line in payroll_lines:
            total_deductions = line.get('cpp', 0) + line.get('ei', 0) + line.get('federal_tax', 0) + line.get('provincial_tax', 0)
            
            writer.writerow([
                line.get('employee_name', ''),
                line.get('employee_email', ''),
                line.get('regular_hours', 0),
                line.get('overtime_hours', 0),
                f"{line.get('gross_pay', 0):.2f}",
                f"{line.get('vacation_pay', 0):.2f}",
                f"{line.get('cpp', 0):.2f}",
                f"{line.get('ei', 0):.2f}",
                f"{line.get('federal_tax', 0):.2f}",
                f"{line.get('provincial_tax', 0):.2f}",
                f"{total_deductions:.2f}",
                f"{line.get('net_pay', 0):.2f}",
                f"{line.get('ytd_gross', 0):.2f}",
                f"{line.get('ytd_cpp', 0):.2f}",
                f"{line.get('ytd_ei', 0):.2f}",
                f"{line.get('ytd_tax', 0):.2f}"
            ])
            
            totals['gross'] += line.get('gross_pay', 0)
            totals['vacation'] += line.get('vacation_pay', 0)
            totals['cpp'] += line.get('cpp', 0)
            totals['ei'] += line.get('ei', 0)
            totals['federal'] += line.get('federal_tax', 0)
            totals['provincial'] += line.get('provincial_tax', 0)
            totals['net'] += line.get('net_pay', 0)
        
        # Totals row
        total_all_deductions = totals['cpp'] + totals['ei'] + totals['federal'] + totals['provincial']
        writer.writerow([
            'TOTALS', '', '', '',
            f"{totals['gross']:.2f}",
            f"{totals['vacation']:.2f}",
            f"{totals['cpp']:.2f}",
            f"{totals['ei']:.2f}",
            f"{totals['federal']:.2f}",
            f"{totals['provincial']:.2f}",
            f"{total_all_deductions:.2f}",
            f"{totals['net']:.2f}",
            '', '', '', ''
        ])
        
        return output.getvalue()
    
    def generate_deductions_summary(self, payroll_lines: List[Dict[str, Any]]) -> str:
        """Generate deductions summary CSV"""
        output = StringIO()
        writer = csv.writer(output)
        
        # Header
        writer.writerow([
            'Employee Name',
            'CPP',
            'EI',
            'Federal Tax',
            'Provincial Tax',
            'Other Deductions',
            'Total Deductions'
        ])
        
        totals = {'cpp': 0, 'ei': 0, 'federal': 0, 'provincial': 0, 'other': 0}
        
        for line in payroll_lines:
            total = (line.get('cpp', 0) + line.get('ei', 0) + 
                    line.get('federal_tax', 0) + line.get('provincial_tax', 0) + 
                    line.get('other_deductions', 0))
            
            writer.writerow([
                line.get('employee_name', ''),
                f"{line.get('cpp', 0):.2f}",
                f"{line.get('ei', 0):.2f}",
                f"{line.get('federal_tax', 0):.2f}",
                f"{line.get('provincial_tax', 0):.2f}",
                f"{line.get('other_deductions', 0):.2f}",
                f"{total:.2f}"
            ])
            
            totals['cpp'] += line.get('cpp', 0)
            totals['ei'] += line.get('ei', 0)
            totals['federal'] += line.get('federal_tax', 0)
            totals['provincial'] += line.get('provincial_tax', 0)
            totals['other'] += line.get('other_deductions', 0)
        
        # Totals row
        grand_total = sum(totals.values())
        writer.writerow([
            'TOTALS',
            f"{totals['cpp']:.2f}",
            f"{totals['ei']:.2f}",
            f"{totals['federal']:.2f}",
            f"{totals['provincial']:.2f}",
            f"{totals['other']:.2f}",
            f"{grand_total:.2f}"
        ])
        
        return output.getvalue()
