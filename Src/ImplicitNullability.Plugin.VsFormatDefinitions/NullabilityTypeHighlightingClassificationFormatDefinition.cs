using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace ImplicitNullability.Plugin.VsFormatDefinitions
{
    [ClassificationType(ClassificationTypeNames = HighlightingId)]
    [Order(After = "Formal Language Priority", Before = "Natural Language Priority")]
    [Export(typeof(EditorFormatDefinition))]
    [Name(HighlightingId)]
    [System.ComponentModel.DisplayName(DisplayNameText)]
    [UserVisible(true)]
    public class NullabilityTypeHighlightingClassificationFormatDefinition : ClassificationFormatDefinition
    {
        private const string HighlightingId = "ReSharperImplicitNullabilityTypeHighlighting"; // = StaticNullabilityTypeHighlighting.HighlightingId
        private const string DisplayNameText = "Implicit Nullability Type Highlighting";

        public NullabilityTypeHighlightingClassificationFormatDefinition()
        {
            DisplayName = DisplayNameText;
            ForegroundColor = Color.FromRgb(229, 61, 255); // = 'EffectColor' of the corresponding [RegisterHighlighter]
        }

#pragma warning disable 0649

        [Export, Name(HighlightingId), BaseDefinition("formal language")]
        internal ClassificationTypeDefinition ClassificationTypeDefinition;

#pragma warning restore 0649
    }
}
