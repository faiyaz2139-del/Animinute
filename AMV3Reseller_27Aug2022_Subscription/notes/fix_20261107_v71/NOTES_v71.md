# AnyMinute v71 (2026-11-07) - "Fix all" (held_20261107_v71: Address Verification > Verified Addresses)
Base v70 46e2092. Code commit 1790260. Build stamp v71-20261107; cache-bust ?v=20261107v71 (SupportAddresses.aspx: AnyminuteSupport.css/js).

1. Header buttons (next to View Verified Addresses): "+ Add Address", "Import Address".
2. Add Address (SupportAddresses.aspx?tab=verified&add=1, modal): Street Name (Address), City, Prov/State (datalist CA provinces + US states),
   Country (Canada/USA/UK/India, same list as filters), Postal Code/PIN. Same server validation as Approve (SA.VerifyAddress: all five required,
   postal format per country; NormalizePostal). Inserts AddressMaster (Verifed=1) + AnyminuteAddressReview Status=Verified,
   SubmittedBy "Added by Support (<name>)", submitted/reviewed now; audit "Added (Verified)". Duplicate (verified row with same street + city + postal) is refused.
   On error the modal re-opens with the entered values (session).
3. Import Address (?import=1, modal): CSV (comma/semicolon, quoted, UTF-8 BOM / 1252) or .xlsx (first sheet; minimal zip reader on DeflateStream, no new assembly).
   Header mapped by name (Address/Street, City, Prov/State/Province, Country, Postal/ZIP/PIN); no header = template order. Download template (?template=1) = CSV header.
   Max 2 MB / 5,000 rows. Blank rows skipped; duplicates (in DB or within the file) skipped; invalid rows listed "Row N: reason" (first 50).
   One serializable transaction; one audit entry with counts; SQL error = nothing added. Summary shown on the Verified tab.
Security: page already SA.Require() (Support Admin only); postbacks protected by ViewState MAC with ViewStateUserKey=SessionID (Modern.master), same as Approve/Reject. Form enctype set to multipart on this page.

Files: App_Code/AnyminuteAddressImport.cs NEW, SupportAddresses.aspx(+.cs), css/AnyminuteSupport.css, Main.master, Modern.master.
SQL: none (uses AddressMaster + AnyminuteAddressReview/AnyminuteSupportAudit from REQUIRED_07/08).
Compile: App_Code + SupportAddresses.aspx.cs (mcs stub, /opt/monoenv) 0 errors. CSV + deflated .xlsx parsing unit-tested with mono.
Risks: no live run. Country limited to Canada/USA/UK/India (existing Verify rules). Prov/State and postal are required (Approve rules). City/Province stored as text (not CityMaster ids), like QueuePending.
Large imports hold a serializable transaction on AddressMaster for the duration.
