using System.Threading.Tasks;

namespace ImplicitNullability.Plugin.Tests.test.data.Integrative.TypeHighlightingTests
{
    public class TypeHighlightingInvalidDeclarationsSample
    {
        public /* missing type part triggers null "type usage" */ <string> SomeMethod() { }

        public delegate /* missing type part triggers null "type usage" */ <string> SomeDelegate();

        public static /* missing return type */ operator ++ { }
    }

    public class ItemTypeHighlightingInvalidDeclarationsSample
    {
        public async Task< /* missing type triggers null item "type usage" */ > AsyncMethod()
        {
            return await Async.NotNullResult("");
        }

        public delegate Task< /* missing type triggers null item "type usage" */> AsyncDelegate();
    }
}
