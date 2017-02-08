using ImplicitNullability.Plugin.VsFormatDefinitions;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl.DocumentMarkup;

[assembly: RegisterHighlighter(
    NullabilityItemTypeHighlightingClassificationFormatDefinition.HighlightingId,
    EffectType = EffectType.DOTTED_UNDERLINE,
    Layer = HighlighterLayer.SYNTAX,
    VSPriority = VSPriority.IDENTIFIERS)]

namespace ImplicitNullability.Plugin.TypeHighlighting
{
    [StaticSeverityHighlighting(
        Severity.INFO,
        CSharpLanguage.Name,
        AttributeId = NullabilityItemTypeHighlightingClassificationFormatDefinition.HighlightingId,
        ShowToolTipInStatusBar = false,
        ToolTipFormatString = Message)]
    public class StaticNullabilityItemTypeHighlighting : StaticNullabilityTypeHighlightingBase
    {
        private const string Message = "Inner type '{0}' is (explicitly or implicitly) {1}";

        public StaticNullabilityItemTypeHighlighting(ITreeNode typeNode, string nullabilityKind)
            : base(typeNode, string.Format(Message, typeNode.GetText(), nullabilityKind))
        {
        }
    }
}