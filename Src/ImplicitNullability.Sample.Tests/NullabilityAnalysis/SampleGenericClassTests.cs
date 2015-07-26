using System;
using FluentAssertions;
using ImplicitNullability.Sample.NullabilityAnalysis;
using NUnit.Framework;

namespace ImplicitNullability.Sample.Tests.NullabilityAnalysis
{
    [TestFixture]
    public class SampleGenericClassTests
    {
        [Test]
        public void DateTimeTypeParameter_AndTestMethodWithDefaultOfTArgument()
        {
            var instance = new SampleGenericClass<DateTime>();

            Action act = () => instance.TestMethod(default(DateTime));

            act.ShouldNotThrow("the argument check should not perform a 'param == default(T)' comparison");
        }

        [Test]
        public void NullableIntTypeParameter_AndTestMethodNullArgument()
        {
            var instance = new SampleGenericClass<int?>();

            Action act = () => instance.TestMethod(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.ShouldThrow<ArgumentNullException>(
                "this is a known issue for generic unconstraint types (it is a trade-off between this false positive " +
                "and a false-negative if T is a reference type (like in the StringTypeParameter_AndTestMethodNullArgument test case)")
                .And.ParamName.Should().Be("a");
        }

        [Test]
        public void StringTypeParameter_AndTestMethodNullArgument()
        {
            var instance = new SampleGenericClass<string>();

            Action act = () => instance.TestMethod(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("a");
        }
    }
}