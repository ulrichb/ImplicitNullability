using System;
using System.Collections.Generic;
using System.Linq;
using ImplicitNullability.Plugin.Tests.Infrastructure;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;

namespace ImplicitNullability.Plugin.Tests
{
  public class NullabilityAnalysisTestSolutionTests : HighlightingInPluginTestSolutionTestBase
  {
    protected override IEnumerable<Type> GetHighlightingTypesToAnalyze ()
    {
      return GetNullabilityAnalysisHighlightingTypes();
    }

    protected override List<IProjectFile> GetProjectFilesToAnalyze (ISolution solution)
    {
      return
          (from project in solution.GetAllProjects()
            from file in project.GetAllProjectFiles()
            where ProjectUtil.GetRelativePresentableProjectPath (file, project).StartsWith ("NullabilityAnalysis\\")
            select file).ToList();
    }

    protected override void ProcessResults (int totalIssuesCount, IEnumerable<IPsiSourceFile> filesWithIssues)
    {
      // Fixation: minimum amount of warnings, selected files

      Assert.That (totalIssuesCount, Is.GreaterThan (30));

      var filePaths = filesWithIssues.Select (x => x.Name).ToList();
      Assert.That (filePaths, Has.Some.EndsWith ("AspxSample.aspx"));
      Assert.That (filePaths, Has.Some.EndsWith ("RazorSample.cshtml"));
      Assert.That (filePaths, Has.Some.EndsWith ("MethodsSample.cs"));
      Assert.That (filePaths, Has.Some.EndsWith ("MethodsSampleTests.cs"));
    }
  }
}