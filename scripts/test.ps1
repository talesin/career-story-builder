<#
.SYNOPSIS
    Build and run tests locally.
.DESCRIPTION
    Runs dotnet build and test commands.
.PARAMETER Command
    The command to run: build, test, or all.
.EXAMPLE
    ./test.ps1
    Runs build and test (default).
.EXAMPLE
    ./test.ps1 test
    Runs only dotnet test.
#>
[CmdletBinding()]
param(
    [ValidateSet('build', 'test', 'all')]
    [string]$Command = 'all'
)

$ErrorActionPreference = 'Stop'
Set-Location (Split-Path $PSScriptRoot -Parent)

switch ($Command) {
    'build' {
        dotnet build
    }
    'test' {
        dotnet test
    }
    'all' {
        dotnet build
        dotnet test
    }
}
