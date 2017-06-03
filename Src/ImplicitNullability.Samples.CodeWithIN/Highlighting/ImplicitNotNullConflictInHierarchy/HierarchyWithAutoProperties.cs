using JetBrains.Annotations;

namespace ImplicitNullability.Samples.CodeWithIN.Highlighting.ImplicitNotNullConflictInHierarchy
{
    public static class HierarchyWithAutoProperties
    {
        public static class GetterAndSetterInSuperMember
        {
            public interface IInterface
            {
                [CanBeNull]
                string CanBeNullInInterfaceExplicitNotNullInDerived { get; set; }

                [CanBeNull]
                string CanBeNullInInterfaceImplicitNotNullInDerived { get; set; }

                [NotNull]
                string ExplicitNotNullInInterfaceCanBeNullInDerived { get; set; }

                string ImplicitNotNullInInterfaceCanBeNullInDerived { get; set; }
            }

            public class Implementation : IInterface
            {
                [NotNull] /*Expect:AnnotationConflictInHierarchy*/
                public string CanBeNullInInterfaceExplicitNotNullInDerived { get; set; } = "";

                public string CanBeNullInInterfaceImplicitNotNullInDerived
                    /*Expect:ImplicitNotNullConflictInHierarchy[Implicit]*/ { get; set; } = null;

                [CanBeNull] /*Expect:AnnotationConflictInHierarchy*/
                public string ExplicitNotNullInInterfaceCanBeNullInDerived { get; set; }

                [CanBeNull] /*Expect:AnnotationConflictInHierarchy[Implicit]*/
                public string ImplicitNotNullInInterfaceCanBeNullInDerived { get; set; }
            }

            public interface ISuppressedInputWarningOnProperty
            {
                [CanBeNull]
                string CanBeNullInInterfaceImplicitNotNullInDerived { get; set; }
            }

            public class SuppressedInputWarningOnProperty : ISuppressedInputWarningOnProperty
            {
                // Prove that both warning are emitted (and ImplicitNotNullConflictInHierarchy is prioritized):

                // ReSharper disable ImplicitNotNullConflictInHierarchy
                public string CanBeNullInInterfaceImplicitNotNullInDerived
                    /*Expect:ImplicitNotNullElementCannotOverrideCanBeNull[Implicit]*/ { get; set; }
                // ReSharper restore ImplicitNotNullConflictInHierarchy
            }
        }

        public static class OnlyGetterInSuperMember
        {
            // Proves that input warning is not emitted (because the supper member has only a getter, no setter).

            public interface IInterface
            {
                [CanBeNull]
                string CanBeNullInInterfaceExplicitNotNullInDerived { get; }

                [CanBeNull]
                string CanBeNullInInterfaceImplicitNotNullInDerived { get; }

                [NotNull]
                string ExplicitNotNullInInterfaceCanBeNullInDerived { get; }

                string ImplicitNotNullInInterfaceCanBeNullInDerived { get; }
            }

            public class Implementation : IInterface
            {
                [NotNull] /*Expect no warning*/
                public string CanBeNullInInterfaceExplicitNotNullInDerived { get; set; } = "";

                public string CanBeNullInInterfaceImplicitNotNullInDerived
                    /*Expect:ImplicitNotNullElementCannotOverrideCanBeNull[Implicit]*/ { get; set; } = null;

                [CanBeNull] /*Expect:AnnotationConflictInHierarchy*/
                public string ExplicitNotNullInInterfaceCanBeNullInDerived { get; set; }

                [CanBeNull] /*Expect:AnnotationConflictInHierarchy[Implicit]*/
                public string ImplicitNotNullInInterfaceCanBeNullInDerived { get; set; }
            }
        }

        public static class OnlySetterInSuperMember
        {
            public interface IInterface
            {
                [CanBeNull]
                string CanBeNullInInterfaceExplicitNotNullInDerived { set; }

                [CanBeNull]
                string CanBeNullInInterfaceImplicitNotNullInDerived { set; }

                [NotNull]
                string ExplicitNotNullInInterfaceCanBeNullInDerived { set; }

                string ImplicitNotNullInInterfaceCanBeNullInDerived { set; }
            }

            public class Implementation : IInterface
            {
                [NotNull] /*Expect:AnnotationConflictInHierarchy*/
                public string CanBeNullInInterfaceExplicitNotNullInDerived { get; set; } = "";

                public string CanBeNullInInterfaceImplicitNotNullInDerived
                    /*Expect:ImplicitNotNullConflictInHierarchy[Implicit]*/ { get; set; } = null;

                [CanBeNull] /*Expect no warning*/
                public string ExplicitNotNullInInterfaceCanBeNullInDerived { get; set; }

                [CanBeNull] /*Expect no warning*/
                public string ImplicitNotNullInInterfaceCanBeNullInDerived { get; set; }
            }
        }
    }
}
