using System.Threading.Tasks;
using ImplicitNullability.Samples.CodeWithoutIN;
using JetBrains.Annotations;

namespace ImplicitNullability.Samples.CodeWithIN.Highlighting.IncorrectNullableAttributeUsageAnalyzer
{
    public static class AnnotationRedundancyInHierarchySample
    {
        // Test that ReSharper's AnnotationRedundancyInHierarchy is *not* shown for implicitly nullable code.

        public interface IInterface
        {
            [CanBeNull]
            string ExplicitCanBeNullInBaseAndDerived([CanBeNull] string a);

            [CanBeNull]
            string CanBeNullInBase_ImplicitNotNullInDerived([CanBeNull] string a);

            [NotNull]
            string ExplicitNotNullInBaseAndDerived([NotNull] string a);

            [NotNull]
            string ExplicitNotNullInBase_AndImplicitNotNullInDerived([NotNull] string a);

            string ImplicitNotNullInBase_ExplicitNotNullInDerived(string a);

            [ItemCanBeNull]
            Task<string> ExplicitItemCanBeNullInBaseAndDerived();
        }

        public class Implementation : IInterface
        {
            // Reasons why we hide AnnotationRedundancyInHierarchy for [CanBeNull]:
            // a) It should be possible to repeat the [CanBeNull] annotation to *document* that an element is
            //    nullable *in opposite to the implicit default [NotNull]* (without the need to go to the base declaration).
            // b) AnnotationRedundancyInHierarchy would conflict with ImplicitNotNullConflictInHierarchy (for parameters) and
            //    ImplicitNotNullElementCannotOverrideCanBeNull (for result values) because they require repeating [CanBeNull] to fix
            //    it (see corresponding example below).

            [CanBeNull] /*Expect:AnnotationRedundancyInHierarchy[not Implicit]*/
            public string ExplicitCanBeNullInBaseAndDerived(
                [CanBeNull] /*Expect:AnnotationRedundancyInHierarchy[not Implicit]*/ string a)
            {
                return null;
            }

            public string CanBeNullInBase_ImplicitNotNullInDerived /*Expect:ImplicitNotNullElementCannotOverrideCanBeNull[Implicit]*/(
                string a /*Expect:ImplicitNotNullConflictInHierarchy[Implicit]*/)
            {
                return null;
            }

            // Reason why we hide AnnotationRedundancyInHierarchy for [NotNull]:
            //    It should be possible to repeat the [NotNull] annotation to *document* that an element is not just
            //    implicitly (by default/by accident) [NotNull] but some developer decided to mark this explicitly [NotNull].
            //    Especially, this is important when returning unknown nullability results, where we promote unknown
            //    nullability values to [NotNull] (see corresponding example below).

            [NotNull] /*Expect:AnnotationRedundancyInHierarchy[not Implicit]*/
            public string ExplicitNotNullInBaseAndDerived(
                [NotNull] /*Expect:AnnotationRedundancyInHierarchy[not Implicit]*/ string a)
            {
                return "";
            }

            public string ExplicitNotNullInBase_AndImplicitNotNullInDerived /*Expect no warning*/(
                string a /*Expect no warning*/)
            {
                return "";
            }

            [NotNull] /*Expect no warning*/
            public string ImplicitNotNullInBase_ExplicitNotNullInDerived(
                [NotNull] /*Expect no warning*/ string a)
            {
                return External.UnknownNullabilityString;
            }

            [ItemCanBeNull] /*Expect:AnnotationRedundancyInHierarchy[not Implicit]*/
            public async Task<string> ExplicitItemCanBeNullInBaseAndDerived()
            {
                return await Async.CanBeNullResult<string>();
            }
        }

        public interface IOtherElementsBase
        {
            [CanBeNull]
            string Property { get; set; }

            [CanBeNull]
            string this[string a] { get; set; }
        }

        public class OtherElementsBase : IOtherElementsBase
        {
            [CanBeNull] /*Expect:AnnotationRedundancyInHierarchy*/
            public string Property { get; set; }

            [CanBeNull] /*Expect:AnnotationRedundancyInHierarchy*/
            public string this[string a]
            {
                get { return null; }
                set { }
            }
        }
    }
}
