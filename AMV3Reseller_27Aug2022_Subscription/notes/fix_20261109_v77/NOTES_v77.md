# AnyMinute v77 — unified old round logo across all logins

2026-11-09 (America/Toronto). Base v76 764b032ffbef912b23ba36d86b6ab33b2377bfcf. Code commit 3ed72e4e9c37d4f94785120f3b32cc8a8f4201ff.
Stamp **v77-20261109**; Typography/Reference cache-bust 20261109v77.

Held notes: notes/held_20261109_v77/ (notes.txt + subscriber_logo.jpg). Local lean cumulative on v76.
No CloudAgent, push, live site or database actions. v75 erase/wipe UI removals and HTTP 403
guards remain intact. v76 Support Admin shell (252px), overflow fix, Locations tab, name→details
link, and round help mark are kept.

## Fix

**Same old round logo everywhere** — Support Admin, Reseller, Accountant, Subscriber and Employee
headers all use `Assets/assets/img/logo.jpg` (the round AnyMinute mark shown on the subscriber
Dashboard next to Welcome). Reverts the v76 Support Admin SVG clock + "AnyMinute" wordmark and
the Accountant SVG brand. Reseller Brand keeps the "Reseller" role label beside the same image.
Logo size/position matched via Reference + NavDrawer `.am-logo` rules (36px height).

## SQL
**Not required.**

## Compile
/opt/monoenv mcs C#5/.NET 4.5: App_Code + SupportSubscribers.aspx + Main/Modern/MasterWithoutMenu
masters — **zero errors** (existing warnings only). See COMPILE.txt.

## Files
- Modern.master.cs — Support Admin + Accountant use LogoDefault (logo.jpg); clear Brand SVG
- App_Code/AnyminuteResellerPortal.cs — Brand = logo.jpg + Reseller label
- css/AnyminuteReference.css — unified .am-logo sizing; drop SVG brand sizing
- css/AnyminuteNavDrawer.css — reseller brand img sizing
- Main.master, Modern.master, MasterWithoutMenu.master — stamp v77-20261109

## Ship
Zip: `/workspace/anymin/AnyMinute_Fix_20261109_v77.zip`
Code: `3ed72e4e9c37d4f94785120f3b32cc8a8f4201ff`
SHA256: see AnyMinute_Fix_20261109_v77.zip.sha256
