using System;
using JetBrains.Annotations;

namespace ImplicitNullability.Sample.Highlighting.ImplicitNotNullConflictInHierarchy
{
    public class CanBeNullOnExplicitlyImplementedInterface
    {
        public interface IInterface
        {
            void Method([CanBeNull] string a);
        }

        public class Implementation : IInterface
        {
            void IInterface.Method(string a /*Expect:ImplicitNotNullConflictInHierarchy*/)
            {
            }
        }
    }
}