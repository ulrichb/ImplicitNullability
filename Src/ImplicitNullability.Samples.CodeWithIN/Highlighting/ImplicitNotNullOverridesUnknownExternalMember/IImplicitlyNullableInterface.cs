using System.Threading.Tasks;

namespace ImplicitNullability.Samples.CodeWithIN.Highlighting.ImplicitNotNullOverridesUnknownExternalMember
{
    public interface IImplicitlyNullableInterface
    {
        void Method(string a);
        string Function();
        Task<string> AsyncFunction();
    }
}