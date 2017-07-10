using JetBrains.Annotations;
using static ImplicitNullability.Samples.CodeWithIN.ReSharper;

namespace ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis
{
    public class NonPublicMembersSample
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

        //

        [UsedImplicitly]
        internal string InternalFunction() => null /*Expect:AssignNullToNotNullAttribute[MOut]*/;

        [UsedImplicitly]
        protected string ProtectedFunction() => null /*Expect:AssignNullToNotNullAttribute[MOut]*/;

        [UsedImplicitly]
        private string PrivateFunction() => null /*Expect:AssignNullToNotNullAttribute[MOut]*/;

        //

        // ReSharper disable RedundantDefaultMemberInitializer
        [UsedImplicitly]
        internal readonly string InternalField = null /*Expect:AssignNullToNotNullAttribute[Flds]*/;

        [UsedImplicitly]
        protected readonly string ProtectedField = null /*Expect:AssignNullToNotNullAttribute[Flds]*/;

#pragma warning disable 414
        [UsedImplicitly]
        private readonly string _privateField = null /*Expect:AssignNullToNotNullAttribute[Flds]*/;
#pragma warning restore 414
        // ReSharper restore RedundantDefaultMemberInitializer
    }
}
