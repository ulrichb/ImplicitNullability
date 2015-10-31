using System;
using FluentAssertions;
using ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis;
using JetBrains.Annotations;
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
            Action act = () => IgnoreValue(null + new OperatorOverloadsSample.Simple());

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("left");
        }

        [Test]
        public void SimpleAddOperationWithRightNullValue()
        {
            Action act = () => IgnoreValue(new OperatorOverloadsSample.Simple() + null);

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("right");
        }

        [Test]
        public void CanBeNullAddOperationWithLeftNullValue()
        {
            Action act = () => IgnoreValue(null + new OperatorOverloadsSample.CanBeNull());

            act.ShouldNotThrow();
        }

        [Test]
        public void CanBeNullAddOperationWithRightNullValue()
        {
            Action act = () => IgnoreValue(new OperatorOverloadsSample.CanBeNull() + null);

            act.ShouldNotThrow();
        }

        [Test]
        public void SimpleUnaryIncreaseOperationWithNullValue()
        {
            var value = (OperatorOverloadsSample.Simple) null;
            Action act = () => IgnoreValue(value++);

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("value");
        }

        [Test]
        public void CanBeNullUnaryIncreaseOperationWithNullValue()
        {
            var value = (OperatorOverloadsSample.CanBeNull) null;
            Action act = () => IgnoreValue(value++);

            act.ShouldNotThrow();
        }

        [Test]
        public void NotNullReturnValueWithNullValue()
        {
            var value = new OperatorOverloadsSample.NotNullReturnValue();
            Action act = () => IgnoreValue(value++);

            act.ShouldThrow<InvalidOperationException>().WithMessage("[NullGuard] Return value of *op_Increment* is null.");
        }

        // ReSharper disable once UnusedParameter.Local
        private void IgnoreValue([CanBeNull] object value)
        {
        }
    }
}