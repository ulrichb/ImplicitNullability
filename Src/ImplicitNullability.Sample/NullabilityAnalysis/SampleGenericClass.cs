using System;

namespace ImplicitNullability.Sample.NullabilityAnalysis
{
    public class SampleGenericClass<T>
    {
        public void CallTestMethodWithDefaultOfT()
        {
            TestMethod(default(T)); // FALSE NEGATIVE for argument b, if T is a reference type
        }

        public void TestMethod(T a)
        {
            ReSharper.TestValueAnalysis(a, ReferenceEquals(a, null) /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
        }
    }
}