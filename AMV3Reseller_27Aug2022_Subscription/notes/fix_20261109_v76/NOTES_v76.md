# AnyMinute v76 — Support Admin UI fixes (docx held notes)

2026-11-09 (America/Toronto). Base v75 a5918ba6157b021b8b2551e890a46bc59fd799e5. Code commit 764b032ffbef912b23ba36d86b6ab33b2377bfcf.
Stamp **v76-20261109**; Support CSS/JS and typography/reference cache-bust 20261109v76.

Held notes: notes/held_20261109_v76/ (notes.docx + image1–image6). Local lean cumulative on v75.
No CloudAgent, push, live site or database actions. v75 erase/wipe UI removals and HTTP 403
guards remain intact (RemovedDataReset unchanged; no Clean/Erase/Wipe buttons restored).

## Fixes

1. **Support Admin menu not visible** — v74 Reference.css crushed the SA drawer to 188px while
   NavDrawer still used nowrap + overflow clipping (v37 had widened SA to 252/288px so labels
   like Address Verification stayed readable). v76 restores **252px** for am-sa-navbody,
   solid opaque sidebar, full label visibility and badge layout.

2. **Unnecessary horizontal scroll / broken UI when scrolling** — conflicting
   width calc(100% - Npx) + margin-left rules and page overflow let the document grow
   wider than the viewport; fixed sidebar then overlaid scrolling content. v76 uses
   width:auto with matching margin, overflow-x:hidden on html/body, overflow-x:clip on
   main, and keeps table-level horizontal scroll inside sa-table-wrap only.

3. **Logo + help icon** — Support Admin previously showed logo.jpg (120x120 stamp) at ~32px,
   which looked tiny/blurry. Support Admin now uses the same clear SVG clock mark +
   **AnyMinute** wordmark as Accountant. Help control uses an inline redesigned circular
   mark (soft fill + stroke question mark) instead of the thin generic icon path.

4. **Locations in Subscriber Details** — new **Locations** tab (Support Admin) matching the
   held design: Location / Business Name / Address / Time Zone / Status / Actions, search +
   status filter, details card with real fields. Data from AddBusiness + BusinessAddress
   (+ AnyminuteBusinessPolicy.TimeZoneID when present). Empty values stay dashes — no
   invented offices, phones, cities or time zones. **+ Add Location** reuses Add Business
   (view=bizadd). View opens location details / business view; Edit uses existing business edit.

5. **Subscriber name opens Details** — list name cells are SupportSubscribers.aspx?user=ID
   links with sa-sub-name styling so the click opens Subscriber Details (same destination as View).

## SQL
**Not required** for deploy. No schema migration. Locations reads existing tables only.

## Compile
/opt/monoenv mcs C#5/.NET 4.5: App_Code + SupportSubscribers.aspx + Main/Modern/MasterWithoutMenu
masters — **zero errors** (existing warnings only). See COMPILE.txt.

## Files
- css/AnyminuteReference.css — v76 SA shell / overflow / logo / help overrides
- css/AnyminuteSupport.css — Locations + name-link styles
- Modern.master.cs — Support Admin SVG brand
- App_Code/AnyminuteUiNavigation.cs — help summary SVG
- SupportSubscribers.aspx and .aspx.cs — Locations tab/views; name link title class; cache-bust
- Main.master, Modern.master, MasterWithoutMenu.master — stamp v76-20261109
