using System;
using JetBrains.Annotations;

// ReSharper disable UnusedMember.Global, UnusedParameter.Global
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
            public delegate string SomeDelegate(string a, ref string refParam, out string outParam, params object[] values);

            [CanBeNull]
            public delegate string SomeNullableDelegate([CanBeNull] string a);
        }

        public class Fields
        {
            [NotNull]
            public string SomeField;

            public readonly string SomeReadonlyField = "";

            [CanBeNull]
            public string SomeCanBeNullField;
        }

        public abstract class PropertiesBase
        {
            [NotNull]
            public abstract string VirtualPropertyWithExplicitNullabilityInBase { get; set; }
        }

        // Atm. IN doesn't support properties/indexers

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

        public interface IInterface
        {
            string Method(string a);

            [CanBeNull]
            string MethodWithCanBeNull([CanBeNull] string a);

            [NotNull]
            string SomeProperty { get; set; }

            [CanBeNull]
            string SomeCanBeNullProperty { get; set; }

            [NotNull]
            string SomeGetterOnlyProperty { get; }
        }
    }
}
