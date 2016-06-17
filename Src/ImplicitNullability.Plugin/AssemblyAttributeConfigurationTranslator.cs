using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ReSharper.Psi;

namespace ImplicitNullability.Plugin
{
    /// <summary>
    /// Translates between <see cref="ImplicitNullabilityConfiguration"/> and its representation as
    /// an <see cref="T:System.Reflection.AssemblyMetadataAttribute"/>.
    /// </summary>
    public static class AssemblyAttributeConfigurationTranslator
    {
        private static readonly ClrTypeName AssemblyMetadataAttributeTypeName = new ClrTypeName("System.Reflection.AssemblyMetadataAttribute");

        private const string AppliesToAttributeKey = "ImplicitNullability.AppliesTo";
        private const string InputParametersAssemblyAttributeOption = "InputParameters";
        private const string RefParametersAssemblyAttributeOption = "RefParameters";
        private const string OutParametersAndResultAssemblyAttributeOption = "OutParametersAndResult";

        public static ImplicitNullabilityConfiguration? ParseAttributes(IAttributesSet attributes)
        {
            var assemblyAttributeOptionsText = GetAssemblyAttributeOptionsText(attributes);

            if (assemblyAttributeOptionsText == null)
                return null;

            return ParseFromAssemblyAttributeOptionsText(assemblyAttributeOptionsText);
        }

        public static string GenerateAttributeCode(ImplicitNullabilityConfiguration configuration)
        {
            var optionTexts = new List<string>();

            if (configuration.EnableInputParameters)
                optionTexts.Add(InputParametersAssemblyAttributeOption);

            if (configuration.EnableRefParameters)
                optionTexts.Add(RefParametersAssemblyAttributeOption);

            if (configuration.EnableOutParametersAndResult)
                optionTexts.Add(OutParametersAndResultAssemblyAttributeOption);

            return $"[assembly: {AssemblyMetadataAttributeTypeName.FullName}(" +
                   $"\"{AppliesToAttributeKey}\", " +
                   $"\"{string.Join(", ", optionTexts)}\")]";
        }

        [CanBeNull]
        private static string GetAssemblyAttributeOptionsText(IAttributesSet attributes)
        {
            var assemblyMetadataAttributes = attributes.GetAttributeInstances(AssemblyMetadataAttributeTypeName, false);

            var attributeWithAppliesToAttributeKey = assemblyMetadataAttributes
                .FirstOrDefault(x => Equals(x.PositionParameter(0).ConstantValue.Value, AppliesToAttributeKey));

            if (attributeWithAppliesToAttributeKey == null)
                return null;

            return attributeWithAppliesToAttributeKey.PositionParameter(1).ConstantValue.Value as string;
        }

        private static ImplicitNullabilityConfiguration ParseFromAssemblyAttributeOptionsText(string text)
        {
            var optionTexts = text.Split(',').Select(x => x.Trim()).ToList();

            return new ImplicitNullabilityConfiguration(
                optionTexts.Contains(InputParametersAssemblyAttributeOption),
                optionTexts.Contains(RefParametersAssemblyAttributeOption),
                optionTexts.Contains(OutParametersAndResultAssemblyAttributeOption));
        }
    }
}