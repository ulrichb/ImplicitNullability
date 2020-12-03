using System.Collections.Generic;
using JetBrains.DocumentModel;
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

        public void ConsumeHighlighting(HighlightingInfo highlightingInfo, DocumentRange[] secondaryDocumentRanges = null, string secondaryAttributeId = null)
        {
            if (!(highlightingInfo.Highlighting is AnnotationRedundancyInHierarchyWarning))
            {
                _decorated.ConsumeHighlighting(highlightingInfo);
            }
        }

        public IReadOnlyList<HighlightingInfo> Highlightings =>
            _decorated.Highlightings;
    }
}
