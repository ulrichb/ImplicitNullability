namespace ImplicitNullability.Samples.CodeWithIN.Special.NullabilityAnalysis.GeneratedCode
{
    public class SomeResourcesConsumer
    {
        public void Consume()
        {
            ReSharper.TestValueAnalysis(SomeResources.Culture, SomeResources.Culture == null /*Expect:ConditionIsAlwaysTrueOrFalse[InclGenCode]*/);

            var resourceManager = SomeResources.ResourceManager;
            ReSharper.TestValueAnalysis(resourceManager, resourceManager == null /*Expect:ConditionIsAlwaysTrueOrFalse[InclGenCode]*/);

            ReSharper.TestValueAnalysis(SomeResources.StringA, SomeResources.StringA == null /*Expect:ConditionIsAlwaysTrueOrFalse[InclGenCode]*/);
        }
    }
}
