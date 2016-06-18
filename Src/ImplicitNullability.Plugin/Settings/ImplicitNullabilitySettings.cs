using JetBrains.Application.Settings;
using JetBrains.ReSharper.Resources.Settings;

namespace ImplicitNullability.Plugin.Settings
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

        [SettingsEntry(false, "Fields: Restrict to readonly")]
        public readonly bool FieldsRestrictToReadonly;

        [SettingsEntry(false, "Fields: Restrict to reference types")]
        public readonly bool FieldsRestrictToReferenceTypes;

        [SettingsEntry(true, "Enable type highlighting")]
        public readonly bool EnableTypeHighlighting;
    }
}
