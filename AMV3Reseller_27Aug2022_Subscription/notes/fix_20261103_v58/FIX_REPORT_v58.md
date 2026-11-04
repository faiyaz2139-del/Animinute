# AnyMinute Fix v58 — 2026-11-03 (Support Admin PPT, 6 slides: header, My Profile, Login & Access credentials, "Fix all")
Base: v57 `9ef5cca` (notes `c62d1b7`) → code commit `2680c4f` · meta `v58-20261103` · cache-bust `?v=20261103v58`. No DB script and no Web.config change (uses the existing ResetUrl_New_Account and IsSendMail settings).

## S1/S2 — Header always "Welcome! Support Admin" (HIGH)
- Root cause: on Modern.master pages (Subscribers etc.) an empty Session["user"] fell back to `FN_DESC('AddEmployee', Session["EmployeeId"])`. For a Support Admin, EmployeeId is a **SupportAdminId**, so this picked up an unrelated employee ("Parvin Thakor"). After any Main.master page (My Profile) the header showed the SupportAdmin row name instead.
- `Modern.master.cs`: Support Admin (`IsSupportAdminOnly`) skips the AddEmployee fallback and shows **Welcome! Support Admin** with no role/business sub-line. The real name is read from SupportAdmin and kept in Session["user"], so audit "changed by" stays correct. Reseller and accountant headers are unchanged.
- `Main.master.cs`: a SupportAdmin user gets lblwelcome = "Welcome! Support Admin" (Session["user"] still holds the real name).

## S2/S3 — My profile is a real editable profile (HIGH)
- `MyProfile.aspx.cs`: a Support Admin is redirected to `SupportAdminProfile.aspx`, so the empty business list is gone. The redirect covers the Modern account menu, the Main.master profile button and bookmarks.
- `SupportAdminProfile.aspx(.cs)` rewritten on Modern.master (Support console look, no Home / Back to List / ✕ links):
  - Title, First*, Middle, Last*, Date of Birth, Gender, Main Phone*, Mobile, City, Province, plus Login Email (read-only).
  - Validation covers the required fields, phone format and DOB. Save goes through the existing `Sp_UpdateSupport` and shows the flash "Profile updated successfully.". It refreshes Session["user"] and writes the audit row "Profile Updated" when the table exists.
  - Email and password changes link to Login & security (verified email-change flow, v39) and Change password. The old read uses a parameterised query (it used string concatenation before).

## S4 — Send Password Reset actually resets (HIGH)
- Header **Send Password Reset** opens the confirm page; **Additional Account Actions › Send Password Reset** does the same. On send:
  1. A **new temporary password** is generated (`AnyminuteResellers.NewTempPassword`) and saved in the SP_LOGIN MD2 format. The old password stops working.
  2. An email with the subject **"Your AnyMinute Password Has Been Reset"** goes out. It contains the login email, User ID, the **temporary password** and a **Set New Password** button (ResetUrl_New_Account + EmployeeId), plus a sign-in link.
  3. The audit row "Password Reset" is written, so **Last Password Reset** updates on Login & Access. The page returns to Login & Access with the flash "Password reset sent to … Temporary password (shown once): …".
- It no longer depends on the REQUIRED_08 token table; a missing table previously meant "Nothing was sent". The editable "Send to Email Address" field is kept. Preview and confirm text are updated.

## S5 — Resend Invitation includes the password (HIGH)
- Resend always issues a **fresh temporary password**, for Active accounts too (previously only Pending accounts got one; everyone else got the "(your current password; use Forgot Password…)" placeholder).
- `AnyminuteSubscriberCreate.SendInvitation` now builds its own email: subject **"Welcome to AnyMinute - Your Account Invitation"**, with login email, User ID, temporary password and a **Welcome** button. The TemplateMaster type 3 subject "Reset Password" is no longer used, and the placeholder is gone. A new `SendPasswordReset` shares the same body builder. Add Subscriber (with invitation) uses the same email.
- Card and kebab texts say "with a new temporary password".

## S6 — Welcome page accepts the emailed temporary password (HIGH)
- `ResetPassord_New_Account.aspx.cs`:
  - The temporary-password check is now **parameterised**: the typed text used to be concatenated into SQL. It is trimmed and scoped by LoginType: EMP excludes SupportAdmin rows, Support only matches SupportAdmin, Acc matches by MotherOrgid.
  - The matched UserId is kept in ViewState, and the new password is written to **that one UserAcc row only**. The old Updatepass 'EMP' updated every row with that EmployeeId, which could include a SupportAdmin row with the same id.
  - `Is_First_Time_Login` is cleared, an audit row "Password Changed" is written, and the success alert redirects to Login. Max length is 50 characters (the nvarchar(50) cast).
- Markup: a hint under the box says the temporary password is in the invitation or password reset email, and to use the most recent email.

## Checklist vs slides
| Slide | Ask | Status |
|---|---|---|
| S1 | Header shows Parvin Thakor → should be Support Admin | Done (Modern + Main master) |
| S2 | Header flips to "Support Admin" on My Profile; My Profile is empty list | Done (consistent header; profile redirect) |
| S3 | Can't edit the profile | Done (editable SupportAdminProfile, Save) |
| S4 | Can't reset the password | Done (temp password reset email + Last Password Reset) |
| S5 | Resend Invitation email has no password | Done (fresh temp password in email) |
| S6 | Welcome page: don't know temp password; "Reset Password" subject | Done (email carries it; page accepts it; invitation subject) |

## Build
- `cc.sh` App_Code (mono mcs, langversion 5): **Compilation succeeded**, with 1 pre-existing warning.
- `pc.sh` page compiles against the new App_Code.dll all succeeded: SupportAdminProfile, SupportSubscribers, MyProfile, ResetPassord_New_Account, Modern.master, Main.master.
- Not run: live smoke / DB / real email send (local executor only).

## Risks
- Send Password Reset now **replaces the subscriber's password immediately**; this is by design and matches the Welcome flow. The temporary password is also shown once to the Support/Reseller user in the flash (same as v45 invite).
- Resend Invitation on an **Active** account now also replaces their password with the new temporary one.
- The Welcome link uses `ResetUrl_New_Account` + UserAcc.EmployeeId. If that setting is missing, the email only has the sign-in link (temporary password still works at Login).
- Header shows the literal "Support Admin" (per PPT); the person's name is no longer shown anywhere in the Support header.
- SupportAdminProfile is restricted to Support/SuperAdmin (`SA.Require`); a SuperAdmin without a SupportAdmin row sees "No Support Admin profile was found".
