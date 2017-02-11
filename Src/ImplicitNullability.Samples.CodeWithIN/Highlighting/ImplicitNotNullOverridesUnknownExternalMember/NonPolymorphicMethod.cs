using ImplicitNullability.Samples.CodeWithoutIN;

namespace ImplicitNullability.Samples.CodeWithIN.Highlighting.ImplicitNotNullOverridesUnknownExternalMember
{
    public class NonPolymorphicMethod
    {
        public abstract class MethodHiding : External.Class
        {
            public new void Method(string a /*Expect no warning*/)
            {
            }

            public new string Function /*Expect no warning*/()
            {
                return base.Function();
            }
        }
    }
}
