<#
.SYNOPSIS
    Run the application locally with hot reload.
.DESCRIPTION
    Starts the server with dotnet watch for development.
.EXAMPLE
    ./run.ps1
#>
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
Set-Location (Split-Path $PSScriptRoot -Parent)

$env:ASPNETCORE_ENVIRONMENT = 'Development'
$port = if ($env:APP_HTTP_PORT) { $env:APP_HTTP_PORT } else { '8001' }
$env:ASPNETCORE_URLS = "http://localhost:$port"

dotnet watch run --project src/Server
