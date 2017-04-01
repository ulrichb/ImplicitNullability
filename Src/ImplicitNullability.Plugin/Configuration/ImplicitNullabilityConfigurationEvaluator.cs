using System;
using ImplicitNullability.Plugin.Infrastructure;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;

namespace ImplicitNullability.Plugin.Configuration
{
    /// <summary>
    /// A service for reading / evaluating the <see cref="ImplicitNullabilityConfiguration"/> in a given <see cref="IPsiModule"/>.
    ///
    /// The resulting configuration depends on the content of an <see cref="T:System.Reflection.AssemblyMetadataAttribute"/> (if present)
    /// plus the <see cref="ImplicitNullabilitySettings"/> from the ReSharper settings.
    /// </summary>
    [PsiComponent]
    public class ImplicitNullabilityConfigurationEvaluator
    {
        private readonly ISettingsStore _settingsStore;
        private readonly ISettingsOptimization _settingsOptimization;
        private readonly Lazy<IPsiServices> _psiServices;

        public ImplicitNullabilityConfigurationEvaluator(
            ISettingsStore settingsStore,
            ISettingsOptimization settingsOptimization,
            Lazy<IPsiServices> psiServices /* Must be lazy because of the circular dependency (CodeAnnotationsCache) */)
        {
            _settingsStore = settingsStore;
            _settingsOptimization = settingsOptimization;
            _psiServices = psiServices;
        }

        public ImplicitNullabilityConfiguration EvaluateFor(IPsiModule psiModule)
        {
            // IDEA: Implement a PsiModule=>ImplicitNullabilityConfiguration cache (which gets invalidated on module changes)

            var implicitNullabilitySettings = GetSettings(psiModule);

            if (!implicitNullabilitySettings.Enabled)
                return ImplicitNullabilityConfiguration.AllDisabled;

            var configurationFromAssemblyAttribute = ParseConfigurationFromAssemblyAttribute(psiModule);

            if (configurationFromAssemblyAttribute.HasValue)
                return configurationFromAssemblyAttribute.Value;

            if (!psiModule.IsPartOfSolutionCode())
                return ImplicitNullabilityConfiguration.AllDisabled;

            return ImplicitNullabilityConfiguration.CreateFromSettings(implicitNullabilitySettings);
        }

        private ImplicitNullabilitySettings GetSettings(IPsiModule psiModule)
        {
            var contextRange = ContextRange.Smart(psiModule.ToDataContext());
            var contextBoundSettingsStore = _settingsStore.BindToContextTransient(contextRange);

            return contextBoundSettingsStore.GetKey<ImplicitNullabilitySettings>(_settingsOptimization);
        }

        private ImplicitNullabilityConfiguration? ParseConfigurationFromAssemblyAttribute(IPsiModule psiModule)
        {
            var moduleAttributes = _psiServices.Value.Symbols.GetModuleAttributes(psiModule);

            return AssemblyAttributeConfigurationTranslator.ParseAttributes(moduleAttributes);
        }
    }
}
