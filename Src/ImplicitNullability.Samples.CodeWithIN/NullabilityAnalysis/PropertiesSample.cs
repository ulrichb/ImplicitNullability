using JetBrains.Annotations;

namespace ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis
{
    public class PropertiesSample
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
}
