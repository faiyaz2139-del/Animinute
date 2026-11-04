# AnyMinute — NOTES v59 (SHIPPED) (t188u "erase the data from all modules")
- Status: **SHIPPED v59** (Faiyaz "Fix all with erased data" t189u, Tue Nov 3, 2026 ~8:05 PM ET): code `a499b1d`, meta v59-20261103, ?v=20261103v59. Supersedes HELD notes notes/held_20261103_v59/NOTES_held.md. See FIX_REPORT_v59.md.
- Support Admin › Subscriber Details › **Erase All Data** (button + ⋮ menu) → confirm modal (counts, reason, type ERASE/name, ack) → one-transaction hard delete of all module data under that subscriber, audited — DONE
- Defaults applied: Support Admin only; per subscriber (no global wipe); keep subscriber login/profile shell + PlanHistory/billing + audit; shared employees unlinked only; shared-owner businesses skipped; no drops/truncates; no Stripe.
- Not done: live smoke, DB run, git push.
