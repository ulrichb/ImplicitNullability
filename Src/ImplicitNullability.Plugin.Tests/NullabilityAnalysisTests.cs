using System;
using System.Linq;
using ImplicitNullability.Plugin.Tests.Infrastructure;
using JetBrains.Util;
using NUnit.Framework;
#if !RESHARPER8
using JetBrains.ProjectModel;

#endif

namespace ImplicitNullability.Plugin.Tests
{
    public class NullabilityAnalysisTests : SampleSolutionTestBase
    {
        [Test]
        public void Test()
        {
            UseSampleSolution(solution =>
            {
                EnableImplicitNullabilitySetting(solution.GetProjectByName("ImplicitNullability.Sample").NotNull());

                var projectFilesToAnalyze = solution.GetAllProjectFilesWithPathPrefix("NullabilityAnalysis\\");

                var issues = TestExpectedInspectionComments(solution, projectFilesToAnalyze, GetNullabilityAnalysisHighlightingTypes());

                // Fixation: minimum amount of warnings, selected files

                Assert.That(issues.Count, Is.GreaterThan(30));

                var filePaths = issues.Select(x => x.GetSourceFile().Name).ToList();
                Assert.That(filePaths, Has.Some.EndsWith("AspxSample.aspx"));
                Assert.That(filePaths, Has.Some.EndsWith("RazorSample.cshtml"));
                Assert.That(filePaths, Has.Some.EndsWith("MethodsSample.cs"));
                Assert.That(filePaths, Has.Some.EndsWith("MethodsSampleTests.cs"));
            });
        }
    }
}