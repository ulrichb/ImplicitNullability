using System;
using System.Collections.Generic;
using System.Linq;
using ImplicitNullability.Plugin.Tests.Infrastructure;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
using NUnit.Framework;

namespace ImplicitNullability.Plugin.Tests.Integrative
{
    [Category("Nullability Analysis")]
    public class NullabilityAnalysisTests : SampleSolutionTestBase
    {
        private const string ExternalCodeConsumerProjectName = "ImplicitNullability.Samples.Consumer.OfExternalCodeWithIN";

        [Test]
        public void WithEnabledInputParameters()
        {
            Test(changeSettings: x => x.EnableImplicitNullability(enableInputParameters: true),
                definedExpectedWarningSymbols: new[] { "MIn" },
                //
                assert: issueFiles =>
                {
                    // Fixation of selected files
                    Assert.That(issueFiles, Has.Some.EqualTo("MethodsInputSample.cs"));
                    Assert.That(issueFiles, Has.Some.EqualTo("MethodsInputSampleTests.cs"));

                    Assert.That(issueFiles, Has.Some.EqualTo("AspxSample.aspx"));
                    Assert.That(issueFiles, Has.Some.EqualTo("RazorSample.cshtml"));
                    Assert.That(issueFiles, Has.Some.EqualTo("SomeT4GeneratedClass.partial.cs"));
                });
        }

        [Test]
        public void WithEnabledRefParameters()
        {
            Test(changeSettings: x => x.EnableImplicitNullability(enableRefParameters: true),
                definedExpectedWarningSymbols: new[] { "MRef" },
                //
                assert: issueFiles =>
                {
                    // Fixation of selected files
                    Assert.That(issueFiles, Has.Some.EqualTo("MethodsRefParameterSample.cs"));
                });
        }

        [Test]
        public void WithEnabledOutParametersAndResult()
        {
            Test(changeSettings: x => x.EnableImplicitNullability(enableOutParametersAndResult: true),
                definedExpectedWarningSymbols: new[] { "MOut" },
                //
                assert: issueFiles =>
                {
                    // Fixation of selected files
                    Assert.That(issueFiles, Has.Some.EqualTo("MethodsOutputSample.cs"));
                    Assert.That(issueFiles, Has.Some.EqualTo("MethodsOutputSampleTests.cs"));

                    Assert.That(issueFiles, Has.Some.EqualTo("AspxSample.aspx"));
                    Assert.That(issueFiles, Has.Some.EqualTo("RazorSample.cshtml"));
                    Assert.That(issueFiles, Has.Some.EqualTo("SomeT4GeneratedClass.partial.cs"));
                });
        }

        [Test]
        public void WithEnabledFields()
        {
            Test(changeSettings: x => x.EnableImplicitNullability(enableFields: true),
                definedExpectedWarningSymbols: new[] { "Flds" },
                //
                assert: issueFiles =>
                {
                    // Fixation of selected files
                    Assert.That(issueFiles, Has.Some.EqualTo("FieldsSample.cs"));
                    Assert.That(issueFiles, Has.Some.EqualTo("FieldsSampleTests.cs"));

                    Assert.That(issueFiles, Has.Some.EqualTo("SomeControlWithUninitializedField.xaml.cs"));
                });
        }

        [Test]
        public void WithEnabledFieldsAndRestrictToReadonly()
        {
            Test(changeSettings: x => x.EnableImplicitNullability(enableFields: true, fieldsRestrictToReadonly: true),
                definedExpectedWarningSymbols: new[] { "Flds", "RtRo" });
        }

        [Test]
        public void WithEnabledFieldsAndRestrictToReferenceTypes()
        {
            Test(changeSettings: x => x.EnableImplicitNullability(enableFields: true, fieldsRestrictToReferenceTypes: true),
                definedExpectedWarningSymbols: new[] { "Flds", "RtRT" });
        }

        [Test]
        public void WithoutEnabledImplicitNullabilityOptions()
        {
            // Ensures that when the implicit nullability settings are disabled, the conditional expected warnings are *not* present.
            Test(changeSettings: x => x.EnableImplicitNullability( /* no options*/),
                definedExpectedWarningSymbols: new string[0]);
        }

        [Test]
        public void WithEnabledImplicitNullabilityAndWithoutExcludeGeneratedCode()
        {
            Test(changeSettings: x => x.EnableImplicitNullabilityForAllCodeElements(excludeGeneratedCode: false),
                definedExpectedWarningSymbols: new[] { "MIn", "MRef", "MOut", "Flds", "InclGenCode" },
                //
                assert: issueFiles =>
                {
                    // Fixation of selected files
                    Assert.That(issueFiles, Has.Some.EqualTo("GeneratedCodeSample.cs"));
                    Assert.That(issueFiles, Has.Some.EqualTo("GeneratedCodeSampleTests.cs"));
                    Assert.That(issueFiles, Has.Some.EqualTo("SomeT4GeneratedClass.partial.cs"));
                });
        }

        [Test]
        public void WithEnabledImplicitNullabilityUsingAssemblyMetadataAttributeInExternalCode()
        {
            Test(changeSettings: x => x.EnableImplicitNullability( /* no options*/),
                projectFilter: x => x.Name == ExternalCodeConsumerProjectName,
                // as configured in ImplicitNullabilityAssemblyMetadata.cs:
                definedExpectedWarningSymbols: new[] { "MIn", "MRef", "MOut", "Flds", "RtRo", "RtRT" },
                //
                assert: issueFiles =>
                {
                    // Fixation of selected files
                    Assert.That(issueFiles, Has.Some.EqualTo("DelegatesSampleTests.cs"));
                    Assert.That(issueFiles, Has.Some.EqualTo("MethodsInputSampleTests.cs"));
                    Assert.That(issueFiles, Has.Some.EqualTo("MethodsOutputSampleTests.cs"));
                    Assert.That(issueFiles, Has.Some.EqualTo("FieldsSampleTests.cs"));
                    Assert.That(issueFiles, Has.Some.EqualTo("GeneratedCodeSampleTests.cs"));
                });
        }

        private void Test(
            Action<IContextBoundSettingsStore> changeSettings,
            string[] definedExpectedWarningSymbols,
            Func<IProject, bool> projectFilter = null,
            Action<IList<string>> assert = null)
        {
            UseSampleSolution((solution, solutionSettings) =>
            {
                changeSettings(solutionSettings);

                var projectFilesToAnalyze = solution.GetAllProjects()
                    // By default exclude the "external code consumer" project (which consumes "hard-coded" implicit nullability settings):
                    .Where(projectFilter ?? (x => x.Name != ExternalCodeConsumerProjectName))
                    .GetAllProjectFilesWithPathPrefix(@"NullabilityAnalysis\")
                    .ToList();
                Assert.That(projectFilesToAnalyze, Is.Not.Empty);

                var highlightingTypes = GetNullabilityAnalysisHighlightingTypes();
                var issues = TestExpectedInspectionComments(solution, projectFilesToAnalyze, highlightingTypes, definedExpectedWarningSymbols);

                assert?.Invoke(issues.Select(x => x.GetSourceFile().Name).ToList());
            });
        }
    }
}
