#!/usr/bin/env bash
#
# Inner-loop helper: regenerate the V2 client from a running API server (default: local).
# See regen-from-local.ps1 for the full description.
#
# Usage:
#   ./generate-client/regen-from-local.sh
#       Pulls swagger from a locally-running site-api-repository.
#   ./generate-client/regen-from-local.sh https://api.a.clouddev.laserfiche.ca/repository/swagger/v2/swagger.json
#       Pulls swagger from a deployed dev environment.
#
# Requires: python3, pwsh (PowerShell Core), nswag (npm install -g nswag@14.4.0).
#
# See: site-api-repository/docs/design-server-client-preview-nuget-workflow.md

set -euo pipefail

SWAGGER_URL="${1:-http://localhost:11211/repository/swagger/v2/swagger.json}"

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(dirname "$SCRIPT_DIR")"

echo ">>> Downloading swagger from $SWAGGER_URL"
python3 "$SCRIPT_DIR/download_swagger.py" \
    --swagger-url "$SWAGGER_URL" \
    --output-filepath "$SCRIPT_DIR/swagger.json" \
    --swagger-override-filepath "$SCRIPT_DIR/swagger-override.json"

echo ">>> Regenerating client"
pwsh "$SCRIPT_DIR/generate-client.ps1" \
    -input_folder "$SCRIPT_DIR" \
    -output_folder "$REPO_ROOT/src/Clients"

echo ">>> Done. Updated generate-client/swagger.json and src/Clients/RepositoryClients.cs."
echo ">>> Commit both as part of your feature branch when ready."
