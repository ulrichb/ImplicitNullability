using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ImplicitNullability.Plugin.Tests.test.data.Integrative.TypeHighlightingTests
{
    public class TypeHighlightingAsyncMethodsSample
    {
        public async Task<string> AsyncMethod()
        {
            await Task.Delay(0);
            return "";
        }

        public async Task VoidAsyncMethod()
        {
            await Task.Delay(0);
        }

        [ItemCanBeNull]
        public async Task<string> NullableAsyncMethod()
        {
            await Task.Delay(0);
            return null;
        }

        public Task<string> NonAsyncButTaskResult()
        {
            return Task.FromResult("");
        }

        [ItemCanBeNull]
        public Task<string> NonAsyncButNullableTaskResult()
        {
            return Task.FromResult<string>(null);
        }

        [CanBeNull]
        [ItemCanBeNull]
        public Task<string> NonAsyncCanBeNullAndItemCanBeNullMethod()
        {
            return null;
        }

        // Prove the exemption for async void (see TypeHighlightingProblemAnalyzer):
        public async void AsyncVoidResult()
        {
            await Task.Delay(0);
        }
    }
}
