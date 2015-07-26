using System;
using JetBrains.Annotations;

namespace ImplicitNullability.Sample.NullabilityAnalysis
{
    public class ConstructorsSample
    {
        public ConstructorsSample(string a, string optional = "default")
        {
            ReSharper.TestValueAnalysis(a, a == null /*Expect:ConditionIsAlwaysTrueOrFalse*/);
            ReSharper.TestValueAnalysis(optional, optional == null /*Expect:ConditionIsAlwaysTrueOrFalse*/);
        }

        public ConstructorsSample([CanBeNull] string canBeNull, int? nullableInt, string optional = null)
        {
            ReSharper.TestValueAnalysis(nullableInt /*Expect:AssignNullToNotNullAttribute*/, nullableInt == null);
            ReSharper.TestValueAnalysis(optional /*Expect:AssignNullToNotNullAttribute*/, optional == null);
        }
    }
}