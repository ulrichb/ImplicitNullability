using JetBrains.Annotations;

namespace ImplicitNullability.Sample.NonImplicit
{
    public static class ReSharper
    {
        // ReSharper disable UnusedParameter.Global

        public static T TestValueAnalysis<T>([NotNull] T testNotNull, bool conditionToTestCanBeNull)
        {
            return testNotNull;
        }

        public static void SuppressUnusedWarning(object value)
        {
        }

        // ReSharper restore UnusedParameter.Global
    }
}