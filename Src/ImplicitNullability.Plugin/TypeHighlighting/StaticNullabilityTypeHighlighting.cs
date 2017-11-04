using ImplicitNullability.Plugin.TypeHighlighting;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl.DocumentMarkup;

// For ReSharper/VS's format definition, see NullabilityTypeHighlightingClassificationFormatDefinition

[assembly: RegisterHighlighter(
    StaticNullabilityTypeHighlighting.HighlightingId,
    EffectColor = "#E53DFF",
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
