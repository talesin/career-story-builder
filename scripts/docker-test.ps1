<#
.SYNOPSIS
    Run build and tests in Docker container.
.DESCRIPTION
    Executes dotnet build and test commands inside the Docker container.
.PARAMETER Command
    The command to run: build, test, or all.
.EXAMPLE
    ./docker-test.ps1
    Runs build and test (default).
.EXAMPLE
    ./docker-test.ps1 build
    Runs only dotnet build.
#>
[CmdletBinding()]
param(
    [ValidateSet('build', 'test', 'all')]
    [string]$Command = 'all'
)

$ErrorActionPreference = 'Stop'
Set-Location (Split-Path $PSScriptRoot -Parent)

# Detect if Docker daemon is remote (tcp:// or ssh://)
function Test-DockerRemote {
    $context = docker context inspect --format '{{.Endpoints.docker.Host}}' 2>$null
    return $context -match '^(tcp|ssh)://'
}

$script:isRemote = Test-DockerRemote

function Invoke-InContainer {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [string]$Entrypoint,

        [Parameter(ValueFromRemainingArguments)]
        [string[]]$Arguments
    )

    $composeFiles = if ($script:isRemote) {
        @('-f', 'docker-compose.yml', '-f', 'docker-compose.remote.yml')
    } else {
        @()
    }

    $params = @(
        'run', '--rm', '--no-deps'
        '--entrypoint', $Entrypoint
        'app'
    )
    if ($Arguments) {
        $params += $Arguments
    }
    docker compose @composeFiles @params
}

switch ($Command) {
    'build' {
        Invoke-InContainer -Entrypoint 'dotnet' -Arguments 'build'
    }
    'test' {
        Invoke-InContainer -Entrypoint 'dotnet' -Arguments 'test'
    }
    'all' {
        Invoke-InContainer -Entrypoint 'dotnet' -Arguments 'build'
        Invoke-InContainer -Entrypoint 'dotnet' -Arguments 'test'
    }
}
