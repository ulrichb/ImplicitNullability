using System;
using FluentAssertions;
using ImplicitNullability.Sample.NullabilityAnalysis;
using NUnit.Framework;

namespace ImplicitNullability.Sample.Tests.NullabilityAnalysis
{
    [TestFixture]
    public class MethodsRefParameterSampleTests
    {
        private MethodsRefParameterSample _instance;

        [SetUp]
        public void SetUp()
        {
            _instance = new MethodsRefParameterSample();
        }


        [Test]
        public void MethodWithRefParameter()
        {
            Action act = () =>
            {
                string refParam = "";
                _instance.MethodWithRefParameter(ref refParam);
            };

            act.ShouldNotThrow();
        }

        [Test]
        public void MethodWithRefParameter_WithNullValue()
        {
            Action act = () =>
            {
                string refParam = null;
                _instance.MethodWithRefParameter(ref refParam /* REPORT? constant null value to NotNull parameter */);
            };

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("refParam");
        }

        [Test]
        public void MethodWithExplicitNotNullRefParameterParameter()
        {
            Action act = () =>
            {
                string refParam = "s";
                _instance.MethodWithExplicitNotNullRefParameter(ref refParam);
            };

            act.ShouldNotThrow();
        }

        [Test]
        public void MethodWithExplicitNotNullRefParameter_WithNullValue()
        {
            Action act = () =>
            {
                string refParam = null;
                _instance.MethodWithExplicitNotNullRefParameter(ref refParam /* REPORT? constant null value to NotNull parameter */);
            };

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("refParam");
        }

        [Test]
        public void MethodWithCanBeNullRefParameter()
        {
            Action act = () =>
            {
                string refParam = null;
                _instance.MethodWithCanBeNullRefParameter(ref refParam);
            };

            act.ShouldNotThrow();
        }
    }
}