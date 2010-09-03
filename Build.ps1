param
  (
    [parameter(Position = 0, Mandatory = $false,
        HelpMessage="The build target.")]
    [Alias("t")]
    [string]
    [ValidateSet("Rebuild", "Build", "UnitTests", "Clean")]
    $Target = "Rebuild",
    
    [Alias("v")]
    [ValidateSet("Quiet", "Minimal", "Normal", "Detailed", "Diagnostic")]
    [string]
    $Verbosity = "Normal",
    
    [Alias("h")]
    [Switch]
    $Help
  )

if($Help)
{
    Help -detailed c:\src\bible2\build.ps1
    exit
}

$DotnetPath = (Get-ItemProperty HKLM:\SOFTWARE\Microsoft\.NETFramework).InstallRoot
$Dotnet4Path = (Join-Path $DotnetPath v4.0.30319)
$MsbuildPath = (Join-Path $Dotnet4Path Msbuild.exe)

$Cmd = $MsbuildPath + " "
$Cmd += "Bible2.sln "
$Cmd += "/toolsversion:4.0 "
$Cmd += "/maxcpucount "
$Cmd += "/target:" + $Target + " "
$Cmd += "/verbosity:" + $Verbosity + " "

$Cmd += [string]::Join(" ", $Args)

Write-Verbose ($Cmd)

Invoke-Expression ($Cmd)