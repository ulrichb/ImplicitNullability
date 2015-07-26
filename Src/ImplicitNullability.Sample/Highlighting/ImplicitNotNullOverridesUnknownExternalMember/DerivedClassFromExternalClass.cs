using System;
using ImplicitNullability.Sample.ExternalCode;

namespace ImplicitNullability.Sample.Highlighting.ImplicitNotNullOverridesUnknownExternalMember
{
    public class DerivedClassFromExternalClass
    {
        public class DerivedClass : External.Class
        {
            public override string this[string a /*Expect:ImplicitNotNullOverridesUnknownExternalMember*/]
            {
                get { return null; }
            }

            public override void SomeMethod(
                string a /*Expect:ImplicitNotNullOverridesUnknownExternalMember*/,
                string b /*Expect:ImplicitNotNullOverridesUnknownExternalMember*/)
            {
            }
        }
    }
}