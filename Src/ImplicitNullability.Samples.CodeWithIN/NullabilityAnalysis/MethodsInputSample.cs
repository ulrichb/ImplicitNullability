using System;
using JetBrains.Annotations;

namespace ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis
{
    public class MethodsInputSample
    {
        private const string StringConst = "some string";
        private const string NullStringConst = null;
        private const string DefaultOfStringConst = default(string);

        public void Method(string a)
        {
            ReSharper.TestValueAnalysis(a, a == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
        }

        public void MethodWithExplicitCanBeNull([CanBeNull] string a)
        {
        }

        public void MethodWithImplicitCanBeNullParameters(int? nullableInt, string optional = null)
        {
            ReSharper.TestValueAnalysis(nullableInt /*Expect:AssignNullToNotNullAttribute*/, nullableInt == null);
            ReSharper.TestValueAnalysis(optional /*Expect:AssignNullToNotNullAttribute*/, optional == null);
        }

        public void MethodWithNullDefaultOfStringDefaultArgument(string optional = default(string))
        {
            ReSharper.TestValueAnalysis(optional /*Expect:AssignNullToNotNullAttribute*/, optional == null);
        }

        public void MethodWithNullDefaultArgumentFromConst(string optional = NullStringConst)
        {
            ReSharper.TestValueAnalysis(optional /*Expect:AssignNullToNotNullAttribute*/, optional == null);
        }

        public void MethodWithNullDefaultArgumentFromDefaultOfStringConst(string optional = DefaultOfStringConst)
        {
            ReSharper.TestValueAnalysis(optional /*Expect:AssignNullToNotNullAttribute*/, optional == null);
        }

        public void MethodWithNonNullDefaultArgument(string optional = "default")
        {
            ReSharper.TestValueAnalysis(optional, optional == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
        }

        public void MethodWithNonNullDefaultArgumentFromConst(string optional = StringConst)
        {
            ReSharper.TestValueAnalysis(optional, optional == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
        }

        public void MethodWithParams(params string[] a)
        {
            ReSharper.TestValueAnalysis(a, a == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
        }

        public static void StaticMethod(string a)
        {
            ReSharper.TestValueAnalysis(a, a == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
        }
    }
}