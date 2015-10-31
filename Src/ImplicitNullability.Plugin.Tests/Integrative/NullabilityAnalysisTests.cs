using System;
using System.Collections.Generic;
using System.Linq;
using ImplicitNullability.Plugin.Tests.Infrastructure;
using JetBrains.ProjectModel;
using NUnit.Framework;

namespace ImplicitNullability.Plugin.Tests.Integrative
{
    public class NullabilityAnalysisTests : SampleSolutionTestBase
    {
        private const string ExternalCodeConsumerProjectName = "ImplicitNullability.Samples.Consumer.OfExternalCodeWithIN";

        [Test]
        public void WithEnabledInputParameters()
        {
            Test(changeSettings: solution => EnableImplicitNullability(solution, true, false, false),
                definedExpectedWarningSymbols: new[] {"MIn"},
                //
                assert: (issueCount, issueFilePaths) =>
                {
                    // Fixation: minimum amount of warnings, selected files

                    Assert.That(issueCount, Is.GreaterThanOrEqualTo(70));
                    Assert.That(issueFilePaths, Has.Some.EqualTo("AspxSample.aspx"));
                    Assert.That(issueFilePaths, Has.Some.EqualTo("RazorSample.cshtml"));
                    Assert.That(issueFilePaths, Has.Some.EqualTo("MethodsInputSample.cs"));
                    Assert.That(issueFilePaths, Has.Some.EqualTo("MethodsInputSampleTests.cs"));
                });
        }

        [Test]
        public void WithEnabledRefParameters()
        {
            Test(changeSettings: solution => EnableImplicitNullability(solution, false, true, false),
                definedExpectedWarningSymbols: new[] {"MRef"},
                //
                assert: (issueCount, issueFilePaths) =>
                {
                    // Fixation: minimum amount of warnings, selected files

                    Assert.That(issueCount, Is.GreaterThanOrEqualTo(25));
                    Assert.That(issueFilePaths, Has.Some.EqualTo("MethodsRefParameterSample.cs"));
                });
        }

        [Test]
        public void WithEnabledOutParametersAndResult()
        {
            Test(changeSettings: solution => EnableImplicitNullability(solution, false, false, true),
                definedExpectedWarningSymbols: new[] {"MOut"},
                //
                assert: (issueCount, issueFilePaths) =>
                {
                    // Fixation: minimum amount of warnings, selected files

                    Assert.That(issueCount, Is.GreaterThanOrEqualTo(25));
                    Assert.That(issueFilePaths, Has.Some.EqualTo("AspxSample.aspx"));
                    Assert.That(issueFilePaths, Has.Some.EqualTo("RazorSample.cshtml"));
                    Assert.That(issueFilePaths, Has.Some.EqualTo("MethodsOutputSample.cs"));
                    Assert.That(issueFilePaths, Has.Some.EqualTo("MethodsOutputSampleTests.cs"));
                });
        }

        [Test]
        public void WithoutEnabledImplicitNullabilityOptions()
        {
            // Ensures that when the implicit nullability settings are disabled, the conditional expected warnings are *not* present.
            Test(changeSettings: solution => EnableImplicitNullability(solution, false, false, false),
                definedExpectedWarningSymbols: new string[0]);
        }

        [Test]
        public void WithEnabledImplicitNullabilityUsingAssemblyMetadataAttributeInExternalCode()
        {
            Test(changeSettings: solution => EnableImplicitNullability(solution, false, false, false),
                projectFilter: x => x.Name == ExternalCodeConsumerProjectName,
                definedExpectedWarningSymbols: new[] {"MIn", "MRef", "MOut"} /*as configured in ImplicitNullabilityAssemblyMetadata.cs*/,
                //
                assert: (issueCount, issueFilePaths) =>
                {
                    // Fixation: minimum amount of warnings, selected files

                    Assert.That(issueCount, Is.GreaterThanOrEqualTo(35));
                    Assert.That(issueFilePaths, Has.Some.EqualTo("DelegatesSampleTests.cs"));
                    Assert.That(issueFilePaths, Has.Some.EqualTo("MethodsInputSampleTests.cs"));
                    Assert.That(issueFilePaths, Has.Some.EqualTo("MethodsOutputSampleTests.cs"));
                });
        }

        private void Test(
            Action<ISolution> changeSettings,
            string[] definedExpectedWarningSymbols,
            Func<IProject, bool> projectFilter = null,
            Action<int, IList<string>> assert = null)
        {
            UseSampleSolution(solution =>
            {
                changeSettings(solution);

                var projectFilesToAnalyze = solution.GetAllProjects()
                    // By default exclude the "external code consumer" project (which consumes "hard-coded" implicit nullability settings):
                    .Where(projectFilter ?? (x => x.Name != ExternalCodeConsumerProjectName))
                    .GetAllProjectFilesWithPathPrefix("NullabilityAnalysis\\").ToList();
                Assert.That(projectFilesToAnalyze, Is.Not.Empty);

                var highlightingTypesToAnalyze = GetNullabilityAnalysisHighlightingTypes();
                var issues = TestExpectedInspectionComments(solution, projectFilesToAnalyze, highlightingTypesToAnalyze, definedExpectedWarningSymbols);

                assert?.Invoke(issues.Count, issues.Select(x => x.GetSourceFile().Name).ToList());
            });
        }
    }
}