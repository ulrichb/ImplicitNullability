using System.Threading.Tasks;
using JetBrains.Annotations;

// ReSharper disable UnusedMember.Global

namespace ImplicitNullability.Plugin.Tests.test.data.Integrative.TypeHighlightingTests
{
    public class TypeHighlightingTaskResultDelegatesSample
    {
        public delegate Task<string> AsyncDelegate();

        public delegate Task VoidAsyncDelegate();

        [ItemCanBeNull]
        public delegate Task<string> AsyncDelegateWithItemCanBeNull();

        [CanBeNull]
        public delegate Task<string> AsyncDelegateWithCanBeNull();

        [CanBeNull]
        [ItemCanBeNull]
        public delegate Task<string> AsyncDelegateWithCanBeNullAndItemCanBeNull();
    }
}
