using System.Threading.Tasks;
using JetBrains.Annotations;
namespace ImplicitNullability.Samples.CodeWithIN.Highlighting.NotNullOnImplicitCanBeNull
{
    public class NotNullOnImplicitCanBeNullSample
    {
        public void MethodWithNullableInt([NotNull] int? nullableInt /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/)
        {
            // REPORT? Warning, although explicitly NotNull
            ReSharper.TestValueAnalysis(nullableInt /*Expect:AssignNullToNotNullAttribute*/, nullableInt == null);
        }

        public void MethodWithOptionalParameter(
            // ReSharper disable AssignNullToNotNullAttribute - because in R# 9+ this hides NotNullOnImplicitCanBeNull
            [NotNull] string optional = null /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/)
            // ReSharper restore AssignNullToNotNullAttribute
        {
            // REPORT? Warning, although explicitly NotNull
            ReSharper.TestValueAnalysis(optional /*Expect:AssignNullToNotNullAttribute*/, optional == null);
        }

        public string this[
            [NotNull] int? nullableInt /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/,
            // ReSharper disable AssignNullToNotNullAttribute - because in R# 9+ this hides NotNullOnImplicitCanBeNull
            [NotNull] string optional = null /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/]
            // ReSharper restore AssignNullToNotNullAttribute
        {
            get { return null; }
        }

        public void MethodWithNullableIntRefAndOutParameterMethod(
            [NotNull] ref int? refParam /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/,
            [NotNull] out int? outParam /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/)
        {
            // REPORT? Warning, although explicitly NotNull
            ReSharper.TestValueAnalysis(refParam /*Expect:AssignNullToNotNullAttribute*/, refParam == null);

            outParam = null /*Expect:AssignNullToNotNullAttribute*/; // This warning results from the explicit NotNull
        }

        [NotNull]
        public int? FunctionWithNullableInt /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/()
        {
            return null /*Expect:AssignNullToNotNullAttribute*/; // This warning results from the explicit NotNull
        }

        [ItemNotNull]
        public async Task<int?> AsyncFunctionWithNullableInt /*Expect:NotNullOnImplicitCanBeNull[RS >= 92 && Implicit]*/()
        {
            await Task.Delay(0);
            return null /*Expect:AssignNullToNotNullAttribute[RS >= 92]*/; // This warning results from the explicit NotNull
        }

        [NotNull]
        public delegate int? Delegate /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/(
            [NotNull] int? a /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/,
            [NotNull] ref int? refParam /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/,
            [NotNull] out int? outParam /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/);

        public class Operator
        {
            public static explicit operator Operator([NotNull] int? value /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/)
            {
                return new Operator();
            }

            [NotNull]
            public static explicit operator /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/ int?(Operator value)
            {
                return null /*Expect:AssignNullToNotNullAttribute*/; // This warning results from the explicit NotNull
            }
        }
    }
}