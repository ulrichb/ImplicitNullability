using System;
using ImplicitNullability.Sample.ExternalCode;

namespace ImplicitNullability.Sample.Highlighting.ImplicitNotNullOverridesUnknownExternalMember
{
    public class OverrideExternalCodeWithImplicitCanBeNull
    {
        public class ImplementationWithNullDefaultValue : External.IInterfaceWithMethod<string>
        {
            public void Method(string a = null /* no warning because this parameter is implicitly CanBeNull */)
            {
            }
        }

        public class ImplementationWithNullableValueType : External.IInterfaceWithMethod<DateTime?>
        {
            public void Method(DateTime? a /* no warning because this parameter is implicitly CanBeNull */)
            {
            }
        }
    }
}