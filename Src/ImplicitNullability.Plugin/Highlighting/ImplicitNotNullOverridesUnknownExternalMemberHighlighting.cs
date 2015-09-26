using ImplicitNullability.Plugin.Highlighting;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Tree;
using ReSharperExtensionsShared.Highlighting;

[assembly: RegisterConfigurableSeverity(
    ImplicitNotNullOverridesUnknownExternalMemberHighlighting.SeverityId,
    null,
    HighlightingGroupIds.CodeSmell,
    ImplicitNotNullOverridesUnknownExternalMemberHighlighting.Message,
    ImplicitNotNullOverridesUnknownExternalMemberHighlighting.Description,
    Severity.WARNING,
    /*SolutionAnalysisRequired:*/ false)]

namespace ImplicitNullability.Plugin.Highlighting
{
    [ConfigurableSeverityHighlighting(
        SeverityId,
        "CSHARP",
        OverlapResolve = OverlapResolveKind.WARNING,
        ToolTipFormatString = Message)]
    public class ImplicitNotNullOverridesUnknownExternalMemberHighlighting : SimpleTreeNodeHighlightingBase<ITreeNode>
    {
        public const string SeverityId = "ImplicitNotNullOverridesUnknownExternalMember";
        public const string Message = "Implicit NotNull overrides unknown nullability of external code, nullability should be explicit";

        public const string Description =
            "Warns about implicit NotNull elements that override an external base class/interface member, which has no nullabiliy annotations (neither " +
            "by attributes nor by external XML annotations), e.g. an implicit NotNull parameter of a method, which implements an external interface. " +
            "Because the base member's nullability is unknown, we do not know whether a substitutability violation exists (e.g. implicit NotNull " +
            "parameter overrides base member with a unannotated parameter, which has CanBeNull semantic), we thus require the programmer to " +
            "explicitly annotate this parameter after manually checking the constraints of the base member. " +
            SharedHighlightingTexts.NeedsSettingNoteText;

        public ImplicitNotNullOverridesUnknownExternalMemberHighlighting([NotNull] ITreeNode treeNode)
            : base(treeNode, Message)
        {
        }
    }
}