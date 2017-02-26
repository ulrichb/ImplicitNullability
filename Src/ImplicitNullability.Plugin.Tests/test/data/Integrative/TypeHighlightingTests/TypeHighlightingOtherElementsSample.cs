using System;
using JetBrains.Annotations;

// ReSharper disable NotNullMemberIsNotInitialized

namespace ImplicitNullability.Plugin.Tests.test.data.Integrative.TypeHighlightingTests
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

        public class Delegates
        {
            public delegate string SomeDelegate(string a);

            [CanBeNull]
            public delegate string SomeNullableDelegate([CanBeNull] string a);
        }

        // Atm. IN doesn't support fields & properties

        public class Fields
        {
            [NotNull]
            public string SomeField;

            [CanBeNull]
            public string SomeCanBeNullField;
        }

        public abstract class PropertiesBase
        {
            [NotNull]
            public abstract string VirtualPropertyWithExplicitNullabilityInBase { get; set; }
        }

        public class Properties : PropertiesBase
        {
            [NotNull]
            public string SomeProperty { get; set; }

            [CanBeNull]
            public string SomeCanBeNullProperty { get; set; }

            [NotNull]
            public string SomeExpresssionBodyProperty => "";

            [NotNull]
            public string SomeGetterOnlyProperty { get; } = "";

            public override string VirtualPropertyWithExplicitNullabilityInBase { get; set; }
        }

        public class Indexers
        {
            [NotNull]
            public string this[string a] => "";

            [CanBeNull]
            public string this[[CanBeNull] object b] => "";
        }
    }
}
