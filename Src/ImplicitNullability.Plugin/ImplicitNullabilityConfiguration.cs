using ImplicitNullability.Plugin.Settings;
using JetBrains.Annotations;
using JetBrains.Util;

namespace ImplicitNullability.Plugin
{
    /// <summary>
    /// Represents the implicit nullability configuration (the chosen options in the settings / in the assembly attributes).
    /// </summary>
    public struct ImplicitNullabilityConfiguration
    {
        public static readonly ImplicitNullabilityConfiguration AllDisabled = new ImplicitNullabilityConfiguration(false, false, false);

        public static ImplicitNullabilityConfiguration CreateFromSettings([NotNull] ImplicitNullabilitySettings implicitNullabilitySettings)
        {
            Assertion.Assert(implicitNullabilitySettings.Enabled, "implicitNullabilitySettings.Enabled");

            return new ImplicitNullabilityConfiguration(
                implicitNullabilitySettings.EnableInputParameters,
                implicitNullabilitySettings.EnableRefParameters,
                implicitNullabilitySettings.EnableOutParametersAndResult);
        }

        public ImplicitNullabilityConfiguration(bool enableInputParameters, bool enableRefParameters, bool enableOutParametersAndResult)
        {
            EnableInputParameters = enableInputParameters;
            EnableRefParameters = enableRefParameters;
            EnableOutParametersAndResult = enableOutParametersAndResult;
        }

        public bool EnableInputParameters { get; }

        public bool EnableRefParameters { get; }

        public bool EnableOutParametersAndResult { get; }
    }
}