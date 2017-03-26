using JetBrains.Annotations;

namespace ImplicitNullability.Samples.CodeWithIN.Special
{
    public static class ReSharper
    {
        public static T TestValueAnalysis<T>([NotNull] T testNotNull, bool conditionToTestCanBeNull)
        {
            return testNotNull;
        }

        public static void SuppressUnusedWarning(params object[] values)
        {
        }
    }
}
