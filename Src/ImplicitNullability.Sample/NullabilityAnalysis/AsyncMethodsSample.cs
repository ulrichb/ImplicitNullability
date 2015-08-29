using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NullGuard;

namespace ImplicitNullability.Sample.NullabilityAnalysis
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
                throw new ArgumentNullException("a");

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

        public async Task<string> Function([CanBeNull] string returnValue)
        {
            await Task.Delay(0);
            return returnValue; // REPORTED http://youtrack.jetbrains.com/issue/RSRP-376091, requires extension point for [ItemNotNull]
        }

    }
}