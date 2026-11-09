# AnyMinute v74 — measured reference styling, pending parent visual review
2026-11-08. Base v73 aba82de. Code commit f0b17e1ece14aa32ccacc465fd9a192e52d45d67.
Build v74-20261108; typography/reference styles cache-busted 20261108v74.

## Source and measurements
Original supplied image successfully read and preserved at /workspace/anymin/research_v74/reference.jpg.
Native image is 1600 x 789, NOT 1024 x 505. The latter is its scaled chat display (~64%).
Measurements were translated at native scale: topbar ~48px, sidebar ~188px, main content x~275,
right edge ~1508, section cards ~600px with ~34px gap. Main heading ~42px and section heading ~30px.
Implemented 48px topbar, 188px sidebar, 32px top padding and responsive side gutters capped at 88px
(content x276 at 1600), 32px two-card gap, 42px page headings, 30px section headings.
Small JPEG variations prevent claiming exact original CSS color values. Dominant samples:
- topbar/notice #F6FBFF; page/sidebar/card white
- selected nav/blue action #E0F2FF
- pastel actions #EAFEF3, #E0F2FF, #F1E8FB, #FDF0E7
- chosen navy text #172C3B and subtle border #DCE4E8; card radius 6px, no shadow
Nav text 13px, UI/forms/tables 14px; existing labels and form/content hierarchy retained.

## Fonts: evidence versus choice
Verified current public Wave marketing stylesheet declares Dinero Sans (400/600/700) and STIX Two Text Variable:
https://www.waveapps.com
https://cdn.prod.website-files.com/62446230dcb514b828a6e237/css/wave-apps.shared.5e8a2d235.min.css
These declarations prove marketing-site use only, NOT the exact screenshot dashboard families.
Dinero Sans was NOT copied, downloaded or referenced: reuse rights were not verified.
Chosen editorial face: locally bundled STIX Two Text Variable, regular 400; CSS alias "AnyMinute Editorial";
fallback Times New Roman/serif. This is evidence-informed, not a claimed pixel-based font identification.
Chosen UI face: system Arial, fallback Helvetica/sans-serif. This remains a visual approximation to the screenshot's
compact sans, not a claim that the screenshot uses Arial or that Arial equals Dinero.
Font + SIL Open Font License bundled without modifying the font:
https://raw.githubusercontent.com/google/fonts/main/ofl/stixtwotext/STIXTwoText%5Bwght%5D.ttf
https://raw.githubusercontent.com/google/fonts/main/ofl/stixtwotext/OFL.txt
No new external runtime font calls or paid/licence-restricted font distribution.

## Actual production changes
New shared css/AnyminuteReference.css after the existing typography/global/page styles. The same three masters
and ten standalone screens from v73 gain am-reference and v74 stamps/cache-busts. CSS adapts modern/legacy,
support, reseller and accountant shell variants. Existing JS drawer/pin/collapse semantics are unchanged.
Compact shell, reference-like white/pale-blue surfaces, light card/table borders, regular editorial headings,
outline/pill secondary buttons and pastel DASHBOARD quick actions. Owner-dashboard Quick Actions uses its
actual .am-quick-grid, not a preview-only class. No Wave branding, accounting charts, sample financial metrics,
routes, server IDs, postback/event code or workflow changes. No Wait Me screen changes.
Warnings/destructive colors, validation error borders, focus states, icon fonts and responsive/print behavior retained.
Production page content remains AnyMinute content, so element y-positions and card composition intentionally
vary from Wave. Some roles retain their existing closed drawer default; opening/collapsing remains functional.

## Parent visual review: READY, NOT YET VERIFIED
Local preview: http://127.0.0.1:8740/index.html
Reference: http://127.0.0.1:8740/reference.jpg
Preview file: /workspace/anymin/research_v74/preview/index.html
Open at native 1600 x 789 first. 1024 x 505 is a smaller CSS viewport, NOT the same as scaling a 1600px screenshot
(the source image shown at 1024px in chat was scaled, not necessarily a 1024px viewport).
Preview uses the actual production styles via local asset symlinks and representative real AnyMinute classes,
labels and controls. It combines Add Subscriber form cards and existing dashboard quick-action labels for style
inspection, rather than pretending to be a specific server-rendered page. It contains an explicit STATIC PREVIEW
notice, empty fields/table and no data metrics. Its links cannot submit to the live app. The local server binds only
127.0.0.1 and serves the dedicated preview/asset tree, not the ASP.NET source/config directory.
This is NOT live ASP.NET validation. Parent must inspect the rendered preview before delivering the zip.
No scripted browser launch/screenshot/visual-comparison was run by this executor.

## Verification
- Three masters compiled via /opt/monoenv mcs C#5/.NET4.5: zero errors (existing control stubs).
- Static checks: only style links/classes/stamps changed in markup; CSS braces, font licence/signature,
  no external runtime URLs, no icon pseudo resets; all preview/CSS/font resources return HTTP 200.
- v72 wipe C# and standalone SQL byte-identical to original commits. No SQL or database connections.
- Cumulative archive checks preserve every unchanged v73 entry and all v72 wipe files exactly.
- Limitations: no computed-style/pixel diff or live runtime verification. Font identity cannot be established
  from screenshot pixels alone; UI Arial substitute and retained content/navigation states are residual differences.
- No CloudAgent, push, live-site mutations, database access or wipe execution.
Deployment: normal cumulative site/ overlay; no SQL needed. Inherited wipe SQL is NOT a deployment step.

## Changed files
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
- css/AnyminuteReference.css
- css/fonts/STIXTwoText-OFL.txt
- css/fonts/STIXTwoText-Variable.ttf
- document2.aspx
- document3.aspx
