using System;
using ImplicitNullability.Sample.ExternalCode;

namespace ImplicitNullability.Sample.Highlighting.ImplicitNotNullOverridesUnknownExternalMember
{
    public class BaseClassWithExternalInterfaceInOwnCode
    {
        // Note: Here we tests the "immediate super member" behavior

        public abstract class Base : External.IInterfaceWithMethod<string>
        {
            public virtual void Method(string a /*Expect:ImplicitNotNullOverridesUnknownExternalMember*/)
            {
            }
        }

        public class Derived : Base
        {
            public override void Method(string a /* no warning expected because already displayed in base class */)
            {
            }
        }

        // ReSharper disable once RedundantExtendsListEntry
        public class DerivedAndImplementingTheInterface : Base, External.IInterfaceWithMethod<string>
        {
            public override void Method(string a /*Expect:ImplicitNotNullOverridesUnknownExternalMember*/)
            {
            }
        }

        public class DerivedAndExplicitlyImplementingTheInterface : Base, External.IInterfaceWithMethod<string>
        {
            public override void Method(string a)
            {
            }

            void External.IInterfaceWithMethod<string>.Method(string a /*Expect:ImplicitNotNullOverridesUnknownExternalMember*/)
            {
            }
        }
    }
}