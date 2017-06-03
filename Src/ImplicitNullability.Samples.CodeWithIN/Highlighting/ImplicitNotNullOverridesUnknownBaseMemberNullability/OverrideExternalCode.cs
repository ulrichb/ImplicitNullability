using System;
using System.Threading.Tasks;
using ImplicitNullability.Samples.CodeWithoutIN;
using JetBrains.Annotations;

namespace ImplicitNullability.Samples.CodeWithIN.Highlighting.ImplicitNotNullOverridesUnknownBaseMemberNullability
{
    public class OverrideExternalCode
    {
        public class DerivedClassInOwnCode : External.Class
        {
            public override void Method(string a /*Expect:ImplicitNotNullOverridesUnknownBaseMemberNullability[Implicit]*/)
            {
            }

            public override string Function /*Expect:ImplicitNotNullResultOverridesUnknownBaseMemberNullability[Implicit]*/()
            {
                var baseValue = base.Function();
                // Here we convert an unknown (possibly CanBeNull) value to an implicitly NotNull return value:
                return baseValue;
            }

            public override async Task<string> AsyncFunction /*Expect:ImplicitNotNullResultOverridesUnknownBaseMemberNullability[Implicit]*/()
            {
                var baseValue = await base.AsyncFunction();
                // Here we convert an unknown (possibly CanBeNull) value to an implicitly NotNull return value:
                return baseValue;
            }

            public override string this[string a /*Expect:ImplicitNotNullOverridesUnknownBaseMemberNullability[Implicit]*/] => null;
        }

        public class DerivedClassInOwnCodeWithExplicitCanBeNull : External.Class
        {
            public override void Method([CanBeNull] string a)
            {
            }

            [CanBeNull]
            public override string Function() => base.Function();

            [ItemCanBeNull]
            public override async Task<string> AsyncFunction() => await base.AsyncFunction();

            public override string this[[CanBeNull] string a] => null;
        }

        public class DerivedClassInOwnCodeWithExplicitNotNull : External.Class
        {
            public override void Method([NotNull] string a)
            {
            }

            [NotNull]
            public override string Function() => base.Function();

            [ItemNotNull]
            public override async Task<string> AsyncFunction() => await base.AsyncFunction();

            public override string this[[NotNull] string a] => null;
        }

        public class OverrideWithDefaultValue : External.IInterfaceWithMethod<string>
        {
            public void Method(string a = "default" /*Expect:ImplicitNotNullOverridesUnknownBaseMemberNullability[Implicit]*/)
            {
            }
        }

        public class ValueTypes : External.IInterfaceWithMethod<DateTime>, External.IInterfaceWithFunction<DateTime>
        {
            public void Method(DateTime a)
            {
            }

            public DateTime Function() => default(DateTime);
        }

        // Counter-example
        public class DerivedFromImplicitlyNullableCode : IImplicitlyNullableInterface
        {
            public void Method(string a)
            {
            }

            public string Function() => "";

            public async Task<string> AsyncFunction() => await Async.NotNullResult("");
        }
    }
}
