using System.Threading.Tasks;

namespace ImplicitNullability.Samples.CodeWithIN.TypeHighlighting
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
            await Task.Delay(0);
            return "";
        }
    }
}