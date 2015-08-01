using System;
using ImplicitNullability.Sample.ExternalCode;

namespace ImplicitNullability.Sample.Highlighting.ImplicitNotNullOverridesUnknownExternalMember
{
    public class OverrideExternalCodeWithAnnotations
    {
        public class ExternalCanBeNull : External.IInterfaceWithCanBeNullMethod
        {
            public void Method(string a /*Expect:ImplicitNotNullConflictInHierarchy*/)
            {
            }
        }

        public class ExternalNotNull : External.IInterfaceWithNotNullMethod
        {
            public void Method(string a /* no warning expected because this is a safe case (same nullability as in the base member) */)
            {
            }
        }

        private interface IOwnCodeInterface
        {
            void Method(string a);
        }

        public class AllBaseInterfacesNotNull :
            External.IInterfaceWithNotNullMethod, // OK because of the NotNull annotation
            IOwnCodeInterface // OK because implicitly NotNull
        {
            public void Method(string a)
            {
            }
        }

        public class OneOfThreeBaseInterfacesIsUnknownExternalMember :
            External.IInterfaceWithNotNullMethod, // OK because of the NotNull annotation
            External.IInterfaceWithMethod<string>, // The bad one
            IOwnCodeInterface // OK because implicitly NotNull
        {
            public void Method(string a /*Expect:ImplicitNotNullOverridesUnknownExternalMember*/)
            {
            }
        }
    }
}