# ENTITY Protocol 2 Multi-Language Clean-Room Profile v1

Status: BTG internal clean-room qualification profile derived only from published Protocol 2 normative material.

## Isolation rule
Each implementation SHALL consume only this sealed KIT. It SHALL NOT import, translate, inspect, call, or link BTG Python implementation source.

## Target
Implement an independent verifier for `entity-transaction-evidence-bundle-v1`, sovereign recovery metadata, and the Protocol 2 data-market boundary.

## Canonical JSON
UTF-8 JSON; object keys sorted lexicographically at every depth; arrays retain order; no insignificant whitespace; separators are `,` and `:`; campaign vectors use only integer, string, boolean, null, array, and object values.

`transaction_root_sha256 = SHA256(canonical_json(evidence))`.

The bundle signature payload is canonical JSON of exactly:
`{schema, transaction_id, issuer_entity_id, transaction_root_sha256}`.

Campaign signature algorithm: Ed25519. Public keys are SPKI DER encoded and Base64 wrapped in the signature record.
## Verification result
Each implementation SHALL emit deterministic JSON:
`{schema,transaction_id,transaction_root_sha256,root_valid,signature_valid,required_sections_valid,cross_links_valid,ledger_valid,overall_valid,error_codes}`.

`error_codes` SHALL be sorted lexicographically. The result hash is SHA-256 of canonical JSON of that result.

## Required semantic invariants
1. All required TEB evidence sections exist.
2. Provenance asset ID equals transaction asset ID.
3. Rights asset ID equals transaction asset ID and claimant equals licence grantor.
4. Licence asset/grantor/licensee match transaction parties and references the rights claim.
5. Usage references the licence+asset+licensee and purpose is authorized.
6. Licence settlement references the licence; payer=licensee; payee=grantor.
7. Realized value references verified external settlement and cannot exceed its amount.
8. Digital Commodity references same asset/usage/licence/settlement and contribution cannot exceed settlement.
9. Corporate authorization matches capital share class and issuance request.
10. Capital accounting debits equal credits by currency; outstanding shares reconcile to positions.
11. Share settlement references the same capital event.
12. Event ledger sequence is contiguous and each event hash is SHA256(prev_hash + ':' + canonical_json(payload)).
13. A valid signature is attribution only and SHALL NOT be interpreted as legal ownership.
14. Recovery verification SHALL work without live ENTITY state and regenerated rooted evidence SHALL preserve the transaction root.