using System.Threading.Tasks;

namespace ImplicitNullability.Samples.CodeWithIN.Highlighting.ImplicitNotNullOverridesUnknownBaseMemberNullability
{
    public interface IImplicitlyNullableInterface
    {
        void Method(string a);
        string Function();
        Task<string> AsyncFunction();
        string Property { get; set; }
    }
}
