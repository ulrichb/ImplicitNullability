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

        private static readonly string NewLine = Environment.NewLine;

        private static readonly ImplicitNullabilityFieldOptions AllFieldOptions =
            ImplicitNullabilityFieldOptions.RestrictToReadonly | ImplicitNullabilityFieldOptions.RestrictToReferenceTypes;

        private static readonly ImplicitNullabilityPropertyOptions AllPropertyOptions =
            ImplicitNullabilityPropertyOptions.RestrictToGetterOnly | ImplicitNullabilityPropertyOptions.RestrictToReferenceTypes;

        [Test]
        public void GenerateAttributeCode_WithNoAppliesTo()
        {
            var configuration = ImplicitNullabilityConfiguration.AllDisabled;

            var result = AssemblyAttributeConfigurationTranslator.GenerateAttributeCode(configuration);

            Assert.That(result, Is.EqualTo(ExpectedAssemblyMetadataAttribute("ImplicitNullability.AppliesTo", "None")));
        }

        [Test]
        public void GenerateAttributeCode_WithAllOptionsEnabled()
        {
            var configuration = new ImplicitNullabilityConfiguration(
                ImplicitNullabilityAppliesTo.InputParameters |
                ImplicitNullabilityAppliesTo.RefParameters |
                ImplicitNullabilityAppliesTo.OutParametersAndResult |
                ImplicitNullabilityAppliesTo.Fields |
                ImplicitNullabilityAppliesTo.Properties,
                AllFieldOptions,
                AllPropertyOptions,
                GeneratedCodeOptions.Exclude);

            var result = AssemblyAttributeConfigurationTranslator.GenerateAttributeCode(configuration);

            Assert.That(result, Is.EqualTo(
                ExpectedAssemblyMetadataAttribute(
                    "ImplicitNullability.AppliesTo",
                    "InputParameters, RefParameters, OutParametersAndResult, Fields, Properties") + NewLine +
                ExpectedAssemblyMetadataAttribute("ImplicitNullability.Fields", "RestrictToReadonly, RestrictToReferenceTypes") + NewLine +
                ExpectedAssemblyMetadataAttribute("ImplicitNullability.Properties", "RestrictToGetterOnly, RestrictToReferenceTypes") + NewLine +
                ExpectedAssemblyMetadataAttribute("ImplicitNullability.GeneratedCode", "Exclude")));
        }

        [Test]
        public void GenerateAttributeCode_WithFieldOrPropertyOptions_ButAppliesToFieldsOrPropertiesDisabled()
        {
            var configuration = new ImplicitNullabilityConfiguration(
                ImplicitNullabilityAppliesTo.InputParameters, AllFieldOptions, AllPropertyOptions, GeneratedCodeOptions.Exclude);

            var result = AssemblyAttributeConfigurationTranslator.GenerateAttributeCode(configuration);

            Assert.That(result, Is.EqualTo(
                    ExpectedAssemblyMetadataAttribute("ImplicitNullability.AppliesTo", "InputParameters") + NewLine +
                    ExpectedAssemblyMetadataAttribute("ImplicitNullability.GeneratedCode", "Exclude")),
                "'options'-attribute should not be rendered");
        }

        [Test]
        public void GenerateAttributeCode_WithoutFieldOrPropertyOptions_ButAppliesToFieldsOrPropertiesEnabled()
        {
            var configuration = new ImplicitNullabilityConfiguration(
                ImplicitNullabilityAppliesTo.InputParameters | ImplicitNullabilityAppliesTo.Fields | ImplicitNullabilityAppliesTo.Properties,
                ImplicitNullabilityFieldOptions.NoOption,
                ImplicitNullabilityPropertyOptions.NoOption,
                GeneratedCodeOptions.Exclude);

            var result = AssemblyAttributeConfigurationTranslator.GenerateAttributeCode(configuration);

            Assert.That(result, Is.EqualTo(
                    ExpectedAssemblyMetadataAttribute("ImplicitNullability.AppliesTo", "InputParameters, Fields, Properties") + NewLine +
                    ExpectedAssemblyMetadataAttribute("ImplicitNullability.GeneratedCode", "Exclude")),
                "'options'-attribute should not be rendered");
        }

        [Test]
        public void GenerateAttributeCode_WithIncludeGeneratedCode()
        {
            var configuration = new ImplicitNullabilityConfiguration(
                ImplicitNullabilityAppliesTo.InputParameters,
                ImplicitNullabilityFieldOptions.NoOption,
                ImplicitNullabilityPropertyOptions.NoOption,
                GeneratedCodeOptions.Include);

            var result = AssemblyAttributeConfigurationTranslator.GenerateAttributeCode(configuration);

            Assert.That(result, Is.EqualTo(
                ExpectedAssemblyMetadataAttribute("ImplicitNullability.AppliesTo", "InputParameters") + NewLine +
                ExpectedAssemblyMetadataAttribute("ImplicitNullability.GeneratedCode", "Include")));
        }

        string ExpectedAssemblyMetadataAttribute(string key, string value) =>
            $"[assembly: System.Reflection.AssemblyMetadataAttribute(\"{key}\", \"{value}\")]";
    }
}
