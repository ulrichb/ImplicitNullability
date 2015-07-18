using System;
using ImplicitNullability.Sample.ExternalCode;

namespace ImplicitNullability.Sample.Highlighting.ImplicitNotNullOverridesUnknownExternalMember
{
    public class NonPolymorphicMethod
    {
        public abstract class MethodHiding : External.Class
        {
            public new void SomeMethod(string a, string b)
            {
            }
        }
    }
}