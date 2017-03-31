using ImplicitNullability.Plugin.Settings;
using JetBrains.Application.Settings;
using NUnit.Framework;

namespace ImplicitNullability.Plugin.Tests.Infrastructure
{
    public static class ImplicitNullabilitySettingsExtensions
    {
        public static void EnableImplicitNullability(
            this IContextBoundSettingsStore settingsStore,
            bool enableInputParameters = false,
            bool enableRefParameters = false,
            bool enableOutParametersAndResult = false,
            bool enableFields = false,
            bool fieldsRestrictToReadonly = false,
            bool fieldsRestrictToReferenceTypes = false)
        {
            // Fixate default values:
            Assert.That(settingsStore.GetValue((ImplicitNullabilitySettings s) => s.Enabled), Is.False);
            Assert.That(settingsStore.GetValue((ImplicitNullabilitySettings s) => s.EnableInputParameters), Is.True);
            Assert.That(settingsStore.GetValue((ImplicitNullabilitySettings s) => s.EnableRefParameters), Is.True);
            Assert.That(settingsStore.GetValue((ImplicitNullabilitySettings s) => s.EnableOutParametersAndResult), Is.True);
            Assert.That(settingsStore.GetValue((ImplicitNullabilitySettings s) => s.EnableFields), Is.True);
            Assert.That(settingsStore.GetValue((ImplicitNullabilitySettings s) => s.FieldsRestrictToReadonly), Is.False);
            Assert.That(settingsStore.GetValue((ImplicitNullabilitySettings s) => s.FieldsRestrictToReferenceTypes), Is.False);

            settingsStore.SetValue((ImplicitNullabilitySettings s) => s.Enabled, true);
            settingsStore.SetValue((ImplicitNullabilitySettings s) => s.EnableInputParameters, enableInputParameters);
            settingsStore.SetValue((ImplicitNullabilitySettings s) => s.EnableRefParameters, enableRefParameters);
            settingsStore.SetValue((ImplicitNullabilitySettings s) => s.EnableOutParametersAndResult, enableOutParametersAndResult);
            settingsStore.SetValue((ImplicitNullabilitySettings s) => s.EnableFields, enableFields);
            settingsStore.SetValue((ImplicitNullabilitySettings s) => s.FieldsRestrictToReadonly, fieldsRestrictToReadonly);
            settingsStore.SetValue((ImplicitNullabilitySettings s) => s.FieldsRestrictToReferenceTypes, fieldsRestrictToReferenceTypes);
        }

        public static void EnableImplicitNullabilityForAllCodeElements(this IContextBoundSettingsStore settingsStore)
        {
            EnableImplicitNullability(settingsStore, true, true, true, true);
        }
    }
}
