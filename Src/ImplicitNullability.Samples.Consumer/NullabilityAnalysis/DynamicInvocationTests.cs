using System;
using FluentAssertions;
using ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis;
using NUnit.Framework;

namespace ImplicitNullability.Samples.Consumer.NullabilityAnalysis
{
    [TestFixture]
    public class DynamicInvocationTests
    {
        private dynamic _methodsSample;
        private dynamic _indexersSample;

        [SetUp]
        public void SetUp()
        {
            _methodsSample = new MethodsInputSample();
            _indexersSample = new IndexersSample();
        }

        [Test]
        public void MethodInvocation_WithNullValue()
        {
            Action act = () => _methodsSample.Method(null /* no warning because dynamic invocations are excluded */);

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("a");
        }

        [Test]
        public void IndexerSetterInvocation_WithNullValue()
        {
            Action act = () => _indexersSample[null /* no warning because dynamic invocations are excluded */] = "some value";

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("a");
        }
    }
}
