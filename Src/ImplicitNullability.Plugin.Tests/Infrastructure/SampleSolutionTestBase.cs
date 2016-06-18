using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Application.Settings;
using JetBrains.Application.Settings.Store.Implementation;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Daemon.UsageChecking;
using JetBrains.Util;

namespace ImplicitNullability.Plugin.Tests.Infrastructure
{
    public abstract class SampleSolutionTestBase : InspectionExpectationCommentsTestBase
    {
        protected void UseSampleSolution(Action<ISolution, IContextBoundSettingsStore> testAction)
        {
            var solutionFilePath = FileSystemPath.Parse(TestDataPathUtility.GetPathRelativeToSolution("ImplicitNullability.Sample.sln"));

            DoTestSolution(solutionFilePath, (lifetime, solution) =>
            {
                // We need to change the settings here by code, because the settings stored in the .DotSettings files aren't 
                // evaluated (see https://resharper-support.jetbrains.com/hc/en-us/community/posts/206628865).

                var solutionSettings = solution.GetComponent<SettingsStore>()
                    .BindToContextTransient(ContextRange.ManuallyRestrictWritesToOneContext(solution.ToDataContext()));

                testAction(solution, solutionSettings);
            });

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
                    typeof(PossibleInvalidOperationExceptionWarning),
                    typeof(NotNullMemberIsNotInitializedWarning),
                    typeof(UnassignedReadonlyFieldWarning)
                }
                .Concat(implicitNullabilityProblemAnalyzerHighlightingTypes);
        }
    }
}
