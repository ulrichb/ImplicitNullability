using System;
using JetBrains.Annotations;

namespace ImplicitNullability.Sample.NonImplicit.NullabilityAnalysis
{
    public class NonImplicitNullabilitySettingsSample
    {
        // This sample ensures that the nullability settings work on project level (.csproj.DotSettings)

        public void MethodInput(string a)
        {
            ReSharper.TestValueAnalysis(a, a == null);
        }

        public void MethodInputWithNotNull([NotNull] string a)
        {
            ReSharper.TestValueAnalysis(a, a == null /*Expect:ConditionIsAlwaysTrueOrFalse*/);
        }

        public void MethodRefParameter(ref string a)
        {
            ReSharper.SuppressUnusedWarning(a);
            a = null;
        }

        public void MethodRefParameterWithNotNull([NotNull] ref string a)
        {
            ReSharper.SuppressUnusedWarning(a);
            a = null /*Expect:AssignNullToNotNullAttribute*/;
        }

        public string MethodOutput()
        {
            return null;
        }

        [NotNull]
        public string MethodOutputWithNotNull()
        {
            return null /*Expect:AssignNullToNotNullAttribute*/;
        }
    }
}