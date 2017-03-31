using JetBrains.Annotations;

namespace ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis
{
    public unsafe class UnsafeCodeSample
    {
        public static void Method(int* a)
        {
            ReSharper.TestValueAnalysisUnsafe(a, a == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
        }

        public static void MethodWithCanBeNull([CanBeNull] int* a)
        {
            ReSharper.TestValueAnalysisUnsafe(a /*Expect:AssignNullToNotNullAttribute*/, a == null);
        }

        public static void MethodWithRefAndOutParameter(ref int* refParam, out int* outParam)
        {
            // REPORTED false negative https://youtrack.jetbrains.com/issue/RSRP-427414:
            ReSharper.TestValueAnalysisUnsafe(refParam, refParam == null);

            refParam = null /*Expect:AssignNullToNotNullAttribute[MRef]*/;
            outParam = null /*Expect:AssignNullToNotNullAttribute[MOut]*/;
        }

        public static int* Function()
        {
            return null /*Expect:AssignNullToNotNullAttribute[MOut]*/;
        }
    }
}
