using ImplicitNullability.Plugin.Configuration.OptionsPages;
using JetBrains.ReSharper.Psi.Tree;
using ReSharperExtensionsShared.Highlighting;

namespace ImplicitNullability.Plugin.Highlighting
{
    public abstract class ImplicitNullabilityHighlightingBase : SimpleTreeNodeHighlightingBase<ITreeNode>
    {
        protected const int InputWarningPriority = 1;
        protected const int OutputWarningPriority = 0;

        protected const string NeedsSettingNoteText =
            "Note that this warning will only be displayed for elements with enabled implicit nullability (see the \"" +
            ImplicitNullabilityOptionsPage.PageTitle + "\" options page).";

        protected ImplicitNullabilityHighlightingBase(ITreeNode treeNode, string toolTipText) : base(treeNode, toolTipText)
        {
        }
    }
}
