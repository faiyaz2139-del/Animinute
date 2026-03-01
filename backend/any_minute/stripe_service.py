"""
Stripe Integration Service for Any Minute
-----------------------------------------
This module provides Stripe payment processing functionality.
Currently in TEST MODE - switch to live keys when ready.

Environment Variables:
   - STRIPE_SECRET_KEY=sk_test_xxx or sk_live_xxx
   - STRIPE_PUBLISHABLE_KEY=pk_test_xxx or pk_live_xxx
   - STRIPE_WEBHOOK_SECRET=whsec_xxx
   - STRIPE_MODE=test or live

Webhook Events Handled:
   - customer.subscription.created → Activate account, set plan + status
   - customer.subscription.updated → Update plan tier (upgrade/downgrade)
   - customer.subscription.deleted → Deactivate account, block login
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
STRIPE_MODE = os.environ.get("STRIPE_MODE", "test")

# Database reference - set by routes.py
_db = None

def init_stripe_db(database):
    """Initialize database reference for webhook handlers"""
    global _db
    _db = database

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

# Map Stripe price IDs to plan names
PRICE_TO_PLAN = {
    "price_test_basic": "basic",
    "price_live_basic": "basic",
    "price_test_pro": "pro",
    "price_live_pro": "pro",
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


def get_plan_from_subscription(subscription: Dict) -> str:
    """Extract plan name from subscription object"""
    try:
        items = subscription.get("items", {}).get("data", [])
        if items:
            price_id = items[0].get("price", {}).get("id", "")
            return PRICE_TO_PLAN.get(price_id, "basic")
    except Exception as e:
        logger.warning(f"Could not extract plan from subscription: {e}")
    return "basic"


async def handle_subscription_created(subscription: Dict) -> Dict[str, Any]:
    """
    Handle customer.subscription.created event
    Activates the subscriber's account, sets plan + status
    """
    global _db
    
    subscription_id = subscription.get("id")
    customer_id = subscription.get("customer")
    status = subscription.get("status")
    plan = get_plan_from_subscription(subscription)
    
    # Get metadata if available (from checkout session)
    metadata = subscription.get("metadata", {})
    tenant_id = metadata.get("tenant_id")
    
    logger.info(f"Subscription CREATED: id={subscription_id}, customer={customer_id}, plan={plan}, status={status}")
    
    if _db and tenant_id:
        # Update tenant with subscription info
        plan_config = STRIPE_PLANS.get(plan, STRIPE_PLANS["basic"])
        
        update_data = {
            "stripe_customer_id": customer_id,
            "stripe_subscription_id": subscription_id,
            "plan": plan,
            "seat_limit": plan_config["seats"],
            "billing_status": "active",
            "subscription_status": status,
            "subscription_created_at": datetime.now(timezone.utc).isoformat()
        }
        
        result = await _db.am_tenants.update_one(
            {"id": tenant_id},
            {"$set": update_data}
        )
        
        logger.info(f"Updated tenant {tenant_id}: plan={plan}, seats={plan_config['seats']}")
        
        return {
            "success": True,
            "action": "subscription_activated",
            "tenant_id": tenant_id,
            "plan": plan,
            "seats": plan_config["seats"],
            "db_updated": result.modified_count > 0
        }
    elif _db and customer_id:
        # Try to find tenant by customer_id
        tenant = await _db.am_tenants.find_one({"stripe_customer_id": customer_id})
        if tenant:
            plan_config = STRIPE_PLANS.get(plan, STRIPE_PLANS["basic"])
            
            await _db.am_tenants.update_one(
                {"id": tenant["id"]},
                {"$set": {
                    "stripe_subscription_id": subscription_id,
                    "plan": plan,
                    "seat_limit": plan_config["seats"],
                    "billing_status": "active",
                    "subscription_status": status
                }}
            )
            
            logger.info(f"Updated tenant by customer_id: {tenant['id']}")
            return {
                "success": True,
                "action": "subscription_activated",
                "tenant_id": tenant["id"],
                "plan": plan
            }
    
    return {
        "success": True,
        "action": "subscription_created_no_tenant",
        "subscription_id": subscription_id,
        "customer_id": customer_id
    }


async def handle_subscription_updated(subscription: Dict) -> Dict[str, Any]:
    """
    Handle customer.subscription.updated event
    Updates the plan name/tier in DB, handles upgrades and downgrades
    """
    global _db
    
    subscription_id = subscription.get("id")
    customer_id = subscription.get("customer")
    status = subscription.get("status")
    plan = get_plan_from_subscription(subscription)
    
    logger.info(f"Subscription UPDATED: id={subscription_id}, status={status}, plan={plan}")
    
    if not _db:
        return {"success": True, "action": "no_db", "subscription_id": subscription_id}
    
    # Find tenant by subscription_id or customer_id
    tenant = await _db.am_tenants.find_one({
        "$or": [
            {"stripe_subscription_id": subscription_id},
            {"stripe_customer_id": customer_id}
        ]
    })
    
    if not tenant:
        logger.warning(f"No tenant found for subscription {subscription_id}")
        return {
            "success": True,
            "action": "tenant_not_found",
            "subscription_id": subscription_id
        }
    
    tenant_id = tenant["id"]
    old_plan = tenant.get("plan", "free")
    plan_config = STRIPE_PLANS.get(plan, STRIPE_PLANS["basic"])
    
    # Determine if upgrade or downgrade
    action_type = "unchanged"
    if old_plan != plan:
        old_seats = STRIPE_PLANS.get(old_plan, {}).get("seats", 5)
        new_seats = plan_config["seats"]
        action_type = "upgrade" if new_seats > old_seats else "downgrade"
    
    # Build update based on status
    update_data = {
        "plan": plan,
        "seat_limit": plan_config["seats"],
        "subscription_status": status,
        "subscription_updated_at": datetime.now(timezone.utc).isoformat()
    }
    
    # Handle different statuses
    if status in ["active", "trialing"]:
        update_data["billing_status"] = "active"
    elif status in ["past_due", "unpaid"]:
        update_data["billing_status"] = "past_due"
        logger.warning(f"Tenant {tenant_id} has payment issues: {status}")
    elif status in ["canceled", "paused"]:
        update_data["billing_status"] = "inactive"
        update_data["plan"] = "free"
        update_data["seat_limit"] = 5
    
    result = await _db.am_tenants.update_one(
        {"id": tenant_id},
        {"$set": update_data}
    )
    
    logger.info(f"Subscription updated for tenant {tenant_id}: {old_plan} -> {plan} ({action_type})")
    
    return {
        "success": True,
        "action": f"subscription_{action_type}",
        "tenant_id": tenant_id,
        "old_plan": old_plan,
        "new_plan": plan,
        "status": status,
        "db_updated": result.modified_count > 0
    }


async def handle_subscription_deleted(subscription: Dict) -> Dict[str, Any]:
    """
    Handle customer.subscription.deleted event
    Deactivates the account, blocks login with appropriate message
    """
    global _db
    
    subscription_id = subscription.get("id")
    customer_id = subscription.get("customer")
    
    logger.info(f"Subscription DELETED: id={subscription_id}, customer={customer_id}")
    
    if not _db:
        return {"success": True, "action": "no_db", "subscription_id": subscription_id}
    
    # Find tenant
    tenant = await _db.am_tenants.find_one({
        "$or": [
            {"stripe_subscription_id": subscription_id},
            {"stripe_customer_id": customer_id}
        ]
    })
    
    if not tenant:
        logger.warning(f"No tenant found for deleted subscription {subscription_id}")
        return {
            "success": True,
            "action": "tenant_not_found",
            "subscription_id": subscription_id
        }
    
    tenant_id = tenant["id"]
    
    # Deactivate the tenant account
    update_data = {
        "plan": "free",
        "seat_limit": 5,
        "billing_status": "cancelled",
        "subscription_status": "cancelled",
        "stripe_subscription_id": None,
        "subscription_cancelled_at": datetime.now(timezone.utc).isoformat(),
        "status": "subscription_ended"  # This will block login
    }
    
    result = await _db.am_tenants.update_one(
        {"id": tenant_id},
        {"$set": update_data}
    )
    
    logger.info(f"Subscription cancelled for tenant {tenant_id} - account deactivated")
    
    return {
        "success": True,
        "action": "subscription_cancelled",
        "tenant_id": tenant_id,
        "db_updated": result.modified_count > 0
    }


async def handle_invoice_paid(invoice: Dict) -> Dict[str, Any]:
    """Handle successful payment"""
    customer_id = invoice.get("customer")
    amount_paid = invoice.get("amount_paid", 0) / 100
    
    logger.info(f"Invoice paid: customer={customer_id}, amount=${amount_paid}")
    
    return {
        "success": True,
        "action": "payment_recorded",
        "amount": amount_paid
    }


async def handle_payment_failed(invoice: Dict) -> Dict[str, Any]:
    """Handle failed payment"""
    global _db
    
    customer_id = invoice.get("customer")
    
    logger.warning(f"Payment failed: customer={customer_id}")
    
    if _db and customer_id:
        # Update tenant status to show payment warning
        await _db.am_tenants.update_one(
            {"stripe_customer_id": customer_id},
            {"$set": {
                "billing_status": "payment_failed",
                "payment_failed_at": datetime.now(timezone.utc).isoformat()
            }}
        )
    
    return {
        "success": True,
        "action": "payment_failed",
        "customer_id": customer_id
    }


async def handle_webhook_event(payload: bytes, sig_header: str) -> Dict[str, Any]:
    """
    Handle Stripe webhook events
    
    Supported events:
    - customer.subscription.created: New subscription started
    - customer.subscription.updated: Subscription changed (upgrade/downgrade)
    - customer.subscription.deleted: Subscription cancelled
    - invoice.paid: Successful payment
    - invoice.payment_failed: Failed payment
    """
    stripe = get_stripe_client()
    
    # For testing without signature verification
    if not STRIPE_WEBHOOK_SECRET or STRIPE_WEBHOOK_SECRET == "whsec_test_secret":
        # Parse event directly for testing
        import json
        try:
            event = json.loads(payload)
        except Exception as e:
            return {"error": f"Invalid JSON payload: {e}", "success": False}
    else:
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
    
    event_type = event.get("type")
    data = event.get("data", {}).get("object", {})
    
    logger.info(f"Processing Stripe webhook: {event_type}")
    
    # Handle different event types
    if event_type == "customer.subscription.created":
        return await handle_subscription_created(data)
    elif event_type == "customer.subscription.updated":
        return await handle_subscription_updated(data)
    elif event_type == "customer.subscription.deleted":
        return await handle_subscription_deleted(data)
    elif event_type == "checkout.session.completed":
        return await handle_subscription_created(data)  # Similar handling
    elif event_type == "invoice.paid":
        return await handle_invoice_paid(data)
    elif event_type == "invoice.payment_failed":
        return await handle_payment_failed(data)
    else:
        logger.info(f"Unhandled event type: {event_type}")
        return {"success": True, "handled": False, "event_type": event_type}
