[CmdletBinding()]
Param(
  [Parameter()] [string] $NugetExecutable = "Shared\.nuget\nuget.exe",
  [Parameter()] [string] $Configuration = "Debug",
  [Parameter()] [string] $Version = "0.0.1.0-dev",
  [Parameter()] [string] $BranchName,
  [Parameter()] [string] $CoverageBadgeUploadToken,
  [Parameter()] [string] $NugetPushKey
)

Set-StrictMode -Version 2.0; $ErrorActionPreference = "Stop"; $ConfirmPreference = "None"

. Shared\Build\BuildFunctions

$BuildOutputPath = "Build\Output"
$SolutionFilePath = "ImplicitNullability.sln"
$AssemblyVersionFilePath = "Src\ImplicitNullability.Plugin\Properties\AssemblyInfo.cs"
$MSBuildPath = "${env:ProgramFiles(x86)}\MSBuild\14.0\Bin\MSBuild.exe"
$NUnitAdditionalArgs = "--x86 --labels=All"
$NUnitTestAssemblyPaths = @(
    "Src\ImplicitNullability.Plugin.Tests\bin\R91\$Configuration\ImplicitNullability.Plugin.Tests.R91.dll"
    "Src\ImplicitNullability.Plugin.Tests\bin\R92\$Configuration\ImplicitNullability.Plugin.Tests.R92.dll"
    "Src\ImplicitNullability.Plugin.Tests\bin\R100\$Configuration\ImplicitNullability.Plugin.Tests.R100.dll"
    "Src\ImplicitNullability.Samples.Consumer\bin\OfInternalCodeWithIN\$Configuration\ImplicitNullability.Samples.Consumer.OfInternalCodeWithIN.dll"
)
$NUnitFrameworkVersion = "net-4.5"
$TestCoverageFilter = "+[ImplicitNullability*]* -[ImplicitNullability*]ReSharperExtensionsShared.* -[ImplicitNullability.Samples.CodeWithIN.*]* -[ImplicitNullability.Samples.CodeWithoutIN.External]*"
$NuspecPath = "Src\ImplicitNullability.nuspec"
$NugetPackProperties = @(
    "Version=$(CalcNuGetPackageVersion 91);Configuration=$Configuration;DependencyVer=[2.0];BinDirInclude=bin\R91"
    "Version=$(CalcNuGetPackageVersion 92);Configuration=$Configuration;DependencyVer=[3.0];BinDirInclude=bin\R92"
    "Version=$(CalcNuGetPackageVersion 100);Configuration=$Configuration;DependencyVer=[4.0];BinDirInclude=bin\R100"
)
$NugetPushServer = "https://www.myget.org/F/ulrichb/api/v2/package"

Clean
PackageRestore
Build
Test
NugetPack

if ($NugetPushKey) {
    NugetPush
}
