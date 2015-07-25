using System;
using System.Collections.Generic;
using System.Linq;
using ImplicitNullability.Plugin.Settings;
using ImplicitNullability.Plugin.Tests.Infrastructure;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.SolutionAnalysis;
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
        public void WithEnabledInputAndRefParameters()
        {
            Test(settingsStore => settingsStore.SetValue((ImplicitNullabilitySettings s) => s.EnableInputAndRefParameters, true),
                issues =>
                {
                    // Fixation: minimum amount of warnings, selected files

                    Assert.That(issues.Count, Is.GreaterThanOrEqualTo(70));

                    var filePaths = issues.Select(x => x.GetSourceFile().Name).ToList();
                    Assert.That(filePaths, Has.Some.EqualTo("AspxSample.aspx"));
                    Assert.That(filePaths, Has.Some.EqualTo("RazorSample.cshtml"));
                    Assert.That(filePaths, Has.Some.EqualTo("MethodsInputSample.cs"));
                    Assert.That(filePaths, Has.Some.EqualTo("MethodsInputSampleTests.cs"));
                },
                "MIn");
        }

        [Test]
        public void WithEnabledOutParametersAndResult()
        {
            Test(settingsStore => settingsStore.SetValue((ImplicitNullabilitySettings s) => s.EnableOutParametersAndResult, true),
                issues =>
                {
                    // Fixation: minimum amount of warnings, selected files

                    Assert.That(issues.Count, Is.GreaterThanOrEqualTo(25));

                    var filePaths = issues.Select(x => x.GetSourceFile().Name).ToList();
                    Assert.That(filePaths, Has.Some.EqualTo("AspxSample.aspx"));
                    Assert.That(filePaths, Has.Some.EqualTo("RazorSample.cshtml"));
                    Assert.That(filePaths, Has.Some.EqualTo("MethodsOutputSample.cs"));
                    Assert.That(filePaths, Has.Some.EqualTo("MethodsOutputSampleTests.cs"));
                },
                "MOut");
        }

        [Test]
        public void WithoutEnabledSettings()
        {
            // Ensures that when the implicit nullability settings are disabled, the conditional expected warnings are *not* present.
            Test(settingsStore => { }, issues => { });
        }

        private void Test(Action<IContextBoundSettingsStore> enableSettings, Action<IList<IIssue>> assert, string definedExpectedWarningSymbol = null)
        {
            UseSampleSolution(solution =>
            {
                EnableImplicitNullabilitySetting(solution.GetProjectByName("ImplicitNullability.Sample").NotNull(), settingsStore =>
                {
                    settingsStore.SetValue((ImplicitNullabilitySettings s) => s.EnableInputAndRefParameters, false);
                    settingsStore.SetValue((ImplicitNullabilitySettings s) => s.EnableOutParametersAndResult, false);
                    enableSettings(settingsStore);
                });

                var projectFilesToAnalyze = solution.GetAllProjectFilesWithPathPrefix("NullabilityAnalysis\\");

                var issues = TestExpectedInspectionComments(solution, projectFilesToAnalyze, GetNullabilityAnalysisHighlightingTypes(),
                    definedExpectedWarningSymbol);

                assert(issues);
            });
        }
    }
}