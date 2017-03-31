using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ImplicitNullability.Plugin.Tests.test.data.Integrative.TypeHighlightingTests
{
    public static class Async
    {
        public static Task NopTask => Task.Delay(0);

        [ItemNotNull]
        public static async Task<T> NotNullResult<T>([NotNull] T value)
        {
            await Task.Delay(0);
            return value;
        }

        [ItemCanBeNull]
        public static async Task<T> CanBeNullResult<T>()
        {
            await Task.Delay(0);
            return default(T);
        }
    }
}
