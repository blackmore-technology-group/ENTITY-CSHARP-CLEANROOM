# ENTITY-CSHARP-CLEANROOM

[![Clean-room verification](https://github.com/blackmore-technology-group/ENTITY-CSHARP-CLEANROOM/actions/workflows/cleanroom-verify.yml/badge.svg)](https://github.com/blackmore-technology-group/ENTITY-CSHARP-CLEANROOM/actions/workflows/cleanroom-verify.yml)
[![License](https://img.shields.io/github/license/blackmore-technology-group/ENTITY-CSHARP-CLEANROOM)](LICENSE)

**BTG-controlled C# / .NET conformance baseline for ENTITY v3.4.2.**

> This repository is maintained and controlled by Blackmore Technology Group. It is cross-language reproducibility evidence. It is **not** an unrelated third-party implementation and must not be cited as independent external validation.

[ENTITY](https://github.com/blackmore-technology-group/ENTITY) · [v3.4.2 release](https://github.com/blackmore-technology-group/ENTITY/releases/tag/v3.4.2) · [Documentation portal](https://blackmore-technology-group.github.io/ENTITY-DOCS/) · [External verification challenge](https://github.com/blackmore-technology-group/ENTITY/issues/55)

## What this repository verifies

This C# / .NET implementation exercises published ENTITY conformance campaigns, including the frozen/earlier clean-room baseline, v3.2 adoption, v3.3 Verifiable Reality and the **v3.4.2 Global Passport campaign**.

CI verifies sealed-kit checksums before executing the language-native classifiers and retains verification evidence as workflow artifacts.

## ENTITY v3.4.2 Global Passport campaign

The current .NET CI target requires:

- sealed vectors: **26/26 PASS**;
- sealed-kit SHA-256: `ced70113f1d153627eb972b11adbf20e502ed086e0b13e8abf1dc5adc4c2e716`;
- canonical campaign result SHA-256: `45af773554a7191c1b49a75c636a1106afb1de36d788bb00d7af56097b8d1b0e`;
- `overall_valid: true`.

Run the current .NET campaign:

```powershell
dotnet run --project passport-v34/PassportV34.csproj -c Release
```

Historical sealed material from earlier campaigns remains in the repository for provenance and reproducibility and should not be confused with the current v3.4.2 campaign target.

## Other controlled campaigns

```powershell
.\bin\Release\net48\EntityCSharpCleanroom.exe test
dotnet run --project adoption-v32/AdoptionV32.csproj -c Release
dotnet run --project reality-v33/RealityV33.csproj -c Release
```

See [`.github/workflows/cleanroom-verify.yml`](.github/workflows/cleanroom-verify.yml) for the current CI procedure and evidence capture.

## Where this fits in ENTITY

ENTITY v3.4.2 adds the Blackmore Technology Data Universe (BTDU) while preserving the Global Passport/profile architecture and the core primitives:

`ENTITY → AUTHORITY → RIGHT → EVENT → VALUE`

If you are evaluating ENTITY rather than this .NET baseline specifically, start at the [ENTITY repository](https://github.com/blackmore-technology-group/ENTITY), the [documentation portal](https://blackmore-technology-group.github.io/ENTITY-DOCS/), or the [external verification challenge](https://github.com/blackmore-technology-group/ENTITY/issues/55).

## Verification boundary

A passing result shows that this **BTG-controlled C# / .NET implementation** classifies its current sealed campaign consistently with the published v3.4.2 target. It does **not** establish unrelated third-party validation, objective external truth, regulatory compliance, legal title, accounting fair value or independent live interoperability merely because another BTG-controlled language converges on the same result.

## Contributing

Useful contributions include reproducible build failures, portability fixes, language-idiomatic improvements, test corrections, specification ambiguities and independently authored counterexamples. Independent conformance evidence should be produced in a repository controlled outside BTG.

## License

Apache License 2.0. See [LICENSE](LICENSE).
