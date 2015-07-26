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
        public void FunctionWithNullReturnValue()
        {
            Action act = () => _instance.Function(null);

            act.ShouldNotThrow("at the moment, method return values should not be guarded");
        }

        [Test]
        public void FunctionWithCanBeNullResultAndNullReturnValue()
        {
            Action act = () => _instance.FunctionWithCanBeNullResult(null);

            act.ShouldNotThrow();
        }
    }
}