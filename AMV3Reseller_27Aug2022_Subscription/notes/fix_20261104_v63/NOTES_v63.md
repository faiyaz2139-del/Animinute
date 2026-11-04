# NOTES v63-20261104
- Bug: two down-chevrons beside account name (reseller). Cause: .am-ap-avatar-name::after draws a chevron AND span.am-account-chevron shown; accountant hid span only under body.am-ap-navbody, reseller lacks it.
- Fix: AnyminuteNavDrawer.css — hide span chevron when summary :has(.am-ap-avatar-name) (all roles); mobile (name hidden) keeps span.
- Build stamp v63-20261104; cache-bust ?v=20261104v63. Code commit 730577f.
