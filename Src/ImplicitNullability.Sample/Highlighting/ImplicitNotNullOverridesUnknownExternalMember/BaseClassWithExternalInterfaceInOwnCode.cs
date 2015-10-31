using System.Diagnostics.CodeAnalysis;
using ImplicitNullability.Sample.ExternalCode;

namespace ImplicitNullability.Sample.Highlighting.ImplicitNotNullOverridesUnknownExternalMember
{
    public class BaseClassWithExternalInterfaceInOwnCode
    {
        // Note: Here we test the "immediate super member" behavior

        public abstract class Base : External.IInterfaceWithMethod<string>, External.IInterfaceWithFunction<string>
        {
            public virtual void Method(string a /*Expect:ImplicitNotNullOverridesUnknownExternalMember[Implicit]*/)
            {
            }

            public virtual string Function /*Expect:ImplicitNotNullResultOverridesUnknownExternalMember[Implicit]*/()
            {
                return "";
            }
        }

        public class Derived : Base
        {
            public override void Method(string a /*Expect no warning because it's already displayed in base class*/)
            {
            }

            public override string Function /*Expect no warning because it's already displayed in base class*/()
            {
                return "";
            }
        }

        [SuppressMessage("ReSharper", "RedundantExtendsListEntry")]
        public class DerivedAndImplementingTheInterface :
            Base, External.IInterfaceWithMethod<string>, External.IInterfaceWithFunction<string>
        {
            public override void Method(string a /*Expect:ImplicitNotNullOverridesUnknownExternalMember[Implicit]*/)
            {
            }

            public override string Function /*Expect:ImplicitNotNullResultOverridesUnknownExternalMember[Implicit]*/()
            {
                return "";
            }
        }

        public class DerivedAndExplicitlyImplementingTheInterface :
            Base, External.IInterfaceWithMethod<string>, External.IInterfaceWithFunction<string>
        {
            public override void Method(string a)
            {
            }

            void External.IInterfaceWithMethod<string>.Method(string a /*Expect:ImplicitNotNullOverridesUnknownExternalMember[Implicit]*/)
            {
            }

            string External.IInterfaceWithFunction<string>.Function /*Expect:ImplicitNotNullResultOverridesUnknownExternalMember[Implicit]*/()
            {
                return "";
            }
        }
    }
}