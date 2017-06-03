using ImplicitNullability.Plugin.Highlighting;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;

[assembly: RegisterConfigurableSeverity(
    ImplicitNotNullOverridesUnknownBaseMemberNullabilityHighlighting.SeverityId,
    CompoundItemName: null,
    Group: HighlightingGroupIds.CodeSmell,
    Title: ImplicitNotNullOverridesUnknownBaseMemberNullabilityHighlighting.Message,
    Description: ImplicitNotNullOverridesUnknownBaseMemberNullabilityHighlighting.Description,
    DefaultSeverity: Severity.WARNING,
    AlternativeIDs = "ImplicitNotNullOverridesUnknownExternalMember")]

namespace ImplicitNullability.Plugin.Highlighting
{
    [ConfigurableSeverityHighlighting(
        SeverityId,
        CSharpLanguage.Name,
        OverlapResolve = OverlapResolveKind.WARNING,
        OverloadResolvePriority = InputWarningPriority,
        ToolTipFormatString = Message)]
    public class ImplicitNotNullOverridesUnknownBaseMemberNullabilityHighlighting : ImplicitNullabilityHighlightingBase
    {
        public const string SeverityId = "ImplicitNotNullOverridesUnknownBaseMemberNullability";

        public const string Message = "Implicit NotNull overrides unknown nullability of base member, nullability should be explicit";

        public const string Description =
            "Warns about implicit [NotNull] elements that override a base class/interface member " +
            "which has no corresponding nullability annotations (neither by attributes nor by external XML annotations). " +
            "Because the base member's nullability is unknown, we do not know whether a substitutability " +
            "violation exists (e.g. implicit [NotNull] parameter overrides base member with an unannotated " +
            "parameter which actually has [CanBeNull] semantics), we thus encourage the programmer to explicitly " +
            "annotate [NotNull] or [CanBeNull] on those elements after manually checking the nullability of the base member. " +
            NeedsSettingNoteText;

        public ImplicitNotNullOverridesUnknownBaseMemberNullabilityHighlighting(ITreeNode treeNode)
            : base(treeNode, Message)
        {
        }
    }
}
