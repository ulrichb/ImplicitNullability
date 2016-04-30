using NUnit.Framework;

namespace ImplicitNullability.Plugin.Tests.UnitTests
{
    [TestFixture]
    public class AssemblyAttributeConfigurationTranslatorTest
    {
        // This fixture specifically tests GenerateAttributeCode(), the remaining stuff is tested in 
        // ImplicitNullabilityConfigurationEvaluatorTest.

        [Test]
        public void ConvertToAssemblyMetadataAttributeCode_WithAllOptionsDisabled()
        {
            var configuration = ImplicitNullabilityConfiguration.AllDisabled;

            var result = AssemblyAttributeConfigurationTranslator.GenerateAttributeCode(configuration);

            Assert.That(result, Is.EqualTo("[assembly: System.Reflection.AssemblyMetadataAttribute(\"ImplicitNullability.AppliesTo\", \"\")]"));
        }

        [Test]
        public void ConvertToAssemblyMetadataAttributeCode_WithAllOptionsEnabled()
        {
            var configuration = new ImplicitNullabilityConfiguration(true, true, true);

            var result = AssemblyAttributeConfigurationTranslator.GenerateAttributeCode(configuration);

            Assert.That(result, Is.EqualTo("[assembly: System.Reflection.AssemblyMetadataAttribute(\"ImplicitNullability.AppliesTo\", " +
                                           "\"InputParameters, RefParameters, OutParametersAndResult\")]"));
        }
    }
}