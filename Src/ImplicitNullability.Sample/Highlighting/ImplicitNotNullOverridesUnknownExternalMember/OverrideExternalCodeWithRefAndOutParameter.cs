using System;
using ImplicitNullability.Sample.ExternalCode;

namespace ImplicitNullability.Sample.Highlighting.ImplicitNotNullOverridesUnknownExternalMember
{
    public class OverrideExternalCodeWithRefAndOutParameter
    {
        public class Implementation : External.IInterfaceWithRefAndOutParameterMethod
        {
            public void Method(
                ref string refParam /*Expect:ImplicitNotNullOverridesUnknownExternalMember*/,
                out string outParam)
            {
                ReSharper.SuppressUnusedWarning(refParam);
                refParam = "";
                outParam = "";
            }
        }
    }
}