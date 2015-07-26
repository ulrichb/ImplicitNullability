using System;
using FluentAssertions;
using ImplicitNullability.Sample.NullabilityAnalysis;
using NUnit.Framework;

namespace ImplicitNullability.Sample.Tests.NullabilityAnalysis
{
    [TestFixture]
    public class DynamicInvocationTests
    {
        private dynamic _methodsSample;
        private dynamic _indexersSample;

        [SetUp]
        public void SetUp()
        {
            _methodsSample = new MethodsSample();
            _indexersSample = new IndexersSample();
        }

        [Test]
        public void TestProcedureWithNullValue()
        {
            Action act = () => _methodsSample.TestProcedure(null /* no warning because dynamic invocations should be excluded */);

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("a");
        }

        [Test]
        public void IndexerSetterWithNullValue()
        {
            Action act = () => _indexersSample[null /* no warning because dynamic invocations should be excluded */] = null;

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("a");
        }
    }
}