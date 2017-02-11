using JetBrains.Annotations;

namespace ImplicitNullability.Samples.CodeWithIN.Highlighting.ImplicitNotNullConflictInHierarchy
{
    public class LongInheritanceChain
    {
        public class Base
        {
            public virtual void Method([CanBeNull] string a)
            {
                ReSharper.TestValueAnalysis(a /*Expect:AssignNullToNotNullAttribute*/, a == null);
            }

            [CanBeNull]
            public virtual string Function()
            {
                return null;
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
            public override void Method([CanBeNull] /*Expect:AnnotationRedundancyInHierarchy[not Implicit]*/ string a)
            {
            }

            [NotNull]
            public override string Function()
            {
                return "";
            }
        }

        public class DerivedBad : Base3
        {
            public override void Method(string a /*Expect:ImplicitNotNullConflictInHierarchy[Implicit]*/)
            {
            }

            public override string Function /*Expect:ImplicitNotNullElementCannotOverrideCanBeNull[Implicit]*/()
            {
                return null;
            }
        }

        public class DerivedBadAgain : DerivedBad
        {
            public override void Method(string a /*Expect:ImplicitNotNullConflictInHierarchy[Implicit]*/)
            {
            }

            public override string Function /*Expect:ImplicitNotNullElementCannotOverrideCanBeNull[Implicit]*/()
            {
                return null;
            }
        }
    }
}
