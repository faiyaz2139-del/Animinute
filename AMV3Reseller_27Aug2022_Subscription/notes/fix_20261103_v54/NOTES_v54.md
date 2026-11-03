# AnyMinute v54 — Fix-all notes (docx 2026-11-03)

Source: NOTES.docx (+ img_01 Send Password Reset without sticky header; img_02 Login & Access with red sticky header + Send Password Reset action). Base: v53 `b392854`. Local lean cumulative. NO CloudAgent. NO push. NO live smoke.

## MUST-FIX

### Send Password Reset keeps sticky Subscriber Details header
When on Subscriber Details → Login & Access, clicking **Send Password Reset** (header button or the action card) opens `view=reset`. Today that is a standalone page (Title/Lead + Reset form only) — the red-boxed Details header disappears (img_01).

**Required:** the same sticky Details header (avatar, name, status, plan since, Edit / Send Password Reset / More, Email/Phone/Address, Activation/Next Billing, Subscription Limits) must stay visible, same as every other tab (img_02 / NOTES_v53 sticky rule). Put `view=reset` inside the Details + DTabs shell.

- Render: `Details(c,r) + DTabs(c,r, "access") + Reset(...)` (Login & Access tab stays selected).
- Title1/Lead empty (no separate "Send Password Reset" page chrome that replaces the header).
- Breadcrumb: Subscribers > Subscriber Details > Send Password Reset (or Login & Access > Send Password Reset) OK.
- Cancel on Reset → back to Login & Access (`view=access`), not only the Details root.
- Edit Subscriber stays a separate page (not in this note).
- Keep existing Reset form steps/content; only wrap it in the sticky shell.

## Ship
- Meta `v54-20261103`; CSS/JS `?v=20261103v54` where bumped
- Zip `/workspace/anymin/AnyMinute_Fix_20261103_v54.zip` cumulative on v53
- FIX_REPORT_v54.md, CHANGES_v54.diff, notes in site/notes/fix_20261103_v54/
- mcs compile SupportSubscribers
