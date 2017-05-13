using System.Threading.Tasks;
using JetBrains.Annotations;

// ReSharper disable UnusedMember.Global

namespace ImplicitNullability.Plugin.Tests.test.data.Integrative.TypeHighlightingTests
{
    public class TypeHighlightingAsyncMethodsSample
    {
        public async Task<string> AsyncMethod()
        {
            return await Async.NotNullResult("");
        }

        public async Task VoidAsyncMethod()
        {
            await Async.NopTask;
        }

        [ItemCanBeNull]
        public async Task<string> AsyncMethodWithItemCanBeNull()
        {
            return await Async.CanBeNullResult<string>();
        }

        [CanBeNull]
        public async Task<string> AsyncMethodWithCanBeNull()
        {
            return await Async.NotNullResult("");
        }

        public Task<string> NonAsyncButTaskResult()
        {
            return Async.NotNullResult("");
        }

        [ItemCanBeNull]
        public Task<string> NonAsyncButTaskResultWithItemCanBeNull()
        {
            return Async.CanBeNullResult<string>();
        }

        [CanBeNull]
        [ItemCanBeNull]
        public Task<string> NonAsyncButTaskResultWithCanBeNullAndItemCanBeNull()
        {
            return null;
        }

        // Prove the exemption for async void (see TypeHighlightingProblemAnalyzer):
        public async void AsyncVoidResult()
        {
            await Async.NopTask;
        }
    }
}
