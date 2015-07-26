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
        public void NoConstraintGenericWithDateTimeType_AndFunctionWithDefaultOfTReturnValue()
        {
            var instance = new GenericsSample.NoConstraint<DateTime>();

            Action act = () => instance.Function(returnValue: default(DateTime));

            act.ShouldNotThrow("the return value check should not perform a 'param == default(T)' comparison");
        }

        [Test]
        public void NoConstraintGenericWithNullableIntType_AndMethodWithNullArgument()
        {
            var instance = new GenericsSample.NoConstraint<int?>();

            Action act = () => instance.Method(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.ShouldThrow<ArgumentNullException>(
                "known issue for generics without constraint (it's a trade-off between this false positive and a false-negative if T is a reference type")
                .And.ParamName.Should().Be("a");
        }

        [Test]
        public void NoConstraintGenericWithNullableIntType_AndFunctionWithNullReturnValue()
        {
            var instance = new GenericsSample.NoConstraint<int?>();

            Action act = () => instance.Function(returnValue: null);

            act.ShouldThrow<InvalidOperationException>().WithMessage(
                "[NullGuard] Return value * is null.",
                "known issue for generics without constraint (it's a trade-off between this false positive and a false-negative if T is a reference type");
        }

        [Test]
        public void NoConstraintGenericWithStringType_AndMethodWithNullArgument()
        {
            var instance = new GenericsSample.NoConstraint<string>();

            Action act = () => instance.Method(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("a");
        }

        [Test]
        public void NoConstraintGenericWithStringType_AndFunctionWithNullReturnValue()
        {
            var instance = new GenericsSample.NoConstraint<string>();

            Action act = () =>
            {
                var result = instance.Function(returnValue: null);
                ReSharper.TestValueAnalysis(result, result == null /*Expect:ConditionIsAlwaysTrueOrFalse[MOut]*/);
            };

            act.ShouldThrow<InvalidOperationException>().WithMessage("[NullGuard] Return value * is null.");
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
            Action act = () => GenericsSample.GenericMethods.MethodWithoutConstraint((string) null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("a");
        }
    }
}