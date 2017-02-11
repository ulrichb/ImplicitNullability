using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NullGuard;

namespace ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis
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
                throw new ArgumentNullException(nameof(str));

            yield break;
        }

        public IEnumerable<object> SomeIteratorReturningNullItem()
        {
            yield return null;
        }

        [ItemCanBeNull]
        public IEnumerable<object> SomeIteratorWithItemCanBeNull()
        {
            yield return null;
        }

        [ItemNotNull]
        public IEnumerable<object> SomeIteratorWithItemNotNull()
        {
            yield return null /*Expect:AssignNullToNotNullAttribute*/;
        }
    }
}
