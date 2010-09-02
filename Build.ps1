$DotnetPath = (Get-ItemProperty HKLM:\SOFTWARE\Microsoft\.NETFramework).InstallRoot
$Dotnet4Path = (Join-Path $DotnetPath v4.0.30319)
$MsbuildPath = (Join-Path $Dotnet4Path Msbuild.exe)
Invoke-Expression $MsbuildPath
