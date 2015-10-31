using ImplicitNullability.Plugin.Highlighting;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Tree;
using ReSharperExtensionsShared.Highlighting;

[assembly: RegisterConfigurableSeverity(
    ImplicitNotNullElementCannotOverrideCanBeNullHighlighting.SeverityId,
    null,
    HighlightingGroupIds.CodeSmell,
    ImplicitNotNullElementCannotOverrideCanBeNullHighlighting.Message,
    ImplicitNotNullElementCannotOverrideCanBeNullHighlighting.Description,
    Severity.WARNING,
    /*SolutionAnalysisRequired:*/ false)]

namespace ImplicitNullability.Plugin.Highlighting
{
    [ConfigurableSeverityHighlighting(
        SeverityId,
        "CSHARP",
        OverlapResolve = OverlapResolveKind.WARNING,
        ToolTipFormatString = Message)]
    public class ImplicitNotNullElementCannotOverrideCanBeNullHighlighting : SimpleTreeNodeHighlightingBase<ITreeNode>
    {
        public const string SeverityId = "ImplicitNotNullElementCannotOverrideCanBeNull";

        public const string Message = "Implicit NotNull element cannot override CanBeNull in base type, nullability should be explicit";

        public const string Description =
            "Warns about implicit [NotNull] results or out parameters with a corresponding [CanBeNull] element in a base member. " +
            "It's better to explicitly annotate these occurrences with [NotNull] or [CanBeNull] " +
            "because the implicit [NotNull] doesn't have any effect (ReSharper inherits the base's [CanBeNull]). " +
            SharedHighlightingTexts.NeedsSettingNoteText;

        public ImplicitNotNullElementCannotOverrideCanBeNullHighlighting([NotNull] ITreeNode treeNode)
            : base(treeNode, Message)
        {
        }
    }
}