"""
Email Service for Any Minute
----------------------------
This module provides email notification functionality.
Currently uses a mock implementation that logs emails to console.

To enable real SMTP:
1. Set these environment variables:
   - SMTP_HOST=smtp.example.com
   - SMTP_PORT=587
   - SMTP_USER=your-email@example.com
   - SMTP_PASSWORD=your-password
   - SMTP_FROM_EMAIL=noreply@anyminute.com
   - SMTP_FROM_NAME=Any Minute
   - EMAIL_ENABLED=true

2. No code changes required - it will automatically use SMTP when configured.
"""

import os
import logging
from datetime import datetime
from typing import Optional, List

# Configure logging
logger = logging.getLogger("email_service")
logger.setLevel(logging.INFO)

# Environment configuration
SMTP_HOST = os.environ.get("SMTP_HOST", "")
SMTP_PORT = int(os.environ.get("SMTP_PORT", "587"))
SMTP_USER = os.environ.get("SMTP_USER", "")
SMTP_PASSWORD = os.environ.get("SMTP_PASSWORD", "")
SMTP_FROM_EMAIL = os.environ.get("SMTP_FROM_EMAIL", "noreply@anyminute.com")
SMTP_FROM_NAME = os.environ.get("SMTP_FROM_NAME", "Any Minute")
EMAIL_ENABLED = os.environ.get("EMAIL_ENABLED", "false").lower() == "true"


def is_smtp_configured() -> bool:
    """Check if SMTP is properly configured"""
    return all([SMTP_HOST, SMTP_USER, SMTP_PASSWORD, EMAIL_ENABLED])


async def send_email(
    to_email: str,
    subject: str,
    body_html: str,
    body_text: Optional[str] = None,
    cc: Optional[List[str]] = None,
    bcc: Optional[List[str]] = None
) -> dict:
    """
    Send an email notification.
    
    When SMTP is not configured, logs the email to console (mock mode).
    When SMTP is configured, sends real email via SMTP.
    
    Args:
        to_email: Recipient email address
        subject: Email subject line
        body_html: HTML body content
        body_text: Plain text body (optional)
        cc: Carbon copy recipients (optional)
        bcc: Blind carbon copy recipients (optional)
    
    Returns:
        dict with success status and message
    """
    timestamp = datetime.utcnow().isoformat()
    
    if not is_smtp_configured():
        # MOCK MODE - Log to console
        logger.info("=" * 60)
        logger.info("[MOCK EMAIL] Email would be sent:")
        logger.info(f"  Timestamp: {timestamp}")
        logger.info(f"  From: {SMTP_FROM_NAME} <{SMTP_FROM_EMAIL}>")
        logger.info(f"  To: {to_email}")
        if cc:
            logger.info(f"  CC: {', '.join(cc)}")
        if bcc:
            logger.info(f"  BCC: {', '.join(bcc)}")
        logger.info(f"  Subject: {subject}")
        logger.info(f"  Body Preview: {body_text[:200] if body_text else body_html[:200]}...")
        logger.info("=" * 60)
        
        return {
            "success": True,
            "mode": "mock",
            "message": "Email logged to console (SMTP not configured)",
            "timestamp": timestamp
        }
    
    # REAL SMTP MODE
    try:
        import smtplib
        from email.mime.text import MIMEText
        from email.mime.multipart import MIMEMultipart
        
        msg = MIMEMultipart("alternative")
        msg["Subject"] = subject
        msg["From"] = f"{SMTP_FROM_NAME} <{SMTP_FROM_EMAIL}>"
        msg["To"] = to_email
        
        if cc:
            msg["Cc"] = ", ".join(cc)
        
        # Attach plain text and HTML parts
        if body_text:
            msg.attach(MIMEText(body_text, "plain"))
        msg.attach(MIMEText(body_html, "html"))
        
        # Calculate all recipients
        recipients = [to_email]
        if cc:
            recipients.extend(cc)
        if bcc:
            recipients.extend(bcc)
        
        # Send via SMTP
        with smtplib.SMTP(SMTP_HOST, SMTP_PORT) as server:
            server.starttls()
            server.login(SMTP_USER, SMTP_PASSWORD)
            server.sendmail(SMTP_FROM_EMAIL, recipients, msg.as_string())
        
        logger.info(f"[EMAIL SENT] To: {to_email}, Subject: {subject}")
        
        return {
            "success": True,
            "mode": "smtp",
            "message": "Email sent successfully",
            "timestamp": timestamp
        }
        
    except Exception as e:
        logger.error(f"[EMAIL ERROR] Failed to send email: {str(e)}")
        return {
            "success": False,
            "mode": "smtp",
            "message": f"Failed to send email: {str(e)}",
            "timestamp": timestamp
        }


