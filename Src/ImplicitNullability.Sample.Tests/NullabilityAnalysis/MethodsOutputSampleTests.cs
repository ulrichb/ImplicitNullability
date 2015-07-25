using System;
using FluentAssertions;
using ImplicitNullability.Sample.NullabilityAnalysis;
using NUnit.Framework;

namespace ImplicitNullability.Sample.Tests.NullabilityAnalysis
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
                var result = _instance.Function(returnValue: "a");
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
                var result = _instance.FunctionWithNullableInt(returnValue: null);
                ReSharper.TestValueAnalysis(result /*Expect:AssignNullToNotNullAttribute*/, result == null);
            };

            act.ShouldNotThrow();
        }
    }
}