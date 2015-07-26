using System;
using ImplicitNullability.Sample.ExternalCode;

namespace ImplicitNullability.Sample.Highlighting.ImplicitNotNullOverridesUnknownExternalMember
{
    public class OverrideExternalMethodWithVariousTypes
    {
        private class Implementation : External.IInterface<string>
        {
            public void SomeMethod(string a /*Expect:ImplicitNotNullOverridesUnknownExternalMember*/)
            {
            }
        }

        private class ImplementationWithDefaultValue : External.IInterface<string>
        {
            public void SomeMethod(string a = "default" /*Expect:ImplicitNotNullOverridesUnknownExternalMember*/)
            {
            }
        }

        private class ImplementationWithNullDefaultValue : External.IInterface<string>
        {
            public void SomeMethod(string a = null /* no warning because this parameter is implicitly CanBeNull */)
            {
            }
        }

        private class ImplementationWithValueType : External.IInterface<DateTime>
        {
            public void SomeMethod(DateTime a)
            {
            }
        }

        private class ImplementationWithNullableValueType : External.IInterface<DateTime?>
        {
            public void SomeMethod(DateTime? a /* no warning because this parameter is implicitly CanBeNull */)
            {
            }
        }
    }
}