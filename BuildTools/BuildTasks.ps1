#Requires -Version 2

# This scripts uses the Psake build system to build the Bible software.
# For more information see https://github.com/JamesKovacs/psake/wiki

Properties { 
	$ViewRoot = Resolve-Path "$(Split-Path $psake.build_script_file)\.."
	$BinDir = "$ViewRoot\Bin"
	$BuildToolsDir =  "$ViewRoot\BuildTools"
	$ExternalAssetsDir = "$ViewRoot\ExternalAssets"
	$Configuration = "Release"
	$Slns = "$ViewRoot\Sln\Bible.sln",
		"$ViewROot\Sln\Bible.WP7.sln"
}

Task default -depends UnitTests

Task UnitTests -depends Build { 
	if(!(Test-Path $ViewRoot\Artifacts\BuildLogs))
	{
		New-Item -ItemType Directory $ViewRoot\Artifacts\BuildLogs
	}
	Exec { 
		..\ExternalAssets\Nunit\nunit-console.exe `
			$ViewRoot\Bible.nunit `
			/xml:$ViewRoot\Artifacts\BuildLogs\TestResult.xml 
	}
}

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
	foreach($sln in $Slns)
	{
		Write-Host "msbuild $sln /t:$($Targets -join ';') /maxcpucount /p:Configuration=$Configuration"
		msbuild $sln /t:$($Targets -join ";") /maxcpucount /p:Configuration=$Configuration
	}
}