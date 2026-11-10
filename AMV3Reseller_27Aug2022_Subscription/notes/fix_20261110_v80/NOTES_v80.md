# AnyMinute v80 (stamp v80-20261110) - already-Verified address must not go to Pending Verification

## Root cause (found in code + legacy SP text in the schema dump)
Add Subscriber (AnyminuteSubscriberCreate -> EmpAddressController.InsertEmpAddress -> dbo.Sp_InsertEmpAddress) and every
business save (dbo.Sp_InsertBusinessAddress) also write AddressMaster themselves:
  IF NOT EXISTS(SELECT * FROM AddressMaster WHERE PostalCode=@Postalcode) INSERT AddressMaster(Street_Name,City,Province,PostalCode)
  VALUES(@StreetName,@CityId,@ProvinceId,@Postalcode)   (+ same again when the exact street text is new)
- Verified/imported rows store the postal code without a space ("M1E0B6"); the form sends "M1E 0B6" -> "not exists" -> a new row.
- City/Province get @CityId/@ProvinceId; AnyminuteSubscriberCreate never set them -> 0. Street_Name = Address Line 1 only (no number).
- Verifed is left NULL, and TR_AnyminuteAddressMaster_Submitted makes every new AddressMaster row a 'Pending' review row.
=> "Neelands Cres | 0 | 0 | Canada | M1E 0B6 | Fawaz Thakor (Subscriber)". AD.QueuePending itself had correctly found the
Verified match and inserted nothing; the stray row came from the legacy SP. Same SP behaviour gave new (unverified) addresses City 0.

## Fixes
1. AnyminuteSupportAddress.AfterLegacyInsert (called right after the legacy SP in EmpAddressController.InsertEmpAddress,
   AddBusinessControllerClass.InsertBusinessAddress and AnyminuteSupportBusinessCreate, so it covers Support/Reseller Add Subscriber,
   registration, payment success, Add/Edit Employee, Add/Edit Business, profile/address pages that use these):
   rows the SP just added for this address (id > max id read before the call, not verified, same compact postal + street) are
   - removed when the address is already Verified, or already waiting in Pending (no duplicate pending row);
   - otherwise kept once, with City/Province NAMES (ids resolved via CityMaster/ProvinceMaster) and the full submitted street
     (with number) on the review row.
   Never throws (logged), so a save never fails because of it.
2. Matching (Status/QueuePending/AfterLegacyInsert): postal ignoring spaces/dashes/case + street ignoring case, also when the number
   is typed inside Address Line 1 ("19 Neelands Cres" = "NEELANDS CRES"). City/province are not compared: verified rows hold
   CityMaster/ProvinceMaster ids and postal code + street already pins the address.
3. Address Verification list + View panel: City/Prov show names; '0' / unknown ids fall back to the submitted value or to the linked
   subscriber/employee/business address, else a dash (never "0"). Address (Submitted) shows the street number (review OrigStreet,
   else the linked EmpAddress/BusinessAddress StreetNo). Verified tab unchanged (no extra lookups).

## SQL (not run; no DB access)
find_pending_already_verified.sql - read-only list of Pending rows that match a Verified address + Pending rows with City/Prov 0;
optional cleanup at the bottom is commented out. The existing Fawaz Thakor row will show in it; v80 only prevents new ones.

## Not verified
No browser/DB test (box cannot run the app). After deploy: run the SQL, then Add Subscriber with a verified address -> nothing new
in Pending; with a new address -> one Pending row with real City/Prov and street number.
