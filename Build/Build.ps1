[CmdletBinding()]
Param(
  [Parameter()] [string] $NugetExecutable = "Shared\.nuget\nuget.exe",
  [Parameter()] [string] $Configuration = "Debug",
  [Parameter()] [string] $Version = "0.0.1.0",
  [Parameter()] [string] $NugetPushKey
)

Set-StrictMode -Version 2.0; $ErrorActionPreference = "Stop"; $ConfirmPreference = "None"

. Shared\Build\BuildFunctions

$BuildOutputPath = "Build\Output"
$SolutionFilePath = "ImplicitNullability.sln"
$AssemblyVersionFilePath = "Src\ImplicitNullability.Plugin\Properties\AssemblyInfo.cs"
$MSBuildPath = "${env:ProgramFiles(x86)}\MSBuild\12.0\Bin\MSBuild.exe"
$NUnitExecutable = "nunit-console-x86.exe"
$NUnitTestAssemblyPaths = @(
    "Src\ImplicitNullability.Plugin.Tests\bin.R82\$Configuration\ImplicitNullability.Plugin.Tests.R82.dll",
    "Src\ImplicitNullability.Plugin.Tests\bin.R91\$Configuration\ImplicitNullability.Plugin.Tests.R91.dll",
    "Src\ImplicitNullability.Plugin.Tests\bin.R92\$Configuration\ImplicitNullability.Plugin.Tests.R92.dll",
    "Src\ImplicitNullability.Sample.Tests\bin\$Configuration\ImplicitNullability.Sample.Tests.dll"
)
$NUnitFrameworkVersion = "net-4.5"
$TestCoverageFilter = "+[ImplicitNullability*]* -[ImplicitNullability*]ReSharperExtensionsShared.* -[ImplicitNullability.Sample]* -[ImplicitNullability.Sample.ExternalCode]*"
$NuspecPath = "Src\ImplicitNullability.nuspec"
$PackageBaseVersion = StripLastPartFromVersion $Version
$NugetPackProperties = @(
    "Version=$PackageBaseVersion.82;Configuration=$Configuration;DependencyId=ReSharper;DependencyVer=[8.2,8.3);BinDirInclude=bin.R82;TargetDir=ReSharper\v8.2\plugins",
    "Version=$PackageBaseVersion.91;Configuration=$Configuration;DependencyId=Wave;DependencyVer=[2.0];BinDirInclude=bin.R91;TargetDir=dotFiles",
    "Version=$PackageBaseVersion.92;Configuration=$Configuration;DependencyId=Wave;DependencyVer=[3.0];BinDirInclude=bin.R92;TargetDir=dotFiles"
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
