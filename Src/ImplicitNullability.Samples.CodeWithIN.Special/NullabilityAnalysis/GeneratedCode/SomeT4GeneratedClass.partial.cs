using JetBrains.Annotations;

namespace ImplicitNullability.Samples.CodeWithIN.Special.NullabilityAnalysis.GeneratedCode
{
    public partial class SomeT4GeneratedClass
    {
        partial void PartialMethodWithImplementation(string a)
        {
            ReSharper.TestValueAnalysis(a, a == null /*Expect:ConditionIsAlwaysTrueOrFalse[MIn]*/);
        }

        partial void PartialMethodWithCanBeNullInImplementation([CanBeNull] string a)
        {
            ReSharper.TestValueAnalysis(a /*Expect:AssignNullToNotNullAttribute*/, a == null);
        }

        public void Consume()
        {
            Method(null /*Expect:AssignNullToNotNullAttribute[InclGenCode]*/);
            MethodExplicit(null /*Expect:AssignNullToNotNullAttribute*/);

            //

            PartialMethodWithImplementation(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);
            PartialMethodWithCanBeNullInImplementation(null);
            // ReSharper disable once InvocationIsSkipped
            PartialMethodWithoutImplementation(null);
        }
    }
}
