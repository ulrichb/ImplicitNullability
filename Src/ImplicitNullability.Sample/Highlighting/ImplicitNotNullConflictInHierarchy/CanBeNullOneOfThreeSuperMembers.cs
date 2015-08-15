using System;
using JetBrains.Annotations;

namespace ImplicitNullability.Sample.Highlighting.ImplicitNotNullConflictInHierarchy
{
    public class CanBeNullOneOfThreeSuperMembers
    {
        private interface IInterface1
        {
            void Method(string a);
        }

        private interface IInterface2
        {
            void Method([CanBeNull] string a);
        }

        private interface IInterface3
        {
            void Method(string a);
        }

        public class Implementation : IInterface1, IInterface2, IInterface3
        {
            public void Method(string a /*Expect:ImplicitNotNullConflictInHierarchy[Implicit]*/)
            {
            }
        }
    }
}