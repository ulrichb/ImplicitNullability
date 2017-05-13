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

            act.ToAction().ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("left");
        }

        [Test]
        public void SimpleAddOperationWithRightNullValue()
        {
            Func<object> act = () => new OperatorOverloadsSample.Simple() + null;

            act.ToAction().ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("right");
        }

        [Test]
        public void CanBeNullAddOperationWithLeftNullValue()
        {
            Func<object> act = () => null + new OperatorOverloadsSample.CanBeNull();

            act.ToAction().ShouldNotThrow();
        }

        [Test]
        public void CanBeNullAddOperationWithRightNullValue()
        {
            Func<object> act = () => new OperatorOverloadsSample.CanBeNull() + null;

            act.ToAction().ShouldNotThrow();
        }

        [Test]
        public void SimpleUnaryIncreaseOperationWithNullValue()
        {
            var value = (OperatorOverloadsSample.Simple) null;

            Func<object> act = () => value++;

            act.ToAction().ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("value");
        }

        [Test]
        public void CanBeNullUnaryIncreaseOperationWithNullValue()
        {
            var value = (OperatorOverloadsSample.CanBeNull) null;

            Func<object> act = () => value++;

            act.ToAction().ShouldNotThrow();
        }

        [Test]
        public void NotNullReturnValueWithNullValue()
        {
            var value = new OperatorOverloadsSample.NotNullReturnValue();

            Func<object> act = () => value++;

            act.ToAction().ShouldThrow<InvalidOperationException>().WithMessage("[NullGuard] Return value of *op_Increment* is null.");
        }
    }
}
