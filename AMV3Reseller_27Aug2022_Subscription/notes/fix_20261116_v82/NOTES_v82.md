# AnyMinute v82 (stamp v82-20261116) - "Your plan is not active" on Add Business for active subscribers

## Symptom
Subscriber (Fawaz T...) opens Business > Add Business and gets the red message
"Your plan is not active. Please pay the amount from the link received in the registered email id." The form is disabled.

## Root cause (code)
The page-load gate (direct-subscriber branch, i.e. the MotherOrg email is not a reseller email) is:

    PlanHistory rows for EmpId with IsActive=1 exist          (else "Please contact admin. Your plan is not active.")
    and no Is_Cancelled=1 row                                 (else "Your plan has been cancelled.")
    and total PlanHistory rows for EmpId == 1
    and rows with Is_Requested_Plan=1 == 0   <-- BUG (inverted)
    => "Your plan is not active. Please pay ..."

Every other copy of this gate (17 places: Home_Page x4, Add_Project x2, View_Project x2, Schedular x2,
AddEditRole_Add_Employee x2, Add_EmployeeV2_Accountant x2, Add_EmployeeV2 reseller branch, Add_Business reseller branch,
Home_Page_EmpPopUpList) uses `== 1`, meaning "the only plan row is a requested (Enterprise) plan still awaiting payment".
Add_Business (line 139) and Add_EmployeeV2 (line 63) had `== 0` since the original code, which blocks exactly the
legitimate case: a subscriber with one ACTIVE plan row that was never a payment request. That is every
- Support Admin "Add Subscriber" account (AnyminuteSubscriberCreate inserts one PlanHistory row, IsActive=1, TransId='SUPPORT',
  Is_Requested_Plan NULL),
- self-registered Basic (free) subscriber (Register_As_A_Bo inserts one row, IsActive=1, Is_Requested_Plan NULL),
- subscriber whose single plan row was activated directly by Support with no request.
The dashboard (Home_Page, `== 1`) lets them in, so they only hit it on Add Business / Add Employee (V2).

No date or time-zone comparison is involved in this gate (StartDate/EndDate are not checked), so the us-east-2 -> Canada
move is not a factor.

## Fix
`Add_Business.aspx.cs` line 139 and `Add_EmployeeV2.aspx.cs` line 63: `Rows.Count == 0` -> `Rows.Count == 1`.
The genuine payment gate is kept: a subscriber whose only PlanHistory row is an unpaid requested plan
(Is_Requested_Plan=1) still sees the message, as on every other page. Reseller-subscriber branch, cancelled, max-limit and
"contact admin" checks are unchanged.

## Data
No data change needed for the code bug. If Fawaz is still blocked after v82, run `diag_plan_active.sql` (read-only):
- 4_gate_direct.v82_pay_msg = BLOCKED => his only row has Is_Requested_Plan=1 (requested plan not paid) - genuine, pay link
  or Support "Approve Plan" (cost 0) activates it.
- active_rows = 0 => different message ("contact admin"), plan row IsActive not 1.
- 2_branch says RESELLER-SUBSCRIBER and 5_reseller_plan shows reseller_total_rows=1 with Is_Requested_Plan=1 => the
  reseller's own plan is unpaid (legit gate, on the reseller).

## Deploy
Copy site/ over the live site (Web.config is not in the zip; keep the live one). No SQL required. Same compile-on-server
method as v80/v81. Check: view-source meta anyminute-build = v82-20261116; Fawaz -> Business > Add Business shows the form.

## Not in this drop
No deploy, no GitHub push, no CloudAgent, no SQL writes.