# ===================== EMAIL TEMPLATES =====================

async def send_ticket_created_notification(
    admin_email: str,
    ticket_number: int,
    subject: str,
    created_by: str,
    priority: str
):
    """Send notification when a new support ticket is created"""
    html = f"""
    <html>
    <body style="font-family: Arial, sans-serif; padding: 20px;">
        <h2 style="color: #1a73e8;">New Support Ticket #{ticket_number}</h2>
        <p>A new support ticket has been created:</p>
        <table style="border-collapse: collapse; margin: 20px 0;">
            <tr><td style="padding: 8px; border: 1px solid #ddd;"><strong>Ticket #</strong></td><td style="padding: 8px; border: 1px solid #ddd;">{ticket_number}</td></tr>
            <tr><td style="padding: 8px; border: 1px solid #ddd;"><strong>Subject</strong></td><td style="padding: 8px; border: 1px solid #ddd;">{subject}</td></tr>
            <tr><td style="padding: 8px; border: 1px solid #ddd;"><strong>Created By</strong></td><td style="padding: 8px; border: 1px solid #ddd;">{created_by}</td></tr>
            <tr><td style="padding: 8px; border: 1px solid #ddd;"><strong>Priority</strong></td><td style="padding: 8px; border: 1px solid #ddd;">{priority.upper()}</td></tr>
        </table>
        <p>Please log in to the Any Minute admin panel to respond.</p>
        <p style="color: #666; font-size: 12px;">This is an automated notification from Any Minute.</p>
    </body>
    </html>
    """
    text = f"New Support Ticket #{ticket_number}\nSubject: {subject}\nCreated By: {created_by}\nPriority: {priority}"
    
    return await send_email(
        to_email=admin_email,
        subject=f"[Any Minute] New Support Ticket #{ticket_number}: {subject}",
        body_html=html,
        body_text=text
    )


async def send_ticket_reply_notification(
    to_email: str,
    ticket_number: int,
    ticket_subject: str,
    reply_by: str,
    reply_preview: str
):
    """Send notification when a ticket receives a reply"""
    html = f"""
    <html>
    <body style="font-family: Arial, sans-serif; padding: 20px;">
        <h2 style="color: #1a73e8;">Reply to Ticket #{ticket_number}</h2>
        <p>Your support ticket has received a reply:</p>
        <table style="border-collapse: collapse; margin: 20px 0;">
            <tr><td style="padding: 8px; border: 1px solid #ddd;"><strong>Ticket #</strong></td><td style="padding: 8px; border: 1px solid #ddd;">{ticket_number}</td></tr>
            <tr><td style="padding: 8px; border: 1px solid #ddd;"><strong>Subject</strong></td><td style="padding: 8px; border: 1px solid #ddd;">{ticket_subject}</td></tr>
            <tr><td style="padding: 8px; border: 1px solid #ddd;"><strong>Reply By</strong></td><td style="padding: 8px; border: 1px solid #ddd;">{reply_by}</td></tr>
        </table>
        <div style="background-color: #f5f5f5; padding: 15px; border-radius: 5px; margin: 20px 0;">
            <p style="margin: 0;">{reply_preview}...</p>
        </div>
        <p>Log in to Any Minute to view the full conversation and respond.</p>
        <p style="color: #666; font-size: 12px;">This is an automated notification from Any Minute.</p>
    </body>
    </html>
    """
    text = f"Reply to Ticket #{ticket_number}\nSubject: {ticket_subject}\nFrom: {reply_by}\n\n{reply_preview}..."
    
    return await send_email(
        to_email=to_email,
        subject=f"[Any Minute] Reply to Ticket #{ticket_number}: {ticket_subject}",
        body_html=html,
        body_text=text
    )


