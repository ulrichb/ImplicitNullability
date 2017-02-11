using ImplicitNullability.Plugin.VsFormatDefinitions;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl.DocumentMarkup;

[assembly: RegisterHighlighter(
    NullabilityTypeHighlightingClassificationFormatDefinition.HighlightingId,
    EffectType = EffectType.DOTTED_UNDERLINE,
    Layer = HighlighterLayer.SYNTAX,
    VSPriority = VSPriority.IDENTIFIERS)]

namespace ImplicitNullability.Plugin.TypeHighlighting
{
    [StaticSeverityHighlighting(
        Severity.INFO,
        CSharpLanguage.Name,
        AttributeId = NullabilityTypeHighlightingClassificationFormatDefinition.HighlightingId,
        ShowToolTipInStatusBar = false,
        ToolTipFormatString = Message)]
    public class StaticNullabilityTypeHighlighting : StaticNullabilityTypeHighlightingBase
    {
        private const string Message = "'{0}' is (explicitly or implicitly) {1}";

        public StaticNullabilityTypeHighlighting(ITreeNode typeNode, string nullabilityKind)
            : base(typeNode, string.Format(Message, typeNode.GetText(), nullabilityKind))
        {
        }
    }
}
