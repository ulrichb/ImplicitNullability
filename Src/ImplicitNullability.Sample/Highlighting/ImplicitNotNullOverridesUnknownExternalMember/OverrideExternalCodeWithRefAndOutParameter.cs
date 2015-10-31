using ImplicitNullability.Sample.ExternalCode;
using JetBrains.Annotations;

namespace ImplicitNullability.Sample.Highlighting.ImplicitNotNullOverridesUnknownExternalMember
{
    public class OverrideExternalCodeWithRefAndOutParameter
    {
        public class Implementation : External.IInterfaceWithRefAndOutParameterMethod
        {
            public void Method(
                ref string refParam /*Expect:ImplicitNotNullOverridesUnknownExternalMember[Implicit]*/,
                out string outParam /*Expect:ImplicitNotNullResultOverridesUnknownExternalMember[Implicit]*/)
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