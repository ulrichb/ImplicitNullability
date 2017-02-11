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
            Action act = () => IgnoreValue(_instance[""]);

            act.ShouldNotThrow();
        }

        [Test]
        public void IndexerGetterWithNullValue()
        {
            Action act = () => IgnoreValue(_instance[null /*Expect:AssignNullToNotNullAttribute[MIn]*/]);

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("a");
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
            Action act = () => IgnoreValue(_instance[canBeNull: null, nullableInt: null, optional: null]);

            act.ShouldNotThrow();
        }

        // ReSharper disable once UnusedParameter.Local
        private void IgnoreValue(object value)
        {
        }
    }
}
