using System;
using JetBrains.Annotations;

namespace ImplicitNullability.Samples.CodeWithIN.TypeHighlighting
{
    public class TypeHighlightingOtherElementsSample
    {
        public class Constructor
        {
            public Constructor(string a, [CanBeNull] string b)
            {
                Console.WriteLine(a + b);
            }
        }

        public delegate string SomeDelegate(string a);

        [CanBeNull]
        public delegate string SomeNullableDelegate([CanBeNull] string a);
    }
}