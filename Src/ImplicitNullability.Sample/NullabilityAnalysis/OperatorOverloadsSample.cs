using System;
using JetBrains.Annotations;

namespace ImplicitNullability.Sample.NullabilityAnalysis
{
    public static class OperatorOverloadsSample
    {
        public class Simple
        {
            public static int operator +(Simple left, Simple right)
            {
                ReSharper.TestValueAnalysis(left, left == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
                ReSharper.TestValueAnalysis(right, right == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
                return 0;
            }

            public static Simple operator ++(Simple value)
            {
                ReSharper.TestValueAnalysis(value, value == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
                return new Simple();
            }
        }

        public class CanBeNull
        {
            public static int operator +([CanBeNull] CanBeNull left, [CanBeNull] CanBeNull right)
            {
                ReSharper.TestValueAnalysis(left /*Expect:AssignNullToNotNullAttribute*/, left == null);
                ReSharper.TestValueAnalysis(right /*Expect:AssignNullToNotNullAttribute*/, right == null);
                return 0;
            }

            [CanBeNull]
            public static CanBeNull operator ++([CanBeNull] CanBeNull value)
            {
                ReSharper.TestValueAnalysis(value /*Expect:AssignNullToNotNullAttribute*/, value == null);
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