using System;
using FluentAssertions;
using ImplicitNullability.Samples.CodeWithIN;
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
            Action act = () => _instance[""] = "";

            act.Should().NotThrow();
        }

        [Test]
        public void IndexerSetterWithNullParameter()
        {
            Action act = () => _instance[null /*Expect:AssignNullToNotNullAttribute[MIn]*/] = "";

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("a");
        }

        [Test]
        public void IndexerSetterWithNullValue()
        {
            Action act = () => _instance[""] = null /*Expect:AssignNullToNotNullAttribute[Prps && !RtGo]*/;

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("value");
        }

        [Test]
        public void IndexerGetterWithNonNullParameterAndResult()
        {
            Action act = () =>
            {
                var value = _instance[""];
                ReSharper.TestValueAnalysis(value, value == null /*Expect:ConditionIsAlwaysTrueOrFalse[Prps && !RtGo]*/);
            };

            act.Should().NotThrow();
        }

        [Test]
        public void IndexerGetterWithNullParameter()
        {
            Func<object> act = () => _instance[null /*Expect:AssignNullToNotNullAttribute[MIn]*/];

            act.ToAction().Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("a");
        }

        [Test]
        public void IndexerGetterWithNullResult()
        {
            Func<object> act = () => _instance["<return null>"];

            act.ToAction().Should().Throw<InvalidOperationException>().WithMessage("[NullGuard] Return value *Item* is null.");
        }

        //

        [Test]
        public void IndexerSetterWithNullableParameters()
        {
            Action act = () => _instance[canBeNull: null, nullableInt: null, optional: null] = null;

            act.Should().NotThrow();
        }

        [Test]
        public void IndexerGetterWithNullableParametersAndResult()
        {
            Action act = () =>
            {
                var value = _instance[canBeNull: null, nullableInt: null, optional: null];
                ReSharper.TestValueAnalysis(value /*Expect:AssignNullToNotNullAttribute*/, value == null);
            };

            act.Should().NotThrow();
        }
    }
}
