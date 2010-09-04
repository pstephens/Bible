<#
.SYNOPSIS
Builds, cleans, or runs regression tests for the Bible project.

.DESCRIPTION
This script calls MSBuild. Any parameters not recognized by this script will be passed to MSBuild.

This script requires Powershell v2 and Microsoft.NET framework v4.0.

.PARAMETER Target
The operation to build. Valid options are:
  Clean     - removes build time temporary files.
  Build     - Builds the binaries from source code.
  Rebuild   - Cleans and then builds.
  UnitTests - Runs all unit tests.

Multiple targets can be specified using semicolons. The default value is "Clean;Build;UnitTests"

.PARAMETER Verbosity
Valid options are Quiet, Minimal, Normal, Detailed, Diagnostic.



#>

param
  (
    [parameter(Position = 0)]
    [Alias("t")]
    [string]
    $Target = "Clean;Build;UnitTests",
    
    [Alias("v")]
    [ValidateSet("Quiet", "Minimal", "Normal", "Detailed", "Diagnostic")]
    [string]
    $Verbosity = "Normal"
  )

$DotnetPath = (Get-ItemProperty HKLM:\SOFTWARE\Microsoft\.NETFramework).InstallRoot
$Dotnet4Path = (Join-Path $DotnetPath v4.0.30319)
$MsbuildPath = (Join-Path $Dotnet4Path Msbuild.exe)

$Cmd = $MsbuildPath + " "
$Cmd += (Join-Path ([System.IO.Path]::GetDirectoryName($MyInvocation.MyCommand.Path)) "Build/Bible.build") + " "
$Cmd += "/toolsversion:4.0 "
$Cmd += "/maxcpucount "
$Cmd += "/target:'" + $Target + "' "
$Cmd += "/verbosity:" + $Verbosity + " "

$Cmd += [string]::Join(" ", $Args)

Write-Verbose ($Cmd)

Invoke-Expression ($Cmd)