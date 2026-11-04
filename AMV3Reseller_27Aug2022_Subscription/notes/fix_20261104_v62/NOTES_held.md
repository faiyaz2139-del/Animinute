# Held notes — AnyMinute v62 — SHIPPED (v62-20261104)

**Source:** t193u screenshot (Soft Services LLC reseller, Add Subscriber)
**Live confirmed:** Build stamp `v61-20261104` visible in sidebar → v61 is deployed.
**User:** "When reseller logins, the top header is not aligned properly." + blue underline on the topbar.
**Trigger:** t194u "Fix"

## Gap
Reseller portal top header (`.am-topbar` on Modern.master) is vertically / horizontally misaligned:
- Elements: hamburger, AnyMinute brand (clock + wordmark), "Reseller" role label, "Welcome! Soft Services LLC", account avatar/menu.
- "Reseller" and Welcome text appear to float higher than the logo and avatar; no consistent baseline / vertical center across the bar.
- Same class of issue as the earlier Accountant header fix (v38 `am-ap-navbody` topbar rules in AnyminuteNavDrawer.css).

## Screenshot
`notes/held_20261104_v62/reseller_header_misaligned.jpg`
URL shown: `SupportSubscribers.aspx?view=add`

## Scope for Fix
- Reseller top header alignment only (all reseller pages using Modern.master / Main.master reseller chrome).
- Keep existing brand: clock + AnyMinute + Reseller role; Welcome! {reseller name}; avatar initials + name.
- Do not change sidebar, Add Subscriber form, or other v61 features.
- Bump build stamp + meta to v62-20261104; cache-bust `?v=20261104v62`.
