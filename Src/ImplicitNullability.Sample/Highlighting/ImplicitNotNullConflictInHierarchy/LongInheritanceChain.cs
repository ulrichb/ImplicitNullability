using System;
using JetBrains.Annotations;

namespace ImplicitNullability.Sample.Highlighting.ImplicitNotNullConflictInHierarchy
{
    public class LongInheritanceChain
    {
        public class Base
        {
            public virtual void Method([CanBeNull] string a)
            {
                ReSharper.TestValueAnalysis(a /*Expect:AssignNullToNotNullAttribute*/, a == null);
            }

            public virtual string Function()
            {
                return "";
            }
        }

        public class Base2 : Base
        {
        }

        public class Base3 : Base2
        {
        }

        public class DerivedGood : Base3
        {
            // This documents that the hierarchy conflict warnings compete with AnnotationRedundancyInHierarchy

            public override void Method([CanBeNull] /*Expect:AnnotationRedundancyInHierarchy[RS >= 9]*/ string a)
            {
            }

            [NotNull] /*Expect:AnnotationRedundancyInHierarchy[Implicit && RS >= 9]*/
            public override string Function()
            {
                return "";
            }
        }

        public class DerivedBad : Base3
        {
            public override void Method(string a /*Expect:ImplicitNotNullConflictInHierarchy[Implicit]*/)
            {
                // Note that ReSharper takes the inherited [CanBeNull]:
                ReSharper.TestValueAnalysis(a /*Expect:AssignNullToNotNullAttribute*/, a == null);
            }

            [CanBeNull] /*Expect:AnnotationConflictInHierarchy[Implicit]*/
            public override string Function()
            {
                return null;
            }
        }

        public class DerivedBadAgain : DerivedBad
        {
            public override void Method(string a /*Expect:ImplicitNotNullConflictInHierarchy[Implicit]*/)
            {
                // Note that ReSharper takes the inherited [CanBeNull]:
                ReSharper.TestValueAnalysis(a /*Expect:AssignNullToNotNullAttribute*/, a == null);
            }
        }
    }
}