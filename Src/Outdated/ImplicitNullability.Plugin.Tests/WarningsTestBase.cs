using System;
using System.Collections.Generic;
using System.Linq;
using ImplicitNullability.Plugin.Settings;
using ImplicitNullability.Plugin.Warnings;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.CSharp;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.Util;

namespace ImplicitNullability.Plugin.Tests.Warnings
{
  public abstract class WarningsTestBase : CSharpHighlightingTestBase
  {
    protected override IEnumerable<string> GetReferencedAssemblies ()
    {
      return new[]
             {
                 typeof (CanBeNullAttribute).Assembly.Location
             };
    }

    private string[] AuxiliaryFiles
    {
      get { return new[] { TestDataPathUtility.GetPathRelativeToSampleProject ("ReSharper.cs") }; }
    }

    protected override bool HighlightingPredicate (IHighlighting highlighting, IContextBoundSettingsStore settingsstore)
    {
      var result = highlighting is ImplicitNotNullParameterConflictInHierarchyWarning ||
                   highlighting is NotNullAnnotationOnImplicitCanBeNullParameterWarning ||
                   // ReSharper's own warning:
                   highlighting is AnnotationConflictInHierarchyWarning ||
                   IsReSharperNullabilityAnalysisWarning (highlighting);

      return result;
    }

    private bool IsReSharperNullabilityAnalysisWarning (IHighlighting highlighting)
    {
      return highlighting is PossibleInvalidOperationExceptionWarning ||
             highlighting is PossibleNullReferenceExceptionWarning ||
             highlighting is AssignNullToNotNullAttributeWarning;
    }

    protected override void DoTestSolution (params string[] fileSet)
    {
      ExecuteWithinSettingsTransaction (
          settingsStore =>
          {
            RunGuarded (() => settingsStore.SetValue ((ImplicitNullabilitySettings x) => x.Enabled, true));

            fileSet = fileSet.Concat (AuxiliaryFiles).ToArray();
            base.DoTestSolution (fileSet);
          });
    }
  }
}