using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ImplicitNullability.Samples.CodeWithIN.Highlighting.ImplicitNotNullConflictInHierarchy
{
    public class NullabilityInheritanceBehavior
    {
        public interface IInterface
        {
            [NotNull]
            string ExplicitNotNullInBase([NotNull] string a);

            [CanBeNull]
            string ExplicitCanBeNullInBase([CanBeNull] string a);

            [CanBeNull]
            string OverriddenNullabilityInDerived([NotNull] string a);
        }

        public class Implementation : IInterface
        {
            public string ExplicitNotNullInBase(string a)
            {
                // Here the implicit NotNull matches with the base class.
                ReSharper.TestValueAnalysis(a, a == null /*Expect:ConditionIsAlwaysTrueOrFalse*/);
                return null /*Expect:AssignNullToNotNullAttribute*/;
            }

            public string ExplicitCanBeNullInBase /*Expect:ImplicitNotNullElementCannotOverrideCanBeNull[Implicit]*/(
                string a /*Expect:ImplicitNotNullConflictInHierarchy[Implicit]*/)
            {
                // Here ReSharper inherits the [CanBeNull] from the base member.
                // Note that this is one of the reasons for the ImplicitNotNullElementCannotOverrideCanBeNull warning.
                ReSharper.TestValueAnalysis(a /*Expect:AssignNullToNotNullAttribute*/, a == null);
                return null /*Expect no warning*/;
            }

            [NotNull]
            public string OverriddenNullabilityInDerived([CanBeNull] string a)
            {
                // Here the explicit NotNull/CanBeNull overrides the base annotation.
                ReSharper.TestValueAnalysis(a /*Expect:AssignNullToNotNullAttribute*/, a == null);
                return null /*Expect:AssignNullToNotNullAttribute*/;
            }
        }

        // The following sample documents the same behavior for async methods.

        public interface ITaskInterface
        {
            [ItemNotNull]
            Task<string> ExplicitNotNullInBase();

            [ItemCanBeNull]
            Task<string> ExplicitCanBeNullInBase();

            [ItemCanBeNull]
            Task<string> OverriddenNullabilityInDerived();
        }

        public class AsyncImplementation : ITaskInterface
        {
            public async Task<string> ExplicitNotNullInBase()
            {
                return await Async.CanBeNullResult<string>() /*Expect:AssignNullToNotNullAttribute*/;
            }

            public async Task<string> ExplicitCanBeNullInBase /*Expect:ImplicitNotNullElementCannotOverrideCanBeNull[Implicit]*/()
            {
                return await Async.CanBeNullResult<string>();
            }

            [ItemNotNull]
            public async Task<string> OverriddenNullabilityInDerived()
            {
                return await Async.CanBeNullResult<string>() /*Expect:AssignNullToNotNullAttribute*/;
            }
        }
    }
}
