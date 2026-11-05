# Held notes — AnyMinute v64 — SHIPPED (v64-20261104, code 38ed15d)

**Source:** t197ua0 PPTX (6 slides) — `notes.pptx` + media/image1..6.png
**Context:** Subscriber login shamsuthakor@gmail.com after password reset; also Accountant redesign notes.
**Trigger:** wait for Fix / Fix all. Do not code yet.

## Slide 1 — My Profile wrong for subscriber
- After password reset, login as `shamsuthakor@gmail.com` in **subscriber** module.
- Clicking **profile** opens a business-list “My Profile” (search + table of businesses with View/Edit Profile).
- **Should not** be that screen. Profile should be the **subscriber info / account details** screen (personal + contact), not a business picker list.
- Shot: media/image1.png (Welcome! Shamsuddin M Thakor; table of 3 businesses).

## Slides 2–3 — Businesses actions → error
- On Businesses page, clicking the red-boxed actions fails with generic error page (“SOMETHING WENT WRONG” / Go to Homepage).
- Boxed items on media/image2.png:
  1. **Add Business** (quick actions)
  2. **Edit Business** (under business name)
  3. **User List** (under User Details)
- Error page: media/image3.png

## Slides 4–5 — Redesign Accountant screen
- Current **Assign Accountant** page is marked wrong (big red X) — media/image4.png.
- Target mock is **Accountant & Payroll Access** (media/image5.png):
  - Top: **Invite Accountant or Payroll Administrator** form (User Information | Access & Permissions | Business Access) + Clear form / Send Invitation.
  - Bottom: **Invited & Active Users** tabs (All / Pending / Active), filters, table with Resend/Cancel or Edit access/Remove access.
- Slide text: “We have to Redesign accountant Screen as follows” (repeated s4/s5).

## Slide 6 — Action column: View + ⋮ menu
- On the redesigned Accountant list (media/image6.png shows current Action icons: pencil, PDF, ⋮).
- Change Action button pattern:
  - **Below Action:** show **View**
  - **Under three-dot (⋮):** Edit, Delete, and the rest
- Align with prior Support/Reseller list pattern (View + ⋮).

## Scope notes for Fix (when said)
- Subscriber My Profile routing / page for owner/subscriber role.
- Fix broken Add Business / Edit Business / User List navigation (error page).
- Accountant & Payroll Access redesign per mock (invite form + invited/active users).
- Accountant list Action → View + ⋮ (Edit, Delete, rest).
- Bump build stamp when shipping.

## Open questions (defaults if Fix without answer)
- Subscriber “profile” = same MyProfile personal/contact form used by Support Admin editable profile, scoped to the logged-in subscriber (not business list).
- “rest” under ⋮ = whatever actions exist today besides View (Edit access, Remove, Resend, Cancel, PDF/docs if still needed) — View stays primary.
