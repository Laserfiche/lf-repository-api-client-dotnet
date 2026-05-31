---
name: regen-dotnet-client
description: This skill should be used when the user wants to "regenerate the dotnet V2 client", "regen lf-repository-api-client-dotnet", "update the C# swagger client", "refresh the V2 client library", "pull new V2 methods into the dotnet client", "publish a preview NuGet", or otherwise rebuild `src/Clients/RepositoryClients.cs` from a server swagger. Use it eagerly whenever a server-side V2 endpoint has just been added or modified in `site-api-repository`. Covers the NSwag regen mechanics, the mandatory `patch_optional_multipart.py` post-processing step, the branch-protection rules on `v1`/`v2`, and the two ways to consume the new surface (UseLocalClientLib for fast iteration, preview NuGet for shipping).
---

# Regenerate the dotnet V2 client

Procedural guide for refreshing `lf-repository-api-client-dotnet/src/Clients/RepositoryClients.cs` against a server swagger.

## When to run this

- A new V2 endpoint was added (or an existing one's signature changed) in `site-api-repository`, and the dotnet client needs to expose it.
- An integration test in `tests/integration/` fails with "method does not exist" or "parameter property missing" against a server that has already shipped the change.
- A preview NuGet needs to be published for `site-api-repository` integration tests to consume the new surface.

For testing alone (no NuGet publish), prefer the [server-side `UseLocalClientLib` path](#fast-path-no-publish-needed) — it bypasses this regen entirely for the iteration loop and only requires a regen when you're ready to publish.

## Branch protection — must use PRs (even for admins)

Both `v1` and `v2` have `enforce_admins: true`. **Admins cannot push directly** to either branch. Every change goes through a PR with:
- One approval
- Code-owner review
- Required status checks: `build-n-test` + `build-documentation` (both green and strict — i.e. up-to-date with target)
- Conversation resolution
- No force-push, no deletion

If `git push origin v2` is rejected: this is expected. Open a PR from a feature branch.

**One-time force-push surgery** (e.g. retroactive history fix) is only possible via the GitHub API:
```bash
gh api -X PUT /repos/Laserfiche/lf-repository-api-client-dotnet/branches/v2/protection --input <json with allow_force_pushes:true>
# push
gh api -X PUT /repos/Laserfiche/lf-repository-api-client-dotnet/branches/v2/protection --input <json with allow_force_pushes:false>
```
Document the rationale in a notes file before doing this. (Last instance: 2026-04-29 retro after 12 unreviewed direct-to-v2 commits.)

## The regen mechanics (current state on `origin/v2`)

All commands run from `lf-repository-api-client-dotnet/`.

### Canonical path — `regen-from-local.ps1` (one command)

Use the wrapper for almost every regen. It downloads swagger from a running server, runs NSwag, patches the client, and writes it to `src/Clients/` — Steps 1–3 below in one call, with a `$LASTEXITCODE` guard so a failed download can't silently regen from a stale `swagger.json`:

```powershell
# Default: pulls from a locally-running site-api-repository on http://localhost:11211/
.\generate-client\regen-from-local.ps1

# Or point at a deployed dev environment (e.g. when the change isn't on localhost)
.\generate-client\regen-from-local.ps1 -SwaggerUrl 'https://api.a.clouddev.laserfiche.ca/repository/swagger/v2/swagger.json'
```

`regen-from-local.sh` is the POSIX `bash` equivalent. After it succeeds, commit **both** `generate-client/swagger.json` and `src/Clients/RepositoryClients.cs` to the feature branch — the per-branch preview NuGet job publishes from the committed swagger (see "Ship path" below).

The manual two-step form below is what the wrapper runs under the hood — reach for it only when you need to drive a step in isolation.

### Step 1 — download the swagger

```powershell
py generate-client/download_swagger.py `
    --swagger-url "http://localhost:11211/repository/swagger/v2/swagger.json" `
    --output-filepath "generate-client/swagger.json" `
    --swagger-override-filepath "generate-client/swagger-override.json"
```

A `generate-client/swagger-override.json` exists but is effectively empty (`{"components":{"schemas":{}}}`). Unlike the JS client, dotnet NSwag handles the abstract `Entry` correctly without a discriminator injection. If a future case needs an override, add it there.

Use `py` or `C:\Python314\python.exe` — avoid `python3` (broken Windows Store alias).

### Step 2 — run NSwag + cleanup

```powershell
.\generate-client\generate-client.ps1 `
    -input_folder generate-client `
    -output_folder src/Clients
```

> **Trap — pass `src/Clients`, NOT `src`.** `generate-client.ps1` now writes the client to `$output_folder/RepositoryClients.cs` **verbatim** (`Move-Item … -Destination "$output_folder/RepositoryClients.cs"`). Passing `-output_folder src` silently produces `src/RepositoryClients.cs` and leaves the real `src/Clients/RepositoryClients.cs` stale — the regen "succeeds" against the wrong path. The CI workflow and `regen-from-local.ps1` both pass `src/Clients`; match them.

The script:
1. Invokes `nswag run generate-client/nswag.json` to produce `RepositoryClients.cs`.
2. Strips long System.* namespaces from the generated client for readable docs.
3. Fixes `<br/>` placement in XML-doc lines.
4. Moves the cleaned client to `$output_folder/RepositoryClients.cs`.
5. **Runs `patch_optional_multipart.py` as the final step — see Step 3.**

### Step 3 — `patch_optional_multipart.py` (mandatory, not optional)

This is invoked at the end of `generate-client.ps1`. It rewrites NSwag's hard-coded multipart `null` checks.

**Why it's needed:** NSwag's C# generator emits

```csharp
if (x == null)
    throw new ArgumentNullException("parameters.X");
else { /* append to MultipartFormDataContent */ }
```

for **every** multipart property — ignoring the swagger's `required` list. For parameters the server accepts as absent (e.g. `imageFiles`, optional request payloads on PATCH `/Document`), this turns a legitimate omission into a client-side crash. The script rewrites each such block whose operationId's multipart schema does NOT list the parameter as required into:

```csharp
if (x != null) { /* append to MultipartFormDataContent */ }
```

**Don't skip this step.** Symptoms if you do: integration tests that send legitimately-empty multipart fields fail with `ArgumentNullException` originating in the generated client.

### Step 4 — verify

```bash
dotnet build src/Laserfiche.Repository.Api.Client.csproj
dotnet test tests/unit/
```

Then start a local `site-api-repository` server and run integration tests with the new method against it (see "Consuming the new client" below).

## Consuming the new client

### Fast path — no publish needed

For server-repo developers iterating on a new endpoint, switch `site-api-repository`'s test projects to a local `ProjectReference` instead of the published NuGet. See the [`UseLocalClientLib` workflow in site-api-repository](../../../site-api-repository/SiteAPIRepositoryTests/README.md) — copy `Directory.Build.props.user.example` → `Directory.Build.props.user` in the server repo. No preview publish required; the test projects compile straight against the local source.

### Ship path — per-branch preview NuGet publish

Per-feature-branch preview publishing is now live on `v2` (work item [#659276](https://v-dev-tfs.laserfiche.com/DefaultCollection/Cloud/_workitems/edit/659276), merged via PR #200). It decouples the preview NuGet from a server deploy: `.github/workflows/main.yml` runs on **every branch push** (`branches: ['**']`) and publishes from the **committed** `generate-client/swagger.json` — no live server needed at publish time.

1. **Commit** the regenerated `swagger.json` + `RepositoryClients.cs` and push your feature branch.
2. **CI runs and validates** (`build-n-test` + `build-documentation`). On this first run, the publish jobs are **skipped by design**.
3. **Re-run the workflow to publish** — the publish jobs gate on `if: github.run_attempt != 1`. The initial run validates; a re-run (run attempt 2+) actually publishes. This is the **re-run-to-publish convention** — a fresh push/merge alone never publishes.
4. **Consume it.** Bump `<ClientLibVersion>` in `site-api-repository/Directory.Build.props` to the published preview version, then server-repo tests pull it from the feed.

Preview version shapes (`VERSION_PREFIX` is `2.1.0` today):

| Branch | Preview version | Tagged? |
|---|---|---|
| `v2` | `${VERSION_PREFIX}-beta-${run_id}` | yes |
| any feature branch | `${VERSION_PREFIX}-feature-${branch_slug}-${run_id}` | no (short-lived) |

Production publish (`publish-production-package`, version `${VERSION_PREFIX}`) stays gated on `v2` **and** `run_attempt != 1` **and** a manual `preview`/production environment approval.

> **Gotcha that bit us:** merging a regen PR to `v2` produces a `run_attempt == 1` push build, so the NuGet does **not** publish on merge. You must re-run that `v2` CI run (`gh run rerun <id>`) to get attempt 2 and trigger the preview + production publish. If a merged regen never showed up on the feed, this is why.

Full background: [`site-api-repository/docs/design-server-client-preview-nuget-workflow.md`](../../../site-api-repository/docs/design-server-client-preview-nuget-workflow.md).

## NSwag config — read this before changing tag conventions

`generate-client/nswag.json` is configured to produce a **single combined client** (unlike the JS client, which splits per `[OpenApiTag]`). Adding outlier tags on server actions therefore doesn't break the dotnet client — but **don't rely on this**: keep server tags consistent so JS doesn't break. See the [`add-v2-endpoint`](../../../site-api-repository/.claude/skills/add-v2-endpoint/SKILL.md) skill for the canonical rule.

## Related skills

- [`add-v2-endpoint`](../../../site-api-repository/.claude/skills/add-v2-endpoint/SKILL.md) (`site-api-repository`) — full pipeline for adding the server endpoint that this client wraps.
- [`regen-js-client`](../../../lf-api-js/.claude/skills/regen-js-client/SKILL.md) (`lf-api-js`) — companion JS client regen. Keep server-side tag discipline consistent between the two so neither client silently breaks.
