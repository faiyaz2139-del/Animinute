# AnyMinute v73 — Wave-style typography hierarchy only
Date: 2026-11-08. Base: v72 (ce7428b). Code commit: ed73f992b94cf9f3f34fc574b91016720c64bf01.
Build stamp: v73-20261108. New stylesheet cache-bust: ?v=20261108v73.

## Actual font choices
- Editorial page titles: Georgia, "Times New Roman", serif; regular weight 400, normal letter spacing.
  Also applies to the accountant dashboard's existing section headings (.am-ap-sechead h2).
- Ordinary navigation, cards, labels, forms, buttons, tables and dialog headings:
  "Helvetica Neue", Arial, Helvetica, sans-serif. Existing font sizes and line heights remain unchanged.
- Dialog h1 headings remain sans-serif/600 rather than editorial page titles.
- The repository has bundled icon fonts, but no suitable bundled text serif/sans font files.
  These choices use system fonts only: no paid font, downloaded font, @font-face, @import or new network call.
  Actual rendering depends on fonts installed on the user's device; Helvetica Neue typically falls back to Arial.
- This is NOT an exact identification of Wave's font. The screenshot attachment was unavailable on this box.
  Implemented from the supplied description: regular editorial serif headings + compact, clean sans UI.

## Scope
One shared css/AnyminuteTypography.css, loaded AFTER existing page/global head styles, with an am-typography
body class in Main.master, Modern.master and MasterWithoutMenu.master. Covers the subscriber, reseller,
support/admin, accountant and legacy master shells consistently. Also wired the same typography into standalone
login/registration/reset/verification/external-login/report/document/dialog screens (no master page).
Existing classes remain; only the typography class, stylesheet link and master build stamps changed in markup.

No layout, spacing, color, font size, line-height, workflows, business code, database, or Wait Me screen changes.
No blanket * font reset or icon/pseudo-element reset; Font Awesome, glyphicon, simple-line/Flaticon and SVG icon
rules remain in place. Ordinary native controls are explicitly sans-serif, excluding font-icon class controls.
Existing code/pre monospace elements are not targeted.

## Files
- ExternalLogin.aspx
- Login.aspx
- Main.master
- MasterWithoutMenu.master
- ModalPopUpSelectEffectiveDate.aspx
- Modern.master
- Registration.aspx
- Report3.aspx
- ResetPassord.aspx
- ResetPassord_New_Account.aspx
- VerifyLoginEmail.aspx
- css/AnyminuteTypography.css
- document2.aspx
- document3.aspx

## Checks and limits
- Three master code-behinds compiled with /opt/monoenv mcs (C#5/.NET4.5), zero errors; existing generated
  control stubs used for Main and Modern. MasterWithoutMenu needs no stub. No .cs file changed.
- Automated reverse-diff check proves existing markup was otherwise untouched.
- Checked all 13 markup files have one versioned shared stylesheet, last in the head stylesheet cascade.
- CSS blocks balanced; declarations limited to font families, font weights, letter spacing and font-family variables.
  No dimensions, positioning, spacing, colors, network dependencies or universal descendant/pseudo resets added.
- git diff whitespace check passed allowing existing CRLF line endings (core.whitespace=cr-at-eol).
- Cumulative zip integrity, unique member names, new stylesheet references and byte-for-byte preservation of all
  unchanged v72 entries checked, including v72 wipe C#/SQL. No wipe or other SQL was run.
- Screenshot unavailable. No browser pixel/computed-style checks, full ASP.NET markup compilation or live
  smoke performed. Final font rendering/line wrapping/icon appearance should be checked after deployment.

## Deployment
Use the normal cumulative site/ overlay process. No SQL is needed. No push or live/database mutations occurred.
v72 wipe stays exactly as shipped; its inherited standalone SQL remains an optional destructive alternative,
NOT a deployment step. Do not run it while deploying this typography release.
