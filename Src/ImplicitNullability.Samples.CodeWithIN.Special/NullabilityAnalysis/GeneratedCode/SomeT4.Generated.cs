
using JetBrains.Annotations;

namespace ImplicitNullability.Samples.CodeWithIN.Special.NullabilityAnalysis.GeneratedCode
{
    public partial class SomeT4GeneratedClass
    {
        public void Method(string a)
        {
        }

        public void MethodExplicit([NotNull] string a)
        {
        }

        //

        partial void PartialMethodWithImplementation(string a);

        partial void PartialMethodWithCanBeNullInImplementation(string a);

        partial void PartialMethodWithoutImplementation(string a);
    }
}
