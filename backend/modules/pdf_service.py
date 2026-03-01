"""
PDF Service Module
Generates payslip PDFs using ReportLab
"""
from reportlab.lib import colors
from reportlab.lib.pagesizes import letter
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.lib.units import inch
from reportlab.platypus import SimpleDocTemplate, Table, TableStyle, Paragraph, Spacer, Image
from reportlab.lib.enums import TA_CENTER, TA_RIGHT, TA_LEFT
from io import BytesIO
from typing import Dict, Any

class PDFService:
    def __init__(self):
        self.styles = getSampleStyleSheet()
        self._setup_custom_styles()
    
    def _setup_custom_styles(self):
        """Setup custom paragraph styles"""
        # Use unique style names to avoid conflicts with built-in styles
        self.styles.add(ParagraphStyle(
            name='PayrollTitle',
            fontName='Helvetica-Bold',
            fontSize=18,
            alignment=TA_CENTER,
            spaceAfter=20
        ))
        self.styles.add(ParagraphStyle(
            name='PayrollSubTitle',
            fontName='Helvetica',
            fontSize=12,
            alignment=TA_CENTER,
            spaceAfter=10
        ))
        self.styles.add(ParagraphStyle(
            name='PayrollSectionHeader',
            fontName='Helvetica-Bold',
            fontSize=11,
            spaceBefore=15,
            spaceAfter=5
        ))
        self.styles.add(ParagraphStyle(
            name='PayrollDisclaimer',
            fontName='Helvetica-Oblique',
            fontSize=8,
            textColor=colors.gray,
            alignment=TA_CENTER,
            spaceBefore=20
        ))
    
    def generate_payslip(self, company: Dict[str, Any], employee: Dict[str, Any], 
                        period: Dict[str, Any], payroll_line: Dict[str, Any]) -> bytes:
        """Generate a PDF payslip"""
        buffer = BytesIO()
        doc = SimpleDocTemplate(buffer, pagesize=letter, 
                               rightMargin=0.75*inch, leftMargin=0.75*inch,
                               topMargin=0.75*inch, bottomMargin=0.75*inch)
        
        story = []
        
        # Company Header
        story.append(Paragraph(company.get('name', 'Company'), self.styles['PayrollTitle']))
        story.append(Paragraph("PAYSLIP", self.styles['PayrollSubTitle']))
        story.append(Spacer(1, 0.25*inch))
        
        # Employee & Pay Period Info
        info_data = [
            ["Employee:", f"{employee.get('first_name', '')} {employee.get('last_name', '')}", 
             "Pay Period:", f"{period.get('start_date', '')} to {period.get('end_date', '')}"],
            ["Email:", employee.get('email', ''), "Pay Date:", period.get('pay_date', '')],
            ["Employment Type:", employee.get('employment_type', '').capitalize(), "Province:", company.get('province', 'ON')]
        ]
        
        info_table = Table(info_data, colWidths=[1.25*inch, 2.25*inch, 1.25*inch, 2.25*inch])
        info_table.setStyle(TableStyle([
            ('FONTNAME', (0, 0), (0, -1), 'Helvetica-Bold'),
            ('FONTNAME', (2, 0), (2, -1), 'Helvetica-Bold'),
            ('FONTSIZE', (0, 0), (-1, -1), 10),
            ('BOTTOMPADDING', (0, 0), (-1, -1), 8),
            ('TOPPADDING', (0, 0), (-1, -1), 4),
        ]))
        story.append(info_table)
        story.append(Spacer(1, 0.3*inch))
        
        # Earnings Section
        story.append(Paragraph("EARNINGS", self.styles['PayrollSectionHeader']))
        
        earnings_data = [["Description", "Hours", "Rate", "Amount"]]
        
        regular_hours = payroll_line.get('regular_hours', 0)
        overtime_hours = payroll_line.get('overtime_hours', 0)
        hourly_rate = employee.get('hourly_rate', 0)
        
        if employee.get('employment_type') == 'salary':
            annual_salary = employee.get('annual_salary', 0)
            periods = 26 if company.get('pay_frequency') == 'biweekly' else 52
            period_salary = annual_salary / periods
            earnings_data.append(["Salary", "-", f"${annual_salary:,.2f}/yr", f"${period_salary:,.2f}"])
        else:
            if regular_hours > 0:
                regular_pay = regular_hours * hourly_rate
                earnings_data.append(["Regular Pay", f"{regular_hours:.1f}", f"${hourly_rate:.2f}", f"${regular_pay:.2f}"])
            
            if overtime_hours > 0:
                ot_rate = hourly_rate * 1.5
                ot_pay = overtime_hours * ot_rate
                earnings_data.append(["Overtime Pay (1.5x)", f"{overtime_hours:.1f}", f"${ot_rate:.2f}", f"${ot_pay:.2f}"])
        
        vacation_pay = payroll_line.get('vacation_pay', 0)
        if vacation_pay > 0:
            earnings_data.append(["Vacation Pay (4%)", "-", "-", f"${vacation_pay:.2f}"])
        
        gross_pay = payroll_line.get('gross_pay', 0) + vacation_pay
        earnings_data.append(["", "", "Gross Pay:", f"${gross_pay:.2f}"])
        
        earnings_table = Table(earnings_data, colWidths=[3*inch, 1*inch, 1.5*inch, 1.5*inch])
        earnings_table.setStyle(TableStyle([
            ('BACKGROUND', (0, 0), (-1, 0), colors.HexColor('#1a237e')),
            ('TEXTCOLOR', (0, 0), (-1, 0), colors.white),
            ('FONTNAME', (0, 0), (-1, 0), 'Helvetica-Bold'),
            ('FONTNAME', (0, -1), (-1, -1), 'Helvetica-Bold'),
            ('FONTSIZE', (0, 0), (-1, -1), 10),
            ('ALIGN', (1, 0), (-1, -1), 'RIGHT'),
            ('GRID', (0, 0), (-1, -2), 0.5, colors.lightgrey),
            ('BOTTOMPADDING', (0, 0), (-1, -1), 8),
            ('TOPPADDING', (0, 0), (-1, -1), 8),
            ('LINEABOVE', (2, -1), (-1, -1), 1, colors.black),
        ]))
        story.append(earnings_table)
        story.append(Spacer(1, 0.3*inch))
        
        # Deductions Section
        story.append(Paragraph("DEDUCTIONS", self.styles['PayrollSectionHeader']))
        
        deductions_data = [
            ["Description", "Current", "YTD"],
            ["CPP (Canada Pension Plan)", f"${payroll_line.get('cpp', 0):.2f}", f"${payroll_line.get('ytd_cpp', 0):.2f}"],
            ["EI (Employment Insurance)", f"${payroll_line.get('ei', 0):.2f}", f"${payroll_line.get('ytd_ei', 0):.2f}"],
            ["Federal Income Tax", f"${payroll_line.get('federal_tax', 0):.2f}", "-"],
            ["Provincial Income Tax (ON)", f"${payroll_line.get('provincial_tax', 0):.2f}", "-"],
        ]
        
        total_deductions = (payroll_line.get('cpp', 0) + payroll_line.get('ei', 0) + 
                          payroll_line.get('federal_tax', 0) + payroll_line.get('provincial_tax', 0))
        deductions_data.append(["", "Total Deductions:", f"${total_deductions:.2f}"])
        
        deductions_table = Table(deductions_data, colWidths=[4*inch, 1.5*inch, 1.5*inch])
        deductions_table.setStyle(TableStyle([
            ('BACKGROUND', (0, 0), (-1, 0), colors.HexColor('#6c8fef')),
            ('TEXTCOLOR', (0, 0), (-1, 0), colors.white),
            ('FONTNAME', (0, 0), (-1, 0), 'Helvetica-Bold'),
            ('FONTNAME', (0, -1), (-1, -1), 'Helvetica-Bold'),
            ('FONTSIZE', (0, 0), (-1, -1), 10),
            ('ALIGN', (1, 0), (-1, -1), 'RIGHT'),
            ('GRID', (0, 0), (-1, -2), 0.5, colors.lightgrey),
            ('BOTTOMPADDING', (0, 0), (-1, -1), 8),
            ('TOPPADDING', (0, 0), (-1, -1), 8),
            ('LINEABOVE', (1, -1), (-1, -1), 1, colors.black),
        ]))
        story.append(deductions_table)
        story.append(Spacer(1, 0.4*inch))
        
        # Net Pay Summary
        summary_data = [
            ["NET PAY", f"${payroll_line.get('net_pay', 0):.2f}"],
            ["YTD Gross Earnings", f"${payroll_line.get('ytd_gross', 0):.2f}"],
            ["YTD Total Tax", f"${payroll_line.get('ytd_tax', 0):.2f}"],
        ]
        
        summary_table = Table(summary_data, colWidths=[5.5*inch, 1.5*inch])
        summary_table.setStyle(TableStyle([
            ('BACKGROUND', (0, 0), (-1, 0), colors.HexColor('#1a237e')),
            ('TEXTCOLOR', (0, 0), (-1, 0), colors.white),
            ('FONTNAME', (0, 0), (-1, -1), 'Helvetica-Bold'),
            ('FONTSIZE', (0, 0), (-1, 0), 14),
            ('FONTSIZE', (0, 1), (-1, -1), 10),
            ('ALIGN', (1, 0), (-1, -1), 'RIGHT'),
            ('BOTTOMPADDING', (0, 0), (-1, -1), 10),
            ('TOPPADDING', (0, 0), (-1, -1), 10),
            ('BOX', (0, 0), (-1, -1), 1, colors.HexColor('#1a237e')),
        ]))
        story.append(summary_table)
        
        # Disclaimer
        story.append(Paragraph(
            "This payslip is generated by Payroll Canada MVP. Calculations may not reflect the latest CRA rates. "
            "Please verify results with your accountant.",
            self.styles['PayrollDisclaimer']
        ))
        
        # Build PDF
        doc.build(story)
        return buffer.getvalue()
