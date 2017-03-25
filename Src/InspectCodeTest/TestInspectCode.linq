<Query Kind="Statements" />

var scriptDir = Path.GetDirectoryName(Util.CurrentQueryPath).Dump();
var solutionDir = Path.Combine(scriptDir, "..", "..");
var runInspectCodeScriptPath = Path.Combine(solutionDir, "Shared", "RunInspectCode.linq");
var testSolutionPath = Path.Combine(solutionDir, "ImplicitNullability.Sample.sln").Dump();

Util.Run(runInspectCodeScriptPath, QueryResultFormat.Html, scriptDir, testSolutionPath).Dump();