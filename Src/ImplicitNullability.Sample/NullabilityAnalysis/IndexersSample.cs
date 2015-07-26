using System;
using JetBrains.Annotations;

namespace ImplicitNullability.Sample.NullabilityAnalysis
{
    public class IndexersSample
    {
        public string this[string a]
        {
            get { return null; }
            set
            {
                ReSharper.TestValueAnalysis(a, a == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
                ReSharper.TestValueAnalysis(value, value == null);
            }
        }

        public string this[[CanBeNull] string canBeNull, int? nullableInt, string optional = null]
        {
            get { return null; }
            set
            {
                ReSharper.TestValueAnalysis(nullableInt /*Expect:AssignNullToNotNullAttribute*/, nullableInt == null);
                ReSharper.TestValueAnalysis(optional /*Expect:AssignNullToNotNullAttribute*/, optional == null);
            }
        }
    }
}