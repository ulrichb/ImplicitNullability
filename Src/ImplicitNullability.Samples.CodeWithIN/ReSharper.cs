using JetBrains.Annotations;

namespace ImplicitNullability.Samples.CodeWithIN
{
    [NullGuard.NullGuard(NullGuard.ValidationFlags.None)]
    public static class ReSharper
    {
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
    }
}
