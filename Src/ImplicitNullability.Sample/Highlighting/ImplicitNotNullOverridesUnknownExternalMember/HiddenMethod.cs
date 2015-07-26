using System;
using ImplicitNullability.Sample.ExternalCode;

namespace ImplicitNullability.Sample.Highlighting.ImplicitNotNullOverridesUnknownExternalMember
{
    public class HiddenMethod
    {
        private abstract class Implementation : External.Class
        {
            public new void SomeMethod(string a, string b) /* no warning because the contract of the base method hasn't changed (no polymorphism) */
            {
            }
        }
    }
}