using System;
using JetBrains.Annotations;
using static ImplicitNullability.Samples.CodeWithIN.ReSharper;

namespace ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis
{
    public class GenericsSample
    {
        public class NoConstraint<T>
        {
            public void CallMethodWithCanBeNullArgument([CanBeNull] T a)
            {
                Method(a /*Expect:AssignNullToNotNullAttribute[MIn]*/);
            }

            public void CallMethodWithDefaultOfT()
            {
                Method(default(T) /*Expect:AssignNullToNotNullAttribute[RS >= 20161 && MIn]*/);
            }

            public void Method(T a)
            {
                TestValueAnalysis(a, ReferenceEquals(a, null) /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
            }

            public T Function([CanBeNull] T returnValue)
            {
                return returnValue /*Expect:AssignNullToNotNullAttribute[MOut]*/;
            }
        }

        public class ClassConstraint<T>
            where T : class
        {
            public void CallMethodWithDefaultOfT()
            {
                Method(default(T) /*Expect:AssignNullToNotNullAttribute[MIn]*/);
                Method(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);
            }

            public void Method(T a)
            {
                TestValueAnalysis(a, a == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
            }
        }

        public class StructConstraint<T>
            where T : struct
        {
            public void CallMethodWithDefaultOfT()
            {
                Method(default(T));
            }

            public void Method(T a)
            {
                TestValueAnalysis(a, ReferenceEquals(a, null) /*Expect:ConditionIsAlwaysTrueOrFalse*/);
            }

            public void MethodWithNullableParameter(T? a)
            {
                TestValueAnalysis(a /*Expect:AssignNullToNotNullAttribute*/, a == null);
            }
        }

        public class GenericMethods

        {
            public static void MethodWithoutConstraint<T>(T a)
            {
                TestValueAnalysis(a, ReferenceEquals(a, null) /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
            }
        }
    }
}