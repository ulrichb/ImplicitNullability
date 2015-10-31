using System.Threading.Tasks;
using ImplicitNullability.Sample.ExternalCode;

namespace ImplicitNullability.Sample.Highlighting.ImplicitNotNullOverridesUnknownExternalMember
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

            public override async Task<string> AsyncFunction /*Expect:ImplicitNotNullElementCannotOverrideCanBeNull[RS >= 92 && Implicit]*/()
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

        private interface IOwnCodeInterface
        {
            void Method(string a);
            string Function();
            Task<string> AsyncFunction();
        }

        public class AllBaseTypesNotNull :
            // OK because of the NotNull annotation:
            External.BaseClassWithNotNull,
            // OK because implicitly NotNull:
            IOwnCodeInterface
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

        public class OneOfThreeBaseTypesIsUnknownExternalMember :
            // OK because of the NotNull annotation:
            External.BaseClassWithNotNull,
            // The bad ones:
            External.IInterfaceWithMethod<string>, External.IInterfaceWithFunction<string>, External.IInterfaceWithAsyncFunction<string>,
            // OK because implicitly NotNull:
            IOwnCodeInterface
        {
            public override void Method(string a /*Expect:ImplicitNotNullOverridesUnknownExternalMember[Implicit]*/)
            {
            }

            public override string Function /*Expect:ImplicitNotNullResultOverridesUnknownExternalMember[Implicit]*/()
            {
                return "";
            }

            public override async Task<string> AsyncFunction /*Expect:ImplicitNotNullResultOverridesUnknownExternalMember[RS >= 92 && Implicit]*/()
            {
                await Task.Delay(0);
                return "";
            }
        }
    }
}