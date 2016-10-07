using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using ImplicitNullability.Samples.CodeWithIN.Highlighting.ImplicitNotNullOverridesUnknownExternalMember;
using ImplicitNullability.Samples.CodeWithoutIN;

// ReSharper disable UnusedMember.Global

namespace ImplicitNullability.Samples.Consumer.Highlighting.ImplicitNotNullOverridesUnknownExternalMember
{
    [ExcludeFromCodeCoverage]
    public class ExternalUnknownVsImplicitlyNullableCode
    {
        public class DerivedFromUnknownNullabilityCode : External.Class
        {
            public override void Method(string a /*Expect:ImplicitNotNullOverridesUnknownExternalMember[Implicit]*/)
            {
            }

            public override string Function /*Expect:ImplicitNotNullResultOverridesUnknownExternalMember[Implicit]*/()
            {
                return "";
            }

            public override async Task<string> AsyncFunction /*Expect:ImplicitNotNullResultOverridesUnknownExternalMember[Implicit]*/()
            {
                await Task.Delay(0);
                return "";
            }
        }

        public class DerivedFromImplicitlyNullableCode : IImplicitlyNullableInterface
        {
            public void Method(string a)
            {
            }

            public string Function()
            {
                return "";
            }

            public async Task<string> AsyncFunction()
            {
                await Task.Delay(0);
                return "";
            }
        }
    }
}