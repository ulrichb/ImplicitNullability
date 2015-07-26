using System;
using System.Collections.Generic;
using NullGuard;

namespace ImplicitNullability.Sample.NullabilityAnalysis
{
    public class IteratorsSample
    {
        public IEnumerable<object> TestIterator(string str)
        {
            ReSharper.TestValueAnalysis(str, str == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
            yield break;
        }

        public IEnumerable<object> TestIteratorWithManualNullCheck([AllowNull /* just to avoid rewriting this method */] string str)
        {
            if (str == null)
                throw new ArgumentNullException("str");

            yield break;
        }
    }
}