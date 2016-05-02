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
            public void ExplicitNotNullOutParameterInInterfaceCanBeNullInDerived(
                [CanBeNull] /*Expect:AnnotationConflictInHierarchy[RS >= 20161]*/ out string a)
            {
                a = null /*Expect:AssignNullToNotNullAttribute[RS >= 20161]*/;
            }

            public void ImplicitNotNullOutParameterInInterfaceCanBeNullInDerived(
                [CanBeNull] /*Expect:AnnotationConflictInHierarchy[RS >= 20161 && Implicit]*/ out string a)
            {
                a = null;
            }

            [CanBeNull] /*Expect:AnnotationConflictInHierarchy*/
            public string FunctionWithExplicitNotNullInInterfaceCanBeNullInDerived()
            {
                // The invalid CanBeNull does not override the NotNull:
                return null /*Expect:AssignNullToNotNullAttribute[RS >= 20161]*/;
            }

            [CanBeNull] /*Expect:AnnotationConflictInHierarchy[Implicit]*/
            public string FunctionWithImplicitNotNullInInterfaceCanBeNullInDerived()
            {
                return null;
            }

            [ItemCanBeNull] /*Expect:AnnotationConflictInHierarchy[RS >= 20161]*/
            public async Task<string> TaskFunctionWithExplicitNotNullInInterfaceCanBeNullInDerived()
            {
                await Task.Delay(0);
                return null;
            }

            [ItemCanBeNull] /*Expect:AnnotationConflictInHierarchy[RS >= 20161 && Implicit]*/
            public async Task<string> TaskFunctionWithImplicitNotNullInInterfaceCanBeNullInDerived()
            {
                await Task.Delay(0);
                return null;
            }
        }
    }
}