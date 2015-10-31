using JetBrains.Annotations;

namespace ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis
{
    public class NonPublicMethodsSample
    {
        [UsedImplicitly]
        internal void InternalMethod(string internalMethodParameter)
        {
            ReSharper.TestValueAnalysis(internalMethodParameter, internalMethodParameter == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
        }

        [UsedImplicitly]
        protected void ProtectedMethod(string protectedMethodParameter)
        {
            ReSharper.TestValueAnalysis(protectedMethodParameter, protectedMethodParameter == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
        }

        [UsedImplicitly]
        private void PrivateMethod(string privateMethodParameter)
        {
            ReSharper.TestValueAnalysis(privateMethodParameter, privateMethodParameter == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
        }

        [UsedImplicitly]
        internal string InternalFunction()
        {
            return null; /*Expect:AssignNullToNotNullAttribute[MOut]*/
        }

        [UsedImplicitly]
        protected string ProtectedFunction()
        {
            return null; /*Expect:AssignNullToNotNullAttribute[MOut]*/
        }

        [UsedImplicitly]
        private string PrivateFunction()
        {
            return null; /*Expect:AssignNullToNotNullAttribute[MOut]*/
        }
    }
}