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

        [Test]
        public void GenerateAttributeCode_WithNoAppliesTo()
        {
            var configuration = ImplicitNullabilityConfiguration.AllDisabled;

            var result = AssemblyAttributeConfigurationTranslator.GenerateAttributeCode(configuration);

            Assert.That(result, Is.EqualTo(ExpectedAssemblyMetadataAttribute("ImplicitNullability.AppliesTo", "")));
        }

        [Test]
        public void GenerateAttributeCode_WithAllOptionsEnabled()
        {
            var configuration = new ImplicitNullabilityConfiguration(
                ImplicitNullabilityAppliesTo.InputParameters |
                ImplicitNullabilityAppliesTo.RefParameters |
                ImplicitNullabilityAppliesTo.OutParametersAndResult |
                ImplicitNullabilityAppliesTo.Fields,
                AllFieldOptions,
                GeneratedCodeOptions.Exclude);

            var result = AssemblyAttributeConfigurationTranslator.GenerateAttributeCode(configuration);

            Assert.That(result, Is.EqualTo(
                ExpectedAssemblyMetadataAttribute(
                    "ImplicitNullability.AppliesTo",
                    "InputParameters, RefParameters, OutParametersAndResult, Fields") + NewLine +
                ExpectedAssemblyMetadataAttribute("ImplicitNullability.Fields", "RestrictToReadonly, RestrictToReferenceTypes") + NewLine +
                ExpectedAssemblyMetadataAttribute("ImplicitNullability.GeneratedCode", "Exclude")));
        }

        [Test]
        public void GenerateAttributeCode_WithFieldOptionsButAppliesToFieldsDisabled()
        {
            var configuration = new ImplicitNullabilityConfiguration(
                ImplicitNullabilityAppliesTo.InputParameters, AllFieldOptions, GeneratedCodeOptions.Exclude);

            var result = AssemblyAttributeConfigurationTranslator.GenerateAttributeCode(configuration);

            Assert.That(result, Is.EqualTo(
                    ExpectedAssemblyMetadataAttribute("ImplicitNullability.AppliesTo", "InputParameters") + NewLine +
                    ExpectedAssemblyMetadataAttribute("ImplicitNullability.GeneratedCode", "Exclude")),
                "'ImplicitNullability.Fields'-attribute must not be rendered");
        }

        [Test]
        public void GenerateAttributeCode_WithoutFieldOptionsButAppliesToFieldsEnabled()
        {
            var configuration = new ImplicitNullabilityConfiguration(
                ImplicitNullabilityAppliesTo.InputParameters | ImplicitNullabilityAppliesTo.Fields,
                ImplicitNullabilityFieldOptions.None,
                GeneratedCodeOptions.Exclude);

            var result = AssemblyAttributeConfigurationTranslator.GenerateAttributeCode(configuration);

            Assert.That(result, Is.EqualTo(
                    ExpectedAssemblyMetadataAttribute("ImplicitNullability.AppliesTo", "InputParameters, Fields") + NewLine +
                    ExpectedAssemblyMetadataAttribute("ImplicitNullability.GeneratedCode", "Exclude")),
                "'ImplicitNullability.Fields'-attribute must not be rendered");
        }

        [Test]
        public void GenerateAttributeCode_WithIncludeGeneratedCode()
        {
            var configuration = new ImplicitNullabilityConfiguration(
                ImplicitNullabilityAppliesTo.InputParameters,
                ImplicitNullabilityFieldOptions.None,
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
