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

            public override string SetterOnlyProperty /*Expect:ImplicitNotNullOverridesUnknownBaseMemberNullability[Implicit]*/
            {
                set { }
            }

            public override string GetterOnlyProperty /*Expect:ImplicitNotNullResultOverridesUnknownBaseMemberNullability[Implicit]*/
            {
                get
                {
                    var baseValue = base.GetterOnlyProperty;
                    // Here we convert an unknown (possibly CanBeNull) value to an implicitly NotNull return value:
                    return baseValue;
                }
            }

            public override string Property /*Expect:ImplicitNotNullOverridesUnknownBaseMemberNullability[Implicit]*/ { get; set; }

            public override string this /*Expect:ImplicitNotNullOverridesUnknownBaseMemberNullability[Implicit]*/[
                string a /*Expect:ImplicitNotNullOverridesUnknownBaseMemberNullability[Implicit]*/]
            {
                get
                {
                    var baseValue = base[a];
                    // Here we convert an unknown (possibly CanBeNull) value to an implicitly NotNull return value:
                    return baseValue;
                }
                set { }
            }
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

            [CanBeNull]
            public override string Property { get; set; }

            [CanBeNull]
            public override string this[[CanBeNull] string a]
            {
                get { return base[a]; }
                set { }
            }
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

            [NotNull]
            public override string Property { get; set; }

            [NotNull]
            public override string this[[NotNull] string a]
            {
                get { return base[a]; }
                set { }
            }
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

        public class SuppressedInputWarningOnPropertyOrIndexer : External.Class
        {
            // Prove that both warning are emitted (and ImplicitNotNullOverridesUnknownBaseMemberNullability is prioritized):

            // ReSharper disable ImplicitNotNullOverridesUnknownBaseMemberNullability
            public override string Property
                /*Expect:ImplicitNotNullResultOverridesUnknownBaseMemberNullability[Implicit]*/ { get; set; }
            // ReSharper restore ImplicitNotNullOverridesUnknownBaseMemberNullability

            // ReSharper disable ImplicitNotNullOverridesUnknownBaseMemberNullability
            public override string this
                /*Expect:ImplicitNotNullResultOverridesUnknownBaseMemberNullability[Implicit]*/
                // ReSharper restore ImplicitNotNullOverridesUnknownBaseMemberNullability
                [[CanBeNull] string a]
            {
                get { return base[a]; }
                set { }
            }
        }

        // Counter-example
        public class DerivedFromImplicitlyNullableCode : IImplicitlyNullableInterface
        {
            public void Method(string a)
            {
            }

            public string Function() => "";

            public async Task<string> AsyncFunction() => await Async.NotNullResult("");

            public string Property { get; set; } = "";
        }
    }
}
