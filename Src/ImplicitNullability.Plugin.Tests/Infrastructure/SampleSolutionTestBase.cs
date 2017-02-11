using System;
using System.Collections.Generic;
using System.Reflection;
using ImplicitNullability.Plugin.Settings;
using JetBrains.Application.Settings;
using JetBrains.Application.Settings.Store.Implementation;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.Util;
using NUnit.Framework;

namespace ImplicitNullability.Plugin.Tests.Infrastructure
{
    public abstract class SampleSolutionTestBase : InspectionExpectationCommentsTestBase
    {
        protected void UseSampleSolution(Action<ISolution> testAction)
        {
            var solutionFilePath = FileSystemPath.Parse(TestDataPathUtility.GetPathRelativeToSolution("ImplicitNullability.Sample.sln"));

            DoTestSolution(solutionFilePath, (lifetime, solution) => { testAction(solution); });

            // Close the solution to isolate all solution-level settings changes (this has a small performance hit because normally the
            // solution is only "cleaned" at the end of DoTestSolution())
            RunGuarded(() => CloseSolution());
        }

        protected IEnumerable<Type> GetNullabilityAnalysisHighlightingTypes()
        {
            var implicitNullabilityProblemAnalyzerHighlightingTypes =
                typeof(ImplicitNullabilityProblemAnalyzer).GetCustomAttribute<ElementProblemAnalyzerAttribute>(false).HighlightingTypes;

            return new[]
                {
                    typeof(AssignNullToNotNullAttributeWarning),
                    typeof(ConditionIsAlwaysTrueOrFalseWarning),
                    typeof(PossibleNullReferenceExceptionWarning),
                    typeof(PossibleInvalidOperationExceptionWarning)
                }
                .Concat(implicitNullabilityProblemAnalyzerHighlightingTypes);
        }

        protected static void EnableImplicitNullability(
            ISolution sampleSolution,
            bool enableInputParameters = false,
            bool enableRefParameters = false,
            bool enableOutParametersAndResult = false)
        {
            // We need to change the settings here by code, because the settings stored in the .DotSettings files aren't 
            // evaluated (see https://resharper-support.jetbrains.com/hc/en-us/community/posts/206628865).
            // Note that the settings changes are cleaned in UseSampleSolution() => no reset mechanism necessary.

            var solutionSettings = sampleSolution.GetComponent<SettingsStore>()
                .BindToContextTransient(ContextRange.ManuallyRestrictWritesToOneContext(sampleSolution.ToDataContext()));

            // Fixate default values:
            Assert.That(solutionSettings.GetValue((ImplicitNullabilitySettings s) => s.Enabled), Is.False);
            Assert.That(solutionSettings.GetValue((ImplicitNullabilitySettings s) => s.EnableInputParameters), Is.True);
            Assert.That(solutionSettings.GetValue((ImplicitNullabilitySettings s) => s.EnableRefParameters), Is.True);
            Assert.That(solutionSettings.GetValue((ImplicitNullabilitySettings s) => s.EnableOutParametersAndResult), Is.True);

            solutionSettings.SetValue((ImplicitNullabilitySettings s) => s.Enabled, true);
            solutionSettings.SetValue((ImplicitNullabilitySettings s) => s.EnableInputParameters, enableInputParameters);
            solutionSettings.SetValue((ImplicitNullabilitySettings s) => s.EnableRefParameters, enableRefParameters);
            solutionSettings.SetValue((ImplicitNullabilitySettings s) => s.EnableOutParametersAndResult, enableOutParametersAndResult);
        }

        protected static void EnableImplicitNullabilityWithAllOptions(ISolution sampleSolution)
        {
            EnableImplicitNullability(sampleSolution, true, true, true);
        }
    }
}
