using System;
using JetBrains.Annotations;

namespace ImplicitNullability.Sample.Highlighting.ImplicitNotNullConflictInHierarchy
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
        }

        public class Implementation : IInterface
        {
            // REPORTED false positive http://youtrack.jetbrains.com/issue/RSRP-415431
            public void CanBeNullOutParameterInInterfaceExplicitNotNullInDerived([NotNull] /*Expect:AnnotationConflictInHierarchy*/ out string a)
            {
                a = "";
            }

            public void CanBeNullOutParameterInInterfaceImplicitNotNullInDerived(out string a)
            {
                a = null;
            }

            [NotNull]
            public string FunctionWithCanBeNullInInterfaceExplicitNotNullInDerived()
            {
                return "";
            }

            public string FunctionWithCanBeNullInInterfaceImplicitNotNullInDerived()
            {
                return "";
            }
        }
    }
}