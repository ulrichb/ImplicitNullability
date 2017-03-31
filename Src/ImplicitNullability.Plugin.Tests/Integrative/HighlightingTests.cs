using System;
using System.Collections.Generic;
using System.Linq;
using ImplicitNullability.Plugin.Tests.Infrastructure;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
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
            TestWithEnabledImplicitNullability((issueCount, issueSourceFiles) =>
            {
                // Fixation: minimum amount of warnings, selected files
                Assert.That(issueCount, Is.GreaterThanOrEqualTo(20));
                Assert.That(issueSourceFiles.Select(x => x.Name), Has.Some.EqualTo("CanBeNullOneOfThreeSuperMembers.cs"));
                Assert.That(issueSourceFiles.Select(x => x.Name), Has.Some.EqualTo("HierarchyWithPostconditionsWeakerInDerived.cs"));
                Assert.That(issueSourceFiles.Select(x => x.Name), Has.Some.EqualTo("HierarchyWithPreconditionsStrongerInDerived.cs"));
                Assert.That(issueSourceFiles.Select(x => x.Name), Has.Some.EqualTo("LongInheritanceChain.cs"));
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
            TestWithEnabledImplicitNullability((issueCount, issueSourceFiles) =>
            {
                // Fixation: minimum amount of warnings, selected files
                Assert.That(issueCount, Is.GreaterThanOrEqualTo(15));
                Assert.That(issueSourceFiles.Select(x => x.Name), Has.Some.EqualTo("OverrideExternalCodeWithAnnotations.cs"));
                Assert.That(issueSourceFiles.Select(x => x.Name), Has.Some.EqualTo("OverrideExternalCode.cs"));
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
            TestWithEnabledImplicitNullability((issueCount, issueSourceFiles) =>
            {
                // Fixation: minimum amount of warnings, selected files
                Assert.That(issueCount, Is.GreaterThanOrEqualTo(7));
                Assert.That(issueSourceFiles.Select(x => x.Name), Has.Some.EqualTo("NotNullOnImplicitCanBeNullSample.cs"));
                Assert.That(issueSourceFiles.Select(x => x.Name), Has.Some.EqualTo("NotNullOnImplicitCanBeNullSampleTests.cs"));
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
            TestWithEnabledImplicitNullability((issueCount, issueSourceFiles) =>
            {
                // Fixation: minimum amount of warnings, selected files
                Assert.That(issueCount, Is.GreaterThanOrEqualTo(4));
                Assert.That(issueSourceFiles.Select(x => x.Name), Has.Some.EqualTo("AnnotationRedundancyInHierarchySample.cs"));
                Assert.That(issueSourceFiles.Select(x => x.Name), Has.Some.EqualTo("OtherWarningsSample.cs"));
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

        private void TestWithEnabledImplicitNullability(Action<int, IList<IPsiSourceFile>> assert)
        {
            UseSampleSolution((solution, solutionSettings) =>
            {
                solutionSettings.EnableImplicitNullabilityForAllCodeElements();

                var projectFilesToAnalyze = GetProjectFilesToAnalyze(solution);

                var issues = TestExpectedInspectionComments(solution, projectFilesToAnalyze, GetNullabilityAnalysisHighlightingTypes(), "Implicit");

                assert(issues.Count, issues.Select(x => x.GetSourceFile()).ToList());
            });
        }

        private static IEnumerable<IProjectFile> GetProjectFilesToAnalyze(ISolution solution)
        {
            var pathPrefix = "Highlighting\\" + TestContext.CurrentContext.Test.Name.Split('_')[0];

            return solution.GetAllProjectFilesWithPathPrefix(pathPrefix).ToList();
        }
    }
}
