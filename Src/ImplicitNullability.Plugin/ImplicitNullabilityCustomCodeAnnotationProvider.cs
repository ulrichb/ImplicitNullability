using System;
using System.Diagnostics;
using ImplicitNullability.Plugin.Infrastructure;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.Util;
using ReSharperExtensionsShared;

namespace ImplicitNullability.Plugin
{
    [PsiComponent]
    public class ImplicitNullabilityCustomCodeAnnotationProvider : ICustomCodeAnnotationProvider
    {
        private static readonly ILogger Logger = JetBrains.Util.Logging.Logger.GetLogger(typeof (ImplicitNullabilityCustomCodeAnnotationProvider));

        private readonly ImplicitNullabilityProvider _implicitNullabilityProvider;

        public ImplicitNullabilityCustomCodeAnnotationProvider(ImplicitNullabilityProvider implicitNullabilityProvider)
        {
            Logger.LogMessage(LoggingLevel.INFO, ".ctor");
            _implicitNullabilityProvider = implicitNullabilityProvider;
        }

        public CodeAnnotationNullableValue? GetNullableAttribute(IDeclaredElement element)
        {
#if DEBUG
            var stopwatch = Stopwatch.StartNew();
#endif

            CodeAnnotationNullableValue? result = null;

            var parameter = element as IParameter;
            if (parameter != null)
                result = _implicitNullabilityProvider.AnalyzeParameter(parameter);

            var function = element as IFunction;
            if (function != null)
                result = _implicitNullabilityProvider.AnalyzeFunction(function);

            var @delegate = element as IDelegate;
            if (@delegate != null)
                result = _implicitNullabilityProvider.AnalyzeDelegate(@delegate);

#if DEBUG
            var message = DebugUtilities.FormatIncludingContext(element) + " => " + (result.IsUnknown() ? "<unknown>" : result.ToString());

            Logger.LogMessage(LoggingLevel.VERBOSE, DebugUtilities.FormatWithElapsed(message, stopwatch));
#endif
            return result;
        }

#if !(RESHARPER8 || RESHARPER91)
        public CodeAnnotationNullableValue? GetContainerElementNullableAttribute(IDeclaredElement element)
        {
            return null;
        }
#endif
    }
}