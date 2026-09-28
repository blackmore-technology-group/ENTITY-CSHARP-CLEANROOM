# NuGet distribution boundary

`Blackmore.Entity.Passport.V34` packages the existing BTG-controlled v3.4.2 Global Passport verifier as a .NET global tool (`entity-passport-v34`). The packaging change exists for discoverability and reproducible execution; it does not make BTG-controlled evidence independent third-party validation.

The sealed kit is packaged with the tool and its SHA-256 remains verified at runtime. Registry packaging must not change expected classifications, hashes, or fail-closed semantics.
