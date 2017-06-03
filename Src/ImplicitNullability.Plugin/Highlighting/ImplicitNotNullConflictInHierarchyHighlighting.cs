using ImplicitNullability.Plugin.Highlighting;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;

[assembly: RegisterConfigurableSeverity(
    ImplicitNotNullConflictInHierarchyHighlighting.SeverityId,
    CompoundItemName: null,
    Group: HighlightingGroupIds.CodeSmell,
    Title: ImplicitNotNullConflictInHierarchyHighlighting.Message,
    Description: ImplicitNotNullConflictInHierarchyHighlighting.Description,
    DefaultSeverity: Severity.WARNING)]

namespace ImplicitNullability.Plugin.Highlighting
{
    [ConfigurableSeverityHighlighting(
        SeverityId,
        CSharpLanguage.Name,
        OverlapResolve = OverlapResolveKind.WARNING,
        OverloadResolvePriority = InputWarningPriority,
        ToolTipFormatString = Message)]
    public class ImplicitNotNullConflictInHierarchyHighlighting : ImplicitNullabilityHighlightingBase
    {
        public const string SeverityId = "ImplicitNotNullConflictInHierarchy";
        public const string Message = "Implicit NotNull conflicts with nullability in base type";

        public const string Description =
            "Warns about substitutability violations, e.g. implicit [NotNull] input parameter with a corresponding explicit [CanBeNull] " +
            "parameter in the base member. " +
            "This is the equivalent to \"Annotation conflict in hierarchy\" for explicit [NotNull] annotations. " +
            NeedsSettingNoteText;

        public ImplicitNotNullConflictInHierarchyHighlighting(ITreeNode treeNode)
            : base(treeNode, Message)
        {
        }
    }
}
