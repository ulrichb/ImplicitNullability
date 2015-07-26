using System;
using FluentAssertions;
using ImplicitNullability.Sample.NullabilityAnalysis;
using NUnit.Framework;

namespace ImplicitNullability.Sample.Tests.NullabilityAnalysis
{
    [TestFixture]
    public class ConstructorsSampleTests
    {
        [Test]
        public void CtorWithNotNullArgument()
        {
            Action act = () => new ConstructorsSample("a");

            act.ShouldNotThrow();
        }

        [Test]
        public void CtorWithViolatedNotNullArgument()
        {
            Action act = () => new ConstructorsSample(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("a");
        }

        [Test]
        public void CtorWithOverriddenOptionalArgument()
        {
            Action act = () => new ConstructorsSample("s", optional: "overridden default");

            act.ShouldNotThrow();
        }

        [Test]
        public void CtorWithViolatedBothNotNullArguments()
        {
            Action act =
                () => new ConstructorsSample(null /*Expect:AssignNullToNotNullAttribute[MIn]*/, optional: null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("a", "the evaluation of the arguments should start from left");
        }

        [Test]
        public void CtorWithViolatedNotNullOptionalArgument()
        {
            Action act = () => new ConstructorsSample("a", optional: null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("optional");
        }

        [Test]
        public void CtorWithCanBeNullValues()
        {
            Action act = () => new ConstructorsSample(canBeNull: null, nullableInt: null, optional: null);

            act.ShouldNotThrow();
        }
    }
}