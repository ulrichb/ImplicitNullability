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

        public class Properties : PropertiesBase
        {
            public string SomeProperty { get; set; }

            [CanBeNull]
            public string SomeCanBeNullProperty { get; set; }

            public string SomeExpresssionBodyProperty => "";

            public string SomeGetterOnlyProperty { get; } = "";

            public override string VirtualPropertyWithExplicitNullabilityInBase { get; set; }
        }

        public class Indexers
        {
            public string this[string a] => "";

            [CanBeNull]
            public string this[[CanBeNull] object b] => "";
        }

        public interface IInterface
        {
            string Method(string a);

            [CanBeNull]
            string MethodWithCanBeNull([CanBeNull] string a);

            string SomeProperty { get; set; }

            [CanBeNull]
            string SomeCanBeNullProperty { get; set; }

            string SomeGetterOnlyProperty { get; }
        }
    }
}
