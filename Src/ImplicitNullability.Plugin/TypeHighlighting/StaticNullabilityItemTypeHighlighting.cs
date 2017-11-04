using ImplicitNullability.Plugin.TypeHighlighting;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl.DocumentMarkup;

// For ReSharper/VS's format definition, see NullabilityItemTypeHighlightingClassificationFormatDefinition

[assembly: RegisterHighlighter(
    StaticNullabilityItemTypeHighlighting.HighlightingId,
    EffectColor = "#FF7CE3",
    EffectType = EffectType.DOTTED_UNDERLINE,
    Layer = HighlighterLayer.SYNTAX,
    VSPriority = VSPriority.IDENTIFIERS)]

namespace ImplicitNullability.Plugin.TypeHighlighting
{
    [StaticSeverityHighlighting(
        Severity.INFO,
        CSharpLanguage.Name,
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
