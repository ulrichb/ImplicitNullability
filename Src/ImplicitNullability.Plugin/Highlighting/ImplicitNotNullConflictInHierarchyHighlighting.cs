using ImplicitNullability.Plugin.Highlighting;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Tree;
using ReSharperExtensionsShared.Highlighting;

[assembly: RegisterConfigurableSeverity(
    ImplicitNotNullConflictInHierarchyHighlighting.SeverityId,
    null,
    HighlightingGroupIds.CodeSmell,
    ImplicitNotNullConflictInHierarchyHighlighting.Message,
    ImplicitNotNullConflictInHierarchyHighlighting.Description,
    Severity.WARNING
#if RESHARPER20161 || RESHARPER20162
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
    public class ImplicitNotNullConflictInHierarchyHighlighting : SimpleTreeNodeHighlightingBase<ITreeNode>
    {
        public const string SeverityId = "ImplicitNotNullConflictInHierarchy";
        public const string Message = "Implicit NotNull conflicts with nullability in base type";

        public const string Description =
            "Warns about substitutability violations, e.g. implicit [NotNull] input parameter with a corresponding explicit [CanBeNull] " +
            "parameter in the base member. " +
            "This is the equivalent to \"Annotation conflict in hierarchy\" for explicit [NotNull] annotations. " +
            SharedHighlightingTexts.NeedsSettingNoteText;

        public ImplicitNotNullConflictInHierarchyHighlighting(ITreeNode treeNode)
            : base(treeNode, Message)
        {
        }
    }
}