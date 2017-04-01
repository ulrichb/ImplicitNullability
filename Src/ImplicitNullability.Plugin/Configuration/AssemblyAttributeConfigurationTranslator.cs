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
            var appliesToText = configuration.AppliesTo == ImplicitNullabilityAppliesTo.None ? "" : configuration.AppliesTo.ToString();

            string fieldsText = null;

            if (configuration.HasAppliesTo(ImplicitNullabilityAppliesTo.Fields))
            {
                if (configuration.FieldOptions != ImplicitNullabilityFieldOptions.None)
                    fieldsText = configuration.FieldOptions.ToString();
            }

            return new AssemblyMetadataAttributeValues(appliesToText, fieldsText).GenerateAttributeCode();
        }

        private static ImplicitNullabilityConfiguration ParseFromAssemblyAttributeOptionsText(AssemblyMetadataAttributeValues attributeValues)
        {
            var appliesTo = (ImplicitNullabilityAppliesTo) ParseFlags(attributeValues.AppliesTo, AppliesToNamesToValue);
            var fieldOptions = (ImplicitNullabilityFieldOptions) ParseFlags(attributeValues.Fields, FieldOptionsNamesToValue);

            return new ImplicitNullabilityConfiguration(appliesTo, fieldOptions);
        }

        private static Dictionary<string, int> CreateNamesToValueDictionary<T>()
        {
            var enumType = typeof(T);
            return Enum.GetValues(enumType).Cast<int>().ToDictionary(x => enumType.GetEnumName(x), x => x);
        }

        private static int ParseFlags([CanBeNull] string text, IDictionary<string, int> toDict)
        {
            // Manually implement the parsing because 'Enum.TryParse()' returns 0 if the contains invalid names.

            var result = 0;

            if (text != null)
            {
                foreach (var part in text.Split(','))
                    result |= toDict.TryGetValue(part.Trim());
            }

            return result;
        }

        private struct AssemblyMetadataAttributeValues
        {
            // Note: Optimize for performance because attribute parsing happens in every IN provider call

            [CanBeNull]
            public readonly string AppliesTo;

            [CanBeNull]
            public readonly string Fields;

            public AssemblyMetadataAttributeValues([CanBeNull] string appliesTo, [CanBeNull] string fields)
            {
                AppliesTo = appliesTo;
                Fields = fields;
            }

            public static AssemblyMetadataAttributeValues Parse(IAttributesSet attributes)
            {
                var assemblyMetadataAttributes = attributes.GetAttributeInstances(AssemblyMetadataAttributeTypeName, false);

                string appliesTo = null;
                string fields = null;

                foreach (var attributeInstance in assemblyMetadataAttributes)
                {
                    var key = attributeInstance.PositionParameter(0).ConstantValue.Value as string;
                    var value = attributeInstance.PositionParameter(1).ConstantValue.Value as string;

                    switch (key)
                    {
                        case AppliesToAttributeKey:
                            appliesTo = value;
                            break;

                        case FieldsAttributeKey:
                            fields = value;
                            break;
                    }
                }

                return new AssemblyMetadataAttributeValues(appliesTo, fields);
            }

            public string GenerateAttributeCode()
            {
                var attributeType = AssemblyMetadataAttributeTypeName.FullName;

                var values = new Dictionary<string, string>
                {
                    { AppliesToAttributeKey, AppliesTo },
                    { FieldsAttributeKey, Fields }
                };

                return string.Join(
                    Environment.NewLine,
                    values.Where(x => x.Value != null).Select(x => $"[assembly: {attributeType}(\"{x.Key}\", \"{x.Value}\")]"));
            }
        }
    }
}
