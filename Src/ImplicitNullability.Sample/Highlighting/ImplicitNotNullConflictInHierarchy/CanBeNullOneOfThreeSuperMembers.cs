using System;
using JetBrains.Annotations;

namespace ImplicitNullability.Sample.Highlighting.ImplicitNotNullConflictInHierarchy
{
  public class CanBeNullOneOfThreeSuperMembers
  {
    private interface IInterface1
    {
      void TestMethod (string a);
    }

    private interface IInterface2
    {
      void TestMethod ([CanBeNull] string a);
    }

    private interface IInterface3
    {
      void TestMethod (string a);
    }

    private class Implementation : IInterface1, IInterface2, IInterface3
    {
      public void TestMethod (string a /*Expect:ImplicitNotNullConflictInHierarchy*/)
      {
      }
    }
  }
}