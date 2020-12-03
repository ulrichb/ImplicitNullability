[CmdletBinding()]
Param(
  [Parameter()] [string] $NugetExecutable = "Shared\.nuget\nuget.exe",
  [Parameter()] [string] $Configuration = "Debug",
  [Parameter()] [string] $Version = "0.0.0.1",
  [Parameter()] [string] $BranchName,
  [Parameter()] [string] $CoverageBadgeUploadToken,
  [Parameter()] [string] $NugetPushKey
)

Set-StrictMode -Version 2.0; $ErrorActionPreference = "Stop"; $ConfirmPreference = "None"
trap { $error[0] | Format-List -Force; $host.SetShouldExit(1) }

. Shared\Build\BuildFunctions

$BuildOutputPath = "Build\Output"
$SolutionFilePath = "ImplicitNullability.sln"
$MSBuildPath = (Get-ChildItem "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\*\MSBuild\Current\Bin\MSBuild.exe").FullName
$MSBuildAdditionalArgs = "/nowarn:MSB3277"
$NUnitAdditionalArgs = "--x86 --labels=All --agents=1"
$NUnitTestAssemblyPaths = @(
    "Src\ImplicitNullability.Plugin.Tests\bin\RD20202\$Configuration\ImplicitNullability.Plugin.Tests.RD20202.dll"
    "Src\ImplicitNullability.Plugin.Tests\bin\RS20202\$Configuration\ImplicitNullability.Plugin.Tests.RS20202.dll"
    "Src\ImplicitNullability.Samples.Consumer\bin\OfInternalCodeWithIN\$Configuration\ImplicitNullability.Samples.Consumer.OfInternalCodeWithIN.dll"
)
$NUnitFrameworkVersion = "net-4.5"
$TestCoverageFilter = "+[ImplicitNullability*]* -[ImplicitNullability*]ReSharperExtensionsShared.* -[ImplicitNullability.Samples.CodeWithIN.*]* -[ImplicitNullability.Samples.CodeWithoutIN.External]*"
$NuspecPath = "Src\ImplicitNullability.Plugin\ImplicitNullability.nuspec"
$NugetPackProperties = @(
    "Version=$(CalcNuGetPackageVersion 20202);Configuration=$Configuration;DependencyVer=[202.0];BinDirInclude=bin\RS20202"
)
$RiderPluginProject = "Src\RiderPlugin"
$NugetPushServer = "https://www.myget.org/F/ulrichb/api/v2/package"

Clean
PackageRestore
Build
NugetPack
BuildRiderPlugin
Test

if ($NugetPushKey) {
    NugetPush
}
