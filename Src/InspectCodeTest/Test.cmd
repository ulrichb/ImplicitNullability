SET NugetExePath=..\..\Shared\.nuget\nuget.exe

%NugetExePath% install JetBrains.ReSharper.CommandLineTools -Pre -ExcludeVersion || EXIT /B 1
SET RsatPath=.\JetBrains.ReSharper.CommandLineTools\tools

XCOPY ..\..\Build\Output\*.0.0.1.20163-dev.nupkg %RsatPath%\ /Y || EXIT /B 1

%RsatPath%\InspectCode.exe --caches-home:_ReSharperInspectCodeCache --toolset=14.0 -o:Inspections.xml ..\..\ImplicitNullability.Sample.sln || EXIT /B 1
