using System;
using JetBrains.Annotations;
using static ImplicitNullability.Samples.CodeWithIN.ReSharper;

namespace ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis
{
    public class IndexersSample
    {
        public string this[string a]
        {
            get { return null; }
            set
            {
                TestValueAnalysis(a, a == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
                TestValueAnalysis(value, value == null);
            }
        }

        public string this[[CanBeNull] string canBeNull, int? nullableInt, string optional = null]
        {
            get { return null; }
            set
            {
                SuppressUnusedWarning(value);
                TestValueAnalysis(nullableInt /*Expect:AssignNullToNotNullAttribute*/, nullableInt == null);
                TestValueAnalysis(optional /*Expect:AssignNullToNotNullAttribute*/, optional == null);
            }
        }
    }
}
