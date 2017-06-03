using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ImplicitNullability.Samples.CodeWithIN.Highlighting.ImplicitNotNullConflictInHierarchy
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

            [ItemNotNull]
            Task<string> TaskFunctionWithExplicitNotNull();

            [ItemCanBeNull]
            Task<string> TaskFunctionWithExplicitCanBeNull();

            [NotNull]
            string PropertyWithExplicitNotNull { get; }

            [NotNull]
            string PropertyWithExplicitNotNullInInterfaceAndImplicitNotNullInDerived { get; }

            [CanBeNull]
            string PropertyWithExplicitCanBeNull { get; }
        }

        public class Implementation : IInterface
        {
            [NotNull] /*Expect:AnnotationRedundancyInHierarchy[not Implicit]*/
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

            [CanBeNull] /*Expect:AnnotationRedundancyInHierarchy[not Implicit]*/
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

            [ItemNotNull] /*Expect:AnnotationRedundancyInHierarchy[not Implicit]*/
            public async Task<string> TaskFunctionWithExplicitNotNull()
            {
                return await Async.NotNullResult("");
            }

            [ItemCanBeNull] /*Expect:AnnotationRedundancyInHierarchy[not Implicit]*/
            public async Task<string> TaskFunctionWithExplicitCanBeNull()
            {
                return await Async.CanBeNullResult<string>();
            }

            [NotNull] /*Expect:AnnotationRedundancyInHierarchy[not Implicit]*/
            public string PropertyWithExplicitNotNull => "";

            public string PropertyWithExplicitNotNullInInterfaceAndImplicitNotNullInDerived => "";

            [CanBeNull] /*Expect:AnnotationRedundancyInHierarchy[not Implicit]*/
            public string PropertyWithExplicitCanBeNull => null;
        }
    }
}
