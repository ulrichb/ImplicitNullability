using System.Collections.Generic;
using System.Diagnostics;
using ImplicitNullability.Plugin.Infrastructure;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.Util;
using ReSharperExtensionsShared.Debugging;

namespace ImplicitNullability.Plugin
{
    /// <summary>
    /// An implementation of ReSharper's <see cref="ICustomCodeAnnotationProvider"/> extension point which uses
    /// the <see cref="ImplicitNullabilityProvider"/> to implement the Implicit Nullability rules.
    /// 
    /// Note that these "custom" providers are called by R# at last (basically if there are no (inherited) nullability attributes).
    /// </summary>
    [PsiComponent]
    public class ImplicitNullabilityCustomCodeAnnotationProvider : ICustomCodeAnnotationProvider
    {
        private static readonly ILogger Logger = JetBrains.Util.Logging.Logger.GetLogger(typeof(ImplicitNullabilityCustomCodeAnnotationProvider));

        private readonly ImplicitNullabilityProvider _implicitNullabilityProvider;

        public ImplicitNullabilityCustomCodeAnnotationProvider(ImplicitNullabilityProvider implicitNullabilityProvider)
        {
            Logger.Verbose(".ctor");
            _implicitNullabilityProvider = implicitNullabilityProvider;
        }

        public CodeAnnotationNullableValue? GetNullableAttribute([CanBeNull] IDeclaredElement element)
        {
#if DEBUG
            var stopwatch = Stopwatch.StartNew();
#endif

            var result = _implicitNullabilityProvider.AnalyzeDeclaredElement(element);

#if DEBUG
            LogResult("Attribute for ", element, stopwatch, result);
#endif
            return result;
        }

        public CodeAnnotationNullableValue? GetContainerElementNullableAttribute([CanBeNull] IDeclaredElement element)
        {
#if DEBUG
            var stopwatch = Stopwatch.StartNew();
#endif

            var result = _implicitNullabilityProvider.AnalyzeDeclaredElementContainerElement(element);

#if DEBUG
            LogResult("Elem attr for ", element, stopwatch, result);
#endif

            return result;
        }

        public ICollection<IAttributeInstance> GetSpecialAttributeInstances([CanBeNull] IClrDeclaredElement element)
        {
            return EmptyList<IAttributeInstance>.InstanceList;
        }

#if DEBUG

        private static void LogResult(
            string messagePrefix,
            [CanBeNull] IDeclaredElement element,
            Stopwatch stopwatch,
            CodeAnnotationNullableValue? result)
        {
            var resultText = result.IsUnknown() ? "UNKNOWN" : result.ToString();
            var message = messagePrefix + DebugUtility.FormatIncludingContext(element) + " => " + resultText;
            Logger.Verbose(DebugUtility.FormatWithElapsed(message, stopwatch));
        }

#endif
    }
}
