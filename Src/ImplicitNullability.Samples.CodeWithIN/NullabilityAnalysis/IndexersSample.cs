using JetBrains.Annotations;
using static ImplicitNullability.Samples.CodeWithIN.ReSharper;

// ReSharper disable ArrangeAccessorOwnerBody

namespace ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis
{
    public class IndexersSample
    {
        public string this[string a]
        {
            get { return a == "<return null>" ? null : a /*Expect:AssignNullToNotNullAttribute[Prps && !RtGo]*/; }
            set
            {
                TestValueAnalysis(value, value == null /*Expect:ConditionIsAlwaysTrueOrFalse[Prps && !RtGo]*/);

                TestValueAnalysis(a, a == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
            }
        }

        // Getter-only:
        public string this[int i]
        {
            get { return null /*Expect:AssignNullToNotNullAttribute[Prps]*/; }
        }

        [CanBeNull]
        public string this[[CanBeNull] string canBeNull, int? nullableInt, string optional = null]
        {
            get { return canBeNull; }
            set
            {
                TestValueAnalysis(value /*Expect:AssignNullToNotNullAttribute*/, value == null);

                TestValueAnalysis(nullableInt /*Expect:AssignNullToNotNullAttribute*/, nullableInt == null);
                TestValueAnalysis(optional /*Expect:AssignNullToNotNullAttribute*/, optional == null);
            }
        }
    }
}
