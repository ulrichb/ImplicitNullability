using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ImplicitNullability.Samples.CodeWithIN
{
    [NullGuard.NullGuard(NullGuard.ValidationFlags.None)]
    public static class Async
    {
        public static Task NopTask => Task.Delay(0);

        [ItemNotNull]
        public static async Task<T> NotNullResult<T>([NotNull] T value)
        {
            await Task.Delay(0);
            return value;
        }

        [ItemCanBeNull]
        public static async Task<T> CanBeNullResult<T>()
        {
            return await CanBeNullResult(default(T));
        }

        [ItemCanBeNull]
        public static async Task<T> CanBeNullResult<T>([CanBeNull] T returnValue)
        {
            await Task.Delay(0);
            return returnValue;
        }
    }
}
