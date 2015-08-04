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

        public void MethodWithNullableIntRefAndOutParameterMethod(
            [NotNull] ref int? refParam /*Expect:NotNullOnImplicitCanBeNull*/,
            [NotNull] out int? outParam /*Expect:NotNullOnImplicitCanBeNull*/)
        {
            // REPORT? Warning, although explicitly NotNull
            ReSharper.TestValueAnalysis(refParam /*Expect:AssignNullToNotNullAttribute*/, refParam == null);

            outParam = null /*Expect:AssignNullToNotNullAttribute*/; // This warning results from the explicit NotNull
        }

        [NotNull]
        public int? FunctionWithNullableInt /*Expect:NotNullOnImplicitCanBeNull*/()
        {
            return null /*Expect:AssignNullToNotNullAttribute*/; // This warning results from the explicit NotNull
        }

        [NotNull]
        public delegate int? Delegate /*Expect:NotNullOnImplicitCanBeNull*/(
            [NotNull] int? a /*Expect:NotNullOnImplicitCanBeNull*/,
            [NotNull] ref int? refParam /*Expect:NotNullOnImplicitCanBeNull*/,
            [NotNull] out int? outParam /*Expect:NotNullOnImplicitCanBeNull*/);

        public class Operator
        {
            public static explicit operator Operator([NotNull] int? value /*Expect:NotNullOnImplicitCanBeNull*/)
            {
                return new Operator();
            }

            [NotNull]
            public static explicit operator /*Expect:NotNullOnImplicitCanBeNull*/ int?(Operator value)
            {
                return null /*Expect:AssignNullToNotNullAttribute*/; // This warning results from the explicit NotNull
            }
        }
    }
}