using JetBrains.Annotations;
using static ImplicitNullability.Samples.CodeWithIN.ReSharper;

namespace ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis
{
    public class NonPublicMethodsSample
    {
        [UsedImplicitly]
        internal void InternalMethod(string internalMethodParameter)
        {
            TestValueAnalysis(internalMethodParameter, internalMethodParameter == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
        }

        [UsedImplicitly]
        protected void ProtectedMethod(string protectedMethodParameter)
        {
            TestValueAnalysis(protectedMethodParameter, protectedMethodParameter == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
        }

        [UsedImplicitly]
        private void PrivateMethod(string privateMethodParameter)
        {
            TestValueAnalysis(privateMethodParameter, privateMethodParameter == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
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