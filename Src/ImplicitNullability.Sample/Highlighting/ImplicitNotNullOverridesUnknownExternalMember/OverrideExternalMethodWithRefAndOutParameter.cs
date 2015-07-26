using System;
using ImplicitNullability.Sample.ExternalCode;

namespace ImplicitNullability.Sample.Highlighting.ImplicitNotNullOverridesUnknownExternalMember
{
    public class OverrideExternalMethodWithRefAndOutParameter
    {
        public class Implementation : External.IInterfaceWithRefAndOutParameter
        {
            public void SomeMethod(
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