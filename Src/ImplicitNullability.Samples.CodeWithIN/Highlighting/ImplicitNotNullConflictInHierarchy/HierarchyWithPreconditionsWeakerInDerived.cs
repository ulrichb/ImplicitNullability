using System;
using JetBrains.Annotations;

// ReSharper disable AssignNullToNotNullAttribute - because not relevant for this test case
// ReSharper disable NotNullOnImplicitCanBeNull - because not relevant for this test case

namespace ImplicitNullability.Samples.CodeWithIN.Highlighting.ImplicitNotNullConflictInHierarchy
{
    public static class HierarchyWithPreconditionsWeakerInDerived
    {
        public interface IInterface
        {
            void ExplicitNotNullParamsInInterfaceExplicitCanBeNullInDerived([NotNull] int? nullableInt, [NotNull] string optional = null);

            void ImplicitCanBeNullParametersInInterfaceExplicitNotNullInDerived([NotNull] int? nullableInt, [NotNull] string optional = null);

            void ExplicitNotNullParameterInInterfaceCanBeNullInDerived([NotNull] string a);
            void ImplicitNotNullParameterInInterfaceCanBeNullInDerived(string a);
        }

        public class Implementation : IInterface
        {
            public void ExplicitNotNullParamsInInterfaceExplicitCanBeNullInDerived([CanBeNull] int? nullableInt, [CanBeNull] string optional = null)
            {
            }

            public void ImplicitCanBeNullParametersInInterfaceExplicitNotNullInDerived(int? nullableInt, string optional = null)
            {
            }

            public void ExplicitNotNullParameterInInterfaceCanBeNullInDerived([CanBeNull] string a)
            {
            }

            public void ImplicitNotNullParameterInInterfaceCanBeNullInDerived([CanBeNull] string a)
            {
            }
        }
    }
}
