using ImplicitNullability.Plugin.Infrastructure;
using ImplicitNullability.Plugin.Settings;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
#if RESHARPER92
using JetBrains.Util.Lazy;
using JetBrains.Metadata.Reader.API;

#else
using System;

#endif

namespace ImplicitNullability.Plugin
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

        public ImplicitNullabilityConfiguration EvaluateFor([NotNull] IPsiModule psiModule)
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

        [NotNull]
        private ImplicitNullabilitySettings GetSettings([NotNull] IPsiModule psiModule)
        {
            var contextRange = ContextRange.Smart(psiModule.ToDataContext());
            var contextBoundSettingsStore = _settingsStore.BindToContextTransient(contextRange);

            return contextBoundSettingsStore.GetKey<ImplicitNullabilitySettings>(_settingsOptimization);
        }

        private ImplicitNullabilityConfiguration? ParseConfigurationFromAssemblyAttribute(IPsiModule psiModule)
        {
            var moduleAttributes = _psiServices.Value.Symbols.GetModuleAttributes(psiModule

#if RESHARPER92
                ,
                // The generic resolve context (e.g. necessary to resolve the attribute argument types) is sufficient in our case:
                UniversalModuleReferenceContext.Instance
#endif
                );

            return AssemblyAttributeConfigurationTranslator.ParseAttributes(moduleAttributes);
        }
    }
}