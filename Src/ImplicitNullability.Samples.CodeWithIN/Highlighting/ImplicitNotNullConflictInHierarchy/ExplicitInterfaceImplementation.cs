using System;
using JetBrains.Annotations;

namespace ImplicitNullability.Samples.CodeWithIN.Highlighting.ImplicitNotNullConflictInHierarchy
{
    public class ExplicitInterfaceImplementation
    {
        public interface IInterface
        {
            void Method([CanBeNull] string a);

            [CanBeNull]
            string Function();
        }

        public class Implementation : IInterface
        {
            void IInterface.Method(string a /*Expect:ImplicitNotNullConflictInHierarchy[Implicit]*/)
            {
            }

            string IInterface.Function /*Expect:ImplicitNotNullElementCannotOverrideCanBeNull[Implicit]*/()
            {
                return null;
            }
        }
    }
}