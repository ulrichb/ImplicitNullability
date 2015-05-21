using System;
using System.Collections.Generic;
using System.Linq;
using ImplicitNullability.Plugin.Highlighting;
using ImplicitNullability.Plugin.Tests.Infrastructure;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;
using NUnit.Framework;

namespace ImplicitNullability.Plugin.Tests
{
  public class HighlightingTestSolutionTests : HighlightingInPluginTestSolutionTestBase
  {
    protected override IEnumerable<Type> GetHighlightingTypesToAnalyze ()
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
          .Concat (GetNullabilityAnalysisHighlightingTypes());
    }

    protected override List<IProjectFile> GetProjectFilesToAnalyze (ISolution solution)
    {
      return
          (from project in solution.GetAllProjects()
            from file in project.GetAllProjectFiles()
            where ProjectUtil.GetRelativePresentableProjectPath (file, project).StartsWith ("Highlighting\\")
            select file).ToList();
    }

    protected override void ProcessResults (int totalIssuesCount, IEnumerable<IPsiSourceFile> filesWithIssues)
    {
      // Fixation: minimum amount of warnings, selected files

      Assert.That (totalIssuesCount, Is.GreaterThanOrEqualTo (35));

      var paths = filesWithIssues.Select (x => x.ToProjectFile().NotNull().GetPresentableProjectPath()).ToList();

      Assert.That (paths, Has.Some.EndsWith ("ImplicitNotNullConflictInHierarchy\\CanBeNullOneOfThreeSuperMembers.cs"));
      Assert.That (paths, Has.Some.EndsWith ("ImplicitNotNullConflictInHierarchy\\HierarchyWithPostconditionsWeakerInDerived.cs"));
      Assert.That (paths, Has.Some.EndsWith ("ImplicitNotNullConflictInHierarchy\\HierarchyWithPreconditionsStrongerInDerived.cs"));
      Assert.That (paths, Has.Some.EndsWith ("ImplicitNotNullConflictInHierarchy\\LongInheritanceChain.cs"));

      Assert.That (paths, Has.Some.EndsWith ("ImplicitNotNullOverridesUnknownExternalMember\\OverrideExternalMethodWithAnnotations.cs"));
      Assert.That (paths, Has.Some.EndsWith ("ImplicitNotNullOverridesUnknownExternalMember\\DerivedClassFromExternalClass.cs"));

      Assert.That (paths, Has.Some.EndsWith ("NotNullOnImplicitCanBeNull\\MethodAndIndexerParameters.cs"));
    }
  }
}