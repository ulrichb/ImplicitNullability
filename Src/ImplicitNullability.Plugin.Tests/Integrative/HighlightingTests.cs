using System;
using System.Collections.Generic;
using System.Linq;
using ImplicitNullability.Plugin.Tests.Infrastructure;
using JetBrains.ProjectModel;
using NUnit.Framework;

namespace ImplicitNullability.Plugin.Tests.Integrative
{
    public class HighlightingTests : SampleSolutionTestBase
    {
        [Test]
        public void ImplicitNotNullConflictInHierarchy_WithDisabledImplicitNullability()
        {
            TestWithDisabledImplicitNullability();
        }

        [Test]
        public void ImplicitNotNullConflictInHierarchy_WithEnabledImplicitNullability()
        {
            TestWithEnabledImplicitNullability(issueFiles =>
            {
                // Fixation of selected files
                Assert.That(issueFiles, Has.Some.EqualTo("CanBeNullOneOfThreeSuperMembers.cs"));
                Assert.That(issueFiles, Has.Some.EqualTo("HierarchyWithPostconditionsWeakerInDerived.cs"));
                Assert.That(issueFiles, Has.Some.EqualTo("HierarchyWithPreconditionsStrongerInDerived.cs"));
                Assert.That(issueFiles, Has.Some.EqualTo("LongInheritanceChain.cs"));
            });
        }

        [Test]
        public void ImplicitNotNullOverridesUnknownBaseMemberNullability_WithDisabledImplicitNullability()
        {
            TestWithDisabledImplicitNullability();
        }

        [Test]
        public void ImplicitNotNullOverridesUnknownBaseMemberNullability_TestWithEnabledImplicitNullability()
        {
            TestWithEnabledImplicitNullability(issueFiles =>
            {
                // Fixation of selected files
                Assert.That(issueFiles, Has.Some.EqualTo("OverrideExternalCodeWithAnnotations.cs"));
                Assert.That(issueFiles, Has.Some.EqualTo("OverrideExternalCode.cs"));
            });
        }

        [Test]
        public void NotNullOnImplicitCanBeNull_WithDisabledImplicitNullability()
        {
            TestWithDisabledImplicitNullability();
        }

        [Test]
        public void NotNullOnImplicitCanBeNull_TestWithEnabledImplicitNullability()
        {
            TestWithEnabledImplicitNullability(issueFiles =>
            {
                // Fixation of selected files
                Assert.That(issueFiles, Has.Some.EqualTo("NotNullOnImplicitCanBeNullSample.cs"));
                Assert.That(issueFiles, Has.Some.EqualTo("NotNullOnImplicitCanBeNullSampleTests.cs"));
            });
        }

        [Test]
        public void IncorrectNullableAttributeUsageAnalyzer_WithDisabledImplicitNullability()
        {
            TestWithDisabledImplicitNullability();
        }

        [Test]
        public void IncorrectNullableAttributeUsageAnalyzer_TestWithEnabledImplicitNullability()
        {
            TestWithEnabledImplicitNullability(issueFiles =>
            {
                // Fixation of selected files
                Assert.That(issueFiles, Has.Some.EqualTo("AnnotationRedundancyInHierarchySample.cs"));
                Assert.That(issueFiles, Has.Some.EqualTo("OtherWarningsSample.cs"));
            });
        }

        private void TestWithDisabledImplicitNullability()
        {
            UseSampleSolution((solution, _) =>
            {
                var projectFilesToAnalyze = GetProjectFilesToAnalyze(solution);

                TestExpectedInspectionComments(solution, projectFilesToAnalyze, GetNullabilityAnalysisHighlightingTypes());
            });
        }

        private void TestWithEnabledImplicitNullability(Action<IList<string>> assert)
        {
            UseSampleSolution((solution, solutionSettings) =>
            {
                solutionSettings.EnableImplicitNullabilityForAllCodeElements();

                var projectFilesToAnalyze = GetProjectFilesToAnalyze(solution);

                var issues = TestExpectedInspectionComments(solution, projectFilesToAnalyze, GetNullabilityAnalysisHighlightingTypes(), "Implicit");

                assert.Invoke(issues.Select(x => x.GetSourceFile().Name).ToList());
            });
        }

        private static IEnumerable<IProjectFile> GetProjectFilesToAnalyze(ISolution solution)
        {
            var pathPrefix = "Highlighting\\" + TestContext.CurrentContext.Test.Name.Split('_')[0];

            return solution.GetAllProjectFilesWithPathPrefix(pathPrefix).ToList();
        }
    }
}
