using System;

namespace ImplicitNullability.Sample.NullabilityAnalysis
{
    public class SampleGenericClassWithClassConstraint<T>
        where T : class
    {
        public void CallTestMethodWithDefaultOfT()
        {
            TestMethod(default(T) /*Expect:AssignNullToNotNullAttribute*/);
        }

        public void TestMethod(T a)
        {
            ReSharper.TestValueAnalysis(a, a == null /*Expect:ConditionIsAlwaysTrueOrFalse*/);
        }
    }
}