using ImplicitNullability.Plugin.Highlighting;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Tree;
using ReSharperExtensionsShared.Highlighting;

[assembly: RegisterConfigurableSeverity(
    NotNullOnImplicitCanBeNullHighlighting.SeverityId,
    null,
    HighlightingGroupIds.CodeSmell,
    NotNullOnImplicitCanBeNullHighlighting.Message,
    NotNullOnImplicitCanBeNullHighlighting.Description,
    Severity.WARNING
#if RESHARPER20162
    , SolutionAnalysisRequired: false
#endif
)]

namespace ImplicitNullability.Plugin.Highlighting
{
    [ConfigurableSeverityHighlighting(
        SeverityId,
        "CSHARP",
        OverlapResolve = OverlapResolveKind.WARNING,
        ToolTipFormatString = Message)]
    public class NotNullOnImplicitCanBeNullHighlighting : SimpleTreeNodeHighlightingBase<ITreeNode>
    {
        public const string SeverityId = "NotNullOnImplicitCanBeNull";

        public const string Message = "Implicit CanBeNull element has an explicit NotNull annotation";

        public const string Description =
            "Warns about explicit [NotNull] annotations on implicit CanBeBull elements (like parameters with a null default value). " +
            "This warning is useful when using tools like Fody NullGuard, which do not process explicit [NotNull] annotations. " +
            SharedHighlightingTexts.NeedsSettingNoteText;

        public NotNullOnImplicitCanBeNullHighlighting(ITreeNode treeNode)
            : base(treeNode, Message)
        {
        }
    }
}
