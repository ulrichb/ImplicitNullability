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
        }

        public class Implementation : IInterface
        {
            // REPORTED false positive http://youtrack.jetbrains.com/issue/RSRP-415431
            public void CanBeNullOutParameterInInterfaceExplicitNotNullInDerived([NotNull] /*Expect:AnnotationConflictInHierarchy*/ out string a)
            {
                a = "";
            }

            public void CanBeNullOutParameterInInterfaceImplicitNotNullInDerived(out string a
                /*Expect:ImplicitNotNullElementCannotOverrideCanBeNull[Implicit]*/)
            {
                a = null;
            }

            [NotNull]
            public string FunctionWithCanBeNullInInterfaceExplicitNotNullInDerived()
            {
                return "";
            }

            public string FunctionWithCanBeNullInInterfaceImplicitNotNullInDerived /*Expect:ImplicitNotNullElementCannotOverrideCanBeNull[Implicit]*/()
            {
                return "";
            }

            [ItemNotNull]
            public async Task<string> TaskFunctionWithCanBeNullInInterfaceExplicitNotNullInDerived()
            {
                await Task.Delay(0);
                return "";
            }

            public async Task<string> TaskFunctionWithCanBeNullInInterfaceImplicitNotNullInDerived
                /*Expect:ImplicitNotNullElementCannotOverrideCanBeNull[RS >= 92 && Implicit]*/()
            {
                await Task.Delay(0);
                return "";
            }
        }
    }
}