using System;
using JetBrains.Annotations;

namespace ImplicitNullability.Samples.CodeWithIN.Highlighting.ImplicitNotNullConflictInHierarchy
{
    public static class HierarchyWithRefParameters
    {
        public interface IInterface
        {
            void CanBeNullRefParameterInInterfaceExplicitNotNullInDerived([CanBeNull] ref string a);
            void CanBeNullRefParameterInInterfaceImplicitNotNullInDerived([CanBeNull] ref string a);
            void ExplicitNotNullRefParameterInInterfaceCanBeNullInDerived([NotNull] ref string a);
            void ImplicitNotNullRefParameterInInterfaceCanBeNullInDerived(ref string a);
        }

        public class Implementation : IInterface
        {
            public void CanBeNullRefParameterInInterfaceExplicitNotNullInDerived([NotNull] /*Expect:AnnotationConflictInHierarchy*/ ref string a)
            {
                ReSharper.SuppressUnusedWarning(a);
                a = null /*Expect:AssignNullToNotNullAttribute*/;
            }

            public void CanBeNullRefParameterInInterfaceImplicitNotNullInDerived(ref string a /*Expect:ImplicitNotNullConflictInHierarchy[Implicit]*/)
            {
                ReSharper.SuppressUnusedWarning(a);
                a = null; // Interestingly the CodeAnnotationProvider is not asked here => [CanBeNull] is inherited
            }

            // REPORTED false negative http://youtrack.jetbrains.com/issue/RSRP-415431
            public void ExplicitNotNullRefParameterInInterfaceCanBeNullInDerived([CanBeNull] ref string a)
            {
                ReSharper.SuppressUnusedWarning(a);
            }

            // REPORTED false negative http://youtrack.jetbrains.com/issue/RSRP-415431 may also fix this issue
            public void ImplicitNotNullRefParameterInInterfaceCanBeNullInDerived([CanBeNull] ref string a)
            {
                ReSharper.SuppressUnusedWarning(a);
            }
        }
    }
}