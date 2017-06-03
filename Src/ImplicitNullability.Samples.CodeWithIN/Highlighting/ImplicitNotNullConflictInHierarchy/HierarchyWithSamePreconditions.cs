using JetBrains.Annotations;

namespace ImplicitNullability.Samples.CodeWithIN.Highlighting.ImplicitNotNullConflictInHierarchy
{
    public static class HierarchyWithSamePreconditions
    {
        public interface IInterface
        {
            void ExplicitNotNull([NotNull] string a);
            void ExplicitNotNullInInterfaceAndImplicitNotNullInDerived([NotNull] string a);
            void ImplicitNotNullInInterfaceAndExplicitNotNullInDerived(string a);

            void ExplicitCanBeNull([CanBeNull] string canBeNull);
            void ExplicitCanBeNullInInterfaceAndImplicitCanBeNullInDerived([CanBeNull] int? nullableInt, [CanBeNull] string optional = null);
            void ImplicitCanBeNullInInterfaceAndExplicitCanBeNullInDerived(int? nullableInt, string optional = null);

            [NotNull]
            string PropertyWithExplicitNotNull { set; }

            [NotNull]
            string PropertyWithNotNullInInterfaceAndImplicitNotNullInDerived { set; }

            string PropertyWithImplicitNotNullInInterfaceAndExplicitNotNullInDerived { set; }

            [CanBeNull]
            string PropertyWithExplicitCanBeNull { set; }
        }

        public class Implementation : IInterface
        {
            public void ExplicitNotNull([NotNull] /*Expect:AnnotationRedundancyInHierarchy[not Implicit]*/ string a)
            {
            }

            public void ExplicitNotNullInInterfaceAndImplicitNotNullInDerived(string a)
            {
            }

            public void ImplicitNotNullInInterfaceAndExplicitNotNullInDerived([NotNull] string a)
            {
            }

            public void ExplicitCanBeNull([CanBeNull] /*Expect:AnnotationRedundancyInHierarchy[not Implicit]*/ string canBeNull)
            {
            }

            public void ExplicitCanBeNullInInterfaceAndImplicitCanBeNullInDerived(int? nullableInt, string optional = null)
            {
            }

            public void ImplicitCanBeNullInInterfaceAndExplicitCanBeNullInDerived([CanBeNull] int? nullableInt, [CanBeNull] string optional = null)
            {
            }

            [NotNull] /*Expect:AnnotationRedundancyInHierarchy[not Implicit]*/
            public string PropertyWithExplicitNotNull
            {
                set { }
            }

            public string PropertyWithNotNullInInterfaceAndImplicitNotNullInDerived
            {
                set { }
            }

            [NotNull]
            public string PropertyWithImplicitNotNullInInterfaceAndExplicitNotNullInDerived
            {
                set { }
            }

            [CanBeNull] /*Expect:AnnotationRedundancyInHierarchy[not Implicit]*/
            public string PropertyWithExplicitCanBeNull
            {
                set { }
            }
        }
    }
}
