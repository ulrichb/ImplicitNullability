using System;
using JetBrains.Annotations;

namespace ImplicitNullability.Sample.Highlighting.ImplicitNotNullConflictInHierarchy
{
  public class CanBeNullOnExplicitlyImplementedInterface
  {
    private interface IInterface
    {
      void TestMethod ([CanBeNull] string a);
    }

    private class Implementation : IInterface
    {
      void IInterface.TestMethod (string a /*Expect:ImplicitNotNullConflictInHierarchy*/)
      {
      }
    }
  }
}