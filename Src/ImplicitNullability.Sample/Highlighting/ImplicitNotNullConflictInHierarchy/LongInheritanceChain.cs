using System;
using JetBrains.Annotations;

namespace ImplicitNullability.Sample.Highlighting.ImplicitNotNullConflictInHierarchy
{
  public class LongInheritanceChain
  {
    private class Base1
    {
      public virtual void TestMethod ([CanBeNull] string a)
      {
      }
    }

    private class Base2 : Base1
    {
    }

    private class Base3 : Base2
    {
    }

    private class Base4 : Base3
    {
    }

    private class Derived : Base4
    {
      public override void TestMethod (string a /*Expect:ImplicitNotNullConflictInHierarchy*/)
      {
      }
    }
  }
}