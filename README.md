# ENTITY-CSHARP-CLEANROOM

[![Clean-room verification](https://github.com/blackmore-technology-group/ENTITY-CSHARP-CLEANROOM/actions/workflows/cleanroom-verify.yml/badge.svg)](https://github.com/blackmore-technology-group/ENTITY-CSHARP-CLEANROOM/actions/workflows/cleanroom-verify.yml)
[![License](https://img.shields.io/github/license/blackmore-technology-group/ENTITY-CSHARP-CLEANROOM)](LICENSE)

**BTG-controlled C# / .NET conformance baseline for ENTITY v3.3.**

> This repository is maintained and controlled by Blackmore Technology Group. It is cross-language reproducibility evidence. It is **not** an unrelated third-party implementation and must not be cited as independent external validation.

[ENTITY](https://github.com/blackmore-technology-group/ENTITY) | [v3.3.0 release](https://github.com/blackmore-technology-group/ENTITY/releases/tag/v3.3.0) | [Engineering evidence](https://github.com/blackmore-technology-group/ENTITY/blob/main/docs/ENGINEERING_EVIDENCE.md) | [Independent interoperability challenge](https://github.com/blackmore-technology-group/ENTITY/blob/main/docs/INTEROPERABILITY_CHALLENGE.md)

## What this repository verifies

This implementation exercises the published ENTITY conformance campaigns in C# / .NET, including the v3.2 adoption campaign and the v3.3 Verifiable Reality campaign. CI verifies the sealed kit checksum before executing the language-native classifier.

For v3.3 the required public result is:

- release: 3.3.0
- protected ENTITY release commit: 9c79f987207592cb6791e1a8956f23351cdfb2d3
- vectors: **20/20** (10 valid / 10 invalid)
- sealed-kit SHA-256: 8b39ee01fb7346f33a57530e925b545d2bf9a770c7ec60724e28a4971d55a46
- deterministic result SHA-256: 82bd1f1fb328edd37a26d8ea60ede5a599c7d9af5027bffd73b9e52843b5a51d

## Run the v3.3 campaign

`	ext
dotnet run --project reality-v33/RealityV33.csproj -c Release
`

The primary v3.3 implementation is $(System.Collections.Hashtable.layout).

## Other controlled campaigns

`	ext
.\\bin\\Release\\net48\\EntityCSharpCleanroom.exe test`ndotnet run --project adoption-v32/AdoptionV32.csproj -c Release
`

See .github/workflows/cleanroom-verify.yml for the complete CI procedure, exact toolchain setup, checksum verification, evidence capture and artifact retention.

## Verification boundary

A passing result shows that this BTG-controlled C# / .NET implementation classifies the sealed public vectors consistently with the published campaign. It does **not** establish that a real-world claim is objectively true, and it does **not** establish independent external interoperability merely because another BTG-controlled language converges on the same result.

The stronger external question remains open:

> Can an unrelated engineer or organization reproduce the same semantics from public specification/test material without using BTG implementation code?

A deliberately narrow starting point is [ENTITY issue #27](https://github.com/blackmore-technology-group/ENTITY/issues/27).

## Contributing

Reproducible build failures, portability fixes, language-idiomatic improvements, test corrections and specification ambiguities are welcome. If your goal is to produce **independent** conformance evidence, use a repository controlled outside BTG and follow the independence rules in the [interoperability challenge](https://github.com/blackmore-technology-group/ENTITY/blob/main/docs/INTEROPERABILITY_CHALLENGE.md).

## License

Apache License 2.0. See [LICENSE](LICENSE).