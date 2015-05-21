using System;
using ImplicitNullability.Sample.ExternalCode;

namespace ImplicitNullability.Sample.Highlighting.ImplicitNotNullOverridesUnknownExternalMember
{
  public class BaseClassWithExternalInterfaceInOwnCode
  {
    private abstract class Base : External.IInterface<string>
    {
      public virtual void SomeMethod (string a /*Expect:ImplicitNotNullOverridesUnknownExternalMember*/)
      {
      }
    }

    private class Derived : Base
    {
      public override void SomeMethod (string a /* no warning expected because already displayed in base class */)
      {
      }
    }

    private class DerivedAndImplementingTheInterface : Base, External.IInterface<string>
    {
      public override void SomeMethod (string a /*Expect:ImplicitNotNullOverridesUnknownExternalMember*/)
      {
      }
    }

    private class DerivedAndExplicitlyImplementingTheInterface : Base, External.IInterface<string>
    {
      public override void SomeMethod (string a)
      {
      }

      void External.IInterface<string>.SomeMethod (string a /*Expect:ImplicitNotNullOverridesUnknownExternalMember*/)
      {
      }
    }
  }
}