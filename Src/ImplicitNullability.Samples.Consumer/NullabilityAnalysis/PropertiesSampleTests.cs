using System;
using FluentAssertions;
using ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis;
using NUnit.Framework;

namespace ImplicitNullability.Samples.Consumer.NullabilityAnalysis
{
    [TestFixture]
    public class PropertiesSampleTests
    {
        private PropertiesSample _instance;

        [SetUp]
        public void SetUp()
        {
            _instance = new PropertiesSample();
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
