using System;
using JetBrains.Annotations;

namespace ImplicitNullability.Sample.NullabilityAnalysis
{
    public unsafe class UnsafeCodeSample
    {
        public static void SomeMethod(int* a)
        {
            ReSharper.TestValueAnalysisUnsafe(a, a == null /*Expect:ConditionIsAlwaysTrueOrFalse*/);
        }

        public static void SomeMethodWithCanbeNull([CanBeNull] int* a)
        {
            ReSharper.TestValueAnalysisUnsafe(a /*Expect:AssignNullToNotNullAttribute*/, a == null);
        }
    }
}