# AnyMinute Fix v57 — 2026-11-03 (Subscriber Details PPT, 4 slides, "Fix all")
Base v56 `a8a6086` (notes `d7ab381`) → code commit `9ef5cca` · meta `v57-20261103` · cache-bust `?v=20261103v57`. No new DB script (uses REQUIRED_08 AnyminuteAddressReview when present). Web.config unchanged.

## S4 — Manual / unverified subscriber address → Address Verification (HIGH)
- `App_Code/AnyminuteSupportAddress.cs`: `Render(..., status, verify)` adds a hidden `{prefix}_src` (lookup|manual) and status pill: **Verified Address** (green) / **Pending Verification** (amber) / **Address Rejected** (red) / **Not Verified** (grey). Badge is no longer a static "Selected Address". New `Status()`, `QueuePending()`, `ReviewReady()`, `Pill()`, `Gate(..., badge)`.
- `QueuePending`: verified match (postal + street, AddressMaster or review original) → nothing queued; existing pending → reused; otherwise inserts AddressMaster (Verifed=0, Street_Name = Line 1) + AnyminuteAddressReview row Status=Pending, SubmittedBy "<name> (Subscriber)", OrigStreet = full street, Country, Notes. It then shows on SupportAddresses.aspx in the Pending Verification tab and in the sidebar badge count.
- `SupportSubscribers.aspx.cs`: when Edit Subscriber changes the address, the new address is queued in the same transaction and the page flashes "sent to Address Verification (Pending Verification)". Add Subscriber also queues the address. Details header and the Edit address gate show the status pill.
- `Scripts/AnyminuteSupport.js`: "Enter Address Manually", or editing fields after a lookup pick, switches the pill to Pending Verification and sets `_src=manual`.

## S1 — Add Business under plan limit + Upgrade Plan works (HIGH)
- Businesses tab: when active < plan limit (e.g. 5/6) or no limit is set, **+ Add Business** opens `view=bizadd`. The form has Business Name*, Primary Contact Name*, Primary Contact Number* (10 digits), Main Email, and a required Business Address (lookup/manual). At or over the limit it shows the "Business limit reached" panel with a working Upgrade Plan link.
- New `App_Code/AnyminuteSupportBusinessCreate.cs`: same SP chain as the owner's Add Business (Sp_InsertBusiness → Sp_InsertAssginBussiness owner UserTypeId 1 → MappingTable → Sp_InsertBusinessAddress + optional Country). It runs in one transaction, re-checks the limit and duplicate name server-side, queues the business address as Pending when it is unverified, and writes the audit entry "Business Added". The reseller NoBiz gate blocks both the view and the post.
- All Upgrade Plan links (header kebab, limits header, Businesses panel) → `view=edit&up=1#plan`. With `up=1`, Edit shows the New Plan picker, highlights the plan card, scrolls to it and focuses the select.

## S2 — Businesses row "Edit" → "View" (MEDIUM)
- Row button: eye icon + **View** → `view=bizview&biz=id`. This is a read-only card inside the Subscriber Details shell, scoped to the subscriber's businesses. It shows name, operating name, number, contact, phone, email, address + status pill, employees/projects, and the buttons Back / View Employees / View Projects / Edit Business. The kebab is kept ("View Business" now opens the same card). Employees/Projects tabs are unchanged.

## S3 — Address Not Found card red (LOW/MED)
- `css/AnyminuteAddressLookup.css`, `css/AnyminuteSupport.css`, `css/AnyminuteBusinessEditor.css`: the card has a red border with a 4px red left bar, a light red background, a solid red "!" icon and a bold red title. This is shared, so Add Business / Employee Editor / Profile match. The icon glyph changed from i to ! in AnyminuteSupportAddress.cs and AnyminuteAddressLookup.cs.

## Build
- `cc.sh` App_Code (mono mcs, langversion 5): **Compilation succeeded** (1 pre-existing warning).
- `pc.sh` smoke compile of SupportSubscribers.aspx + code-behind against the new App_Code.dll: **succeeded**. JS `node --check` OK.
- Not run: live smoke / DB execution (local executor only).

## Risks
- Support-side Add Business has not been run against a live DB. The SP parameter list mirrors AnyminuteBusinessSave. BusinessNumber, LegalEntityType and RegistrationDate are sent empty.
- If an admin corrects the street when approving a review, the subscriber pill can read "Not Verified" (the match is postal + street).
- Lookup picks taken from unverified EmpAddress/BusinessAddress sources are also queued as Pending (by design: only Verified addresses skip the queue).
- If AnyminuteAddressReview is missing, it falls back to AddressMaster Verifed=0 only.
