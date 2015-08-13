using System;
using JetBrains.Annotations;

// ReSharper disable AnnotationRedundancyInHierarchy

namespace ImplicitNullability.Sample.Highlighting.ImplicitNotNullConflictInHierarchy
{
    public static class HierarchyWithSamePostconditions
    {
        public interface IInterface
        {
            [NotNull]
            string ExplicitNotNull();

            [NotNull]
            string ExplicitNotNullInInterfaceAndImplicitNotNullInDerived();

            string ImplicitNotNullInInterfaceAndExplicitNotNullInDerived();

            [CanBeNull]
            string ExplicitCanBeNull();

            [CanBeNull]
            int? ExplicitCanBeNullInInterfaceAndImplicitCanBeNullInDerived();

            int? ImplicitCanBeNullInInterfaceAndExplicitCanBeNullInDerived();
        }

        public class Implementation : IInterface
        {
            [NotNull]
            public string ExplicitNotNull()
            {
                return "";
            }

            public string ExplicitNotNullInInterfaceAndImplicitNotNullInDerived()
            {
                return "";
            }

            [NotNull]
            public string ImplicitNotNullInInterfaceAndExplicitNotNullInDerived()
            {
                return "";
            }

            [CanBeNull]
            public string ExplicitCanBeNull()
            {
                return null;
            }

            public int? ExplicitCanBeNullInInterfaceAndImplicitCanBeNullInDerived()
            {
                return null;
            }

            [CanBeNull]
            public int? ImplicitCanBeNullInInterfaceAndExplicitCanBeNullInDerived()
            {
                return null;
            }
        }
    }
}