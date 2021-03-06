using System;
using System.Threading.Tasks;
using FluentAssertions;
using ImplicitNullability.Samples.CodeWithIN;
using ImplicitNullability.Samples.CodeWithoutIN;
using NUnit.Framework;

namespace ImplicitNullability.Samples.Consumer.NullabilityAnalysis
{
    [TestFixture]
    public class ExternalCodeExclusionTests
    {
        // Proves that external code is not implicitly NotNull

        private External.Class _externalClass;

        [SetUp]
        public void SetUp()
        {
            _externalClass = new External.Class();
        }

        [Test]
        public void Method()
        {
            Action act = () => _externalClass.Method(null /*Expect no warning*/);

            act.Should().NotThrow();
        }

        [Test]
        public void Function()
        {
            Action act = () =>
            {
                var result = _externalClass.Function();
                ReSharper.TestValueAnalysis(result, result == null /*Expect no warning*/);
            };

            act.Should().NotThrow();
        }

        [Test]
        public void FunctionDelegate()
        {
            Action act = () =>
            {
                // ReSharper disable once ConvertToLocalFunction
                External.SomeFunctionDelegate @delegate = () => null /*Expect no warning*/;
                var result = @delegate();
                ReSharper.TestValueAnalysis(result, result == null /*Expect no warning*/);
            };

            act.Should().NotThrow();
        }

        [Test]
        public void AsyncFunction()
        {
            Func<Task> act = async () =>
            {
                var result = await _externalClass.AsyncFunction();
                ReSharper.TestValueAnalysis(result, result == null /*Expect no warning*/);
            };

            act.Should().NotThrow();
        }

        [Test]
        public void Field()
        {
            var value = _externalClass.Field;

            ReSharper.TestValueAnalysis(value, value == null /*Expect no warning*/);
            value.Should().BeNull();
        }

        [Test]
        public void Property()
        {
            var value = _externalClass.Property;

            ReSharper.TestValueAnalysis(value, value == null /*Expect no warning*/);
            value.Should().BeNull();
        }
    }
}
