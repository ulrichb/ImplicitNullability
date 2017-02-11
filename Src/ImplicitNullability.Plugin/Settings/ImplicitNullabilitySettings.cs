using JetBrains.Application.Settings;
using JetBrains.ReSharper.Resources.Settings;

namespace ImplicitNullability.Plugin.Settings
{
    [SettingsKey(typeof(CodeInspectionSettings), "Implicit Nullability")]
    public class ImplicitNullabilitySettings
    {
        [SettingsEntry(false, "Enabled")]
        public readonly bool Enabled;

        [SettingsEntry(true, "EnableInputParameters")]
        public readonly bool EnableInputParameters;

        [SettingsEntry(true, "EnableRefParameters")]
        public readonly bool EnableRefParameters;

        [SettingsEntry(true, "EnableOutParametersAndResult")]
        public readonly bool EnableOutParametersAndResult;

        [SettingsEntry(true, "EnableTypeHighlighting")]
        public readonly bool EnableTypeHighlighting;
    }
}
