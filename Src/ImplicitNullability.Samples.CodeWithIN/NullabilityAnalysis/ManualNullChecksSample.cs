// Exclude[RS < 20171]

using System;
using JetBrains.Annotations;
using static ImplicitNullability.Samples.CodeWithIN.ReSharper;

namespace ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis
{
    public static class ManualNullChecksSample
    {
        public abstract class Base
        {
            public abstract void PreconditionExpressionCheckWithNotNullInBase([NotNull] string a);
        }

        public class Derived : Base
        {
            public void PreconditionIfCheck(string a)
            {
                if (a == null /*Expect no warning*/)
                    throw new ArgumentException(nameof(a));

                TestValueAnalysis(a, a == null /*Expect:ConditionIsAlwaysTrueOrFalse*/);
            }

            //

            public void PreconditionExpressionCheckWithExplicitNotNull([NotNull] string a)
            {
                a = a ?? throw new ArgumentException(nameof(a));
                TestValueAnalysis(a, a == null /*Expect:ConditionIsAlwaysTrueOrFalse*/);
            }

            public void PreconditionExpressionCheck(string a)
            {
                // REPORTED false positive https://youtrack.jetbrains.com/issue/RSRP-464760
                a = a ?? throw new ArgumentException(nameof(a)) /*Expect:ConstantNullCoalescingCondition[MIn]*/;
                TestValueAnalysis(a, a == null /*Expect:ConditionIsAlwaysTrueOrFalse*/);
            }

            public override void PreconditionExpressionCheckWithNotNullInBase(string a)
            {
                a = a ?? throw new ArgumentException(nameof(a)) /*Expect:ConstantNullCoalescingCondition*/;
                TestValueAnalysis(a, a == null /*Expect:ConditionIsAlwaysTrueOrFalse*/);
            }
        }
    }
}
