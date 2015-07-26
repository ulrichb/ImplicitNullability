using System;

namespace ImplicitNullability.Sample.NullabilityAnalysis
{
    public class GenericsSample
    {
        public class NoConstraint<T>
        {
            public void CallMethodWithDefaultOfT()
            {
                Method(default(T)); // FALSE NEGATIVE for argument b, if T is a reference type
            }

            public void Method(T a)
            {
                ReSharper.TestValueAnalysis(a, ReferenceEquals(a, null) /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
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
                ReSharper.TestValueAnalysis(a, a == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
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
                ReSharper.TestValueAnalysis(a, ReferenceEquals(a, null) /*Expect:ConditionIsAlwaysTrueOrFalse*/);
            }

            public void MethodWithNullableParameter(T? a)
            {
                ReSharper.TestValueAnalysis(a /*Expect:AssignNullToNotNullAttribute*/, a == null);
            }
        }

        public class GenericMethods

        {
            public static void MethodWithoutConstraint<T>(T a)
            {
                ReSharper.TestValueAnalysis(a, ReferenceEquals(a, null) /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
            }
        }
    }
}