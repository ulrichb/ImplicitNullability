using System;
using JetBrains.Annotations;

namespace ImplicitNullability.Sample.Highlighting.ImplicitNotNullConflictInHierarchy
{
    public class ExplicitInterfaceImplementation
    {
        public interface IInterface
        {
            void Method([CanBeNull] string a);

            string Function();
        }

        public class Implementation : IInterface
        {
            void IInterface.Method(string a /*Expect:ImplicitNotNullConflictInHierarchy*/)
            {
            }

            [CanBeNull] /*Expect:AnnotationConflictInHierarchy*/
            string IInterface.Function()
            {
                return null;
            }
        }
    }
}