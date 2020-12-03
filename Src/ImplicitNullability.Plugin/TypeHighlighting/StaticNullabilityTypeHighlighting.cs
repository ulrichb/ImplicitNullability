using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl.DocumentMarkup;

// For ReSharper/VS's format definition, see NullabilityTypeHighlightingClassificationFormatDefinition

namespace ImplicitNullability.Plugin.TypeHighlighting
{
    [RegisterHighlighter(
        HighlightingId,
        EffectColor = "#E53DFF",
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
    public class StaticNullabilityTypeHighlighting : StaticNullabilityTypeHighlightingBase
    {
        public const string HighlightingId = "ReSharperImplicitNullabilityTypeHighlighting";
        private const string Message = "Info: '{0}' is (explicitly or implicitly) {1}";

        public StaticNullabilityTypeHighlighting(ITreeNode typeNode, string nullabilityKind)
            : base(typeNode, string.Format(Message, typeNode.GetText(), nullabilityKind))
        {
        }
    }
}
