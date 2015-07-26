using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NullGuard;

namespace ImplicitNullability.Sample.NullabilityAnalysis
{
    public class AsyncMethodsSample
    {
        public async Task AsyncMethod(string a)
        {
            ReSharper.TestValueAnalysis(a, a == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
            await Task.Delay(0);
        }

        public async Task AsyncMethodWithManualNullCheck([AllowNull /* avoid method rewriting */] string a)
        {
            if (a == null)
                throw new ArgumentNullException("a");

            await Task.Delay(0);
        }

        public async Task CallAsyncMethodWithNullArgument()
        {
            await AsyncMethod(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);
        }

        public async Task<string> AsyncFunction([CanBeNull] string returnValue)
        {
            await Task.Delay(0);

            return returnValue; // REPORTED http://youtrack.jetbrains.com/issue/RSRP-376091, requires extension point for [ItemNotNull]
        }
    }
}