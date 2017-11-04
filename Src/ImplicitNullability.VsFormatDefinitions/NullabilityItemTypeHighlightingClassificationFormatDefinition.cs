using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace ImplicitNullability.VsFormatDefinitions
{
    [ClassificationType(ClassificationTypeNames = HighlightingId)]
    [Order(After = "Formal Language Priority", Before = "Natural Language Priority")]
    [Export(typeof(EditorFormatDefinition))]
    [Name(HighlightingId)]
    [System.ComponentModel.DisplayName(DisplayNameText)]
    [UserVisible(true)]
    public class NullabilityItemTypeHighlightingClassificationFormatDefinition : ClassificationFormatDefinition
    {
        private const string HighlightingId = "ReSharperImplicitNullabilityItemTypeHighlighting"; // = StaticNullabilityItemTypeHighlighting.HighlightingId
        private const string DisplayNameText = "Implicit Nullability Item Type Highlighting";

        public NullabilityItemTypeHighlightingClassificationFormatDefinition()
        {
            DisplayName = DisplayNameText;
            ForegroundColor = Color.FromRgb(255, 124, 227); // = 'EffectColor' of the corresponding [RegisterHighlighter]
        }

#pragma warning disable 0649

        [Export, Name(HighlightingId), BaseDefinition("formal language")]
        internal ClassificationTypeDefinition ClassificationTypeDefinition;

#pragma warning restore 0649
    }
}
