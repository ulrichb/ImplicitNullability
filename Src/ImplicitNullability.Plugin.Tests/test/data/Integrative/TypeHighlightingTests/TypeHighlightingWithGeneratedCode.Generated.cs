using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ImplicitNullability.Plugin.Tests.test.data.Integrative.TypeHighlightingTests
{
    public partial class TypeHighlightingWithGeneratedCode
    {
        // Proves exclusion of (non-implemented) generated code and their type highlighting.
        // See also the other "generated code" samples.

        public string Field;

        public void Method(string a) { }

        public void MethodExplicit([NotNull] string a) { }

        public string Function() => "";

        public delegate string SomeDelegate(string a);

        public async Task<string> AsyncFunction()
        {
            return await Async.CanBeNullResult<string>();
        }

        //

        partial void PartialMethodWithImplementation(string a);

        partial void PartialMethodWithCanBeNullInImplementation(string a);

        partial void PartialMethodWithoutImplementation(string a);
    }
}
