using System;
using JetBrains.Annotations;

namespace ImplicitNullability.Sample.Highlighting.ImplicitNotNullConflictInHierarchy
{
    public class LongInheritanceChain
    {
        public class Base1
        {
            public virtual void Method([CanBeNull] string a)
            {
            }

            public virtual string Function()
            {
                return "";
            }
        }

        public class Base2 : Base1
        {
        }

        public class Base3 : Base2
        {
        }

        public class Base4 : Base3
        {
        }

        public class Derived : Base4
        {
            public override void Method(string a /*Expect:ImplicitNotNullConflictInHierarchy*/)
            {
            }

            [CanBeNull] /*Expect:AnnotationConflictInHierarchy*/
            public override string Function()
            {
                return null;
            }
        }
    }
}