using System.Collections.Generic;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace ImplicitNullability.Plugin
{
    public class AnnotationRedundancyInHierarchyWarningFilteringDecorator : IHighlightingConsumer
    {
        private readonly IHighlightingConsumer _decorated;

        public AnnotationRedundancyInHierarchyWarningFilteringDecorator(IHighlightingConsumer decorated)
        {
            _decorated = decorated;
        }

        public void ConsumeHighlighting(HighlightingInfo highlightingInfo)
        {
            if (!(highlightingInfo.Highlighting is AnnotationRedundancyInHierarchyWarning))
            {
                _decorated.ConsumeHighlighting(highlightingInfo);
            }
        }

#if RS20173
        public IList<HighlightingInfo>Highlightings =>
#else
        public IReadOnlyCollection<HighlightingInfo> Highlightings =>
#endif
            _decorated.Highlightings;
    }
}
