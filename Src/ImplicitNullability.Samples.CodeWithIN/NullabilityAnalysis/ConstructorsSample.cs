using System;
using JetBrains.Annotations;
using static ImplicitNullability.Samples.CodeWithIN.ReSharper;

namespace ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis
{
    public class ConstructorsSample
    {
        public ConstructorsSample(string a, string optional = "default")
        {
            TestValueAnalysis(a, a == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
            TestValueAnalysis(optional, optional == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
        }

        public ConstructorsSample([CanBeNull] string canBeNull, int? nullableInt, string optional = null)
        {
            SuppressUnusedWarning(canBeNull);
            TestValueAnalysis(nullableInt /*Expect:AssignNullToNotNullAttribute*/, nullableInt == null);
            TestValueAnalysis(optional /*Expect:AssignNullToNotNullAttribute*/, optional == null);
        }
    }
}
