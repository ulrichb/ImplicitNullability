using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace ImplicitNullability.Plugin.Configuration
{
    /// <summary>
    /// Translates between <see cref="ImplicitNullabilityConfiguration"/> and its representation as
    /// an <see cref="T:System.Reflection.AssemblyMetadataAttribute"/>.
    /// </summary>
    public static class AssemblyAttributeConfigurationTranslator
    {
        private static readonly ClrTypeName AssemblyMetadataAttributeTypeName = new ClrTypeName("System.Reflection.AssemblyMetadataAttribute");

        private const string AppliesToAttributeKey = "ImplicitNullability.AppliesTo";
        private const string FieldsAttributeKey = "ImplicitNullability.Fields";
        private const string ExcludeGeneratedCodeKey = "ImplicitNullability.ExcludeGeneratedCode";

        private static readonly IDictionary<string, int> AppliesToNamesToValue = CreateNamesToValueDictionary<ImplicitNullabilityAppliesTo>();
        private static readonly IDictionary<string, int> FieldOptionsNamesToValue = CreateNamesToValueDictionary<ImplicitNullabilityFieldOptions>();

        public static ImplicitNullabilityConfiguration? ParseAttributes(IAttributesSet attributes)
        {
            var assemblyMetadataValues = AssemblyMetadataAttributeValues.Parse(attributes);

            if (assemblyMetadataValues.AppliesTo == null)
                return null;

            return ParseFromAssemblyAttributeOptionsText(assemblyMetadataValues);
        }

        public static string GenerateAttributeCode(ImplicitNullabilityConfiguration configuration)
        {
            AssemblyMetadataAttributeValues GenerateAttributeValues()
            {
                if (configuration.AppliesTo == ImplicitNullabilityAppliesTo.None)
                    return new AssemblyMetadataAttributeValues(appliesTo: "", fields: null, excludeGeneratedCode: null);

                var appliesToText = configuration.AppliesTo.ToString();

                var fieldsText =
                    configuration.HasAppliesTo(ImplicitNullabilityAppliesTo.Fields) &&
                    configuration.FieldOptions != ImplicitNullabilityFieldOptions.None
                        ? configuration.FieldOptions.ToString()
                        : null;

                var excludeGeneratedCodeText = configuration.ExcludeGeneratedCode.ToString();

                return new AssemblyMetadataAttributeValues(appliesToText, fieldsText, excludeGeneratedCodeText);
            }

            return GenerateAttributeValues().GenerateAttributeCode();
        }

        private static ImplicitNullabilityConfiguration ParseFromAssemblyAttributeOptionsText(AssemblyMetadataAttributeValues attributeValues)
        {
            var appliesTo = (ImplicitNullabilityAppliesTo) ParseFlags(attributeValues.AppliesTo, AppliesToNamesToValue);
            var fieldOptions = (ImplicitNullabilityFieldOptions) ParseFlags(attributeValues.Fields, FieldOptionsNamesToValue);

            // Fall back to `false` if `null` (versus default = `true` in the UI) for backwards compatibility with IN <= 3.6.0:
            var excludeGeneratedCode = ParseBoolean(attributeValues.ExcludeGeneratedCode);

            return new ImplicitNullabilityConfiguration(appliesTo, fieldOptions, excludeGeneratedCode);
        }

        private static Dictionary<string, int> CreateNamesToValueDictionary<T>()
        {
            var enumType = typeof(T);
            return Enum.GetValues(enumType).Cast<int>().ToDictionary(x => enumType.GetEnumName(x), x => x);
        }

        private static int ParseFlags([CanBeNull] string text, IDictionary<string, int> namesToValueDictionary)
        {
            // Manually implement the parsing because 'Enum.TryParse()' returns 0 if the input text contains invalid names.

            var result = 0;

            if (text != null)
            {
                foreach (var part in text.Split(','))
                    result |= namesToValueDictionary.TryGetValue(part.Trim());
            }

            return result;
        }

        private static bool ParseBoolean([CanBeNull] string text) => text == "True" || text == "true";

        private struct AssemblyMetadataAttributeValues
        {
            [CanBeNull]
            public readonly string AppliesTo;

            [CanBeNull]
            public readonly string Fields;

            [CanBeNull]
            public readonly string ExcludeGeneratedCode;

            public AssemblyMetadataAttributeValues([CanBeNull] string appliesTo, [CanBeNull] string fields, [CanBeNull] string excludeGeneratedCode)
            {
                AppliesTo = appliesTo;
                Fields = fields;
                ExcludeGeneratedCode = excludeGeneratedCode;
            }

            public static AssemblyMetadataAttributeValues Parse(IAttributesSet attributes)
            {
                var assemblyMetadataAttributes = attributes.GetAttributeInstances(AssemblyMetadataAttributeTypeName, false);

                var attributeValuesDictionary = new Dictionary<string, string>();

                foreach (var attributeInstance in assemblyMetadataAttributes)
                {
                    var key = attributeInstance.PositionParameter(0).ConstantValue.Value as string;

                    if (key != null)
                        attributeValuesDictionary[key] = attributeInstance.PositionParameter(1).ConstantValue.Value as string;
                }

                return new AssemblyMetadataAttributeValues(
                    attributeValuesDictionary.TryGetValue(AppliesToAttributeKey),
                    attributeValuesDictionary.TryGetValue(FieldsAttributeKey),
                    attributeValuesDictionary.TryGetValue(ExcludeGeneratedCodeKey));
            }

            public string GenerateAttributeCode()
            {
                var attributeType = AssemblyMetadataAttributeTypeName.FullName;

                var attributeValuesDictionary = new Dictionary<string, string>
                {
                    { AppliesToAttributeKey, AppliesTo },
                    { FieldsAttributeKey, Fields },
                    { ExcludeGeneratedCodeKey, ExcludeGeneratedCode },
                };

                return string.Join(
                    Environment.NewLine,
                    attributeValuesDictionary.Where(x => x.Value != null).Select(x => $"[assembly: {attributeType}(\"{x.Key}\", \"{x.Value}\")]"));
            }
        }
    }
}
