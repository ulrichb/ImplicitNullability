using System;
using System.Collections.Generic;
using NullGuard;

namespace ImplicitNullability.Sample.NullabilityAnalysis
{
    public class IteratorsSample
    {
        public IEnumerable<object> SomeIterator(string str)
        {
            ReSharper.TestValueAnalysis(str, str == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
            yield break;
        }

        public IEnumerable<object> SomeIteratorWithManualNullCheck([AllowNull /* avoid method rewriting */] string str)
        {
            if (str == null)
                throw new ArgumentNullException("str");

            yield break;
        }

        public IEnumerable<object> SomeIteratorReturningNullItem()
        {
            yield return null;
        }
    }
}