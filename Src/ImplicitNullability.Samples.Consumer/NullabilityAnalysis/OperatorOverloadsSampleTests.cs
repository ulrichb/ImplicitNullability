using System;
using FluentAssertions;
using ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis;
using NUnit.Framework;

namespace ImplicitNullability.Samples.Consumer.NullabilityAnalysis
{
    [TestFixture]
    public class OperatorOverloadsSampleTests
    {
        // REPORT? Although the nullability provides implicit NotNull for operator parameters (see the warnings in the operator method bodies),
        // R# shows no warning at the call site

        [Test]
        public void SimpleAddOperationWithLeftNullValue()
        {
            Func<object> act = () => null + new OperatorOverloadsSample.Simple();

            act.ToAction().Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("left");
        }

        [Test]
        public void SimpleAddOperationWithRightNullValue()
        {
            Func<object> act = () => new OperatorOverloadsSample.Simple() + null;

            act.ToAction().Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("right");
        }

        [Test]
        public void CanBeNullAddOperationWithLeftNullValue()
        {
            Func<object> act = () => null + new OperatorOverloadsSample.CanBeNull();

            act.ToAction().Should().NotThrow();
        }

        [Test]
        public void CanBeNullAddOperationWithRightNullValue()
        {
            Func<object> act = () => new OperatorOverloadsSample.CanBeNull() + null;

            act.ToAction().Should().NotThrow();
        }

        [Test]
        public void SimpleUnaryIncreaseOperationWithNullValue()
        {
            var value = (OperatorOverloadsSample.Simple) null;

            Func<object> act = () => value++;

            act.ToAction().Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("value");
        }

        [Test]
        public void CanBeNullUnaryIncreaseOperationWithNullValue()
        {
            var value = (OperatorOverloadsSample.CanBeNull) null;

            Func<object> act = () => value++;

            act.ToAction().Should().NotThrow();
        }

        [Test]
        public void NotNullReturnValueWithNullValue()
        {
            var value = new OperatorOverloadsSample.NotNullReturnValue();

            Func<object> act = () => value++;

            act.ToAction().Should().Throw<InvalidOperationException>().WithMessage("[NullGuard] Return value of *op_Increment* is null.");
        }
    }
}
