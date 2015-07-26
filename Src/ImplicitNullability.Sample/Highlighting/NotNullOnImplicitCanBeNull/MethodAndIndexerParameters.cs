using System;
using JetBrains.Annotations;

// ReSharper disable AssignNullToNotNullAttribute - because in R# 9+ this hides NotNullOnImplicitCanBeNull on the 'optional = null' parameters

namespace ImplicitNullability.Sample.Highlighting.NotNullOnImplicitCanBeNull
{
    public class MethodAndIndexerParameters
    {
        public string this[
            [NotNull] int? nullableInt /*Expect:NotNullOnImplicitCanBeNull*/,
            [NotNull] string optional = null /*Expect:NotNullOnImplicitCanBeNull*/]
        {
            get { return null; }
        }

        public void TestMethod(
            [NotNull] int? nullableInt /*Expect:NotNullOnImplicitCanBeNull*/,
            [NotNull] string optional = null /*Expect:NotNullOnImplicitCanBeNull*/)
        {
        }
    }
}