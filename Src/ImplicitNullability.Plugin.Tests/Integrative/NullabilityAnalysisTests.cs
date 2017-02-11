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
            Test(changeSettings: solution => EnableImplicitNullability(solution, enableInputParameters: true),
                definedExpectedWarningSymbols: new[] { "MIn" },
                //
                assert: issueFilePaths =>
                {
                    // Fixation of selected files
                    Assert.That(issueFilePaths, Has.Some.EqualTo("AspxSample.aspx"));
                    Assert.That(issueFilePaths, Has.Some.EqualTo("RazorSample.cshtml"));
                    Assert.That(issueFilePaths, Has.Some.EqualTo("MethodsInputSample.cs"));
                    Assert.That(issueFilePaths, Has.Some.EqualTo("MethodsInputSampleTests.cs"));
                });
        }

        [Test]
        public void WithEnabledRefParameters()
        {
            Test(changeSettings: solution => EnableImplicitNullability(solution, enableRefParameters: true),
                definedExpectedWarningSymbols: new[] { "MRef" },
                //
                assert: issueFilePaths =>
                {
                    // Fixation of selected files
                    Assert.That(issueFilePaths, Has.Some.EqualTo("MethodsRefParameterSample.cs"));
                });
        }

        [Test]
        public void WithEnabledOutParametersAndResult()
        {
            Test(changeSettings: solution => EnableImplicitNullability(solution, enableOutParametersAndResult: true),
                definedExpectedWarningSymbols: new[] { "MOut" },
                //
                assert: issueFilePaths =>
                {
                    // Fixation of selected files
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
            Test(changeSettings: solution => EnableImplicitNullability(solution /* no options*/),
                definedExpectedWarningSymbols: new string[0]);
        }

        [Test]
        public void WithEnabledImplicitNullabilityUsingAssemblyMetadataAttributeInExternalCode()
        {
            Test(changeSettings: solution => EnableImplicitNullability(solution /* no options*/),
                projectFilter: x => x.Name == ExternalCodeConsumerProjectName,
                definedExpectedWarningSymbols: new[] { "MIn", "MRef", "MOut" } /*as configured in ImplicitNullabilityAssemblyMetadata.cs*/,
                //
                assert: issueFilePaths =>
                {
                    // Fixation of selected files
                    Assert.That(issueFilePaths, Has.Some.EqualTo("DelegatesSampleTests.cs"));
                    Assert.That(issueFilePaths, Has.Some.EqualTo("MethodsInputSampleTests.cs"));
                    Assert.That(issueFilePaths, Has.Some.EqualTo("MethodsOutputSampleTests.cs"));
                });
        }

        private void Test(
            Action<ISolution> changeSettings,
            string[] definedExpectedWarningSymbols,
            Func<IProject, bool> projectFilter = null,
            Action<IList<string>> assert = null)
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

                assert?.Invoke(issues.Select(x => x.GetSourceFile().Name).ToList());
            });
        }
    }
}
