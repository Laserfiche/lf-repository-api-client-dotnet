<#
.SYNOPSIS
Inner-loop helper: regenerate the V2 client from a running API server (default: local).

.DESCRIPTION
Pulls swagger.json from the given URL, runs nswag, and patches the generated
RepositoryClients.cs in place. Intended for the developer feature-loop:

    1. Run the API server locally with WIP changes.
    2. Run this script to refresh swagger.json + RepositoryClients.cs against that server.
    3. Build/test consumers (e.g. site-api-repository SharedTest with UseLocalClientLib=true)
       against the freshly regenerated client without going through NuGet.
    4. Commit both swagger.json and RepositoryClients.cs to the feature branch when ready —
       the per-branch preview NuGet workflow will publish from the committed swagger.

See: site-api-repository/docs/design-server-client-preview-nuget-workflow.md

.PARAMETER SwaggerUrl
Swagger document URL. Defaults to a locally-running site-api-repository
(http://localhost:11211/repository/swagger/v2/swagger.json).

.EXAMPLE
PS> ./generate-client/regen-from-local.ps1
Pulls swagger from localhost:11211 and regenerates the client.

.EXAMPLE
PS> ./generate-client/regen-from-local.ps1 -SwaggerUrl 'https://api.a.clouddev.laserfiche.ca/repository/swagger/v2/swagger.json'
Pulls swagger from a deployed dev environment.
#>
param(
    [string]$SwaggerUrl = "http://localhost:11211/repository/swagger/v2/swagger.json"
)
$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot  = Split-Path -Parent $scriptDir

Write-Host ">>> Downloading swagger from $SwaggerUrl"
# Pick the first available Python launcher: Windows ships `py`, Linux/macOS ship
# `python3`, some envs only have `python`. Same fallback as generate-client.ps1.
$python = @('py', 'python3', 'python') | Where-Object { Get-Command $_ -ErrorAction SilentlyContinue } | Select-Object -First 1
if (-not $python) { throw "Python not found in PATH (tried 'py', 'python3', 'python')." }
& $python "$scriptDir/download_swagger.py" `
    --swagger-url $SwaggerUrl `
    --output-filepath "$scriptDir/swagger.json" `
    --swagger-override-filepath "$scriptDir/swagger-override.json"

Write-Host ">>> Regenerating client"
& "$scriptDir/generate-client.ps1" `
    -input_folder $scriptDir `
    -output_folder "$repoRoot/src/Clients"

Write-Host ">>> Done. Updated generate-client/swagger.json and src/Clients/RepositoryClients.cs."
Write-Host ">>> Commit both as part of your feature branch when ready."
