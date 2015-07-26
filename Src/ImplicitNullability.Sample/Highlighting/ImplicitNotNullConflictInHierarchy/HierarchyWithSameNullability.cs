using System;
using JetBrains.Annotations;

namespace ImplicitNullability.Sample.Highlighting.ImplicitNotNullConflictInHierarchy
{
    public static class HierarchyWithSameNullability
    {
        public interface IInterface
        {
            void ExplicitNotNullParameter([NotNull] string a);
            void ExplicitNotNullParameterInInterfaceAndImplicitNotNullInDerived([NotNull] string a);
            void ImplicitNotNullParameterInInterfaceAndExplicitNotNullInDerived(string a);

            void ExplicitCanBeNullParameter([CanBeNull] string canBeNull);
            void ExplicitCanBeNullParametersInInterfaceImplicitCanBeNullInDerived([CanBeNull] int? nullableInt, [CanBeNull] string optional = null);
            void ImplicitCanBeNullParametersInInterfaceExplicitCanBeNullInDerived(int? nullableInt, string optional = null);
        }

        public class Implementation : IInterface
        {
            public void ExplicitNotNullParameter([NotNull] string a)
            {
            }

            public void ExplicitNotNullParameterInInterfaceAndImplicitNotNullInDerived(string a)
            {
            }

            public void ImplicitNotNullParameterInInterfaceAndExplicitNotNullInDerived([NotNull] string a)
            {
            }

            public void ExplicitCanBeNullParameter([CanBeNull] string canBeNull)
            {
            }

            public void ExplicitCanBeNullParametersInInterfaceImplicitCanBeNullInDerived(int? nullableInt, string optional = null)
            {
            }

            public void ImplicitCanBeNullParametersInInterfaceExplicitCanBeNullInDerived(
                [CanBeNull] int? nullableInt,
                [CanBeNull] string optional = null)
            {
            }
        }
    }
}