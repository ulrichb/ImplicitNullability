using JetBrains.Util;

namespace ImplicitNullability.Plugin.Configuration
{
    /// <summary>
    /// Represents the implicit nullability configuration (the chosen options in the settings / in the assembly attributes).
    /// </summary>
    public struct ImplicitNullabilityConfiguration
    {
        public static readonly ImplicitNullabilityConfiguration AllDisabled = new ImplicitNullabilityConfiguration(
            ImplicitNullabilityAppliesTo.None,
            ImplicitNullabilityFieldOptions.NoOption,
            ImplicitNullabilityPropertyOptions.NoOption,
            GeneratedCodeOptions.Include);

        public static ImplicitNullabilityConfiguration CreateFromSettings(ImplicitNullabilitySettings implicitNullabilitySettings)
        {
            Assertion.Assert(implicitNullabilitySettings.Enabled, "implicitNullabilitySettings.Enabled");

            return new ImplicitNullabilityConfiguration(
                appliesTo:
                (implicitNullabilitySettings.EnableInputParameters ? ImplicitNullabilityAppliesTo.InputParameters : 0) |
                (implicitNullabilitySettings.EnableRefParameters ? ImplicitNullabilityAppliesTo.RefParameters : 0) |
                (implicitNullabilitySettings.EnableOutParametersAndResult ? ImplicitNullabilityAppliesTo.OutParametersAndResult : 0) |
                (implicitNullabilitySettings.EnableFields ? ImplicitNullabilityAppliesTo.Fields : 0) |
                (implicitNullabilitySettings.EnableProperties ? ImplicitNullabilityAppliesTo.Properties : 0),
                fieldOptions:
                (implicitNullabilitySettings.FieldsRestrictToReadonly ? ImplicitNullabilityFieldOptions.RestrictToReadonly : 0) |
                (implicitNullabilitySettings.FieldsRestrictToReferenceTypes ? ImplicitNullabilityFieldOptions.RestrictToReferenceTypes : 0),
                propertyOptions:
                (implicitNullabilitySettings.PropertiesRestrictToGetterOnly ? ImplicitNullabilityPropertyOptions.RestrictToGetterOnly : 0) |
                (implicitNullabilitySettings.PropertiesRestrictToReferenceTypes ? ImplicitNullabilityPropertyOptions.RestrictToReferenceTypes : 0),
                generatedCode: implicitNullabilitySettings.GeneratedCode);
        }

        public ImplicitNullabilityConfiguration(
            ImplicitNullabilityAppliesTo appliesTo,
            ImplicitNullabilityFieldOptions fieldOptions,
            ImplicitNullabilityPropertyOptions propertyOptions,
            GeneratedCodeOptions generatedCode)
        {
            AppliesTo = appliesTo;
            FieldOptions = fieldOptions;
            PropertyOptions = propertyOptions;
            GeneratedCode = generatedCode;
        }

        public ImplicitNullabilityAppliesTo AppliesTo { get; }

        public ImplicitNullabilityFieldOptions FieldOptions { get; }

        public ImplicitNullabilityPropertyOptions PropertyOptions { get; }

        public GeneratedCodeOptions GeneratedCode { get; }

        public bool HasAppliesTo(ImplicitNullabilityAppliesTo flag) => (AppliesTo & flag) > 0;

        public bool HasFieldOption(ImplicitNullabilityFieldOptions flag) => (FieldOptions & flag) > 0;

        public bool HasPropertyOption(ImplicitNullabilityPropertyOptions flag) => (PropertyOptions & flag) > 0;
    }
}
