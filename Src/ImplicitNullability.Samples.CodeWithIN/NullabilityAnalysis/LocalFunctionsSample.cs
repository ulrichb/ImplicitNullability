// Exclude[RS < 20163]

namespace ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis
{
    public class LocalFunctionsSample
    {
        // Proves that local functions and their parameters aren't implicitly not null.

        public void Method()
        {
            string LocalFunction(string a)
            {
                ReSharper.TestValueAnalysis(a, a == null /*Expect no warning*/);
                return null /*Expect no warning*/;
            }

            var result = LocalFunction(null /*Expect no warning*/);
            ReSharper.TestValueAnalysis(result, result == null /*Expect no warning*/);
        }
    }
}
