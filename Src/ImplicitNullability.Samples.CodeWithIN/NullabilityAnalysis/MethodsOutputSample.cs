using JetBrains.Annotations;

namespace ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis
{
    public class MethodsOutputSample
    {
        public string Function([CanBeNull] string returnValue)
        {
            return returnValue /*Expect:AssignNullToNotNullAttribute[MOut]*/;
        }

        [CanBeNull]
        public string FunctionWithCanBeNull([CanBeNull] string returnValue)
        {
            return returnValue;
        }

        public int? FunctionWithNullableInt(int? returnValue, out int? outParam)
        {
            outParam = returnValue;
            return returnValue;
        }

        public void MethodWithOutParameter(out string outParam)
        {
            outParam = null /*Expect:AssignNullToNotNullAttribute[MOut]*/;
        }

        public void MethodWithCanBeNullOutParameter([CanBeNull] out string outParam1)
        {
            outParam1 = null;
        }
    }
}
