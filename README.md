# ENTITY-CSHARP-CLEANROOM

[![Clean-room verification](https://github.com/blackmore-technology-group/ENTITY-CSHARP-CLEANROOM/actions/workflows/cleanroom-verify.yml/badge.svg)](https://github.com/blackmore-technology-group/ENTITY-CSHARP-CLEANROOM/actions/workflows/cleanroom-verify.yml)
[![License](https://img.shields.io/github/license/blackmore-technology-group/ENTITY-CSHARP-CLEANROOM)](LICENSE)

**BTG-controlled C# / .NET conformance baseline for ENTITY v3.4.1.**

> This repository is maintained and controlled by Blackmore Technology Group. It is cross-language reproducibility evidence. It is **not** an unrelated third-party implementation and must not be cited as independent external validation.

[ENTITY](https://github.com/blackmore-technology-group/ENTITY) · [v3.4.1 release](https://github.com/blackmore-technology-group/ENTITY/releases/tag/v3.4.1) · [Developer portal](https://github.com/blackmore-technology-group/ENTITY/blob/main/DEVELOPERS.md) · [Engineering evidence](https://github.com/blackmore-technology-group/ENTITY/blob/main/docs/ENGINEERING_EVIDENCE.md) · [Independent interoperability challenge](https://github.com/blackmore-technology-group/ENTITY/blob/main/docs/INTEROPERABILITY_CHALLENGE.md)

## What this repository verifies

This C# / .NET implementation exercises published ENTITY conformance campaigns, including:

- the frozen Protocol 1.0 / earlier clean-room baseline;
- the v3.2 adoption campaign;
- the v3.3 Verifiable Reality campaign;
- the **v3.4 Global Passport campaign**.

CI verifies sealed-kit checksums before executing the language-native classifiers and retains verification evidence as workflow artifacts.

## ENTITY v3.4.1 Global Passport campaign

Published v3.4.1 target:

- release: **v3.4.0 — Global Passport & Continuous Provenance**;
- sealed vectors: **24/24 PASS** — 12 valid / 12 invalid;
- sealed-kit SHA-256: `f95c2b347da97742fed3f20611f0eec2fd3df48694fed9494fb07163c537cfb7`;
- canonical cross-language result SHA-256: `ac7504cce70576008cff069607619660a4b9bf0cad43b3f3de81078f1e80d9ba`.

Run the .NET v3.4 campaign:

```powershell
dotnet run --project passport-v34/PassportV34.csproj -c Release
```

The GitHub Actions workflow checks that the report contains 24 passing vectors, the canonical result hash above, and `overall_valid: true`.

## Other controlled campaigns

```powershell
.\bin\Release\net48\EntityCSharpCleanroom.exe test
dotnet run --project adoption-v32/AdoptionV32.csproj -c Release
dotnet run --project reality-v33/RealityV33.csproj -c Release
```

See [`.github/workflows/cleanroom-verify.yml`](.github/workflows/cleanroom-verify.yml) for the complete CI procedure, toolchain setup, checksum verification, evidence capture and artifact retention.

## Where this fits in ENTITY

ENTITY v3.4.1 adds one universal **Global Passport** with composable jurisdiction, industry, privacy, trust and technical profiles. The protocol is intended to preserve identity, authority, rights, evidence, provenance and portable economic state without making infrastructure possession equivalent to sovereign authority.

The main repository also publishes executable implementation packages for Healthcare, Finance, Manufacturing, AI, Robotics and Defence/Public-Unclassified.

If you are evaluating ENTITY rather than this .NET baseline specifically, start at the [ENTITY repository](https://github.com/blackmore-technology-group/ENTITY) or the [Developer Portal](https://github.com/blackmore-technology-group/ENTITY/blob/main/DEVELOPERS.md).

## Verification boundary

A passing result shows that this **BTG-controlled C# / .NET implementation** classifies the sealed public vectors consistently with the published campaign.

It does **not** establish unrelated third-party validation, objective truth of an external-world claim, regulatory compliance, legal title, accounting fair value, or independent live interoperability merely because another BTG-controlled language converges on the same result.

The stronger external question remains open:

> Can an unrelated engineer or organization reproduce ENTITY semantics from public specifications and sealed test material without using BTG implementation code?

That is the purpose of the [ENTITY interoperability challenge](https://github.com/blackmore-technology-group/ENTITY/blob/main/docs/INTEROPERABILITY_CHALLENGE.md).

## Contributing

Useful contributions include reproducible build failures, portability fixes, language-idiomatic improvements, test corrections, specification ambiguities and independently authored counterexamples.

If your goal is to produce **independent** conformance evidence, use a repository controlled outside BTG and follow the independence rules in the interoperability challenge.

## License

Apache License 2.0. See [LICENSE](LICENSE).
