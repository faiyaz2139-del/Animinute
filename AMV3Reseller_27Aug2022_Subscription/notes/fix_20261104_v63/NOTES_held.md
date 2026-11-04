# Held notes — AnyMinute v63 — SHIPPED (v63-20261104)

**Source:** t195u + t196ua0 screenshot (Soft Services LLC, ResellerDashboard.aspx localhost)
**Live/local confirmed:** Build stamp `v62-20261104` in sidebar.
**User:** "There are two time small down fa icons. Remove small one. Keep it only one time."

## Gap
Reseller (and likely shared) account menu in `.am-topbar` shows **two** down-chevron / FA angle-down icons next to Soft Services LLC name. Keep exactly one.

## Screenshot
`notes/held_20261104_v63/reseller_double_chevron.jpg`
URL: `ResellerDashboard.aspx`

## Scope for Fix
- Remove duplicate chevron; one remains.
- Likely causes: `am-account-chevron` plus a CSS `::after`/FA icon, or two markup spans, or details marker + custom chevron both visible.
- Check Modern.master account menu (and Main.master if same).
- Bump meta/build stamp to v63-20261104; cache-bust `?v=20261104v63`.
