using ImplicitNullability.Plugin.Highlighting;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Tree;
using ReSharperExtensionsShared.Highlighting;

[assembly: RegisterConfigurableSeverity(
    ImplicitNotNullConflictInHierarchyHighlighting.SeverityId,
    null,
    HighlightingGroupIds.CodeSmell,
    ImplicitNotNullConflictInHierarchyHighlighting.Message,
    ImplicitNotNullConflictInHierarchyHighlighting.Description,
    Severity.WARNING,
    /*SolutionAnalysisRequired:*/ false)]

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
        public const string Message = "Implicit NotNull conflicts with nullability in super type";

        public const string Description =
            "Warns about substitutability violations, e.g. implicit NotNull parameter overrides base member with a corresponding CanBeNull parameter. " +
            "This is the equivalent to \"Annotation conflict in hierarchy\" for explicit NotNull annotations. " +
            SharedHighlightingTexts.NeedsSettingNoteText;

        public ImplicitNotNullConflictInHierarchyHighlighting([NotNull] ITreeNode treeNode)
            : base(treeNode, Message)
        {
        }
    }
}