using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ImplicitNullability.Sample.Highlighting.ImplicitNotNullConflictInHierarchy
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
                await Task.Delay(0);
                return "";
            }

            // Prove that the "non-ItemCanBeNull version" of this warning *is* emitted for non-async Task<T> methods:
            public override Task<string> CanBeNull_WithOverridingNonAsyncMethod
                /*Expect:ImplicitNotNullElementCannotOverrideCanBeNull[Implicit]*/()
            {
                return null;
            }

            public override async Task<string> ItemCanBeNull_WithOverridingAsyncMethod
                /*Expect:ImplicitNotNullElementCannotOverrideCanBeNull[RS >= 92 && Implicit]*/()
            {
                await Task.Delay(0);
                return null;
            }

            public override Task<string> ItemCanBeNull_WithOverridingNonAsyncMethod
                /*Expect:ImplicitNotNullElementCannotOverrideCanBeNull[RS >= 92 && Implicit]*/()
            {
                return Task.FromResult<string>(null);
            }
        }
    }
}