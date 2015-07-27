using System;
using System.Linq;

// ReSharper disable UnusedVariable

namespace ImplicitNullability.Sample.NullabilityAnalysis
{
    public class LinqQueriesSample
    {
        public void MiscLinqQueries()
        {
            var someEnumerable = new[] {""};

            var simpleQuery =
                from x in someEnumerable
                select ReSharper.TestValueAnalysis(x, x == null);

            var selectMany =
                from x in new[] {someEnumerable}
                from y in x
                select ReSharper.TestValueAnalysis(y, y == null);

            var letStatement =
                from x in someEnumerable
                // This creates a TransparentVariable, which is an IParameter:
                let y = x
                select ReSharper.TestValueAnalysis(y, y == null);

            var groupInto =
                from x in someEnumerable
                group x by ReSharper.TestValueAnalysis(x, x == null)
                into g
                select ReSharper.TestValueAnalysis(g, g == null);

            var join =
                from x in someEnumerable
                join y in someEnumerable on x equals ReSharper.TestValueAnalysis(y, y == null)
                select new {X = x, Y = ReSharper.TestValueAnalysis(y, y == null)};

            var joinInto =
                from x in someEnumerable
                join y in someEnumerable on x equals ReSharper.TestValueAnalysis(y, y == null) into g
                select ReSharper.TestValueAnalysis(g, g == null);
        }
    }
}