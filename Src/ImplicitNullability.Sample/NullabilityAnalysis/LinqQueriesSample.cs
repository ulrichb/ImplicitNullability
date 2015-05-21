using System;
using System.Linq;

namespace ImplicitNullability.Sample.NullabilityAnalysis
{
  internal static class LinqQueriesSample
  {
    public static void MiscLinqQueries ()
    {
      var simpleQuery =
          from x in new[] { "" }
          select ReSharper.TestValueAnalysis (x, x == null);

      var selectMany =
          from x in new[] { new[] { "" } }
          from y in x
          select ReSharper.TestValueAnalysis (y, y == null);

      var letStatement =
          from x in new[] { "" }
          // This creates a TransparentVariable, which is an IParameter:
          let y = x
          select ReSharper.TestValueAnalysis (y, y == null);

      var groupInto =
          from x in new[] { "" }
          group x by ReSharper.TestValueAnalysis (x, x == null)
          into g
          select ReSharper.TestValueAnalysis (g, g == null);
      
      var join =
          from x in new[] { "" }
          join y in new[] { "" } on x equals ReSharper.TestValueAnalysis (y, y == null)
          select new { X = x, Y = ReSharper.TestValueAnalysis (y, y == null) };

      var joinInto =
          from x in new[] { "" }
          join y in new[] { "" } on x equals ReSharper.TestValueAnalysis (y, y == null) into g
          select ReSharper.TestValueAnalysis (g, g == null);
    }
  }
}