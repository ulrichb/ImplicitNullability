using System;
using ImplicitNullability.Sample.ExternalCode;

namespace ImplicitNullability.Sample.Highlighting.ImplicitNotNullOverridesUnknownExternalMember
{
  public class OverrideExternalMethodWithRefAndOutParameter
  {
    private class InterfaceWithReferenceTypeRefParameter : External.IInterfaceWithRefAndOutParameter
    {
      public void SomeMethod (ref string a /*Expect:ImplicitNotNullOverridesUnknownExternalMember*/, out string b)
      {
        Console.WriteLine (a);
        a = null /*Expect:AssignNullToNotNullAttribute*/;
        b = null;
      }
    }
  }
}