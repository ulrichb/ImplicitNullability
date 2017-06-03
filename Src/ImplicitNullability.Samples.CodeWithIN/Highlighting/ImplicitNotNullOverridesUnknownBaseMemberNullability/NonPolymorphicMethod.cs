using ImplicitNullability.Samples.CodeWithoutIN;

namespace ImplicitNullability.Samples.CodeWithIN.Highlighting.ImplicitNotNullOverridesUnknownBaseMemberNullability
{
    public class NonPolymorphicMethod
    {
        public abstract class MethodHiding : External.Class
        {
            public new void Method(string a /*Expect no warning*/)
            {
            }

            public new string Function /*Expect no warning*/() => "";

            public new string Property /*Expect no warning*/ { get; set; } = "";

            public new string this /*Expect no warning*/[string a]
            {
                get { return ""; }
                // ReSharper disable once ValueParameterNotUsed
                set { }
            }
        }
    }
}
