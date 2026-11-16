# AnyMinute v83 (stamp v83-20261116) - Address Not Found stays after a manual address on Add Employee

## Symptom
On Add Employee (EmployeeEditor.aspx, the dialog-style page from Employees / Dashboard "Add Employee"), a postal search for L1V 3M3 shows the red "Address Not Found" card. Faiyaz then entered the address manually (Street Number 1975, Address Line 1 Memory Lane, Suite Type Unit, Suite Number 8, City Pickering, Province ONTARIO, Postal L1V 3M3, Country Canada). The card stayed visible after later postbacks (Add Assignment and Save) and he thought it was stopping the save.

## Root cause (code)
The card is an `asp:Literal AddressNotFound` set `Visible=true` in `SearchPostal_Click` when the postal search has no matches. That Visible flag is kept in ViewState, so every later postback (Add Assignment, Save, and so on) renders the card again.

Client script (`AnyminuteAddressLookup.newAddress`) already tried to hide `.am-al-nf` when "Enter Address Manually" is clicked, but:
- the Employee Address fields are always visible, so typing a manual address does not have to go through that button, and the card is left on screen;
- even when the button is used, the hide is only in the browser; the server still has `AddressNotFound.Visible=true`, so the next postback brings the card back.

The lookup result does **not** block save. `AnyminuteEmployeeEditor.Validate` only requires street number, address line 1, city, province/state and postal code (unchanged). The two red banners on Faiyaz's screenshots were separate, valid checks:
- "Add at least one business assignment." when no assignment had been added yet;
- "This email is already registered in the selected business." after he added the M.A. Consulting Corp. assignment (duplicate email for that business). Neither was changed.

## Fix
On Add Employee / Edit Employee (`EmployeeEditor`):
- "Enter Address Manually" dismisses the card immediately (JS) and sets a hidden `AddressManual` flag. The button always `return false` so it cannot post back and clear fields already typed.
- Filling the required address fields also dismisses the card (JS, on input/change and on load).
- `Page_PreRender` (and `AddNewAddress_Click`) hide `AddressNotFound` when the flag is set or those required fields are filled, so a postback cannot re-show the card.
Required-field validation for the manual address is unchanged. The business-assignment and duplicate-email checks are unchanged.

## Data
No data or SQL change.

## Deploy
Copy site/ over the live site (Web.config is not in the zip; keep the live one). No SQL required. Same compile-on-server method as v81/v82. Check: view-source meta anyminute-build = v83-20261116; on Add Employee, search a postal code that has no match, enter the address manually (or fill Street Number, Address Line 1, City, Province, Postal), and confirm the Address Not Found card hides and stays hidden after Save / Add Assignment. The save can still be stopped by a missing assignment or a duplicate email, which is intended.

## Not in this drop
No deploy, no GitHub push, no CloudAgent, no SQL writes. EmployeeProfile has the same lookup card but was not in the screenshots and was left unchanged except that it shares the updated JS file.

## Also in this drop (same stamp)
On Employee Profile (EmployeeDetails.aspx, titled "Employee Profile"; ProfileUrl from the Employees list — not the legacy EmployeeProfile.aspx), the "Business assignment" label sat flush against the business dropdown (M.A. Consulting Corp.). The label wrapped the DropDownList with no gap. Added class `am-context-assign` and `display:inline-flex;align-items:center;gap:12px` in AnyminuteEmployeesV2.css. Cache-bust `?v=20261116v83` on that page only. No other behaviour changed.
