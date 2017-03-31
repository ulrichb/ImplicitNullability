using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ImplicitNullability.Samples.CodeWithIN.Highlighting.ImplicitNotNullConflictInHierarchy
{
    public static class AsyncMethods
    {
        public abstract class Base
        {
            [CanBeNull]
            public abstract Task<string> CanBeNull_WithOverridingAsyncMethod();

            [CanBeNull]
            public abstract Task<string> CanBeNull_WithOverridingNonAsyncMethod();

            [ItemCanBeNull]
            public abstract Task<string> ItemCanBeNull_WithOverridingAsyncMethod();

            [ItemCanBeNull]
            public abstract Task<string> ItemCanBeNull_WithOverridingNonAsyncMethod();
        }

        public class Derived : Base
        {
            // Prove that the "non-ItemCanBeNull version" of this warning *is not* emitted for async Task<T> methods:
            public override async Task<string> CanBeNull_WithOverridingAsyncMethod /*Expect no warning*/()
            {
                return await Async.NotNullResult("");
            }

            // Prove that the "non-ItemCanBeNull version" of this warning *is* emitted for non-async Task<T> methods:
            public override Task<string> CanBeNull_WithOverridingNonAsyncMethod
                /*Expect:ImplicitNotNullElementCannotOverrideCanBeNull[Implicit]*/()
            {
                return null;
            }

            public override async Task<string> ItemCanBeNull_WithOverridingAsyncMethod
                /*Expect:ImplicitNotNullElementCannotOverrideCanBeNull[Implicit]*/()
            {
                return await Async.CanBeNullResult<string>();
            }

            public override Task<string> ItemCanBeNull_WithOverridingNonAsyncMethod
                /*Expect:ImplicitNotNullElementCannotOverrideCanBeNull[Implicit]*/()
            {
                return Async.CanBeNullResult<string>();
            }
        }
    }
}
