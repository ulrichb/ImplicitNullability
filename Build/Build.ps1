[CmdletBinding()]
Param(
  [Parameter()] [string] $NugetExecutable = "Shared\.nuget\nuget.exe",
  [Parameter()] [string] $Configuration = "Debug",
  [Parameter()] [string] $Version = "0.0.0.1-local",
  [Parameter()] [string] $BranchName,
  [Parameter()] [string] $CoverageBadgeUploadToken,
  [Parameter()] [string] $NugetPushKey
)

Set-StrictMode -Version 2.0; $ErrorActionPreference = "Stop"; $ConfirmPreference = "None"
trap { $error[0] | Format-List -Force; $host.SetShouldExit(1) }

. Shared\Build\BuildFunctions

$BuildOutputPath = "Build\Output"
$SolutionFilePath = "ImplicitNullability.sln"
$AssemblyVersionFilePath = "Src\SharedAssemblyInfo.cs"
$MSBuildPath = (Get-ChildItem "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2017\*\MSBuild\15.0\Bin\MSBuild.exe").FullName
$NUnitAdditionalArgs = "--x86 --labels=All --agents=1"
$NUnitTestAssemblyPaths = @(
    "Src\ImplicitNullability.Plugin.Tests\bin\R20163\$Configuration\ImplicitNullability.Plugin.Tests.R20163.dll"
    "Src\ImplicitNullability.Plugin.Tests\bin\R20171\$Configuration\ImplicitNullability.Plugin.Tests.R20171.dll"
    "Src\ImplicitNullability.Plugin.Tests\bin\R20172\$Configuration\ImplicitNullability.Plugin.Tests.R20172.dll"
    "Src\ImplicitNullability.Samples.Consumer\bin\OfInternalCodeWithIN\$Configuration\ImplicitNullability.Samples.Consumer.OfInternalCodeWithIN.dll"
)
$NUnitFrameworkVersion = "net-4.5"
$TestCoverageFilter = "+[ImplicitNullability*]* -[ImplicitNullability*]ReSharperExtensionsShared.* -[ImplicitNullability.Samples.CodeWithIN.*]* -[ImplicitNullability.Samples.CodeWithoutIN.External]*"
$NuspecPath = "Src\ImplicitNullability.nuspec"
$NugetPackProperties = @(
    "Version=$(CalcNuGetPackageVersion 20163);Configuration=$Configuration;DependencyVer=[7.0];BinDirInclude=bin\R20163"
    "Version=$(CalcNuGetPackageVersion 20171);Configuration=$Configuration;DependencyVer=[8.0];BinDirInclude=bin\R20171"
    "Version=$(CalcNuGetPackageVersion 20172);Configuration=$Configuration;DependencyVer=[9.0];BinDirInclude=bin\R20172"
)
$NugetPushServer = "https://www.myget.org/F/ulrichb/api/v2/package"

Clean
PackageRestore
Build
NugetPack
Test

if ($NugetPushKey) {
    NugetPush
}
