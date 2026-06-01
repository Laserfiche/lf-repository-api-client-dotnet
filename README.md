# Laserfiche Repository API Client .NET
[![NuGet version (Laserfiche.Repository.Api.Client.V2)](https://img.shields.io/nuget/v/Laserfiche.Repository.Api.Client.V2.svg?style=flat-square)](https://www.nuget.org/packages/Laserfiche.Repository.Api.Client.V2)

Use the Laserfiche Repository API to access data in a Laserfiche repository. Import or export documents, modify the repository folder structure, read and modify templates and field values, and more.

## Documentation
- [Laserfiche Developer Center](https://developer.laserfiche.com/)
- [Documentation](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/v2/index.html) for the `Laserfiche.Repository.Api.Client.V2` NuGet package used to access the v2 Laserfiche Repository APIs.
- [Documentation](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/v1/index.html) for the `Laserfiche.Repository.Api.Client` NuGet package used to access the v1 Laserfiche Repository APIs.

## Changelog
See CHANGELOG [here](https://github.com/Laserfiche/lf-repository-api-client-dotnet/blob/v2/CHANGELOG.md).

## How to contribute
Useful commands for building and testing the app.

### Generate the repository client
See the [.github/workflows/generate-client.yml](https://github.com/Laserfiche/lf-repository-api-client-dotnet/blob/v2/.github/workflows/generate-client.yml).

### Build, test, and package
See the [.github/workflows/main.yml](https://github.com/Laserfiche/lf-repository-api-client-dotnet/blob/v2/.github/workflows/main.yml).

#### Branches
The v1 branch stores client code for the Laserfiche Repository API v1; the v2 branch stores client code for the Laserfiche Repository API v2.

### Working on a new server feature

When a new server endpoint requires a corresponding client method, the workflow below avoids the chicken-and-egg cycle between server deploy and preview NuGet publish. Background: [`site-api-repository/docs/design-server-client-preview-nuget-workflow.md`](https://github.com/Laserfiche/site-api-repository/blob/main/docs/design-server-client-preview-nuget-workflow.md).

#### Inner loop — regenerate against a local server

[`generate-client/regen-from-local.ps1`](generate-client/regen-from-local.ps1) (Windows / `pwsh`) and [`generate-client/regen-from-local.sh`](generate-client/regen-from-local.sh) (POSIX `bash`) refresh `generate-client/swagger.json` and `src/Clients/RepositoryClients.cs` from a running API server, with no NuGet round-trip:

```powershell
# Default: pulls from a locally-running site-api-repository on http://localhost:11211/
./generate-client/regen-from-local.ps1

# Or point at a deployed dev environment
./generate-client/regen-from-local.ps1 -SwaggerUrl 'https://api.a.clouddev.laserfiche.ca/repository/swagger/v2/swagger.json'
```

Commit both `swagger.json` and `RepositoryClients.cs` to the feature branch when ready.

Requires `nswag` (`npm install -g nswag@14.4.0`), `python` (3.x), and `pwsh` (PowerShell Core, on POSIX).

#### Per-branch preview NuGet

`main.yml` publishes a preview NuGet on every feature-branch push, not just `v2`. Versions:

| Branch | Preview version shape |
|---|---|
| `v2` | `${VERSION_PREFIX}-beta-${run_id}` (unchanged) |
| anything else | `${VERSION_PREFIX}-feature-${branch_slug}-${run_id}` |

The publish job consumes the **committed** `generate-client/swagger.json` — no live server is required at publish time. The production-publish job is gated on `v2` + manual environment approval (unchanged).

The publish jobs use the `run_attempt != 1` convention: an initial CI run validates; re-running the workflow then triggers the publish.

#### Cloud-not-yet-deployed integration tests

When a feature branch's tests exercise a new endpoint that the upstream cloud test environment hasn't deployed yet, mark the test class (or method) with `[SkipIfEndpointMissing("OperationId")]` from [`tests/integration/Util/`](tests/integration/Util/SkipIfEndpointMissingAttribute.cs):

```csharp
[TestClass]
[SkipIfEndpointMissing("WritePage")]
public class WritePageTest : BaseTest { … }
```

The base-test `[TestInitialize]` probes `{BaseUrl}/swagger/v2/swagger.json` once per process, caches the operationId set, and reports the test `Inconclusive` when any named operation is missing. The skip self-clears as soon as the deployed swagger contains the operation — no follow-up un-Ignore PR is needed.

Replaces the historical `[Ignore("Temporarily ignored: cloud test server not yet updated…")]` pattern.