<#
.SYNOPSIS
    Build the solution locally.
.DESCRIPTION
    Runs dotnet build on the solution.
.EXAMPLE
    ./build.ps1
#>
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
Set-Location (Split-Path $PSScriptRoot -Parent)

dotnet build
