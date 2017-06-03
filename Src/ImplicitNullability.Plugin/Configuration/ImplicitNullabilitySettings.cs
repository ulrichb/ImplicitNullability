using JetBrains.Application.Settings;
using JetBrains.ReSharper.Resources.Settings;

namespace ImplicitNullability.Plugin.Configuration
{
    [SettingsKey(typeof(CodeInspectionSettings), "Implicit Nullability")]
    public class ImplicitNullabilitySettings
    {
        [SettingsEntry(false, "Enable Implicit Nullability")]
        public readonly bool Enabled;

        [SettingsEntry(true, "Enable for input parameters")]
        public readonly bool EnableInputParameters;

        [SettingsEntry(true, "Enable for ref parameters")]
        public readonly bool EnableRefParameters;

        [SettingsEntry(true, "Enable for return values and output parameters")]
        public readonly bool EnableOutParametersAndResult;

        [SettingsEntry(true, "Enable fields")]
        public readonly bool EnableFields;

        [SettingsEntry(true, "Fields: Restrict to readonly")]
        public readonly bool FieldsRestrictToReadonly;

        [SettingsEntry(false, "Fields: Restrict to reference types")]
        public readonly bool FieldsRestrictToReferenceTypes;

        [SettingsEntry(true, "Enable properties")]
        public readonly bool EnableProperties;

        [SettingsEntry(true, "Properties: Restrict to Getter-only")]
        public readonly bool PropertiesRestrictToGetterOnly;

        [SettingsEntry(false, "Properties: Restrict to reference types")]
        public readonly bool PropertiesRestrictToReferenceTypes;

        [SettingsEntry(GeneratedCodeOptions.Exclude, "Generated code option")]
        public readonly GeneratedCodeOptions GeneratedCode;

        [SettingsEntry(true, "Enable type highlighting")]
        public readonly bool EnableTypeHighlighting;
    }
}
