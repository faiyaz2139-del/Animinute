# AnyMinute v87 (stamp v87-20261117) - Employees list sort icons

Base: v86 (b892ad1 / notes 2d9bf2a). Cumulative and lean like v86. No SQL. No data deleted. Not deployed, not pushed.

## Bug
On the subscriber Employees list (Users.aspx), the column-header sort indicators next to Businesses, Roles, Rate and Status rendered as broken tiny boxes / missing glyphs. The one next to Employee used the same markup but also looked small. All five headers used `&#8661;` (U+21D5 ⇕). The UI font is Helvetica Neue / Arial / Helvetica (`--am-font-ui`), which does not provide a crisp glyph for that code point, so the character fell back to a tiny or missing box.

## Change
Replaced the unicode in all five sortable headers (Employee, Businesses, Roles, Rate, Status) with the same inline SVG up/down sort icon (`span.am-sort-icon > svg`). Shared CSS in css/AnyminuteEmployeesV2.css sizes and colors the icon consistently (10x13px, #9aabbd). LinkButton CommandArgument / OnCommand="Sort_Command" / CausesValidation are unchanged, so sorting behaviour is intact. Cache-buster on the V2 stylesheet is now `?v=20261117v87`.

`.am-sort` is only used on this Employees list, so no other grid shared that component. SupportSubscribers.aspx.cs still has a separate `sa-sort` / `&#8661;` icon on the SuperAdmin subscribers list; it was left alone.

## Stamp
Modern.master, Main.master, MasterWithoutMenu.master: `anyminute-build` = v87-20261117, and existing `?v=20261116v86` cache-busters on those masters (and BusinessProfile.aspx data-build / dashboard CSS query) bumped to `?v=20261117v87`. No other behaviour change on those files.

## Deploy
Copy site/ over the live site (Web.config is not in the zip; keep the live one). No SQL required. Same compile-on-server method as v86. Check: view-source meta anyminute-build = v87-20261117; open Employees and confirm every sortable header shows a crisp, matching up/down sort icon.

## Not in this drop
No deploy, no GitHub push, no CloudAgent, no SQL, no flat-screen or Supervisor notes (those stay on hold).
