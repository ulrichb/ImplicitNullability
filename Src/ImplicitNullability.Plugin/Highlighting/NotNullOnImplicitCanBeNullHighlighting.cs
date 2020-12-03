using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;

namespace ImplicitNullability.Plugin.Highlighting
{
    [RegisterConfigurableSeverity(
        SeverityId,
        CompoundItemName: null,
        Group: HighlightingGroupIds.CodeSmell,
        Title: Message,
        Description: Description,
        DefaultSeverity: Severity.WARNING)]
    [ConfigurableSeverityHighlighting(
        SeverityId,
        CSharpLanguage.Name,
        OverlapResolve = OverlapResolveKind.WARNING,
        ToolTipFormatString = Message)]
    public class NotNullOnImplicitCanBeNullHighlighting : ImplicitNullabilityHighlightingBase
    {
        public const string SeverityId = "NotNullOnImplicitCanBeNull";

        public const string Message = "Implicit CanBeNull element has an explicit NotNull annotation";

        public const string Description =
            "Warns about explicit [NotNull] annotations on implicit CanBeBull elements (like parameters with a null default value). " +
            "This warning is useful when using tools like Fody NullGuard, which do not process explicit [NotNull] annotations. " +
            NeedsSettingNoteText;

        public NotNullOnImplicitCanBeNullHighlighting(ITreeNode treeNode)
            : base(treeNode, Message)
        {
        }
    }
}
