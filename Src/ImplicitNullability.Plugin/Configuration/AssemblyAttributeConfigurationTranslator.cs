using System;
using System.Collections.Concurrent;
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
        private const string GeneratedCodeKey = "ImplicitNullability.GeneratedCode";

        private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<string, int>> EnumTypeToValueDictionary =
            new ConcurrentDictionary<Type, IReadOnlyDictionary<string, int>>();

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
                    return new AssemblyMetadataAttributeValues(configuration.AppliesTo.ToString(), fields: null, generatedCode: null);

                var appliesToText = configuration.AppliesTo.ToString();

                var fieldsText =
                    configuration.HasAppliesTo(ImplicitNullabilityAppliesTo.Fields) &&
                    configuration.FieldOptions != ImplicitNullabilityFieldOptions.NoOption
                        ? configuration.FieldOptions.ToString()
                        : null;

                var generatedCodeText = configuration.GeneratedCode.ToString();

                return new AssemblyMetadataAttributeValues(appliesToText, fieldsText, generatedCodeText);
            }

            return GenerateAttributeValues().GenerateAttributeCode();
        }

        private static ImplicitNullabilityConfiguration ParseFromAssemblyAttributeOptionsText(AssemblyMetadataAttributeValues attributeValues)
        {
            var appliesTo = ParseFlags<ImplicitNullabilityAppliesTo>(attributeValues.AppliesTo);
            var fieldOptions = ParseFlags<ImplicitNullabilityFieldOptions>(attributeValues.Fields);

            // Fall back to `Include` if null/invalid (versus default = `Exclude` in the UI) for backwards compatibility with IN <= 3.6.0:
            var generatedCode = ParseEnum(attributeValues.GeneratedCode, defaultValue: GeneratedCodeOptions.Include);

            return new ImplicitNullabilityConfiguration(appliesTo, fieldOptions, generatedCode);
        }

        private static IReadOnlyDictionary<string, int> GetValueDictionary<TEnum>() where TEnum : struct
        {
            return EnumTypeToValueDictionary.GetOrAdd(
                typeof(TEnum),
                enumType => Enum.GetValues(enumType).Cast<int>().ToDictionary(x => Enum.GetName(enumType, x), x => x));
        }

        private static TEnum ParseFlags<TEnum>([CanBeNull] string text) where TEnum : struct
        {
            // Manually implement the parsing because 'Enum.TryParse()' returns 0 if the input text contains invalid names.

            var result = 0;

            if (text != null)
            {
                var valueDictionary = GetValueDictionary<TEnum>();
                foreach (var part in text.Split(','))
                {
                    valueDictionary.TryGetValue(part.Trim(), out int value);
                    result |= value;
                }
            }

            return (TEnum) (object) result;
        }

        private static TEnum ParseEnum<TEnum>([CanBeNull] string text, TEnum defaultValue) where TEnum : struct
        {
            if (text == null || !GetValueDictionary<TEnum>().TryGetValue(text, out var result))
                return defaultValue;

            return (TEnum) (object) result;
        }

        private struct AssemblyMetadataAttributeValues
        {
            [CanBeNull]
            public readonly string AppliesTo;

            [CanBeNull]
            public readonly string Fields;

            [CanBeNull]
            public readonly string GeneratedCode;

            public AssemblyMetadataAttributeValues([CanBeNull] string appliesTo, [CanBeNull] string fields, [CanBeNull] string generatedCode)
            {
                AppliesTo = appliesTo;
                Fields = fields;
                GeneratedCode = generatedCode;
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
                    attributeValuesDictionary.TryGetValue(GeneratedCodeKey));
            }

            public string GenerateAttributeCode()
            {
                var attributeType = AssemblyMetadataAttributeTypeName.FullName;

                var attributeValuesDictionary = new Dictionary<string, string>
                {
                    { AppliesToAttributeKey, AppliesTo },
                    { FieldsAttributeKey, Fields },
                    { GeneratedCodeKey, GeneratedCode },
                };

                return string.Join(
                    Environment.NewLine,
                    attributeValuesDictionary.Where(x => x.Value != null).Select(x => $"[assembly: {attributeType}(\"{x.Key}\", \"{x.Value}\")]"));
            }
        }
    }
}
