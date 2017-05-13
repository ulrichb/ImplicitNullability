using System;

namespace ImplicitNullability.Samples.Consumer
{
    /// <summary>
    /// Workaround for https://github.com/fluentassertions/fluentassertions/issues/422.
    /// </summary>
    public static class FluentAssertionsExtensions
    {
        public static Action ToAction(this Func<object> func)
        {
            return () => func();
        }
    }
}
