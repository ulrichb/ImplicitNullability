using ImplicitNullability.Plugin.Highlighting;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Tree;
using ReSharperExtensionsShared.Highlighting;

[assembly: RegisterConfigurableSeverity(
    ImplicitNotNullOverridesUnknownExternalMemberHighlighting.SeverityId,
    null,
    HighlightingGroupIds.CodeSmell,
    ImplicitNotNullOverridesUnknownExternalMemberHighlighting.Message,
    ImplicitNotNullOverridesUnknownExternalMemberHighlighting.Description,
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
    public class ImplicitNotNullOverridesUnknownExternalMemberHighlighting : SimpleTreeNodeHighlightingBase<ITreeNode>
    {
        // IDEA: Due to symmetry reasons with ImplicitNotNullConflictInHierarchyHighlighting and to emphasize that this is about
        // input/ref elements, rename to ImplicitNotNullPotentialConflictWithExternalBaseMember?

        public const string SeverityId = "ImplicitNotNullOverridesUnknownExternalMember";
        public const string Message = "Implicit NotNull overrides unknown nullability of external code, nullability should be explicit";

        public const string Description =
            "Warns about implicit [NotNull] elements that override an external base class/interface member " +
            "which has no corresponding nullability annotations (neither by attributes nor by external XML annotations). " +
            "Because the base member's nullability is unknown, we do not know whether a substitutability " +
            "violation exists (e.g. implicit [NotNull] parameter overrides base member with an unannotated " +
            "parameter, which has [CanBeNull] semantic), we thus encourage the programmer to explicitly " +
            "annotate [NotNull] or [CanBeNull] on those elements after manually checking the nullability of the base member. " +
            SharedHighlightingTexts.NeedsSettingNoteText;

        public ImplicitNotNullOverridesUnknownExternalMemberHighlighting(ITreeNode treeNode)
            : base(treeNode, Message)
        {
        }
    }
}
