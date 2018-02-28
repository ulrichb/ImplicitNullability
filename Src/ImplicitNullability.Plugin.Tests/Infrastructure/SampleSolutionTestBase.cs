using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Daemon.UsageChecking;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.Util;

namespace ImplicitNullability.Plugin.Tests.Infrastructure
{
    public abstract class SampleSolutionTestBase : InspectionExpectationCommentsTestBase
    {
        protected void UseSampleSolution(Action<ISolution, IContextBoundSettingsStore> testAction)
        {
            var solutionFilePath = FileSystemPath.Parse(TestDataPathUtility.GetPathRelativeToSolution("ImplicitNullability.Sample.sln"));

            ExecuteWithinSettingsTransaction(settingsStore =>
            {
                DoTestSolution(solutionFilePath, (lifetime, solution) =>
                {
                    // We need to change the settings by code (via `settingsStore`), because the settings stored in the .DotSettings files aren't
                    // evaluated (see https://resharper-support.jetbrains.com/hc/en-us/community/posts/206628865).

                    testAction(solution, settingsStore);
                });
            });
        }

        protected static IEnumerable<Type> GetNullabilityAnalysisHighlightingTypes()
        {
            var implicitNullabilityProblemAnalyzerHighlightingTypes =
                typeof(ImplicitNullabilityProblemAnalyzer).GetCustomAttribute<ElementProblemAnalyzerAttribute>(inherit: false).HighlightingTypes;

            return new[]
                {
                    typeof(AssignNullToNotNullAttributeWarning),
                    typeof(ConditionIsAlwaysTrueOrFalseWarning),
                    typeof(PossibleNullReferenceExceptionWarning),
                    typeof(PossibleInvalidOperationExceptionWarning),
                    typeof(NotNullMemberIsNotInitializedWarning),
                    typeof(UnassignedReadonlyFieldWarning),
                    typeof(NullCoalescingConditionIsAlwaysNotNullWarning),
                    typeof(NullCoalescingConditionIsAlwaysNullWarning),
                    typeof(NullCoalescingRightOperandIsAlwaysNullWarning),
                }
                .Concat(implicitNullabilityProblemAnalyzerHighlightingTypes);
        }
    }
}
