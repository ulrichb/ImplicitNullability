using JetBrains.Annotations;

// Because not relevant for this test case:
// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable NotNullOnImplicitCanBeNull

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

            [NotNull]
            string PropertyWithExplicitNotNullInInterfaceCanBeNullInDerived { set; }

            string PropertyWithImplicitNotNullInInterfaceCanBeNullInDerived { set; }
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

            [CanBeNull]
            public string PropertyWithExplicitNotNullInInterfaceCanBeNullInDerived
            {
                set { }
            }

            [CanBeNull]
            public string PropertyWithImplicitNotNullInInterfaceCanBeNullInDerived
            {
                set { }
            }
        }
    }
}
