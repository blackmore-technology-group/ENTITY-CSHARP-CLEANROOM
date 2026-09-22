# ENTITY Data Market Interoperability Profile v1
Status: Protocol 2 development interoperability profile; not part of frozen ENTITY Protocol 1.0.
Schema family: `entity-data-market-*`

## Purpose
This profile lets an unrelated client or market implementation submit signed data-right orders and independently verify an ENTITY market evidence package without receiving the underlying raw data.

## Object separation
Implementations SHALL preserve:
`Asset -> DATA_COMMODITY -> Rights Instrument -> Order -> Trade -> Settlement -> Rights Position/Consumption`.
None of these transitions by itself establishes legal ownership or regulatory classification.

## Signed order envelope
A remote order SHALL use `entity-data-market-order-v1` and contain:
- `entity_id`, `market_id`, `instrument_id`;
- `side`, `quantity`, `limit_price_minor`, `currency`;
- `time_in_force`, optional `purpose`;
- replay-resistant `order_nonce`;
- `created_at_ms`, `expires_at_ms`;
- ENTITY signature record over the complete unsigned body.

The receiving market SHALL verify the signer manifest, expiry, nonce, instrument policy and any buyer/collateral requirements before admission.
## External eligibility and collateral
When an instrument declares buyer requirements, the receiver SHALL require cryptographically verifiable external evidence from a configured trust registry.
Eligibility evidence SHALL bind the buyer and applicable jurisdiction/purpose.
Collateral/funds-reservation evidence SHALL bind the buyer, currency, maximum order notional and expiry.
A valid signature is evidence attribution, not a legal determination by ENTITY.

## Matching and settlement
Canonical matching is price-time ordered and excludes self-matching.
Matched trades remain `PENDING_SETTLEMENT`; rights SHALL NOT transfer at match time.
Settlement evidence SHALL bind the exact `trade_id`, buyer, seller, amount, currency and final settlement reference.
Failed settlement SHALL release the seller's reserved rights.

## Market evidence package
`entity-data-market-package-v1` SHALL contain:
- the market row and final sequence/head hash;
- commodities, rights instruments and positions relevant to the market;
- issuances, orders, trades and consumptions;
- signed/hash-chained market events;
- order authority evidence and remote signed order envelopes;
- royalty allocations and external settlement evidence;
- all Entity public manifests needed to verify included signatures;
- public external trust records needed to verify included authority attestations;
- `package_sha256` and market-operator ENTITY signature.

Raw underlying data SHALL NOT be required in a conforming market evidence package.
## Royalty attribution
If an instrument references a royalty plan, a settled trade SHALL generate a deterministic allocation record whose distribution sums exactly to the allocated trade amount.
Royalty allocation is an obligation/accounting result unless separately paid; it SHALL NOT be represented as completed payment without settlement evidence.

## Recovery and verification
Encrypted full-state backup/restore SHALL preserve exchange SQLite state and market-chain continuity.
Entity-scoped portable export SHALL include exchange rows reachable from the target Entity/object closure.
Independent market verification SHALL be possible from the package plus its included public manifests/trust records without reading the live exchange database.
Tampering with package content, market events, signatures, supply quantities or external authority evidence SHALL fail verification.

## External venue adapter boundary
ENTITY MAY submit an order to an externally authorized venue only through a configured provider adapter and valid venue-authorization evidence.
Provider submission/cancellation receipts are evidence of provider interaction; they are not settlement, custody, broker/dealer authority or proof of legal execution by ENTITY.
The adapter SHALL fail closed when authorization or provider integration is absent.

## Conformance boundary
Internal qualification MAY prove cross-state signed-order interoperability between independently instantiated runtimes.
A claim of independent external interoperability requires an unrelated implementation operated outside the BTG reference implementation and remains an external evidence gate.
This profile SHALL NOT alter ENTITY Protocol 1.0 or imply securities-exchange authorization.
