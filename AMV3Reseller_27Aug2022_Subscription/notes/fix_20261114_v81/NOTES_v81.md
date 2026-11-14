# AnyMinute v81 (stamp v81-20261114) - Duplicate Addresses tab

Source: 3-slide PowerPoint (screenshots only, no slide text). No red-X removals (the only red pixels on slide 1 are the existing Pending "33" badges).

## Slide 1 - Duplicate Addresses tab
Address Verification now has a fourth tab, **Duplicate Addresses**, with a purple count badge (number of rows in open groups). The Status filter includes the same option. The table, filters, pagination and + Add Address / Import Address / View Verified Addresses buttons are unchanged. Matching records are listed together (groups are not split across pages).

## Slide 2 - How AnyMinute checks duplicates
When an address is added, imported or edited, the system reads Street Address and Postal/ZIP Code, standardizes spacing / capitalization / postal formatting, and compares them to existing AddressMaster rows in the same country. If both fields match, the new row is still saved and the pair (or group) is listed as a Potential Duplicate. This replaces v78's "block Add/Import of an exact duplicate".

The View panel shows a **Duplicate Group #DUP-00n** card with Record A/B (and C/D if needed): Address, Postal Code, Status (Verified / Pending Review / Rejected), and Submitted By.

## Slide 3 - Actions menu
View stays on the row. The ⋮ menu is: Edit, Keep as Original, Mark as Duplicate, Dismiss Duplicate, Delete Duplicate. All have handlers.

- **Edit** opens the panel with fields enabled and **Save Changes** (correct spelling or postal code; the group is then recomputed).
- **Keep as Original** marks the preferred record in the group.
- **Mark as Duplicate** confirms this record is a duplicate of the original. If the group has two records and no original yet, the other record becomes the original. For 3+ records, choose Keep as Original first.
- **Dismiss Duplicate** marks the match as legitimate and removes this record's pairs from the queue (a 2-record group leaves the tab; a larger group stays if two or more still match).
- **Delete Duplicate** is a **soft-delete** (AnyminuteAddressDup Action=Deleted). The AddressMaster row is not removed. If the address is linked to a subscriber, employee or business, it is **not** deleted; the flash tells the admin to keep the original, mark it as a duplicate, or move the link through an authorized process.

Soft-deleted rows are hidden from all Address Verification tabs and from the nav Pending badge.

## Database
Decisions need `dbo.AnyminuteAddressDup`. Script: `DB_REQUIRED/REQUIRED_10_AddressDuplicates.sql` (idempotent CREATE TABLE / INDEX only; no data is deleted). The page also tries to create the table on first use if the login can CREATE TABLE. The tab still lists potential matches if the table is missing; Keep / Mark / Dismiss / Delete cannot be saved until the table exists.

## Not in this drop
- No deploy, no GitHub push, no CloudAgent, no destructive SQL.
- Soft-delete does not remove the AddressMaster row, so postal lookup may still return it.
- Keep as Original does not auto-verify a Pending record.
- Subscriber / employee / business saves still reuse an existing AddressMaster row (v80); they do not create a second master row. New duplicate *rows* come from Add/Import/Edit (or already-present master rows).
- Import adds at most two copies of the same street+postal+country from one file (further copies are skipped).
