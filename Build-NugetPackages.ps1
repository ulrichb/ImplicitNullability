[CmdletBinding()]
Param(
  [Parameter(Mandatory)] [string] $NugetExecutable,
  [Parameter()] [string] $Configuration = "Debug",
  [Parameter()] [string] $Version = "0.0.1.0",
  [Parameter()] [string] $OutputPath = "NuGetOutput"
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = "Stop"
$ConfirmPreference = "None"
trap { $error[0] | Format-List -Force; $host.SetShouldExit(1) }

. Shared\Build\BuildFunctions

$NuspecPath = "Src\ImplicitNullability.nuspec"
$PackageBaseVersion = StripLastPartFromVersion $Version

New-Item $OutputPath -Type Directory -Force | Out-Null
Remove-Item $OutputPath\* -Recurse -Force

. $NugetExecutable pack $NuspecPath -Properties "Version=$PackageBaseVersion.82;Configuration=$Configuration;DependencyId=ReSharper;DependencyVer=[8.2,8.3);BinDirInclude=bin.R82;TargetDir=ReSharper\v8.2\plugins\" -OutputDirectory $OutputPath
if ($LastExitCode -ne 0) { throw "NuGet failed with exit code $LastExitCode." }
. $NugetExecutable pack $NuspecPath -Properties "Version=$PackageBaseVersion.91;Configuration=$Configuration;DependencyId=Wave;DependencyVer=[2.0];BinDirInclude=bin.R91;TargetDir=dotFiles\" -OutputDirectory $OutputPath
if ($LastExitCode -ne 0) { throw "NuGet failed with exit code $LastExitCode." }
