<#
.SYNOPSIS
    Docker compose orchestration for development and production.
.DESCRIPTION
    Starts, stops, and manages Docker containers for both development
    and production environments.
.PARAMETER Command
    The command to run: dev, prod, stop, or logs.
.EXAMPLE
    ./docker-run.ps1
    Starts development environment (default).
.EXAMPLE
    ./docker-run.ps1 prod
    Starts production environment in detached mode.
.EXAMPLE
    ./docker-run.ps1 stop
    Stops all containers.
#>
[CmdletBinding()]
param(
    [ValidateSet('dev', 'prod', 'stop', 'logs')]
    [string]$Command = 'dev'
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

switch ($Command) {
    'dev' {
        try {
            if ($isRemote) {
                # Remote: start detached, then follow logs (handles remote output streaming better)
                docker compose -f docker-compose.yml -f docker-compose.remote.yml up -d --build
                if ($LASTEXITCODE -ne 0) {
                    throw "Failed to start containers (exit code: $LASTEXITCODE)"
                }
                docker compose -f docker-compose.yml -f docker-compose.remote.yml logs -f
            } else {
                docker compose up
            }
        } finally {
            Write-Host "`nStopping containers..."
            docker compose down 2>&1 | Out-Null
        }
    }
    'prod' {
        docker compose -f docker-compose.prod.yml up -d
        Write-Host 'Production started. Use "./docker-run.ps1 logs" to view logs.'
    }
    'stop' {
        docker compose down 2>&1 | Out-Null
        docker compose -f docker-compose.yml -f docker-compose.remote.yml down 2>&1 | Out-Null
        docker compose -f docker-compose.prod.yml down 2>&1 | Out-Null
    }
    'logs' {
        if ($isRemote) {
            docker compose -f docker-compose.yml -f docker-compose.remote.yml logs -f
        } else {
            docker compose logs -f
        }
    }
}
