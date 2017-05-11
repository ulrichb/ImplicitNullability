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

            act.ShouldNotThrow();
        }

        [Test]
        public void PreconditionIfCheck_WithNullValue()
        {
            Action act = () => _instance.PreconditionIfCheck(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("a");
        }

        [Test]
        public void PreconditionExpressionCheck_WithNonNullValue()
        {
            Action act = () => _instance.PreconditionExpressionCheck("");

            act.ShouldNotThrow();
        }

        [Test]
        public void PreconditionExpressionCheck_WithNullValue()
        {
            Action act = () => _instance.PreconditionExpressionCheck(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("a");
        }
    }
}
