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

            public override string Function()
            {
                var baseValue = base.Function();
                // Here we convert an unknown (possibly CanBeNull) value to an implicitly NotNull return value:
                return baseValue;
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

            [CanBeNull]
            public override string Function()
            {
                var baseValue = base.Function();
                return baseValue;
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

            [NotNull]
            public override string Function()
            {
                var baseValue = base.Function();
                return baseValue;
            }
        }

        public class OverrideWithDefaultValue : External.IInterfaceWithMethod<string>
        {
            public void Method(string a = "default" /*Expect:ImplicitNotNullOverridesUnknownExternalMember[Implicit]*/)
            {
            }
        }

        public class ValueTypes : External.IInterfaceWithMethod<DateTime>, External.IFunctionWithMethod<DateTime>
        {
            public void Method(DateTime a)
            {
            }

            public DateTime Function()
            {
                return default(DateTime);
            }
        }
    }
}