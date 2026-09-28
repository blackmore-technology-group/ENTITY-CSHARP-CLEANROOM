# NuGet publication

This repository is prepared for nuget.org Trusted Publishing through `.github/workflows/publish-nuget.yml`.

## One-time nuget.org setup

1. Sign in to nuget.org with the account that will own `Blackmore.Entity.Passport.V34`.
2. Open **Trusted Publishing** and create a GitHub policy for:
   - repository owner: `blackmore-technology-group`
   - repository: `ENTITY-CSHARP-CLEANROOM`
   - workflow file: `publish-nuget.yml`
   - environment: leave empty unless the workflow is later changed to use one.
3. In GitHub repository variables, set `NUGET_USER` to the nuget.org **profile name**, not an email address.
4. Scope the policy to the intended package/prefix as tightly as nuget.org permits.

The workflow uses `NuGet/login@v1` to exchange GitHub OIDC identity for a one-hour temporary API key. No long-lived NuGet API key is stored in this repository.

## Release gate

A release tag must be exactly `v<PassportV34.csproj Version>` (currently the frozen campaign version `v3.4.2`). Before publishing, the workflow:

- verifies tag/package version equality;
- packs the .NET global tool;
- installs that exact local `.nupkg` into a temporary tool directory;
- executes `entity-passport-v34`;
- requires the expected canonical campaign hash and `overall_valid: true`;
- only then obtains the short-lived NuGet publishing credential and pushes the package.

## Evidence boundary

NuGet publication improves discovery/installability of the BTG-controlled Global Passport verifier. It does not constitute unrelated third-party validation, independent protocol implementation, or full ENTITY v3.4.3 qualification.
