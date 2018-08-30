using System;
using FluentAssertions;
using ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis;
using NUnit.Framework;

namespace ImplicitNullability.Samples.Consumer.NullabilityAnalysis
{
    [TestFixture]
    public class ManualNullChecksSampleTests
    {
        private ManualNullChecksSample.Derived _instance;

        [SetUp]
        public void SetUp()
        {
            _instance = new ManualNullChecksSample.Derived();
        }

        [Test]
        public void PreconditionIfCheck_WithNonNullValue()
        {
            Action act = () => _instance.PreconditionIfCheck("");

            act.Should().NotThrow();
        }

        [Test]
        public void PreconditionIfCheck_WithNullValue()
        {
            Action act = () => _instance.PreconditionIfCheck(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("a");
        }

        [Test]
        public void PreconditionExpressionCheck_WithNonNullValue()
        {
            Action act = () => _instance.PreconditionExpressionCheck("");

            act.Should().NotThrow();
        }

        [Test]
        public void PreconditionExpressionCheck_WithNullValue()
        {
            Action act = () => _instance.PreconditionExpressionCheck(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("a");
        }
    }
}
