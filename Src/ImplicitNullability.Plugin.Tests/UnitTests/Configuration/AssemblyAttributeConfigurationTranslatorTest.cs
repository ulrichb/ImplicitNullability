using System;
using ImplicitNullability.Plugin.Configuration;
using NUnit.Framework;

namespace ImplicitNullability.Plugin.Tests.UnitTests.Configuration
{
    [TestFixture]
    public class AssemblyAttributeConfigurationTranslatorTest
    {
        // This fixture specifically tests GenerateAttributeCode(), the remaining stuff is tested in
        // ImplicitNullabilityConfigurationEvaluatorTest.

        private static readonly ImplicitNullabilityFieldOptions AllFieldOptions =
            ImplicitNullabilityFieldOptions.RestrictToReadonly | ImplicitNullabilityFieldOptions.RestrictToReferenceTypes;

        [Test]
        public void GenerateAttributeCode_WithAllOptionsDisabled()
        {
            var configuration = ImplicitNullabilityConfiguration.AllDisabled;

            var result = AssemblyAttributeConfigurationTranslator.GenerateAttributeCode(configuration);

            Assert.That(result, Is.EqualTo("[assembly: System.Reflection.AssemblyMetadataAttribute(\"ImplicitNullability.AppliesTo\", \"\")]"));
        }

        [Test]
        public void GenerateAttributeCode_WithAllOptionsEnabled()
        {
            var configuration = new ImplicitNullabilityConfiguration(
                ImplicitNullabilityAppliesTo.InputParameters |
                ImplicitNullabilityAppliesTo.RefParameters |
                ImplicitNullabilityAppliesTo.OutParametersAndResult |
                ImplicitNullabilityAppliesTo.Fields,
                AllFieldOptions);

            var result = AssemblyAttributeConfigurationTranslator.GenerateAttributeCode(configuration);

            Assert.That(result, Is.EqualTo(
                "[assembly: System.Reflection.AssemblyMetadataAttribute(\"ImplicitNullability.AppliesTo\"," +
                " \"InputParameters, RefParameters, OutParametersAndResult, Fields\")]" + Environment.NewLine +
                "[assembly: System.Reflection.AssemblyMetadataAttribute(\"ImplicitNullability.Fields\"," +
                " \"RestrictToReadonly, RestrictToReferenceTypes\")]"));
        }

        [Test]
        public void GenerateAttributeCode_WithFieldsSettingsButAppliesToFieldsDisabled()
        {
            var configuration = new ImplicitNullabilityConfiguration(
                ImplicitNullabilityAppliesTo.InputParameters, AllFieldOptions);

            var result = AssemblyAttributeConfigurationTranslator.GenerateAttributeCode(configuration);

            Assert.That(
                result,
                Is.EqualTo("[assembly: System.Reflection.AssemblyMetadataAttribute(\"ImplicitNullability.AppliesTo\", \"InputParameters\")]"),
                "'ImplicitNullability.Fields'-attribute must not be rendered");
        }

        [Test]
        public void GenerateAttributeCode_WithoutFieldsSettingsButAppliesToFieldsEnabled()
        {
            var configuration = new ImplicitNullabilityConfiguration(
                ImplicitNullabilityAppliesTo.InputParameters | ImplicitNullabilityAppliesTo.Fields, ImplicitNullabilityFieldOptions.None);

            var result = AssemblyAttributeConfigurationTranslator.GenerateAttributeCode(configuration);

            Assert.That(
                result,
                Is.EqualTo("[assembly: System.Reflection.AssemblyMetadataAttribute(\"ImplicitNullability.AppliesTo\", \"InputParameters, Fields\")]"),
                "'ImplicitNullability.Fields'-attribute must not be rendered");
        }
    }
}
