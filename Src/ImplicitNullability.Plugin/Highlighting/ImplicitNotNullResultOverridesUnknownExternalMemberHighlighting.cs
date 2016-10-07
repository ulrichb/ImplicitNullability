using ImplicitNullability.Plugin.Highlighting;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Tree;
using ReSharperExtensionsShared.Highlighting;

[assembly: RegisterConfigurableSeverity(
               ImplicitNotNullResultOverridesUnknownExternalMemberHighlighting.SeverityId,
               null,
               HighlightingGroupIds.CodeSmell,
               ImplicitNotNullResultOverridesUnknownExternalMemberHighlighting.Message,
               ImplicitNotNullResultOverridesUnknownExternalMemberHighlighting.Description,
               Severity.HINT
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
    public class ImplicitNotNullResultOverridesUnknownExternalMemberHighlighting : SimpleTreeNodeHighlightingBase<ITreeNode>
    {
        public const string SeverityId = "ImplicitNotNullResultOverridesUnknownExternalMember";
        public const string Message = "Implicit NotNull result or out parameter overrides unknown nullability of external code";

        public const string Description =
            "Warns about implicit [NotNull] results or out parameters that override an external base class/interface member " +
            "which has no corresponding nullability annotations (neither by attributes nor by external XML annotations). " +
            "As the the base member's nullability is unknown, we implicitly convert a possibly [CanBeNull] result to [NotNull] if " +
            "the base calls' return value is returned. " +
            "It's better to explicitly annotate these occurrences with [NotNull] or [CanBeNull] after manually checking " +
            "the nullability of the base member. " +
            SharedHighlightingTexts.NeedsSettingNoteText;

        public ImplicitNotNullResultOverridesUnknownExternalMemberHighlighting(ITreeNode treeNode)
            : base(treeNode, Message)
        {
        }
    }
}