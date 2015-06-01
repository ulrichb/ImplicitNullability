using System;
using System.Diagnostics;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.Util;
using JetBrains.Util.Logging;
using ReSharperExtensionsShared;

namespace ImplicitNullability.Plugin
{
    [PsiComponent]
    public class ImplicitNullabilityAnnotationProvider : ICustomCodeAnnotationProvider
    {
        private static readonly ILogger s_logger = Logger.GetLogger(typeof (ImplicitNullabilityAnnotationProvider));

        private readonly ImplicitNullabilityProvider _implicitNullabilityProvider;

        public ImplicitNullabilityAnnotationProvider(ImplicitNullabilityProvider implicitNullabilityProvider)
        {
            s_logger.LogMessage(LoggingLevel.INFO, ".ctor");
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

            var method = element as IMethod;
            if (method != null)
                result = _implicitNullabilityProvider.AnalyzeMethod(method);

#if DEBUG
            var message = DebugUtilities.FormatIncludingContext(element) + " => " + (result == null ? "NULL" : result.ToString());

            s_logger.LogMessage(LoggingLevel.VERBOSE, DebugUtilities.FormatWithElapsed(message, stopwatch));
#endif
            return result;
        }
    }
}