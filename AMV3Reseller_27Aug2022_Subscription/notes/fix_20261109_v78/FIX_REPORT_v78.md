# AnyMinute v78 — Address Verification actions (held notes + screenshots)

2026-11-09 (America/Toronto). Base v77 3ed72e4e9c37d4f94785120f3b32cc8a8f4201ff. Code commit 07396d1722c18ac86e17043ee68d7ec7d01aac2d.
Stamp **v78-20261109**; Support CSS/JS cache-bust 20261109v78; Typography/Reference 20261109v78.

Held notes: notes/held_20261109_v78/ (notes.txt + image1–6). Local lean cumulative on v77.
No CloudAgent, push, live site or database actions.

## Fix
1. **Rejected tab Actions:** View inline; Restore + Delete moved under ⋮ (images 2/3 callout).
2. **Pending/Verified Actions:** View inline; Pending ⋮ = Reject + Delete; Verified ⋮ = Edit + Reject (images 1/6).
3. **Bulk bar:** Reject Selected (pending/verified); Restore Selected + Delete Selected (rejected); Clear Selection. **Edit Selected removed** (red X in image6) — markup and handlers gone, not hidden.
4. **Duplicates:** Add Address and Import reject same street/city/prov/country/postal with a clear message.
5. **Fonts:** Address Verification page table/tabs/filters/buttons text sizes increased.

## SQL
**Not required.**

## Compile
/opt/monoenv mcs C#5/.NET 4.5: App_Code + SupportAddresses.aspx + Main/Modern/MasterWithoutMenu masters — **zero errors**. See COMPILE.txt.

## Files
- SupportAddresses.aspx(.cs) — row View+⋮, selbar, restore/delete/bulk handlers
- Scripts/AnyminuteSupport.js — selection bar sync/clear
- css/AnyminuteSupport.css — fonts + selbar
- App_Code/AnyminuteAddressImport.cs — stricter duplicate match
- Main/Modern/MasterWithoutMenu.master — stamp v78-20261109

## Ship
Zip: `/workspace/anymin/AnyMinute_Fix_20261109_v78.zip`
Code: `07396d1722c18ac86e17043ee68d7ec7d01aac2d`
SHA256: `accbb74863fdfb641b9549a2c9cc23158d2d32a412885836a1ad9c1f2721db6c`
