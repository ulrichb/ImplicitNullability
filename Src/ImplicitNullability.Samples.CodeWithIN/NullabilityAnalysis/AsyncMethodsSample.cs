using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NullGuard;

namespace ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis
{
    public class AsyncMethodsSample
    {
        public async Task Method(string a)
        {
            ReSharper.TestValueAnalysis(a, a == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
            await Async.NopTask;
        }

        public async Task MethodWithManualNullCheck([AllowNull /* avoid method rewriting */] string a)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            await Async.NopTask;
        }

        public async Task CallMethodWithNullArgument()
        {
            await Method(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);
        }

        public async Task NonVirtualAsyncMethod()
        {
            await Async.NopTask;
        }

        public virtual async Task VirtualAsyncMethod()
        {
            await Async.NopTask;
        }

        [ItemNotNull]
        public async Task<string> FunctionWithExplicitItemNotNull([CanBeNull] string returnValue)
        {
            return await Async.CanBeNullResult(returnValue) /*Expect:AssignNullToNotNullAttribute*/;
        }

        public async Task<string> Function([CanBeNull] string returnValue)
        {
            return await Async.CanBeNullResult(returnValue) /*Expect:AssignNullToNotNullAttribute[MOut]*/;
        }

        [ItemCanBeNull]
        [return: AllowNull] // REPORT? NullGuard doesn't support [ItemCanBeNull]
        public async Task<string> FunctionWithItemCanBeNull([CanBeNull] string returnValue)
        {
            return await Async.CanBeNullResult(returnValue) /*Expect no warning*/;
        }

        public async Task<int?> FunctionWithNullableInt(int? returnValue)
        {
            return await Async.CanBeNullResult(returnValue) /*Expect no warning*/;
        }

        [ItemNotNull]
        public Task<string> NonAsyncTaskResultFunctionWithExplicitNotNull([CanBeNull] string returnValue)
        {
            return Async.CanBeNullResult(returnValue);
        }

        public Task<string> NonAsyncTaskResultFunction([CanBeNull] string returnValue)
        {
            return Async.CanBeNullResult(returnValue);
        }
    }
}
