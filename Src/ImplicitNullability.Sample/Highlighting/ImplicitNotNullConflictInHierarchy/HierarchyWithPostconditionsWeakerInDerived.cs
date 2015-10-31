using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ImplicitNullability.Samples.CodeWithIN.Highlighting.ImplicitNotNullConflictInHierarchy
{
    public static class HierarchyWithPostconditionsWeakerInDerived
    {
        public interface IInterface
        {
            void ExplicitNotNullOutParameterInInterfaceCanBeNullInDerived([NotNull] out string a);
            void ImplicitNotNullOutParameterInInterfaceCanBeNullInDerived(out string a);

            [NotNull]
            string FunctionWithExplicitNotNullInInterfaceCanBeNullInDerived();

            string FunctionWithImplicitNotNullInInterfaceCanBeNullInDerived();

            [ItemNotNull]
            Task<string> TaskFunctionWithExplicitNotNullInInterfaceCanBeNullInDerived();

            Task<string> TaskFunctionWithImplicitNotNullInInterfaceCanBeNullInDerived();
        }

        public class Implementation : IInterface
        {
            // REPORTED false negative http://youtrack.jetbrains.com/issue/RSRP-415431
            public void ExplicitNotNullOutParameterInInterfaceCanBeNullInDerived([CanBeNull] out string a)
            {
                a = null;
            }

            // REPORTED false negative http://youtrack.jetbrains.com/issue/RSRP-415431 may also fix this issue
            public void ImplicitNotNullOutParameterInInterfaceCanBeNullInDerived([CanBeNull] out string a)
            {
                a = null;
            }

            [CanBeNull] /*Expect:AnnotationConflictInHierarchy*/
            public string FunctionWithExplicitNotNullInInterfaceCanBeNullInDerived()
            {
                return null;
            }

            [CanBeNull] /*Expect:AnnotationConflictInHierarchy[Implicit]*/
            public string FunctionWithImplicitNotNullInInterfaceCanBeNullInDerived()
            {
                return null;
            }

            [ItemCanBeNull] // REPORTED false negative https://youtrack.jetbrains.com/issue/RSRP-447891
            public async Task<string> TaskFunctionWithExplicitNotNullInInterfaceCanBeNullInDerived()
            {
                await Task.Delay(0);
                return null;
            }

            [ItemCanBeNull] // REPORTED false negative https://youtrack.jetbrains.com/issue/RSRP-447891 may also fix this issue 
            public async Task<string> TaskFunctionWithImplicitNotNullInInterfaceCanBeNullInDerived()
            {
                await Task.Delay(0);
                return null;
            }
        }
    }
}