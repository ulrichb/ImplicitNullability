using JetBrains.Annotations;

// ReSharper disable UnusedMember.Global, UnusedMember.Local, UnusedParameterInPartialMethod

namespace ImplicitNullability.Plugin.Tests.test.data.Integrative.TypeHighlightingTests
{
    public partial class TypeHighlightingWithGeneratedCode
    {
        partial void PartialMethodWithImplementation(string a) { }

        partial void PartialMethodWithCanBeNullInImplementation([CanBeNull] string a) { }
    }
}
