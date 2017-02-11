using System;
using FluentAssertions;
using ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis;
using NUnit.Framework;

namespace ImplicitNullability.Samples.Consumer.NullabilityAnalysis
{
    [TestFixture]
    public class PropertiesAndFieldsSampleTests
    {
        private PropertiesAndFieldsSample _instance;

        [SetUp]
        public void SetUp()
        {
            _instance = new PropertiesAndFieldsSample();
        }

        [Test]
        public void SomeFieldWithNullValue()
        {
            Action act = () => _instance.SomeField = null;

            act.ShouldNotThrow("field write access cannot throw");
        }

        [Test]
        public void SomeCanBeNullFieldWithNullValue()
        {
            Action act = () => _instance.SomeCanBeNullField = null;

            act.ShouldNotThrow();
        }

        [Test]
        public void SomePropertyWithNullValue()
        {
            Action act = () => _instance.SomeAutoProperty = null;

            act.ShouldNotThrow("at the moment, properties should not be guarded");
        }

        [Test]
        public void SomeCanBeNullPropertyWithNullValue()
        {
            Action act = () => _instance.SomeCanBeNullProperty = null;

            act.ShouldNotThrow();
        }
    }
}
