using System;
using FluentAssertions;
using ImplicitNullability.Sample.NullabilityAnalysis;
using NUnit.Framework;

namespace ImplicitNullability.Sample.Tests.NullabilityAnalysis
{
    [TestFixture]
    public class SampleGenericClassWithStructConstraintTests
    {
        [Test]
        public void DateTimeTypeParameter_AndTestMethodWithDefaultOfTArgument()
        {
            var instance = new SampleGenericClassWithStructConstraint<int>();

            Action act = () => instance.TestMethod(0);

            act.ShouldNotThrow("the argument check should not perform a 'param == default(T)' comparison");
        }

        [Test]
        public void NullableIntTypeParameter_AndTestMethodNullArgument()
        {
            var instance = new SampleGenericClassWithStructConstraint<int>();

            Action act = () => instance.TestMethodWithNullableParameter(null);

            act.ShouldNotThrow("nullable value type parameters should not throw");
        }
    }
}