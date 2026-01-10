#!/usr/bin/env bash
set -e

cd "$(dirname "$0")/.."

export ASPNETCORE_ENVIRONMENT=Development
export ASPNETCORE_URLS="http://localhost:${APP_HTTP_PORT:-8001}"
dotnet watch run --project src/Server
