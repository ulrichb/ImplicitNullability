using ImplicitNullability.Plugin.Highlighting;
using System;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.CSharp.ControlFlow;
using JetBrains.ReSharper.Psi.Tree;
using ReSharperExtensionsShared.Highlighting;
#if RESHARPER8
using JetBrains.ReSharper.Daemon;

#else
using JetBrains.ReSharper.Feature.Services.Daemon;

#endif

[assembly: RegisterConfigurableSeverity(
    LoooolHighlighting.SeverityId,
    null,
    HighlightingGroupIds.CodeSmell,
    LoooolHighlighting.Message,
    LoooolHighlighting.Description,
    Severity.WARNING,
    solutionAnalysisRequired: false)]

namespace ImplicitNullability.Plugin.Highlighting
{
    [ConfigurableSeverityHighlighting(
        SeverityId,
        "CSHARP",
        OverlapResolve = OverlapResolveKind.WARNING,
        ToolTipFormatString = Message)]
    public class LoooolHighlighting : SimpleTreeNodeHighlightingBase<ITreeNode>
    {
        public const string SeverityId = "LoooolHighlighting";

        public const string Message = "Unknown nullability to implicitly NotNull!";

        public const string Description =
            "LoooolHighlighting";

        public LoooolHighlighting([NotNull] ITreeNode treeNode)
            : base(treeNode, Message)
        {
        }
    }
}