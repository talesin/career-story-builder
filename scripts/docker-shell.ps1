<#
.SYNOPSIS
    Open interactive bash shell in Docker container.
.DESCRIPTION
    Drops into a bash shell inside the app container for debugging
    and manual exploration.
.EXAMPLE
    ./shell.ps1
    Opens interactive shell in container.
#>
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
Set-Location (Split-Path $PSScriptRoot -Parent)

# Detect if Docker daemon is remote (tcp:// or ssh://)
function Test-DockerRemote {
    $context = docker context inspect --format '{{.Endpoints.docker.Host}}' 2>$null
    return $context -match '^(tcp|ssh)://'
}

if (Test-DockerRemote) {
    docker compose -f docker-compose.yml -f docker-compose.remote.yml run --rm --no-deps -it --entrypoint "/usr/bin/env bash" app
} else {
    docker compose run --rm --no-deps -it --entrypoint "/usr/bin/env bash" app
}
