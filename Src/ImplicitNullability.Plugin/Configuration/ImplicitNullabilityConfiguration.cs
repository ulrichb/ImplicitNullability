using JetBrains.Util;

namespace ImplicitNullability.Plugin.Configuration
{
    /// <summary>
    /// Represents the implicit nullability configuration (the chosen options in the settings / in the assembly attributes).
    /// </summary>
    public struct ImplicitNullabilityConfiguration
    {
        public static readonly ImplicitNullabilityConfiguration AllDisabled =
            new ImplicitNullabilityConfiguration(false, false, false, false, false, false);

        public static ImplicitNullabilityConfiguration CreateFromSettings(ImplicitNullabilitySettings implicitNullabilitySettings)
        {
            Assertion.Assert(implicitNullabilitySettings.Enabled, "implicitNullabilitySettings.Enabled");

            return new ImplicitNullabilityConfiguration(
                implicitNullabilitySettings.EnableInputParameters,
                implicitNullabilitySettings.EnableRefParameters,
                implicitNullabilitySettings.EnableOutParametersAndResult,
                implicitNullabilitySettings.EnableFields,
                implicitNullabilitySettings.FieldsRestrictToReadonly,
                implicitNullabilitySettings.FieldsRestrictToReferenceTypes);
        }

        public ImplicitNullabilityConfiguration(
            bool enableInputParameters,
            bool enableRefParameters,
            bool enableOutParametersAndResult,
            bool enableFields,
            bool fieldsRestrictToReadonly,
            bool fieldsRestrictToReferenceTypes)
        {
            EnableInputParameters = enableInputParameters;
            EnableRefParameters = enableRefParameters;
            EnableOutParametersAndResult = enableOutParametersAndResult;
            EnableFields = enableFields;
            FieldsRestrictToReadonly = fieldsRestrictToReadonly;
            FieldsRestrictToReferenceTypes = fieldsRestrictToReferenceTypes;
        }

        public bool EnableInputParameters { get; }

        public bool EnableRefParameters { get; }

        public bool EnableOutParametersAndResult { get; }

        public bool EnableFields { get; }

        public bool FieldsRestrictToReadonly { get; }

        public bool FieldsRestrictToReferenceTypes { get; }
    }
}
