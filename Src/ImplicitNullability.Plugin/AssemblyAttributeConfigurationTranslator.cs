using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

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
        private const string FieldsAssemblyAttributeOption = "Fields";

        private const string FieldsAttributeKey = "ImplicitNullability.Fields";
        private const string FieldsRestrictToReadonlyOption = "RestrictToReadonly";
        private const string FieldsRestrictToReferenceTypesOption = "RestrictToReferenceTypes";

        public static ImplicitNullabilityConfiguration? ParseAttributes(IAttributesSet attributes)
        {
            var assemblyMetadataValues = AssemblyMetadataAttributeValues.Parse(attributes);

            if (assemblyMetadataValues.AppliesTo == null)
                return null;

            return ParseFromAssemblyAttributeOptionsText(assemblyMetadataValues);
        }

        public static string GenerateAttributeCode(ImplicitNullabilityConfiguration configuration)
        {
            var appliesToList = new List<string>();

            if (configuration.EnableInputParameters)
                appliesToList.Add(InputParametersAssemblyAttributeOption);

            if (configuration.EnableRefParameters)
                appliesToList.Add(RefParametersAssemblyAttributeOption);

            if (configuration.EnableOutParametersAndResult)
                appliesToList.Add(OutParametersAndResultAssemblyAttributeOption);

            if (configuration.EnableFields)
                appliesToList.Add(FieldsAssemblyAttributeOption);

            var fieldsList = new List<string>();

            if (configuration.EnableFields)
            {
                if (configuration.FieldsRestrictToReadonly)
                    fieldsList.Add(FieldsRestrictToReadonlyOption);

                if (configuration.FieldsRestrictToReferenceTypes)
                    fieldsList.Add(FieldsRestrictToReferenceTypesOption);
            }

            var fields = !fieldsList.Any() ? null : JoinParts(fieldsList);

            return new AssemblyMetadataAttributeValues(JoinParts(appliesToList), fields).GenerateAttributeCode();
        }

        private static ImplicitNullabilityConfiguration ParseFromAssemblyAttributeOptionsText(AssemblyMetadataAttributeValues attributeValues)
        {
            var appliesToParts = SplitParts(attributeValues.AppliesTo);
            var fieldsParts = SplitParts(attributeValues.Fields);

            return new ImplicitNullabilityConfiguration(
                appliesToParts.Contains(InputParametersAssemblyAttributeOption),
                appliesToParts.Contains(RefParametersAssemblyAttributeOption),
                appliesToParts.Contains(OutParametersAndResultAssemblyAttributeOption),
                appliesToParts.Contains(FieldsAssemblyAttributeOption),
                fieldsParts.Contains(FieldsRestrictToReadonlyOption),
                fieldsParts.Contains(FieldsRestrictToReferenceTypesOption));
        }

        private static IList<string> SplitParts([CanBeNull] string text)
        {
            if (text == null)
                return EmptyList<string>.InstanceList;

            return text.Split(',').Select(x => x.Trim()).ToList();
        }

        private static string JoinParts(IEnumerable<string> parts)
        {
            return parts.Join(", ");
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
