using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl.DocumentMarkup;

// For ReSharper/VS's format definition, see NullabilityItemTypeHighlightingClassificationFormatDefinition

namespace ImplicitNullability.Plugin.TypeHighlighting
{
    [RegisterHighlighter(
        HighlightingId,
        EffectColor = "#FF7CE3",
        EffectType = EffectType.DOTTED_UNDERLINE,
        Layer = HighlighterLayer.SYNTAX,
        VSPriority = VSPriority.IDENTIFIERS)]
    [StaticSeverityHighlighting(
        Severity.INFO,
        typeof(HighlightingGroupIds.CodeSmellStatic),
        Languages = CSharpLanguage.Name,
        AttributeId = HighlightingId,
        ShowToolTipInStatusBar = false,
        ToolTipFormatString = Message)]
    public class StaticNullabilityItemTypeHighlighting : StaticNullabilityTypeHighlightingBase
    {
        public const string HighlightingId = "ReSharperImplicitNullabilityItemTypeHighlighting";
        private const string Message = "Info: Inner type '{0}' is (explicitly or implicitly) {1}";

        public StaticNullabilityItemTypeHighlighting(ITreeNode typeNode, string nullabilityKind)
            : base(typeNode, string.Format(Message, typeNode.GetText(), nullabilityKind))
        {
        }
    }
}
