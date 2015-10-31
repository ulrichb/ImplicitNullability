using System;
using JetBrains.Annotations;
using NullGuard;

namespace ImplicitNullability.Samples.CodeWithIN
{
    [NullGuard(ValidationFlags.None)]
    public static class ReSharper
    {
        // ReSharper disable UnusedParameter.Global

        public static T TestValueAnalysis<T>([NotNull] T testNotNull, bool conditionToTestCanBeNull)
        {
            return testNotNull;
        }

        public static unsafe void TestValueAnalysisUnsafe([NotNull] void* testNotNull, bool conditionToTestCanBeNull)
        {
        }

        public static void SuppressUnusedWarning(params object[] values)
        {
        }

        // ReSharper restore UnusedParameter.Global
    }
}