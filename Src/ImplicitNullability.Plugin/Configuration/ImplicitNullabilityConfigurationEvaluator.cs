using System;
using System.Collections.Concurrent;
using ImplicitNullability.Plugin.Infrastructure;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;

namespace ImplicitNullability.Plugin.Configuration
{
    /// <summary>
    /// A service for reading / evaluating the <see cref="ImplicitNullabilityConfiguration"/> in a given <see cref="IPsiModule"/>.
    ///
    /// The resulting configuration depends on the content of declared <see cref="T:System.Reflection.AssemblyMetadataAttribute"/>s (if present)
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
            var cache = psiModule.GetPsiServices().Caches.GetPsiCache<ConfigurationCache>();

            return cache.GetOrAdd(psiModule, CalculateConfiguration);
        }

        private ImplicitNullabilityConfiguration CalculateConfiguration(IPsiModule psiModule)
        {
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

        [PsiComponent]
        private class ConfigurationCache : InvalidatingPsiCache
        {
            private readonly ConcurrentDictionary<IPsiModule, ImplicitNullabilityConfiguration> _dict =
                new ConcurrentDictionary<IPsiModule, ImplicitNullabilityConfiguration>();

            protected override void InvalidateOnPhysicalChange(PsiChangedElementType elementType)
            {
                // Note that this method gets also called on settings changes.

                // Here we clear the complete cache ony _any_ change. This strategy makes cache invalidation easy (least error prone) and
                // is absolutely sufficient for the overall performance of ImplicitNullabilityConfigurationEvaluator as the calculation
                // itself is not the problem, only the vast amount of accesses (per module).

                _dict.Clear();
            }

            public ImplicitNullabilityConfiguration GetOrAdd(IPsiModule psiModule, Func<IPsiModule, ImplicitNullabilityConfiguration> func)
            {
                return _dict.GetOrAdd(psiModule, func);
            }
        }
    }
}
