using JetBrains.Util;

namespace ImplicitNullability.Plugin.Configuration
{
    /// <summary>
    /// Represents the implicit nullability configuration (the chosen options in the settings / in the assembly attributes).
    /// </summary>
    public struct ImplicitNullabilityConfiguration
    {
        public static readonly ImplicitNullabilityConfiguration AllDisabled =
            new ImplicitNullabilityConfiguration(ImplicitNullabilityAppliesTo.None, ImplicitNullabilityFieldOptions.None);

        public static ImplicitNullabilityConfiguration CreateFromSettings(ImplicitNullabilitySettings implicitNullabilitySettings)
        {
            Assertion.Assert(implicitNullabilitySettings.Enabled, "implicitNullabilitySettings.Enabled");

            return new ImplicitNullabilityConfiguration(
                (implicitNullabilitySettings.EnableInputParameters ? ImplicitNullabilityAppliesTo.InputParameters : 0) |
                (implicitNullabilitySettings.EnableRefParameters ? ImplicitNullabilityAppliesTo.RefParameters : 0) |
                (implicitNullabilitySettings.EnableOutParametersAndResult ? ImplicitNullabilityAppliesTo.OutParametersAndResult : 0) |
                (implicitNullabilitySettings.EnableFields ? ImplicitNullabilityAppliesTo.Fields : 0),
                (implicitNullabilitySettings.FieldsRestrictToReadonly ? ImplicitNullabilityFieldOptions.RestrictToReadonly : 0) |
                (implicitNullabilitySettings.FieldsRestrictToReferenceTypes ? ImplicitNullabilityFieldOptions.RestrictToReferenceTypes : 0));
        }

        public ImplicitNullabilityConfiguration(
            ImplicitNullabilityAppliesTo appliesTo,
            ImplicitNullabilityFieldOptions fieldOptions)
        {
            AppliesTo = appliesTo;
            FieldOptions = fieldOptions;
        }

        public ImplicitNullabilityAppliesTo AppliesTo { get; }

        public ImplicitNullabilityFieldOptions FieldOptions { get; }

        public bool HasAppliesTo(ImplicitNullabilityAppliesTo flag) => (AppliesTo & flag) > 0;

        public bool HasFieldOption(ImplicitNullabilityFieldOptions flag) => (FieldOptions & flag) > 0;
    }
}
