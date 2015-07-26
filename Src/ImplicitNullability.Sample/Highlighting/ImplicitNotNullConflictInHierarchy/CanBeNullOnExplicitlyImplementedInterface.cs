using System;
using JetBrains.Annotations;

namespace ImplicitNullability.Sample.Highlighting.ImplicitNotNullConflictInHierarchy
{
    public class CanBeNullOnExplicitlyImplementedInterface
    {
        public interface IInterface
        {
            void TestMethod([CanBeNull] string a);
        }

        public class Implementation : IInterface
        {
            void IInterface.TestMethod(string a /*Expect:ImplicitNotNullConflictInHierarchy*/)
            {
            }
        }
    }
}