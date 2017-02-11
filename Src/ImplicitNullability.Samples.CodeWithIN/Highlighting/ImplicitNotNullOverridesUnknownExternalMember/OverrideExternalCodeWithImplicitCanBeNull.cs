using System;
using ImplicitNullability.Samples.CodeWithoutIN;

namespace ImplicitNullability.Samples.CodeWithIN.Highlighting.ImplicitNotNullOverridesUnknownExternalMember
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
