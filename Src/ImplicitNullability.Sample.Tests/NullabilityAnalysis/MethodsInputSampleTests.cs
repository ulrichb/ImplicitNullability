using System;
using FluentAssertions;
using ImplicitNullability.Sample.NullabilityAnalysis;
using NUnit.Framework;

namespace ImplicitNullability.Sample.Tests.NullabilityAnalysis
{
    [TestFixture]
    public class MethodsInputSampleTests
    {
        private MethodsInputSample _instance;

        [SetUp]
        public void SetUp()
        {
            _instance = new MethodsInputSample();
        }

        [Test]
        public void Method_WithNonNullValue()
        {
            Action act = () => _instance.Method("");

            act.ShouldNotThrow();
        }

        [Test]
        public void Method_WithNullValue()
        {
            Action act = () => _instance.Method(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("a");
        }

        [Test]
        public void MethodWithExplicitCanBeNull()
        {
            Action act = () => _instance.MethodWithExplicitCanBeNull(null);

            act.ShouldNotThrow();
        }

        [Test]
        public void MethodWithImplicitCanBeNullParameters()
        {
            Action act = () => _instance.MethodWithImplicitCanBeNullParameters(nullableInt: null, optional: null);

            act.ShouldNotThrow();
        }

        [Test]
        public void MethodWithNullDefaultOfStringDefaultArgument()
        {
            Action act = () => _instance.MethodWithNullDefaultOfStringDefaultArgument(optional: null);

            act.ShouldNotThrow();
        }

        [Test]
        public void MethodWithNullDefaultArgumentFromConst()
        {
            Action act = () => _instance.MethodWithNullDefaultArgumentFromConst(optional: null);

            act.ShouldNotThrow();
        }

        [Test]
        public void MethodWithNullDefaultArgumentFromDefaultOfStringConst()
        {
            Action act = () => _instance.MethodWithNullDefaultArgumentFromDefaultOfStringConst(optional: null);

            act.ShouldNotThrow();
        }

        [Test]
        public void MethodWithNonNullDefaultArgument()
        {
            Action act = () => _instance.MethodWithNonNullDefaultArgument(optional: "overridden default");

            act.ShouldNotThrow();
        }

        [Test]
        public void MethodWithNonNullDefaultArgument_WithNullValue()
        {
            Action act = () => _instance.MethodWithNonNullDefaultArgument(optional: null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("optional");
        }

        [Test]
        public void MethodWithNonNullDefaultArgumentFromConst()
        {
            Action act = () => _instance.MethodWithNonNullDefaultArgumentFromConst(optional: "overridden default");

            act.ShouldNotThrow();
        }

        [Test]
        public void MethodWithNonNullDefaultArgumentFromConst_WithNullValue()
        {
            Action act = () => _instance.MethodWithNonNullDefaultArgumentFromConst(optional: null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("optional");
        }

        [Test]
        public void MethodWithParams()
        {
            Action act = () => _instance.MethodWithParams("1", "2", "3");

            act.ShouldNotThrow();
        }

        [Test]
        public void MethodWithParams_WithNullValue()
        {
            Action act = () => _instance.MethodWithParams(a: null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("a");
        }

        [Test]
        public void StaticMethod_WithNonNullValue()
        {
            Action act = () => MethodsInputSample.StaticMethod("");

            act.ShouldNotThrow();
        }

        [Test]
        public void StaticMethod_WithNullValue()
        {
            Action act = () => MethodsInputSample.StaticMethod(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("a");
        }
    }
}