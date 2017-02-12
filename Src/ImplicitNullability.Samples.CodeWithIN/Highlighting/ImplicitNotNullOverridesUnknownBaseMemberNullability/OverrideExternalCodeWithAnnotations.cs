using System.Threading.Tasks;
using ImplicitNullability.Samples.CodeWithoutIN;

namespace ImplicitNullability.Samples.CodeWithIN.Highlighting.ImplicitNotNullOverridesUnknownBaseMemberNullability
{
    public class OverrideExternalCodeWithAnnotations
    {
        public class ExternalCanBeNull : External.BaseClassWithCanBeNull
        {
            public override void Method(string a /*Expect:ImplicitNotNullConflictInHierarchy[Implicit]*/)
            {
            }

            public override string Function /*Expect:ImplicitNotNullElementCannotOverrideCanBeNull[Implicit]*/()
            {
                return null;
            }

            public override async Task<string> AsyncFunction /*Expect:ImplicitNotNullElementCannotOverrideCanBeNull[Implicit]*/()
            {
                await Task.Delay(0);
                return null;
            }
        }

        public class ExternalNotNull : External.BaseClassWithNotNull
        {
            public override void Method(string a /*Expect no warning*/)
            {
            }

            public override string Function /*Expect no warning*/()
            {
                return "";
            }

            public override async Task<string> AsyncFunction /*Expect no warning*/()
            {
                await Task.Delay(0);
                return "";
            }
        }

        public class AllBaseTypesNotNull :
            // OK because of the NotNull annotation:
            External.BaseClassWithNotNull,
            // OK because implicitly NotNull:
            IImplicitlyNullableInterface
        {
            public override void Method(string a)
            {
            }

            public override string Function()
            {
                return "";
            }

            public override async Task<string> AsyncFunction()
            {
                await Task.Delay(0);
                return "";
            }
        }

        public class OneOfThreeBaseTypesIsUnknownExternalCode :
            // OK because of the NotNull annotation:
            External.BaseClassWithNotNull,
            // The bad ones:
            External.IInterfaceWithMethod<string>, External.IInterfaceWithFunction<string>, External.IInterfaceWithAsyncFunction<string>,
            // OK because implicitly NotNull:
            IImplicitlyNullableInterface
        {
            public override void Method(string a /*Expect:ImplicitNotNullOverridesUnknownBaseMemberNullability[Implicit]*/)
            {
            }

            public override string Function /*Expect:ImplicitNotNullResultOverridesUnknownBaseMemberNullability[Implicit]*/()
            {
                return "";
            }

            public override async Task<string> AsyncFunction /*Expect:ImplicitNotNullResultOverridesUnknownBaseMemberNullability[Implicit]*/()
            {
                await Task.Delay(0);
                return "";
            }
        }
    }
}
