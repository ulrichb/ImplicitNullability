using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ImplicitNullability.Plugin.Highlighting;
using ImplicitNullability.Plugin.Tests.Infrastructure;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.Util;
using NUnit.Framework;
#if !RESHARPER8
using JetBrains.ProjectModel;

#endif

namespace ImplicitNullability.Plugin.Tests
{
    public class HighlightingTests : SampleSolutionTestBase
    {
        [Test]
        public void ImplicitNotNullConflictInHierarchy()
        {
            Test((issueCount, issueFilePaths) =>
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
        public void ImplicitNotNullOverridesUnknownExternalMember()
        {
            Test((issueCount, issueFilePaths) =>
            {
                // Fixation: minimum amount of warnings, selected files
                Assert.That(issueCount, Is.GreaterThanOrEqualTo(10));
                Assert.That(issueFilePaths, Has.Some.EqualTo("OverrideExternalMethodWithAnnotations.cs"));
                Assert.That(issueFilePaths, Has.Some.EqualTo("DerivedClassFromExternalClass.cs"));
            });
        }

        [Test]
        public void NotNullOnImplicitCanBeNull()
        {
            Test((issueCount, issueFilePaths) =>
            {
                // Fixation: minimum amount of warnings, selected files
                Assert.That(issueCount, Is.GreaterThanOrEqualTo(7));
                Assert.That(issueFilePaths, Has.Some.EqualTo("NotNullOnImplicitCanBeNullSample.cs"));
                Assert.That(issueFilePaths, Has.Some.EqualTo("NotNullOnImplicitCanBeNullSampleTests.cs"));
            });
        }

        private void Test(Action<int, IList<string>> assert)
        {
            UseSampleSolution(solution =>
            {
                EnableImplicitNullabilitySetting(solution.GetProjectByName("ImplicitNullability.Sample").NotNull());

                var pathPrefix = "Highlighting\\" + TestContext.CurrentContext.Test.Name;
                var projectFilesToAnalyze = solution.GetAllProjectFilesWithPathPrefix(pathPrefix).ToList();
                Trace.Assert(projectFilesToAnalyze.Any());

                var issues = TestExpectedInspectionComments(solution, projectFilesToAnalyze, GetHighlightingTypesToAnalyze());

                assert(issues.Count, issues.Select(x => x.GetSourceFile().Name).ToList());
            });
        }

        private IEnumerable<Type> GetHighlightingTypesToAnalyze()
        {
            return new[]
            {
                typeof (NotNullOnImplicitCanBeNullHighlighting),
                //
                typeof (ImplicitNotNullConflictInHierarchyHighlighting),
                typeof (AnnotationConflictInHierarchyWarning), // This is ReSharper's hierarchy conflict warning
                //
                typeof (ImplicitNotNullOverridesUnknownExternalMemberHighlighting)
            }
                .Concat(GetNullabilityAnalysisHighlightingTypes());
        }
    }
}