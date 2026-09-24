# ENTITY v3.1 Global Infrastructure Clean-room Kit

This sealed kit is the complete input for the BTG-controlled v3.1 global-profile clean-room campaign.

An implementation SHALL NOT import, translate, call, inspect or link the BTG Python reference implementation. It SHALL consume only the protocol/profile documents, JSON Schema, clean-room profile and sealed vectors in this directory.

Qualification requires:

1. Verify every file in `SHA256SUMS.txt`.
2. Evaluate all 8 valid and 8 invalid vectors using native code.
3. Reproduce the eight Data Economic Sovereignty doctrine invariants from `ENTITY_GLOBAL_CLEANROOM_PROFILE.json`.
4. Build the deterministic `entity-v3.1-global-cleanroom-result-v1` summary.
5. Produce the exact `expected_result_sha256` in the clean-room profile.
6. Reject any vector whose semantics violate the profile, even if it is syntactically parseable JSON.

This is BTG-controlled clean-room qualification. It is not unrelated third-party implementation evidence.
