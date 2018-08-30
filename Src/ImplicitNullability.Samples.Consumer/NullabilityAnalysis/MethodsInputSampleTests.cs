using System;
using FluentAssertions;
using ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis;
using NUnit.Framework;

namespace ImplicitNullability.Samples.Consumer.NullabilityAnalysis
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

            act.Should().NotThrow();
        }

        [Test]
        public void Method_WithNullValue()
        {
            Action act = () => _instance.Method(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("a");
        }

        [Test]
        public void MethodWithExplicitCanBeNull()
        {
            Action act = () => _instance.MethodWithExplicitCanBeNull(null);

            act.Should().NotThrow();
        }

        [Test]
        public void MethodWithImplicitCanBeNullParameters()
        {
            Action act = () => _instance.MethodWithImplicitCanBeNullParameters(nullableInt: null, optional: null);

            act.Should().NotThrow();
        }

        [Test]
        public void MethodWithNullDefaultOfStringDefaultArgument()
        {
            Action act = () => _instance.MethodWithNullDefaultOfStringDefaultArgument(optional: null);

            act.Should().NotThrow();
        }

        [Test]
        public void MethodWithNullDefaultArgumentFromConst()
        {
            Action act = () => _instance.MethodWithNullDefaultArgumentFromConst(optional: null);

            act.Should().NotThrow();
        }

        [Test]
        public void MethodWithNullDefaultArgumentFromDefaultOfStringConst()
        {
            Action act = () => _instance.MethodWithNullDefaultArgumentFromDefaultOfStringConst(optional: null);

            act.Should().NotThrow();
        }

        [Test]
        public void MethodWithNonNullDefaultArgument()
        {
            Action act = () => _instance.MethodWithNonNullDefaultArgument(optional: "overridden default");

            act.Should().NotThrow();
        }

        [Test]
        public void MethodWithNonNullDefaultArgument_WithNullValue()
        {
            Action act = () => _instance.MethodWithNonNullDefaultArgument(optional: null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("optional");
        }

        [Test]
        public void MethodWithNonNullDefaultArgumentFromConst()
        {
            Action act = () => _instance.MethodWithNonNullDefaultArgumentFromConst(optional: "overridden default");

            act.Should().NotThrow();
        }

        [Test]
        public void MethodWithNonNullDefaultArgumentFromConst_WithNullValue()
        {
            Action act = () => _instance.MethodWithNonNullDefaultArgumentFromConst(optional: null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("optional");
        }

        [Test]
        public void MethodWithParams()
        {
            Action act = () => _instance.MethodWithParams("1", "2", "3");

            act.Should().NotThrow();
        }

        [Test]
        public void MethodWithParams_WithNullValue()
        {
            Action act = () => _instance.MethodWithParams(a: null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("a");
        }

        [Test]
        public void StaticMethod_WithNonNullValue()
        {
            Action act = () => MethodsInputSample.StaticMethod("");

            act.Should().NotThrow();
        }

        [Test]
        public void StaticMethod_WithNullValue()
        {
            Action act = () => MethodsInputSample.StaticMethod(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("a");
        }
    }
}
