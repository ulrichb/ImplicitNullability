using JetBrains.Annotations;

// ReSharper disable EmptyConstructor

namespace ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis
{
    public static class PropertiesSample
    {
        public class Properties
        {
            // Proves that (at the moment) properties should be excluded

            public string SomeProperty
            {
                get { return null; }
                set { ReSharper.TestValueAnalysis(value, value == null); }
            }

            public string SomeAutoProperty { get; set; }

            [CanBeNull]
            public string SomeCanBeNullProperty { get; set; }
        }

        public static class PropertiesInClassWithCtor
        {
            // Regression samples for issue #10

            public class AutoProperty
            {
                public string SomeAutoProperty { get; set; }

                public AutoProperty /*Expect no warning*/()
                {
                }
            }

            public class GetterOnlyProperty
            {
                public string SomeGetterOnlyProperty { get; }

                public GetterOnlyProperty /*Expect no warning*/()
                {
                    SomeGetterOnlyProperty = "";
                }
            }
        }
    }
}
