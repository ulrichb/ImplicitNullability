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
        // Tests that external code is excluded from implicit nullability

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

            act.ShouldNotThrow();
        }

        [Test]
        public void Function()
        {
            Action act = () =>
            {
                var result = _externalClass.Function();
                ReSharper.TestValueAnalysis(result, result == null /*Expect no warning*/);
            };

            act.ShouldNotThrow();
        }

        [Test]
        public void FunctionDelegate()
        {
            Action act = () =>
            {
                External.SomeFunctionDelegate @delegate = () => null /*Expect no warning*/;
                var result = @delegate();
                ReSharper.TestValueAnalysis(result, result == null /*Expect no warning*/);
            };

            act.ShouldNotThrow();
        }

        [Test]
        public void AsyncFunction()
        {
            Func<Task> act = async () =>
            {
                var result = await _externalClass.AsyncFunction();
                ReSharper.TestValueAnalysis(result, result == null /*Expect no warning*/);
            };

            act.ShouldNotThrow();
        }
    }
}
