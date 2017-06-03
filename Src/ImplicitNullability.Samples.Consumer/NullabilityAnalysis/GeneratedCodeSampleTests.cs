using System.Threading.Tasks;
using ImplicitNullability.Samples.CodeWithIN;
using ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis;
using NUnit.Framework;

// ReSharper disable ConvertToLocalFunction

namespace ImplicitNullability.Samples.Consumer.NullabilityAnalysis
{
    [TestFixture]
    public class GeneratedCodeSampleTests
    {
        [Test]
        public async Task TestGeneratedCodeOnType()
        {
            var sample = new GeneratedCodeSample.GeneratedCodeOnType(null /*Expect:AssignNullToNotNullAttribute[InclGenCode]*/);

            sample.Method(null /*Expect:AssignNullToNotNullAttribute[InclGenCode]*/);
            sample.MethodExplicit(null /*Expect:AssignNullToNotNullAttribute*/);
            ReSharper.TestValueAnalysis(sample.Function(), sample.Function() == null /*Expect:ConditionIsAlwaysTrueOrFalse[InclGenCode]*/);
            ReSharper.TestValueAnalysis(sample.Field, sample.Field == null /*Expect:ConditionIsAlwaysTrueOrFalse[InclGenCode]*/);
            ReSharper.TestValueAnalysis(sample.Property, sample.Property == null /*Expect:ConditionIsAlwaysTrueOrFalse[InclGenCode]*/);

            GeneratedCodeSample.GeneratedCodeOnType.SomeDelegate someDelegate = x => null /*Expect:AssignNullToNotNullAttribute[InclGenCode]*/;
            var someDelegateResult = someDelegate(null /*Expect:AssignNullToNotNullAttribute[InclGenCode]*/);
            ReSharper.TestValueAnalysis(someDelegateResult, someDelegateResult == null);

            var asyncFunctionResult = await sample.AsyncFunction();
            ReSharper.TestValueAnalysis(asyncFunctionResult, asyncFunctionResult == null /*Expect:ConditionIsAlwaysTrueOrFalse[InclGenCode]*/);
        }

        [Test]
        public async Task TestGeneratedCodeOnMember()
        {
            var sample = new GeneratedCodeSample.GeneratedCodeOnMember(null /*Expect:AssignNullToNotNullAttribute[InclGenCode]*/);

            sample.Method(null /*Expect:AssignNullToNotNullAttribute[InclGenCode]*/);
            sample.MethodExplicit(null /*Expect:AssignNullToNotNullAttribute*/);
            ReSharper.TestValueAnalysis(sample.Function(), sample.Function() == null /*Expect:ConditionIsAlwaysTrueOrFalse[InclGenCode]*/);
            ReSharper.TestValueAnalysis(sample.Field, sample.Field == null /*Expect:ConditionIsAlwaysTrueOrFalse[InclGenCode]*/);
            ReSharper.TestValueAnalysis(sample.Property, sample.Property == null /*Expect:ConditionIsAlwaysTrueOrFalse[InclGenCode]*/);

            GeneratedCodeSample.GeneratedCodeOnType.SomeDelegate someDelegate = x => null /*Expect:AssignNullToNotNullAttribute[InclGenCode]*/;
            var someDelegateResult = someDelegate(null /*Expect:AssignNullToNotNullAttribute[InclGenCode]*/);
            ReSharper.TestValueAnalysis(someDelegateResult, someDelegateResult == null);

            var asyncFunctionResult = await sample.AsyncFunction();
            ReSharper.TestValueAnalysis(asyncFunctionResult, asyncFunctionResult == null /*Expect:ConditionIsAlwaysTrueOrFalse[InclGenCode]*/);
        }

        [Test]
        public void TestTopLevelDelegates()
        {
            SomeTopLevelGeneratedCodeDelegate generatedCodeDelegate = x => null /*Expect:AssignNullToNotNullAttribute[InclGenCode]*/;
            var generatedCodeDelegateResult = generatedCodeDelegate(null /*Expect:AssignNullToNotNullAttribute[InclGenCode]*/);
            ReSharper.TestValueAnalysis(generatedCodeDelegateResult, generatedCodeDelegateResult == null);

            SomeTopLevelNonGeneratedCodeDelegate nonGeneratedCodeDelegate = x => null /*Expect:AssignNullToNotNullAttribute[MOut]*/;
            var nonGeneratedCodeDelegateResult = nonGeneratedCodeDelegate(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);
            ReSharper.TestValueAnalysis(nonGeneratedCodeDelegateResult,
                nonGeneratedCodeDelegateResult == null /* false negative, see DelegatesSampleTests */);
        }
    }
}
