using System;
using JetBrains.Annotations;
using static ImplicitNullability.Samples.CodeWithIN.ReSharper;

namespace ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis
{
    public static class OperatorOverloadsSample
    {
        public class Simple
        {
            public static int operator +(Simple left, Simple right)
            {
                TestValueAnalysis(left, left == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
                TestValueAnalysis(right, right == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
                return 0;
            }

            public static Simple operator ++(Simple value)
            {
                TestValueAnalysis(value, value == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
                return new Simple();
            }
        }

        public class CanBeNull
        {
            public static int operator +([CanBeNull] CanBeNull left, [CanBeNull] CanBeNull right)
            {
                TestValueAnalysis(left /*Expect:AssignNullToNotNullAttribute*/, left == null);
                TestValueAnalysis(right /*Expect:AssignNullToNotNullAttribute*/, right == null);
                return 0;
            }

            [CanBeNull]
            public static CanBeNull operator ++([CanBeNull] CanBeNull value)
            {
                TestValueAnalysis(value /*Expect:AssignNullToNotNullAttribute*/, value == null);
                return null;
            }
        }

        public class NotNullReturnValue
        {
            public static NotNullReturnValue operator ++(NotNullReturnValue value)
            {
                return null /*Expect:AssignNullToNotNullAttribute[MOut]*/;
            }
        }
    }
}