using System.Threading.Tasks;
using JetBrains.Annotations;

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

            [ItemNotNull]
            Task<string> TaskFunctionWithExplicitNotNull();

            [ItemCanBeNull]
            Task<string> TaskFunctionWithExplicitCanBeNull();
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

            [ItemNotNull] // REPORT? missing AnnotationRedundancyInHierarchy
            public async Task<string> TaskFunctionWithExplicitNotNull()
            {
                await Task.Delay(0);
                return "";
            }

            [ItemCanBeNull] // REPORT? missing AnnotationRedundancyInHierarchy
            public async Task<string> TaskFunctionWithExplicitCanBeNull()
            {
                await Task.Delay(0);
                return null;
            }
        }
    }
}