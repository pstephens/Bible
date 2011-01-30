#Requires -Version 2

# This scripts uses the Psake build system to build the Bible software.
# For more information see https://github.com/JamesKovacs/psake/wiki

Properties { 
	$ViewRoot = Resolve-Path "$(Split-Path $psake.build_script_file)\.."
	$BinDir = "$ViewRoot\Bin"
	$BuildToolsDir =  "$ViewRoot\BuildTools"
	$ExternalAssetsDir = "$ViewRoot\ExternalAssets"
	$Configuration = "Release"
}

Task default -depends UnitTests

Task UnitTests -depends Build { Write-Host 'Unit Tests...' }

Task Rebuild -depends Clean, Build

Task Build -depends CopyExternalAssets, Compile

Task Compile { 
	Exec { ExecMsbuild Build }
}

Task Clean -depends CleanExternalAssets {
	Exec { ExecMsBuild Clean }
}

. .\BuildExternalAssets.ps1

function ExecMsBuild([string[]] $Targets)
{
	msbuild "$ViewRoot\Bible2.sln" /t:$($Targets -join ";") /maxcpucount /p:Configuration=$Configuration
}