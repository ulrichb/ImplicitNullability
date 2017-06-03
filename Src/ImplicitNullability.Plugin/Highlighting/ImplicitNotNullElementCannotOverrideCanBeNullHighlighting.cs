using ImplicitNullability.Plugin.Highlighting;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;

[assembly: RegisterConfigurableSeverity(
    ImplicitNotNullElementCannotOverrideCanBeNullHighlighting.SeverityId,
    CompoundItemName: null,
    Group: HighlightingGroupIds.CodeSmell,
    Title: ImplicitNotNullElementCannotOverrideCanBeNullHighlighting.Message,
    Description: ImplicitNotNullElementCannotOverrideCanBeNullHighlighting.Description,
    DefaultSeverity: Severity.WARNING)]

namespace ImplicitNullability.Plugin.Highlighting
{
    [ConfigurableSeverityHighlighting(
        SeverityId,
        CSharpLanguage.Name,
        OverlapResolve = OverlapResolveKind.WARNING,
        OverloadResolvePriority = OutputWarningPriority,
        ToolTipFormatString = Message)]
    public class ImplicitNotNullElementCannotOverrideCanBeNullHighlighting : ImplicitNullabilityHighlightingBase
    {
        public const string SeverityId = "ImplicitNotNullElementCannotOverrideCanBeNull";

        public const string Message = "Implicit NotNull element cannot override CanBeNull in base type, nullability should be explicit";

        public const string Description =
            "Warns about implicit [NotNull] results or out parameters with a corresponding [CanBeNull] element in a base member. " +
            "It's better to explicitly annotate these occurrences with [NotNull] or [CanBeNull] " +
            "because the implicit [NotNull] doesn't have any effect (ReSharper inherits the base's [CanBeNull]). " +
            NeedsSettingNoteText;

        public ImplicitNotNullElementCannotOverrideCanBeNullHighlighting(ITreeNode treeNode)
            : base(treeNode, Message)
        {
        }
    }
}
