using System;
using JetBrains.Annotations;

namespace ImplicitNullability.Samples.CodeWithIN.Highlighting.ImplicitNotNullConflictInHierarchy
{
    public static class HierarchyWithPreconditionsStrongerInDerived
    {
        public interface IInterface
        {
            string this[[CanBeNull] string a, [CanBeNull] string b] { get; }

            void ExplicitCanBeNullParametersInInterfaceExplicitNotNullInDerived([CanBeNull] int? nullableInt, [CanBeNull] string optional = null);
            void ImplicitCanBeNullParametersInInterfaceExplicitNotNullInDerived(int? nullableInt, string optional = null);

            void ImplicitCanBeNullOptionalParameterInInterfaceExplicitNotNullInDerived(string optional = null);
            void ImplicitCanBeNullOptionalParameterInInterfaceImplicitNotNullInDerived(string optional = null);
            void ImplicitCanBeNullOptionalParameterInInterfaceImplicitNotNullOptionalParameterInDerived(string optional = null);
            void ExplicitCanBeNullOptionalParametersInInterfaceImplicitNotNullInDerived([CanBeNull] string optional = "default");

            void CanBeNullParameterInInterfaceExplicitNotNullInDerived([CanBeNull] string a);
            void CanBeNullParameterInInterfaceImplicitNotNullInDerived([CanBeNull] string a);
        }

        public class Implementation : IInterface
        {
            public string this[
                string a /*Expect:ImplicitNotNullConflictInHierarchy[Implicit]*/,
                string b /*Expect:ImplicitNotNullConflictInHierarchy[Implicit]*/]
            {
                get { return null; }
            }

            public void ExplicitCanBeNullParametersInInterfaceExplicitNotNullInDerived(
                [NotNull] /*Expect:AnnotationConflictInHierarchy*/ int? nullableInt,
                [NotNull] /*Expect:AnnotationConflictInHierarchy*/ string optional)
            {
            }

            public void ImplicitCanBeNullParametersInInterfaceExplicitNotNullInDerived(
                [NotNull] /*Expect:AnnotationConflictInHierarchy[Implicit]*/ int? nullableInt,
                [NotNull] /*Expect:AnnotationConflictInHierarchy[Implicit]*/ string optional)
            {
            }

            public void ImplicitCanBeNullOptionalParameterInInterfaceExplicitNotNullInDerived(
                [NotNull] /*Expect:AnnotationConflictInHierarchy[Implicit]*/ string optional)
            {
                // Here the explicit NotNull overrides the inherited implicit CanBeNull:
                ReSharper.TestValueAnalysis(optional, optional == null /*Expect:ConditionIsAlwaysTrueOrFalse*/);
            }

            public void ImplicitCanBeNullOptionalParameterInInterfaceImplicitNotNullInDerived(
                string optional /*Expect:ImplicitNotNullConflictInHierarchy[Implicit]*/)
            {
                // Here the implicit NotNull overrides the inherited implicit CanBeNull:
                ReSharper.TestValueAnalysis(optional, optional == null /*Expect:ConditionIsAlwaysTrueOrFalse[Implicit]*/);
            }

            public void ImplicitCanBeNullOptionalParameterInInterfaceImplicitNotNullOptionalParameterInDerived(
                string optional = "default" /*Expect:ImplicitNotNullConflictInHierarchy[Implicit]*/)
            {
            }

            public void ExplicitCanBeNullOptionalParametersInInterfaceImplicitNotNullInDerived(
                string optional = "default" /*Expect:ImplicitNotNullConflictInHierarchy[Implicit]*/)
            {
            }

            public void CanBeNullParameterInInterfaceExplicitNotNullInDerived([NotNull] /*Expect:AnnotationConflictInHierarchy*/ string a)
            {
                // Here the explicit NotNull overrides the inherited explicit CanBeNull:
                ReSharper.TestValueAnalysis(a, a == null) /*Expect:ConditionIsAlwaysTrueOrFalse*/;
            }

            public void CanBeNullParameterInInterfaceImplicitNotNullInDerived(string a /*Expect:ImplicitNotNullConflictInHierarchy[Implicit]*/)
            {
                // Here the inherited explicit CanBeBull overrides the implicit NotNull: 
                ReSharper.TestValueAnalysis(a /*Expect:AssignNullToNotNullAttribute*/, a == null);
            }
        }
    }
}