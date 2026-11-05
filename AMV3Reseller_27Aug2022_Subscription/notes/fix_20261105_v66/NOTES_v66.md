# NOTES v66-20261105
Cumulative on v65 (64dc8be). Build stamp v66-20261105; cache-bust ?v=20261105v66. Code commit ca9ada2. Slides 1-4 and invite form / tabs / filters / non-Actions columns untouched.

## Actions column (reference_actions.jpg)
Replaced v65 **View + kebab-with-everything** (live_view_kebab.jpg) with:
1. **Pending (Status=Invited):** outline **Resend** (blue) + outline **Cancel invitation** (grey) inline.
2. **Active:** outline **Edit access** (blue) + red text **Remove access** inline.
3. **⋮ kebab** beside those buttons for both statuses. Menu items exactly: **Edit**, **Delete**, **Reset**.
4. Standalone **View** button removed.

## Wiring
- Resend → CommandName Resend / SendEmail(InviteId) (same as v65 Resend invitation).
- Cancel invitation / Remove access / Delete → CommandName Revoke with prior confirms.
- Edit access (inline) and Edit (menu) → AccountantInvite.aspx?Key=…
- **Reset** → AnyminuteAccessService.Reset: Pending = same as Resend (SendEmail/Provision). Active invitation = re-email current sign-in details via existing SendCredentials (User ID + password reminder + set-password link when ResetUrl is set). Non-destructive; no wipe. Legacy Active rows without InviteId get a clear error (use Edit).

## Other
- Table wrap remains overflow:visible so kebab is not clipped (v65 fix kept).
- Minimal CSS: .am-acx-ghost, .am-acx-link-danger, action button spacing; .am-acx-view unused.
Compile: App_Code OK (1 pre-existing warning); AccountantAccess + AccountantInvite page compile 0 errors.
Not live-tested. No CloudAgent. No GitHub push.
