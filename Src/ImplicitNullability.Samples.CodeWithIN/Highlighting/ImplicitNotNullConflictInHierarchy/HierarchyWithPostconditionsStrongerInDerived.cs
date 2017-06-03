using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ImplicitNullability.Samples.CodeWithIN.Highlighting.ImplicitNotNullConflictInHierarchy
{
    public static class HierarchyWithPostconditionsStrongerInDerived
    {
        public interface IInterface
        {
            void CanBeNullOutParameterInInterfaceExplicitNotNullInDerived([CanBeNull] out string a);
            void CanBeNullOutParameterInInterfaceImplicitNotNullInDerived([CanBeNull] out string a);

            [CanBeNull]
            string FunctionWithCanBeNullInInterfaceExplicitNotNullInDerived();

            [CanBeNull]
            string FunctionWithCanBeNullInInterfaceImplicitNotNullInDerived();

            [ItemCanBeNull]
            Task<string> TaskFunctionWithCanBeNullInInterfaceExplicitNotNullInDerived();

            [ItemCanBeNull]
            Task<string> TaskFunctionWithCanBeNullInInterfaceImplicitNotNullInDerived();

            [CanBeNull]
            string PropertyWithCanBeNullInInterfaceExplicitNotNullInDerived { get; }

            [CanBeNull]
            string PropertyWithCanBeNullInInterfaceImplicitNotNullInDerived { get; }
        }

        public class Implementation : IInterface
        {
            public void CanBeNullOutParameterInInterfaceExplicitNotNullInDerived(
                [NotNull] out string a)
            {
                a = "";
            }

            public void CanBeNullOutParameterInInterfaceImplicitNotNullInDerived(
                out string a /*Expect:ImplicitNotNullElementCannotOverrideCanBeNull[Implicit]*/)
            {
                a = null;
            }

            [NotNull]
            public string FunctionWithCanBeNullInInterfaceExplicitNotNullInDerived()
            {
                return "";
            }

            public string FunctionWithCanBeNullInInterfaceImplicitNotNullInDerived
                /*Expect:ImplicitNotNullElementCannotOverrideCanBeNull[Implicit]*/()
            {
                return null;
            }

            [ItemNotNull]
            public async Task<string> TaskFunctionWithCanBeNullInInterfaceExplicitNotNullInDerived()
            {
                return await Async.NotNullResult("");
            }

            public async Task<string> TaskFunctionWithCanBeNullInInterfaceImplicitNotNullInDerived
                /*Expect:ImplicitNotNullElementCannotOverrideCanBeNull[Implicit]*/()
            {
                return await Async.CanBeNullResult<string>();
            }

            [NotNull]
            public string PropertyWithCanBeNullInInterfaceExplicitNotNullInDerived => "";

            public string PropertyWithCanBeNullInInterfaceImplicitNotNullInDerived
                /*Expect:ImplicitNotNullElementCannotOverrideCanBeNull[Implicit]*/ => null;
        }
    }
}
