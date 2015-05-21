using System;
using JetBrains.Annotations;

namespace ImplicitNullability.Sample.Highlighting.ImplicitNotNullConflictInHierarchy
{
  public class CanBeNullOnHiddenMethod
  {
    private class Base
    {
      public void TestMethod ([CanBeNull] string a)
      {
      }
    }

    private class Derived : Base
    {
      public new void TestMethod (string a /* no warning because the contract of the base method hasn't changed (no polymorphism) */)
      {
      }
    }
  }
}