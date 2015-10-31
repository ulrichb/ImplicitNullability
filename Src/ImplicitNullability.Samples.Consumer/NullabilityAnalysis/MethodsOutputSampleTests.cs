using System;
using FluentAssertions;
using ImplicitNullability.Samples.CodeWithIN;
using ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis;
using NUnit.Framework;

namespace ImplicitNullability.Samples.Consumer.NullabilityAnalysis
{
    [TestFixture]
    public class MethodsOutputSampleTests
    {
        private MethodsOutputSample _instance;

        [SetUp]
        public void SetUp()
        {
            _instance = new MethodsOutputSample();
        }

        [Test]
        public void FunctionWithNonNullReturnValue()
        {
            Action act = () =>
            {
                var result = _instance.Function(returnValue: "");
                ReSharper.TestValueAnalysis(result, result == null /*Expect:ConditionIsAlwaysTrueOrFalse[MOut]*/);
            };

            act.ShouldNotThrow();
        }

        [Test]
        public void FunctionWithNullReturnValue()
        {
            Action act = () => _instance.Function(returnValue: null);

            act.ShouldThrow<InvalidOperationException>().WithMessage("[NullGuard] Return value * is null.");
        }

        [Test]
        public void FunctionWithCanBeNull_AndNullReturnValue()
        {
            Action act = () =>
            {
                var result = _instance.FunctionWithCanBeNull(returnValue: null);
                ReSharper.TestValueAnalysis(result /*Expect:AssignNullToNotNullAttribute*/, result == null);
            };

            act.ShouldNotThrow();
        }

        [Test]
        public void FunctionWithNullableInt_AndNullReturnValue()
        {
            Action act = () =>
            {
                int? outParam;
                var result = _instance.FunctionWithNullableInt(returnValue: null, outParam: out outParam);
                ReSharper.TestValueAnalysis(outParam /*Expect:AssignNullToNotNullAttribute*/, outParam == null);
                ReSharper.TestValueAnalysis(result /*Expect:AssignNullToNotNullAttribute*/, result == null);
            };

            act.ShouldNotThrow();
        }

        [Test]
        public void MethodWithOutParameterWithNullValue()
        {
            Action act = () =>
            {
                string outParam;
                _instance.MethodWithOutParameter(out outParam);
                ReSharper.TestValueAnalysis(outParam, outParam == null /*Expect:ConditionIsAlwaysTrueOrFalse[MOut]*/);
            };

            act.ShouldThrow<InvalidOperationException>().WithMessage("[NullGuard] Out parameter 'outParam' is null.");
        }

        [Test]
        public void MethodWithCanBeNullOutParameterWithNullValue()
        {
            string outParam;
            Action act = () => _instance.MethodWithCanBeNullOutParameter(out outParam);

            act.ShouldNotThrow();
        }
    }
}