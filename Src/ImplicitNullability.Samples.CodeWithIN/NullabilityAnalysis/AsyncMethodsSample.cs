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
            await Task.Delay(0);
        }

        public async Task MethodWithManualNullCheck([AllowNull /* avoid method rewriting */] string a)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            await Task.Delay(0);
        }

        public async Task CallMethodWithNullArgument()
        {
            await Method(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);
        }

        public async Task NonVirtualAsyncMethod()
        {
            await Task.Delay(0);
        }

        public virtual async Task VirtualAsyncMethod()
        {
            await Task.Delay(0);
        }

        [ItemNotNull]
        public async Task<string> FunctionWithExplicitItemNotNull([CanBeNull] string returnValue)
        {
            await Task.Delay(0);
            return returnValue /*Expect:AssignNullToNotNullAttribute*/;
        }

        public async Task<string> Function([CanBeNull] string returnValue)
        {
            await Task.Delay(0);
            return returnValue /*Expect:AssignNullToNotNullAttribute[MOut]*/;
        }

        [ItemCanBeNull]
        [return: AllowNull] // REPORT? NullGuard doesn't support [ItemCanBeNull]
        public async Task<string> FunctionWithItemCanBeNull([CanBeNull] string returnValue)
        {
            await Task.Delay(0);
            return returnValue /*Expect no warning*/;
        }

        public async Task<int?> FunctionWithNullableInt(int? returnValue)
        {
            await Task.Delay(0);
            return returnValue /*Expect no warning*/;
        }

        [ItemNotNull]
        public Task<string> NonAsyncTaskResultFunctionWithExplicitNotNull([CanBeNull] string returnValue)
        {
            return Task.FromResult(returnValue);
        }

        public Task<string> NonAsyncTaskResultFunction([CanBeNull] string returnValue)
        {
            return Task.FromResult(returnValue);
        }
    }
}
