"""
Stripe Integration Service for Any Minute
-----------------------------------------
This module provides Stripe payment processing functionality.
Currently in TEST MODE - switch to live keys when ready.

To enable Stripe:
1. Set these environment variables:
   - STRIPE_SECRET_KEY=sk_live_xxx (or sk_test_xxx for testing)
   - STRIPE_PUBLISHABLE_KEY=pk_live_xxx (or pk_test_xxx for testing)
   - STRIPE_WEBHOOK_SECRET=whsec_xxx
   - STRIPE_MODE=test (or "live" for production)

2. Test webhook endpoints with Stripe CLI:
   stripe listen --forward-to localhost:8001/api/stripe/webhook

Go-Live Steps:
1. Replace test keys with live keys in environment variables
2. Set STRIPE_MODE=live
3. Update webhook endpoint in Stripe Dashboard
4. Test with a real card before enabling for customers
5. Enable the Billing > Upgrade button in the UI
"""

import os
import logging
from typing import Optional, Dict, Any
from datetime import datetime, timezone

logger = logging.getLogger("stripe_service")

# Environment configuration
STRIPE_SECRET_KEY = os.environ.get("STRIPE_SECRET_KEY", "")
STRIPE_PUBLISHABLE_KEY = os.environ.get("STRIPE_PUBLISHABLE_KEY", "")
STRIPE_WEBHOOK_SECRET = os.environ.get("STRIPE_WEBHOOK_SECRET", "")
STRIPE_MODE = os.environ.get("STRIPE_MODE", "test")  # "test" or "live"

# Plan configuration
STRIPE_PLANS = {
    "basic": {
        "name": "Basic Plan",
        "price_id_test": "price_test_basic",
        "price_id_live": "price_live_basic",
        "seats": 25,
        "monthly_price": 49.00
    },
    "pro": {
        "name": "Pro Plan",
        "price_id_test": "price_test_pro",
        "price_id_live": "price_live_pro",
        "seats": 999,
        "monthly_price": 99.00
    }
}


def is_stripe_configured() -> bool:
    """Check if Stripe is properly configured"""
    return bool(STRIPE_SECRET_KEY and STRIPE_PUBLISHABLE_KEY)


def is_live_mode() -> bool:
    """Check if we're in live mode"""
    return STRIPE_MODE == "live" and STRIPE_SECRET_KEY.startswith("sk_live_")


def get_stripe_client():
    """Get Stripe client (lazy import)"""
    if not is_stripe_configured():
        return None
    try:
        import stripe
        stripe.api_key = STRIPE_SECRET_KEY
        return stripe
    except ImportError:
        logger.warning("Stripe library not installed. Run: pip install stripe")
        return None


def get_config() -> Dict[str, Any]:
    """Get Stripe configuration for frontend"""
    return {
        "configured": is_stripe_configured(),
        "mode": STRIPE_MODE,
        "is_live": is_live_mode(),
        "publishable_key": STRIPE_PUBLISHABLE_KEY if is_stripe_configured() else None,
        "plans": {
            k: {
                "name": v["name"],
                "seats": v["seats"],
                "monthly_price": v["monthly_price"]
            }
            for k, v in STRIPE_PLANS.items()
        }
    }


async def create_checkout_session(
    tenant_id: str,
    plan: str,
    success_url: str,
    cancel_url: str,
    customer_email: Optional[str] = None
) -> Dict[str, Any]:
    """Create a Stripe Checkout session for plan upgrade"""
    if not is_stripe_configured():
        return {"error": "Stripe not configured", "success": False}
    
    stripe = get_stripe_client()
    if not stripe:
        return {"error": "Stripe library not available", "success": False}
    
    if plan not in STRIPE_PLANS:
        return {"error": f"Invalid plan: {plan}", "success": False}
    
    plan_config = STRIPE_PLANS[plan]
    price_id = plan_config["price_id_live"] if is_live_mode() else plan_config["price_id_test"]
    
    try:
        session = stripe.checkout.Session.create(
            mode="subscription",
            payment_method_types=["card"],
            line_items=[{
                "price": price_id,
                "quantity": 1
            }],
            success_url=success_url,
            cancel_url=cancel_url,
            customer_email=customer_email,
            metadata={
                "tenant_id": tenant_id,
                "plan": plan
            }
        )
        
        return {
            "success": True,
            "session_id": session.id,
            "url": session.url
        }
    except Exception as e:
        logger.error(f"Failed to create checkout session: {e}")
        return {"error": str(e), "success": False}


async def create_customer_portal_session(
    customer_id: str,
    return_url: str
) -> Dict[str, Any]:
    """Create a Stripe Customer Portal session for subscription management"""
    if not is_stripe_configured():
        return {"error": "Stripe not configured", "success": False}
    
    stripe = get_stripe_client()
    if not stripe:
        return {"error": "Stripe library not available", "success": False}
    
    try:
        session = stripe.billing_portal.Session.create(
            customer=customer_id,
            return_url=return_url
        )
        
        return {
            "success": True,
            "url": session.url
        }
    except Exception as e:
        logger.error(f"Failed to create portal session: {e}")
        return {"error": str(e), "success": False}


