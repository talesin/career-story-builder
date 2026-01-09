<#
.SYNOPSIS
    Build Docker images for development or production.
.DESCRIPTION
    Builds Docker images using the appropriate compose file.
.PARAMETER Target
    The build target: dev or prod.
.EXAMPLE
    ./build.ps1
    Builds development image (default).
.EXAMPLE
    ./build.ps1 prod
    Builds production image.
#>
[CmdletBinding()]
param(
    [ValidateSet('dev', 'prod')]
    [string]$Target = 'dev'
)

$ErrorActionPreference = 'Stop'
Set-Location (Split-Path $PSScriptRoot -Parent)

# Detect if Docker daemon is remote (tcp:// or ssh://)
function Test-DockerRemote {
    $context = docker context inspect --format '{{.Endpoints.docker.Host}}' 2>$null
    return $context -match '^(tcp|ssh)://'
}

$isRemote = Test-DockerRemote
if ($isRemote) {
    Write-Host 'Remote Docker detected - volume mounts disabled (no hot reload)' -ForegroundColor Yellow
}

switch ($Target) {
    'dev' {
        if ($isRemote) {
            docker compose -f docker-compose.yml -f docker-compose.remote.yml build
        } else {
            docker compose build
        }
    }
    'prod' {
        docker compose -f docker-compose.prod.yml build
    }
}
