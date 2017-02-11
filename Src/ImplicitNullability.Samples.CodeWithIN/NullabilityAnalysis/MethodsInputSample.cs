using System;
using JetBrains.Annotations;
using static ImplicitNullability.Samples.CodeWithIN.ReSharper;

namespace ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis
{
    public class MethodsInputSample
    {
        private const string StringConst = "some string";
        private const string NullStringConst = null;
        private const string DefaultOfStringConst = default(string);

        public void Method(string a)
        {
            TestValueAnalysis(a, a == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
        }

        public void MethodWithExplicitCanBeNull([CanBeNull] string a)
        {
        }

        public void MethodWithImplicitCanBeNullParameters(int? nullableInt, string optional = null)
        {
            TestValueAnalysis(nullableInt /*Expect:AssignNullToNotNullAttribute*/, nullableInt == null);
            TestValueAnalysis(optional /*Expect:AssignNullToNotNullAttribute*/, optional == null);
        }

        public void MethodWithNullDefaultOfStringDefaultArgument(string optional = default(string))
        {
            TestValueAnalysis(optional /*Expect:AssignNullToNotNullAttribute*/, optional == null);
        }

        public void MethodWithNullDefaultArgumentFromConst(string optional = NullStringConst)
        {
            TestValueAnalysis(optional /*Expect:AssignNullToNotNullAttribute*/, optional == null);
        }

        public void MethodWithNullDefaultArgumentFromDefaultOfStringConst(string optional = DefaultOfStringConst)
        {
            TestValueAnalysis(optional /*Expect:AssignNullToNotNullAttribute*/, optional == null);
        }

        public void MethodWithNonNullDefaultArgument(string optional = "default")
        {
            TestValueAnalysis(optional, optional == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
        }

        public void MethodWithNonNullDefaultArgumentFromConst(string optional = StringConst)
        {
            TestValueAnalysis(optional, optional == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
        }

        public void MethodWithParams(params string[] a)
        {
            TestValueAnalysis(a, a == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
        }

        public static void StaticMethod(string a)
        {
            TestValueAnalysis(a, a == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
        }
    }
}
