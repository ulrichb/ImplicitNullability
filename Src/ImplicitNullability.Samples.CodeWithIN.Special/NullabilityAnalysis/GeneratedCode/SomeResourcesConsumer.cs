namespace ImplicitNullability.Samples.CodeWithIN.Special.NullabilityAnalysis.GeneratedCode
{
    public class SomeResourcesConsumer
    {
        public void Consume()
        {
            // PROP_SUPPORT: Generated .resx members?
            ReSharper.TestValueAnalysis(SomeResources.Culture, SomeResources.Culture == null);
            ReSharper.TestValueAnalysis(SomeResources.ResourceManager, SomeResources.ResourceManager == null);
            ReSharper.TestValueAnalysis(SomeResources.StringA, SomeResources.StringA == null);
        }
    }
}
