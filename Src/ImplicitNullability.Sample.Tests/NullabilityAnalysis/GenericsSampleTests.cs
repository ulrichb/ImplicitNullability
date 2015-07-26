using System;
using FluentAssertions;
using ImplicitNullability.Sample.NullabilityAnalysis;
using NUnit.Framework;

namespace ImplicitNullability.Sample.Tests.NullabilityAnalysis
{
    [TestFixture]
    public class GenericsSampleTests
    {
        [Test]
        public void NoConstraintGenericWithDateTimeType_AndMethodWithDefaultOfTArgument()
        {
            var instance = new GenericsSample.NoConstraint<DateTime>();

            Action act = () => instance.Method(default(DateTime));

            act.ShouldNotThrow("the argument check should not perform a 'param == default(T)' comparison");
        }

        [Test]
        public void NoConstraintGenericWithNullableIntType_AndMethodWithNullArgument()
        {
            var instance = new GenericsSample.NoConstraint<int?>();

            Action act = () => instance.Method(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.ShouldThrow<ArgumentNullException>(
                "this is a known issue for generic types without constraint (it is a trade-off between this false positive " +
                "and a false-negative if T is a reference type (like in the StringTypeParameter_AndTestMethodNullArgument test case)")
                .And.ParamName.Should().Be("a");
        }

        [Test]
        public void NoConstraintGenericWithStringType_AndMethodWithNullArgument()
        {
            var instance = new GenericsSample.NoConstraint<string>();

            Action act = () => instance.Method(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("a");
        }

        [Test]
        public void ClassConstraintWithMethodWithNullArgument()
        {
            var instance = new GenericsSample.ClassConstraint<string>();

            Action act = () => instance.Method(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("a");
        }

        [Test]
        public void StructConstraintWithMethodWithDefaultOfTArgument()
        {
            var instance = new GenericsSample.StructConstraint<int>();

            Action act = () => instance.Method(0);

            act.ShouldNotThrow("the argument check should not perform a 'param == default(T)' comparison");
        }

        [Test]
        public void StructConstraintWithMethodWithNullableParameter_AndNullArgument()
        {
            var instance = new GenericsSample.StructConstraint<int>();

            Action act = () => instance.MethodWithNullableParameter(null);

            act.ShouldNotThrow("nullable value type parameters should not throw");
        }

        [Test]
        public void MethodWithoutConstraintWithStringType_AndNullArgument()
        {
            Action act = () => GenericsSample.GenericMethods.MethodWithoutConstraint((string)null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("a");
        }
    }
}