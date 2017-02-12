using ImplicitNullability.Samples.CodeWithoutIN;
using JetBrains.Annotations;

namespace ImplicitNullability.Samples.CodeWithIN.Highlighting.ImplicitNotNullOverridesUnknownBaseMemberNullability
{
    public class OverrideExternalCodeWithRefAndOutParameter
    {
        public class Implementation : External.IInterfaceWithRefAndOutParameterMethod
        {
            public void Method(
                ref string refParam /*Expect:ImplicitNotNullOverridesUnknownBaseMemberNullability[Implicit]*/,
                out string outParam /*Expect:ImplicitNotNullResultOverridesUnknownBaseMemberNullability[Implicit]*/)
            {
                outParam = "";
            }
        }

        public class ImplementationWithExplicitNotNull : External.IInterfaceWithRefAndOutParameterMethod
        {
            public void Method([NotNull] ref string refParam /*Expect no warning*/, [NotNull] out string outParam /*Expect no warning*/)
            {
                outParam = "";
            }
        }
    }
}
