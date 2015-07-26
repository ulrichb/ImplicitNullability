using System;
using System.Threading.Tasks;
using NullGuard;

namespace ImplicitNullability.Sample.NullabilityAnalysis
{
    public class AsyncMethodsSample
    {
        public async Task TestAsync(string a)
        {
            ReSharper.TestValueAnalysis(a, a == null /*Expect:ConditionIsAlwaysTrueOrFalse*/);
            await Task.Delay(0);
        }

        public async Task TestAsyncWithManualNullCheck([AllowNull /* just to avoid rewriting this method */] string a)
        {
            if (a == null)
                throw new ArgumentNullException("a");

            await Task.Delay(0);
        }

        public async Task CallTestAsyncWithNullArgument()
        {
            await TestAsync(null /*Expect:AssignNullToNotNullAttribute*/);
        }
    }
}