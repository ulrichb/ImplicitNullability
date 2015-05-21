using System;
using JetBrains.Annotations;

namespace ImplicitNullability.Sample.Highlighting.ImplicitNotNullConflictInHierarchy
{
  public static class HierarchyWithPostconditionsWeakerInDerived
  {
    public interface IInterface
    {
      void ExplicitNotNullOutParameterInInterfaceCanBeNullInDerived ([NotNull] out string a);
      void ImplicitNotNullOutParameterInInterfaceCanBeNullInDerived (out string a);

      [NotNull]
      string FunctionWithExplicitNotNullInInterfaceCanBeNullInDerived ();

      string FunctionWithImplicitNotNullInInterfaceCanBeNullInDerived ();
    }

    public class Implementation : IInterface
    {
      // REPORTED false negative http://youtrack.jetbrains.com/issue/RSRP-415431
      public void ExplicitNotNullOutParameterInInterfaceCanBeNullInDerived ([CanBeNull] out string a)
      {
        a = null;
      }

      public void ImplicitNotNullOutParameterInInterfaceCanBeNullInDerived ([CanBeNull] out string a)
      {
        a = null;
      }

      [CanBeNull] /* Expect:AnnotationConflictInHierarchy*/
      public string FunctionWithExplicitNotNullInInterfaceCanBeNullInDerived ()
      {
        return null;
      }

      [CanBeNull]
      public string FunctionWithImplicitNotNullInInterfaceCanBeNullInDerived ()
      {
        return null;
      }
    }
  }
}