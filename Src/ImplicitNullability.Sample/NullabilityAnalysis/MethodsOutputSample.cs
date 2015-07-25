using JetBrains.Annotations;

namespace ImplicitNullability.Sample.NullabilityAnalysis
{
    public class MethodsOutputSample
    {
        public string Function([CanBeNull] string returnValue)
        {
            return returnValue; /*Expect:AssignNullToNotNullAttribute[MOut]*/
        }

        [CanBeNull]
        public string FunctionWithCanBeNull([CanBeNull] string returnValue)
        {
            return returnValue;
        }

        public int? FunctionWithNullableInt(int? returnValue)
        {
            return returnValue;
        }

        public void MethodWithOutParameter(out string outString)
        {
            outString = null; // TODO
        }
    }
}