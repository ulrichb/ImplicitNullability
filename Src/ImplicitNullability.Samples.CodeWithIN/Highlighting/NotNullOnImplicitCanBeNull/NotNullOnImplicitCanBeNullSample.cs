using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ImplicitNullability.Samples.CodeWithIN.Highlighting.NotNullOnImplicitCanBeNull
{
    public class NotNullOnImplicitCanBeNullSample
    {
        public void MethodWithNullableInt([NotNull] int? nullableInt /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/)
        {
            // R# ignores the [NotNull] here, but respects it at the call site.

            ReSharper.TestValueAnalysis(nullableInt /*Expect:AssignNullToNotNullAttribute*/, nullableInt == null);
        }

        public void MethodWithOptionalParameter(
            // In R# 9+ AssignNullToNotNullAttribute hides NotNullOnImplicitCanBeNull, which is OK:
            [NotNull] string optional = null /*Expect:AssignNullToNotNullAttribute*/)
        {
            // R# ignores the [NotNull] here, but respects it at the call site.

            ReSharper.TestValueAnalysis(optional /*Expect:AssignNullToNotNullAttribute*/, optional == null);
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
        public async Task<int?> AsyncFunctionWithNullableInt /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/()
        {
            return await Async.CanBeNullResult<int?>() /*Expect:AssignNullToNotNullAttribute*/; // This warning results from the explicit NotNull
        }

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

        [NotNull]
        public delegate int? Delegate /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/(
            [NotNull] int? a /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/,
            [NotNull] ref int? refParam /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/,
            [NotNull] out int? outParam /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/);

        [NotNull]
        public readonly int? Field /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/ = null /*Expect:AssignNullToNotNullAttribute*/;

        [NotNull]
        public int? Property /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/ { get; set; } = null /*Expect:AssignNullToNotNullAttribute*/;

        [NotNull]
        public int? this /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/[
            [NotNull] int? nullableInt /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/,
            //
            // In R# 9+ AssignNullToNotNullAttribute hides NotNullOnImplicitCanBeNull, which is OK:
            [NotNull] string optional = null /*Expect:AssignNullToNotNullAttribute*/] => null /*Expect:AssignNullToNotNullAttribute*/;
    }
}