async def handle_webhook_event(payload: bytes, sig_header: str) -> Dict[str, Any]:
    """
    Handle Stripe webhook events
    
    Supported events:
    - checkout.session.completed: New subscription started
    - customer.subscription.updated: Subscription changed (upgrade/downgrade)
    - customer.subscription.deleted: Subscription cancelled
    - invoice.paid: Successful payment
    - invoice.payment_failed: Failed payment
    """
    if not STRIPE_WEBHOOK_SECRET:
        return {"error": "Webhook secret not configured", "success": False}
    
    stripe = get_stripe_client()
    if not stripe:
        return {"error": "Stripe library not available", "success": False}
    
    try:
        event = stripe.Webhook.construct_event(
            payload, sig_header, STRIPE_WEBHOOK_SECRET
        )
    except ValueError as e:
        logger.error(f"Invalid webhook payload: {e}")
        return {"error": "Invalid payload", "success": False}
    except stripe.error.SignatureVerificationError as e:
        logger.error(f"Invalid webhook signature: {e}")
        return {"error": "Invalid signature", "success": False}
    
    event_type = event["type"]
    data = event["data"]["object"]
    
    logger.info(f"Processing Stripe webhook: {event_type}")
    
    # Handle different event types
    if event_type == "checkout.session.completed":
        return await handle_checkout_completed(data)
    elif event_type == "customer.subscription.updated":
        return await handle_subscription_updated(data)
    elif event_type == "customer.subscription.deleted":
        return await handle_subscription_deleted(data)
    elif event_type == "invoice.paid":
        return await handle_invoice_paid(data)
    elif event_type == "invoice.payment_failed":
        return await handle_payment_failed(data)
    else:
        logger.info(f"Unhandled event type: {event_type}")
        return {"success": True, "handled": False}


async def handle_checkout_completed(session: Dict) -> Dict[str, Any]:
    """Handle successful checkout - activate subscription"""
    tenant_id = session.get("metadata", {}).get("tenant_id")
    plan = session.get("metadata", {}).get("plan")
    customer_id = session.get("customer")
    subscription_id = session.get("subscription")
    
    logger.info(f"Checkout completed: tenant={tenant_id}, plan={plan}")
    
    # TODO: Update tenant in database with:
    # - stripe_customer_id = customer_id
    # - stripe_subscription_id = subscription_id
    # - plan = plan
    # - seat_limit = STRIPE_PLANS[plan]["seats"]
    # - status = "active"
    
    return {
        "success": True,
        "action": "subscription_activated",
        "tenant_id": tenant_id,
        "plan": plan
    }


async def handle_subscription_updated(subscription: Dict) -> Dict[str, Any]:
    """Handle subscription changes (upgrade/downgrade)"""
    subscription_id = subscription.get("id")
    status = subscription.get("status")
    
    logger.info(f"Subscription updated: {subscription_id}, status={status}")
    
    # TODO: Update tenant status based on subscription status
    # - active, trialing -> plan active
    # - past_due, unpaid -> show warning
    # - canceled, paused -> downgrade to free
    
    return {
        "success": True,
        "action": "subscription_updated",
        "subscription_id": subscription_id,
        "status": status
    }


async def handle_subscription_deleted(subscription: Dict) -> Dict[str, Any]:
    """Handle subscription cancellation"""
    subscription_id = subscription.get("id")
    
    logger.info(f"Subscription deleted: {subscription_id}")
    
    # TODO: Downgrade tenant to free plan
    # - plan = "free"
    # - seat_limit = 5
    # - stripe_subscription_id = null
    
    return {
        "success": True,
        "action": "subscription_cancelled",
        "subscription_id": subscription_id
    }


async def handle_invoice_paid(invoice: Dict) -> Dict[str, Any]:
    """Handle successful payment"""
    customer_id = invoice.get("customer")
    amount_paid = invoice.get("amount_paid", 0) / 100  # Convert cents to dollars
    
    logger.info(f"Invoice paid: customer={customer_id}, amount=${amount_paid}")
    
    # TODO: Record payment in database for audit
    
    return {
        "success": True,
        "action": "payment_recorded",
        "amount": amount_paid
    }


async def handle_payment_failed(invoice: Dict) -> Dict[str, Any]:
    """Handle failed payment"""
    customer_id = invoice.get("customer")
    
    logger.warning(f"Payment failed: customer={customer_id}")
    
    # TODO: 
    # - Send email notification to tenant admin
    # - Update tenant status to show payment warning
    
    return {
        "success": True,
        "action": "payment_failed",
        "customer_id": customer_id
    }
