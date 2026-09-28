# ENTITY-CSHARP-CLEANROOM

[![Clean-room verification](https://github.com/blackmore-technology-group/ENTITY-CSHARP-CLEANROOM/actions/workflows/cleanroom-verify.yml/badge.svg)](https://github.com/blackmore-technology-group/ENTITY-CSHARP-CLEANROOM/actions/workflows/cleanroom-verify.yml)
[![License](https://img.shields.io/github/license/blackmore-technology-group/ENTITY-CSHARP-CLEANROOM)](LICENSE)

**Document class:** BTG-controlled reproducibility baseline  
**Frozen campaign target:** ENTITY v3.4.2 Global Passport  
**Current supported ENTITY runtime:** v3.4.3  
**BTDU component in v3.4.3:** Blackmore Technology Data Universe (BTDU) 3.4.2 unchanged

> This repository is controlled by Blackmore Technology Group Limited (BTG). It is reproducibility evidence, **not** an unrelated third-party implementation or independent external validation.

[ENTITY](https://github.com/blackmore-technology-group/ENTITY) · [Current v3.4.3 release](https://github.com/blackmore-technology-group/ENTITY/releases/tag/v3.4.3) · [Documentation model](https://blackmore-technology-group.github.io/ENTITY-DOCS/reference/documentation-model.html) · [External verification challenge](https://github.com/blackmore-technology-group/ENTITY/issues/55)

## Frozen v3.4.2 campaign

This C#/.NET baseline retains the exact v3.4.2 Global Passport campaign:

- sealed vectors: **26/26 PASS**;
- sealed-kit SHA-256: `ced70113f1d153627eb972b11adbf20e502ed086e0b13e8abf1dc5adc4c2e716`;
- canonical campaign result SHA-256: `45af773554a7191c1b49a75c636a1106afb1de36d788bb00d7af56097b8d1b0e`;
- required `overall_valid: true`.

The version label identifies the frozen evidence target and does not state that v3.4.2 is the current runtime.

Run it:

```powershell
dotnet run --project passport-v34/PassportV34.csproj -c Release
```

## Other controlled campaigns

```powershell
.\bin\Release\net48\EntityCSharpCleanroom.exe test
dotnet run --project adoption-v32/AdoptionV32.csproj -c Release
dotnet run --project reality-v33/RealityV33.csproj -c Release
```

See [`.github/workflows/cleanroom-verify.yml`](.github/workflows/cleanroom-verify.yml) for CI and evidence capture.

## Where this fits now

ENTITY v3.4.3 is the current supported runtime. BTDU remains component 3.4.2 unchanged. Protocol 1.0 is a separate frozen external clean-room target; BTDU, ADAM and NIKI are not additional Protocol 1.0 requirements unless the sealed Protocol 1.0 material explicitly says so.

## Verification boundary

A PASS here shows that this **BTG-controlled C#/.NET baseline** matches the frozen v3.4.2 target. It does not by itself establish independent implementation, full external interoperability/recovery, objective external truth, legal title, regulatory compliance, accounting fair value, upstream ownership or automatic economic entitlement.

## Contributing

Reproducible failures, portability fixes, language-idiomatic improvements, test corrections, specification ambiguities and counterexamples are useful. Independent conformance evidence should live in a repository controlled outside BTG.

## License

Apache License 2.0. See [LICENSE](LICENSE).
