#Requires -Version 2

<#
.SYNOPSIS
Builds, cleans, or runs regression tests for the Bible project.

.DESCRIPTION
This script invokes the psake build system.

This script requires Powershell v2 and Microsoft.NET framework v4.0.

#>

param (
    [Parameter(Position=0)]
    [string[]]$TaskList = @(),
    [switch]$Docs = $false, 
    [string]$Configuration = "Release",
    [System.Collections.Hashtable]$Parameters = @{},
    [System.Collections.Hashtable]$Properties = @{}
)

$ProjectRoot = Split-Path $MyInvocation.MyCommand.Path
$BuildTasksScript = Join-Path $ProjectRoot BuildTools\BuildTasks.ps1

$Properties.Configuration = $Configuration


if((Get-Module psake) -ne $null)
{
    Remove-Module psake
}
Import-Module (Join-Path $ProjectRoot ExternalAssets\Psake\Psake.psm1)

try
{
    Invoke-Psake -buildFile $BuildTasksScript `
        -taskList $TaskList `
        -framework 4.0 `
        -docs:$Docs `
        -parameters $Parameters `
        -properties $Properties
}
finally
{
    Remove-Module psake
}