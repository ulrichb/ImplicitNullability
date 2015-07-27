using System;
using JetBrains.Annotations;

namespace ImplicitNullability.Sample.NullabilityAnalysis
{
    public class MethodsInputSample
    {
        private const string StringConst = "some string";
        private const string NullStringConst = null;
        private const string DefaultOfStringConst = default(string);

        public void TestMethod(string a)
        {
            ReSharper.TestValueAnalysis(a, a == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
        }

        public void TestMethodWithExplicitCanBeNull([CanBeNull] string canBeNull)
        {
        }

        public void TestMethodWithImplicitCanBeNull(int? nullableInt, string optional = null)
        {
            ReSharper.TestValueAnalysis(nullableInt /*Expect:AssignNullToNotNullAttribute*/, nullableInt == null);
            ReSharper.TestValueAnalysis(optional /*Expect:AssignNullToNotNullAttribute*/, optional == null);
        }

        public void TestMethodWithRefParameter(ref string refString)
        {
            ReSharper.TestValueAnalysis(refString, refString == null /* REPORTED false negative https://youtrack.jetbrains.com/issue/RSRP-427414 */);

            // Note that the implicit not null argument applies also to the outgoing value of 'refString'
            refString = null /*Expect:AssignNullToNotNullAttribute[MIn]*/;
        }

        public void TestMethodWithExplicitNotNullRefParameter([NotNull] ref string refString)
        {
            ReSharper.TestValueAnalysis(refString, refString == null /* REPORTED false negative https://youtrack.jetbrains.com/issue/RSRP-427414 */);
            refString = null /*Expect:AssignNullToNotNullAttribute*/;
        }

        public void TestMethodWithCanBeNullRefParameter([CanBeNull] ref string refString)
        {
            ReSharper.TestValueAnalysis(refString /* REPORTED false negative https://youtrack.jetbrains.com/issue/RSRP-427414 */, refString == null);
            refString = null;
        }

        public void TestMethodWithNullDefaultOfStringDefaultArgument(string optional = default(string))
        {
            ReSharper.TestValueAnalysis(optional /*Expect:AssignNullToNotNullAttribute*/, optional == null);
        }

        public void TestMethodWithNullDefaultArgumentFromConst(string optional = NullStringConst)
        {
            ReSharper.TestValueAnalysis(optional /*Expect:AssignNullToNotNullAttribute*/, optional == null);
        }

        public void TestMethodWithNullDefaultArgumentFromDefaultOfStringConst(string optional = DefaultOfStringConst)
        {
            ReSharper.TestValueAnalysis(optional /*Expect:AssignNullToNotNullAttribute*/, optional == null);
        }

        public void TestMethodWithNonNullDefaultArgument(string optional = "default")
        {
            ReSharper.TestValueAnalysis(optional, optional == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
        }

        public void TestMethodWithNonNullDefaultArgumentFromConst(string optional = StringConst)
        {
            ReSharper.TestValueAnalysis(optional, optional == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
        }

        public void TestMethodWithParams(params string[] a)
        {
            ReSharper.TestValueAnalysis(a, a == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
        }

        public static void StaticTestMethod(string a)
        {
            ReSharper.TestValueAnalysis(a, a == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
        }
    }
}