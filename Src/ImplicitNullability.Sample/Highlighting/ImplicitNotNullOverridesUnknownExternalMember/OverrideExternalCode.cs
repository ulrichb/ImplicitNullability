using System;
using ImplicitNullability.Sample.ExternalCode;
using JetBrains.Annotations;

namespace ImplicitNullability.Sample.Highlighting.ImplicitNotNullOverridesUnknownExternalMember
{
    public class OverrideExternalCode
    {
        public class DerivedClassInOwnCode : External.Class
        {
            public override string this[string a /*Expect:ImplicitNotNullOverridesUnknownExternalMember[Implicit]*/]
            {
                get { return null; }
            }

            public override void Method(string a /*Expect:ImplicitNotNullOverridesUnknownExternalMember[Implicit]*/)
            {
            }
        }

        public class DerivedClassInOwnCodeWithExplicitCanBeNull : External.Class
        {
            public override string this[[CanBeNull] string a]
            {
                get { return null; }
            }

            public override void Method([CanBeNull] string a)
            {
            }
        }

        public class DerivedClassInOwnCodeWithExplicitNotNull : External.Class
        {
            public override string this[[NotNull] string a]
            {
                get { return null; }
            }

            public override void Method([NotNull] string a)
            {
            }
        }

        public class OverrideWithDefaultValue : External.IInterfaceWithMethod<string>
        {
            public void Method(string a = "default" /*Expect:ImplicitNotNullOverridesUnknownExternalMember[Implicit]*/)
            {
            }
        }

        public class ValueTypes : External.IInterfaceWithMethod<DateTime>
        {
            public void Method(DateTime a)
            {
            }
        }
    }
}