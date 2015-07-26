using System;

namespace ImplicitNullability.Sample.NullabilityAnalysis
{
    public class SampleGenericClassWithStructConstraint<T>
        where T : struct
    {
        public void CallTestMethodWithDefaultOfT()
        {
            TestMethod(default(T));
        }

        public void TestMethod(T a)
        {
        }

        public void TestMethodWithNullableParameter(T? a)
        {
            ReSharper.TestValueAnalysis(a /*Expect:AssignNullToNotNullAttribute*/, a == null);
        }
    }
}