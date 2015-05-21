using System;
using ImplicitNullability.Sample.ExternalCode;

namespace ImplicitNullability.Sample.Highlighting.ImplicitNotNullOverridesUnknownExternalMember
{
  public class OverrideExternalMethodWithAnnotations
  {
    private class CanBeNullOnInterface : External.IInterfaceWithCanBeNullParameter
    {
      public void SomeMethod (string a /*Expect:ImplicitNotNullConflictInHierarchy*/)
      {
      }
    }

    private class NotNullOnInterface : External.IInterfaceWithNotNullParameter
    {
      public void SomeMethod (string a /* no warning expected because this is a safe case (same nullability as in the base member) */)
      {
      }
    }

    private interface IOwnCodeInterface
    {
      void SomeMethod (string a);
    }

    private class OneOfThreeInterfacesHasUnknownExternalMember
        :
            // OK because of the NotNull annotation:
            External.IInterfaceWithNotNullParameter,
            // The ugly one:
            External.IInterface<string>,
            // OK because of the implicit NotNull:
            IOwnCodeInterface
    {
      public void SomeMethod (string a /*Expect:ImplicitNotNullOverridesUnknownExternalMember*/)
      {
      }
    }
  }
}