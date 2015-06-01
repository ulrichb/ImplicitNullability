using System;
using System.Collections.Generic;
using System.Linq;
using ImplicitNullability.Plugin.Highlighting;
using ImplicitNullability.Plugin.Tests.Infrastructure;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi;
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
        public void Test()
        {
            UseSampleSolution(solution =>
            {
                EnableImplicitNullabilitySetting(solution.GetProjectByName("ImplicitNullability.Sample").NotNull());

                var projectFilesToAnalyze = solution.GetAllProjectFilesWithPathPrefix("Highlighting\\");

                var issues = TestExpectedInspectionComments(solution, projectFilesToAnalyze, GetHighlightingTypesToAnalyze());

                // Fixation: minimum amount of warnings, selected files

                Assert.That(issues.Count, Is.GreaterThanOrEqualTo(35));

                var paths = issues.Select(x => x.GetSourceFile().ToProjectFile().NotNull().GetPresentableProjectPath()).ToList();
                Assert.That(paths, Has.Some.EndsWith("ImplicitNotNullConflictInHierarchy\\CanBeNullOneOfThreeSuperMembers.cs"));
                Assert.That(paths, Has.Some.EndsWith("ImplicitNotNullConflictInHierarchy\\HierarchyWithPostconditionsWeakerInDerived.cs"));
                Assert.That(paths, Has.Some.EndsWith("ImplicitNotNullConflictInHierarchy\\HierarchyWithPreconditionsStrongerInDerived.cs"));
                Assert.That(paths, Has.Some.EndsWith("ImplicitNotNullConflictInHierarchy\\LongInheritanceChain.cs"));
                Assert.That(paths, Has.Some.EndsWith("ImplicitNotNullOverridesUnknownExternalMember\\OverrideExternalMethodWithAnnotations.cs"));
                Assert.That(paths, Has.Some.EndsWith("ImplicitNotNullOverridesUnknownExternalMember\\DerivedClassFromExternalClass.cs"));
                Assert.That(paths, Has.Some.EndsWith("NotNullOnImplicitCanBeNull\\MethodAndIndexerParameters.cs"));
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