using System;
using System.Collections.Generic;
using System.Linq;
using ImplicitNullability.Plugin.Tests.Infrastructure;
using JetBrains.Annotations;
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
            TestWithEnabledImplicitNullability((issueCount, issueFilePaths) =>
            {
                // Fixation: minimum amount of warnings, selected files
                Assert.That(issueCount, Is.GreaterThanOrEqualTo(20));
                Assert.That(issueFilePaths, Has.Some.EqualTo("CanBeNullOneOfThreeSuperMembers.cs"));
                Assert.That(issueFilePaths, Has.Some.EqualTo("HierarchyWithPostconditionsWeakerInDerived.cs"));
                Assert.That(issueFilePaths, Has.Some.EqualTo("HierarchyWithPreconditionsStrongerInDerived.cs"));
                Assert.That(issueFilePaths, Has.Some.EqualTo("LongInheritanceChain.cs"));
            });
        }

        [Test]
        public void ImplicitNotNullOverridesUnknownExternalMember_WithDisabledImplicitNullability()
        {
            TestWithDisabledImplicitNullability();
        }

        [Test]
        public void ImplicitNotNullOverridesUnknownExternalMember_TestWithEnabledImplicitNullability()
        {
            TestWithEnabledImplicitNullability((issueCount, issueFilePaths) =>
            {
                // Fixation: minimum amount of warnings, selected files
                Assert.That(issueCount, Is.GreaterThanOrEqualTo(9));
                Assert.That(issueFilePaths, Has.Some.EqualTo("OverrideExternalCodeWithAnnotations.cs"));
                Assert.That(issueFilePaths, Has.Some.EqualTo("OverrideExternalCode.cs"));
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
            TestWithEnabledImplicitNullability((issueCount, issueFilePaths) =>
            {
                // Fixation: minimum amount of warnings, selected files
                Assert.That(issueCount, Is.GreaterThanOrEqualTo(7));
                Assert.That(issueFilePaths, Has.Some.EqualTo("NotNullOnImplicitCanBeNullSample.cs"));
                Assert.That(issueFilePaths, Has.Some.EqualTo("NotNullOnImplicitCanBeNullSampleTests.cs"));
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
            TestWithEnabledImplicitNullability((issueCount, issueFilePaths) =>
            {
                // Fixation: minimum amount of warnings, selected files
                Assert.That(issueCount, Is.GreaterThanOrEqualTo(4));
                Assert.That(issueFilePaths, Has.Some.EqualTo("AnnotationRedundancyInHierarchySample.cs"));
                Assert.That(issueFilePaths, Has.Some.EqualTo("OtherWarningsSample.cs"));
            });
        }

        private void TestWithDisabledImplicitNullability()
        {
            UseSampleSolution(solution =>
            {
                var projectFilesToAnalyze = GetProjectFilesToAnalyze(solution);

                TestExpectedInspectionComments(solution, projectFilesToAnalyze, GetNullabilityAnalysisHighlightingTypes());
            });
        }

        private void TestWithEnabledImplicitNullability(Action<int, IList<string>> assert)
        {
            UseSampleSolution(solution =>
            {
                EnableImplicitNullabilitySetting(solution);

                var projectFilesToAnalyze = GetProjectFilesToAnalyze(solution);

                var issues = TestExpectedInspectionComments(solution, projectFilesToAnalyze, GetNullabilityAnalysisHighlightingTypes(), "Implicit");

                assert(issues.Count, issues.Select(x => x.GetSourceFile().Name).ToList());
            });
        }

        private static IEnumerable<IProjectFile> GetProjectFilesToAnalyze([NotNull] ISolution solution)
        {
            var pathPrefix = "Highlighting\\" + TestContext.CurrentContext.Test.Name.Split('_')[0];

            return solution.GetAllProjectFilesWithPathPrefix(pathPrefix).ToList();
        }
    }
}