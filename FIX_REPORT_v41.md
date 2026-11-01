# AnyMinute v41-20261031 (cumulative on v40)

## Google (Gmail) and Microsoft sign-in for everyone
- Login.aspx: "Continue with Google" and "Continue with Microsoft" under SIGN IN for every role (Employee, Supervisor, Subscriber/Business Admin, Reseller, Accountant, Payroll Agency, Support Admin, Super Admin). Roles never hide them. A button only appears once that provider's ClientId and ClientSecret are set in appSettings.
- Password sign-in still works. Both paths now share one `SignIn()` method in Login.aspx.cs, so the first-time-login check, disabled login access, the Login Activity History, the session guard and role routing are identical.
- ExternalLogin.aspx is both the start link (`?p=google|microsoft`) and the OAuth redirect URI. It uses the OpenID Connect authorization-code flow with single-use state, a nonce and PKCE S256. The id_token comes from the token endpoint over TLS, and its iss, aud, exp and nonce are checked.
- Permanent User ID: the first time someone uses a provider, it is linked to the existing `UserAcc.UserId` whose login email matches the provider's **verified** email (case-insensitive). After that, the link is looked up by provider + subject and stays with the same UserId, even if the login email is later changed through v39 Change Login Email. No duplicate account is ever created.
- New users: **no auto-create**. AnyMinute sign-up goes through the Subscriber/Reseller registration forms and Support approval, and other roles are invited by their admin. So when no login uses that email, the user is told to sign in with a password, use Sign Up, or ask their administrator for an invite. If more than one login uses the email, the user is told to contact Support.
- Microsoft email trust: the email is accepted for personal Microsoft accounts, for accounts with the `xms_edov` optional claim (email domain owner verified), or for tenants listed in `Microsoft:TrustedTenantIds`. This protects against unverified-email account takeover.
- One account per provider per user. Linking a second Google or Microsoft account is refused until the first one is disconnected.
- Storage: `dbo.ExternalLogins` keeps the UserId, the provider, a SHA-256 hash of provider|subject (never the raw subject), the email at link time, and linked/last-used times. No provider tokens or passwords are stored. On first link it sets `UserAcc.EmailVerified=1` if that column exists.
- My Profile > Login & Security: a new "Google & Microsoft Sign-in" card lists your connected accounts and has Disconnect buttons (your own login only). Connect, disconnect and sign-in events are written to the Login Activity History.
- Is_First_Time_Login accounts still have to set a password from the invite email before any sign-in, including Google or Microsoft.

## Files
App_Code/AnyminuteExternalLogin.cs (new), ExternalLogin.aspx(.cs) (new), Login.aspx, Login.aspx.cs, LoginSecurity.aspx, LoginSecurity.aspx.cs, Web.config (empty placeholder keys only; Web.config is NOT in the zip), Main.master / Modern.master (meta v41-20261031), DB_OPTIONAL/OPTIONAL_05_ExternalLogins.sql.

## Deploy
1. Overlay `site/`. Keep your server Web.config.
2. Run `DB_OPTIONAL/OPTIONAL_05_ExternalLogins.sql`. Also run v39 `OPTIONAL_04_ChangeLoginEmail.sql` if you have not yet.
3. Add the appSettings keys from WEB_CONFIG_v41_appSettings.txt to the server Web.config with real values.
4. The site must be on HTTPS. Redirect URI for both providers: `https://<your-host>/ExternalLogin.aspx` (for example https://anyminute.net/ExternalLogin.aspx). If you leave `*:RedirectUri` empty, it is built from the request host. Set it explicitly behind a proxy or load balancer.

### Google Cloud (OAuth client)
console.cloud.google.com > APIs & Services > OAuth consent screen: External, app name AnyMinute, scopes openid / email / profile, then publish.
Credentials > Create credentials > OAuth client ID > Web application. Authorized redirect URI: `https://<host>/ExternalLogin.aspx`. Copy the Client ID and Client secret into `Google:ClientId` and `Google:ClientSecret`.

### Microsoft Entra ID (Azure AD app registration)
portal.azure.com > Microsoft Entra ID > App registrations > New registration.
Supported account types: "Accounts in any organizational directory and personal Microsoft accounts" (pairs with `Microsoft:Tenant=common`). Use a tenant ID instead to allow only your organisation.
Redirect URI: platform **Web**, `https://<host>/ExternalLogin.aspx`.
Certificates & secrets > New client secret: put the value in `Microsoft:ClientSecret` and the Application (client) ID in `Microsoft:ClientId`.
Token configuration > Add optional claim > ID token: `email` and `xms_edov`. API permissions: Microsoft Graph delegated openid, email, profile.
Optional: put your organisation's tenant ID(s) in `Microsoft:TrustedTenantIds` (comma separated) so work accounts in those tenants match even without xms_edov.
Client secrets expire. Set a renewal reminder.

### appSettings keys
Google:ClientId, Google:ClientSecret, Google:RedirectUri, Microsoft:ClientId, Microsoft:ClientSecret, Microsoft:Tenant (default common), Microsoft:RedirectUri, Microsoft:TrustedTenantIds.

## Not regressed
The v39 Change Login Email flow, Support Account Recovery, the accountant sidebar without Payroll Hours and no accountant time card, and the v40 Reports changes are not touched.

## Deferred
- A "Connect Google/Microsoft" button inside Login & Security, for linking a provider whose email differs from the login email. Today linking happens only by matching verified email.
- Social self-signup (account creation).
- Verifying the id_token signature with JWKS. It is not required because the token comes directly from the token endpoint over TLS.
- Session ID regeneration at sign-in, which password sign-in also does not do.
- No live smoke test was run.
