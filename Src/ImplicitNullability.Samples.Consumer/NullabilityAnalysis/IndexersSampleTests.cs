using System;
using FluentAssertions;
using ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis;
using NUnit.Framework;

namespace ImplicitNullability.Samples.Consumer.NullabilityAnalysis
{
    [TestFixture]
    public class IndexersSampleTests
    {
        private IndexersSample _instance;

        [SetUp]
        public void SetUp()
        {
            _instance = new IndexersSample();
        }

        [Test]
        public void IndexerSetterWithNonNullValue()
        {
            Action act = () => _instance[""] = null;

            act.ShouldNotThrow();
        }

        [Test]
        public void IndexerSetterWithNullValue()
        {
            Action act = () => _instance[null /*Expect:AssignNullToNotNullAttribute[MIn]*/] = null;

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("a");
        }

        [Test]
        public void IndexerGetterWithNonNullValue()
        {
            Func<string> act = () => _instance[""];

            act.ToAction().ShouldNotThrow();
        }

        [Test]
        public void IndexerGetterWithNullValue()
        {
            Func<string> act = () => _instance[null /*Expect:AssignNullToNotNullAttribute[MIn]*/];

            act.ToAction().ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("a");
        }

        [Test]
        public void IndexerSetterWithNullableParameter()
        {
            Action act = () => _instance[canBeNull: null, nullableInt: null, optional: null] = null;

            act.ShouldNotThrow();
        }

        [Test]
        public void IndexerGetterWithNullableParameter()
        {
            Func<string> act = () => _instance[canBeNull: null, nullableInt: null, optional: null];

            act.ToAction().ShouldNotThrow();
        }
    }
}