async def send_timesheet_approved_notification(
    to_email: str,
    employee_name: str,
    week_start: str,
    total_hours: float,
    approved_by: str
):
    """Send notification when a timesheet week is approved"""
    html = f"""
    <html>
    <body style="font-family: Arial, sans-serif; padding: 20px;">
        <h2 style="color: #34a853;">Timesheet Approved</h2>
        <p>Your timesheet has been approved:</p>
        <table style="border-collapse: collapse; margin: 20px 0;">
            <tr><td style="padding: 8px; border: 1px solid #ddd;"><strong>Employee</strong></td><td style="padding: 8px; border: 1px solid #ddd;">{employee_name}</td></tr>
            <tr><td style="padding: 8px; border: 1px solid #ddd;"><strong>Week Starting</strong></td><td style="padding: 8px; border: 1px solid #ddd;">{week_start}</td></tr>
            <tr><td style="padding: 8px; border: 1px solid #ddd;"><strong>Total Hours</strong></td><td style="padding: 8px; border: 1px solid #ddd;">{total_hours}</td></tr>
            <tr><td style="padding: 8px; border: 1px solid #ddd;"><strong>Approved By</strong></td><td style="padding: 8px; border: 1px solid #ddd;">{approved_by}</td></tr>
        </table>
        <p style="color: #666; font-size: 12px;">This is an automated notification from Any Minute.</p>
    </body>
    </html>
    """
    text = f"Timesheet Approved\nEmployee: {employee_name}\nWeek: {week_start}\nTotal Hours: {total_hours}\nApproved By: {approved_by}"
    
    return await send_email(
        to_email=to_email,
        subject=f"[Any Minute] Timesheet Approved - Week of {week_start}",
        body_html=html,
        body_text=text
    )


async def send_payroll_completed_notification(
    to_emails: List[str],
    run_number: int,
    period_start: str,
    period_end: str,
    total_employees: int,
    total_gross: float
):
    """Send notification when a payroll run is completed"""
    html = f"""
    <html>
    <body style="font-family: Arial, sans-serif; padding: 20px;">
        <h2 style="color: #1a73e8;">Payroll Run Completed</h2>
        <p>A payroll run has been completed:</p>
        <table style="border-collapse: collapse; margin: 20px 0;">
            <tr><td style="padding: 8px; border: 1px solid #ddd;"><strong>Run #</strong></td><td style="padding: 8px; border: 1px solid #ddd;">{run_number}</td></tr>
            <tr><td style="padding: 8px; border: 1px solid #ddd;"><strong>Pay Period</strong></td><td style="padding: 8px; border: 1px solid #ddd;">{period_start} to {period_end}</td></tr>
            <tr><td style="padding: 8px; border: 1px solid #ddd;"><strong>Employees</strong></td><td style="padding: 8px; border: 1px solid #ddd;">{total_employees}</td></tr>
            <tr><td style="padding: 8px; border: 1px solid #ddd;"><strong>Total Gross</strong></td><td style="padding: 8px; border: 1px solid #ddd;">${total_gross:,.2f}</td></tr>
        </table>
        <p>Log in to Any Minute to view payslips and export reports.</p>
        <p style="color: #666; font-size: 12px;">This is an automated notification from Any Minute.</p>
    </body>
    </html>
    """
    text = f"Payroll Run #{run_number} Completed\nPeriod: {period_start} to {period_end}\nEmployees: {total_employees}\nTotal Gross: ${total_gross:,.2f}"
    
    # Send to all recipients
    results = []
    for email in to_emails:
        result = await send_email(
            to_email=email,
            subject=f"[Any Minute] Payroll Run #{run_number} Completed",
            body_html=html,
            body_text=text
        )
        results.append(result)
    
    return results
