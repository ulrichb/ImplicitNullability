using System;
using JetBrains.Annotations;


namespace ImplicitNullability.Sample.Highlighting.NotNullOnImplicitCanBeNull
{
    public class NotNullOnImplicitCanBeNullSample
    {
        public void MethodWithNullableInt([NotNull] int? nullableInt /*Expect:NotNullOnImplicitCanBeNull*/)
        {
            // REPORT? Warning, although explicitly NotNull
            ReSharper.TestValueAnalysis(nullableInt /*Expect:AssignNullToNotNullAttribute*/, nullableInt == null);
        }

        public void MethodWithOptionalParameter(
            // ReSharper disable AssignNullToNotNullAttribute - because in R# 9+ this hides NotNullOnImplicitCanBeNull
            [NotNull] string optional = null /*Expect:NotNullOnImplicitCanBeNull*/)
            // ReSharper restore AssignNullToNotNullAttribute
        {
            // REPORT? Warning, although explicitly NotNull
            ReSharper.TestValueAnalysis(optional /*Expect:AssignNullToNotNullAttribute*/, optional == null);
        }

        public string this[
            [NotNull] int? nullableInt /*Expect:NotNullOnImplicitCanBeNull*/,
            // ReSharper disable AssignNullToNotNullAttribute - because in R# 9+ this hides NotNullOnImplicitCanBeNull
            [NotNull] string optional = null /*Expect:NotNullOnImplicitCanBeNull*/]
            // ReSharper restore AssignNullToNotNullAttribute
        {
            get { return null; }
        }
    }
}