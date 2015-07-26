using System;
using JetBrains.Application.Settings;
#if RESHARPER8
using JetBrains.ReSharper.Settings;

#else
using JetBrains.ReSharper.Resources.Settings;

#endif

namespace ImplicitNullability.Plugin.Settings
{
    [SettingsKey(typeof (CodeInspectionSettings), "Implicit Nullability")]
    public class ImplicitNullabilitySettings
    {
        [SettingsEntry(false, "Enabled")]
        public readonly bool Enabled;

        [SettingsEntry(true, "EnableInputAndRefParameters")]
        public readonly bool EnableInputAndRefParameters;
    }
}