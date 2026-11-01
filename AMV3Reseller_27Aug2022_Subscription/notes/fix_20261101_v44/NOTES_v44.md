# AnyMinute v44-20261101: reseller welcome-email credentials now sign in

## Root cause
Support Admin > Resellers > Add Reseller saved the temporary password only as a salted SHA-256 hash in
AnyminuteReseller.TempPasswordHash, and it never created a login account. Login.aspx authenticates only through SP_LOGIN, which reads
UserAcc and compares DecryptByPassPhrase('MD2', UserAcc.Password). The emailed email and password therefore never matched
any UserAcc row, so the login always failed. Send Password Reset had the same problem.

## Fix
- App_Code/AnyminuteResellers.cs: `EnsureLogin` creates the reseller's sign-in the same way a legacy approved reseller gets one
  (SuperAdmin_Approve_Reseller -> Sp_InsertMotherOrg -> MotherOrg + UserAcc UserTypeid 0 'Admin'). It then writes the exact
  temporary password into UserAcc.Password in the SP_LOGIN format (EncryptByPassPhrase('MD2', nvarchar) hex) and sets
  Is_First_Time_Login=0, because Login.aspx otherwise blocks the sign-in.
  `SyncAccess` mirrors reseller Inactive or Login Disabled into AnyminuteLoginSecurity.LoginDisabled, which Login.aspx enforces.
- SupportResellers.aspx.cs:
  - Create: refuses a login email that is already registered to another AnyMinute login, or that is longer than 50 characters
    (SP_LOGIN uses nvarchar(50)). It also limits the password to 50 characters. After the reseller row is saved, it sets up the
    sign-in with the same password before the welcome email is sent. If the sign-in can't be set up, no email goes out and the
    flash message says why.
  - Send Password Reset: writes the new password to the sign-in, and creates the sign-in for resellers added before v44.
  - The Status and Login toggles sync the sign-in access.
- After sign-in the reseller is routed like other MotherOrg (UserTypeid 0 Admin) logins: Dashboard.aspx.
- No schema change. Meta v44-20261101.

## Existing resellers created before v44
They have no sign-in. Use Reseller Actions > Send Password Reset once: it creates the sign-in and emails a working password.
