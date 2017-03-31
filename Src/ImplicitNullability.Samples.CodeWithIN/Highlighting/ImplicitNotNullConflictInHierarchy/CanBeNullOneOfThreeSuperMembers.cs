using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ImplicitNullability.Samples.CodeWithIN.Highlighting.ImplicitNotNullConflictInHierarchy
{
    public class CanBeNullOneOfThreeSuperMembers
    {
        private interface IInterface1
        {
            void Method(string a);
            string Function();
            Task<string> AsyncFunction();
        }

        private interface IInterface2
        {
            void Method([CanBeNull] string a);

            [CanBeNull]
            string Function();

            [ItemCanBeNull]
            Task<string> AsyncFunction();
        }

        private interface IInterface3
        {
            void Method(string a);
            string Function();
            Task<string> AsyncFunction();
        }

        public class Implementation : IInterface1, IInterface2, IInterface3
        {
            public void Method(string a /*Expect:ImplicitNotNullConflictInHierarchy[Implicit]*/)
            {
            }

            public string Function /*Expect:ImplicitNotNullElementCannotOverrideCanBeNull[Implicit]*/()
            {
                return null;
            }

            public async Task<string> AsyncFunction /*Expect:ImplicitNotNullElementCannotOverrideCanBeNull[Implicit]*/()
            {
                await Task.Delay(0);
                return null;
            }
        }
    }
}
