using System.Collections.Generic;
using JetBrains.Application.Settings;
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

        public IDaemonStageProcess Process => _decorated.Process;

        public bool IsNonUserFile => _decorated.IsNonUserFile;

        public bool IsGeneratedFile => _decorated.IsGeneratedFile;

        public IHighlightingSettingsManager HighlightingSettingsManager => _decorated.HighlightingSettingsManager;

        public IContextBoundSettingsStore SettingsStore => _decorated.SettingsStore;

        public IList<HighlightingInfo> Highlightings => _decorated.Highlightings;
    }
}
