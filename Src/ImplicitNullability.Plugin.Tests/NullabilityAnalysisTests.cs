using System;
using System.Linq;
using ImplicitNullability.Plugin.Settings;
using ImplicitNullability.Plugin.Tests.Infrastructure;
using JetBrains.Application.Settings;
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
        public void TestInput()
        {
            Test("MIn", settingsStore => settingsStore.SetValue((ImplicitNullabilitySettings s) => s.EnableInputAndRefParameters, true));
        }

        [Test]
        public void TestOutput()
        {
            Test("MOut", settingsStore => settingsStore.SetValue((ImplicitNullabilitySettings s) => s.EnableOutParametersAndResult, true));
        }

        private void Test(string definedExpectedWarningSymbol, Action<IContextBoundSettingsStore> additionalChanges)
        {
            UseSampleSolution(solution =>
            {
                EnableImplicitNullabilitySetting(solution.GetProjectByName("ImplicitNullability.Sample").NotNull(), settingsStore =>
                {
                    settingsStore.SetValue((ImplicitNullabilitySettings s) => s.EnableInputAndRefParameters, false);
                    settingsStore.SetValue((ImplicitNullabilitySettings s) => s.EnableOutParametersAndResult, false);
                    additionalChanges(settingsStore);
                });

                var projectFilesToAnalyze = solution.GetAllProjectFilesWithPathPrefix("NullabilityAnalysis\\");

                var issues = TestExpectedInspectionComments(solution, projectFilesToAnalyze, GetNullabilityAnalysisHighlightingTypes(),
                    definedExpectedWarningSymbol);

                // Fixation: minimum amount of warnings, selected files

                Assert.That(issues.Count, Is.GreaterThanOrEqualTo(25));

                var filePaths = issues.Select(x => x.GetSourceFile().Name).ToList();
                Assert.That(filePaths, Has.Some.EqualTo("AspxSample.aspx"));
                Assert.That(filePaths, Has.Some.EqualTo("RazorSample.cshtml"));
                Assert.That(filePaths, Has.Some.EqualTo("MethodsSample.cs"));
                Assert.That(filePaths, Has.Some.EqualTo("MethodsSampleTests.cs"));
            });
        }
    }
}