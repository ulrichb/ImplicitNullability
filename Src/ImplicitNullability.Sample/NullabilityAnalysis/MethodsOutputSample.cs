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
        public string FunctionWithCanBeNullResult([CanBeNull] string returnValue)
        {
            return returnValue;
        }

        public void MethodWithOutParameter(out string outString)
        {
            outString = null; // TODO
        }
    }
}