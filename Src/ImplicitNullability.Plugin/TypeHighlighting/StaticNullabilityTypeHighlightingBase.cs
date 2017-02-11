using JetBrains.ReSharper.Psi.Tree;
using ReSharperExtensionsShared.Highlighting;

namespace ImplicitNullability.Plugin.TypeHighlighting
{
    public abstract class StaticNullabilityTypeHighlightingBase : SimpleTreeNodeHighlightingBase<ITreeNode>
    {
        protected StaticNullabilityTypeHighlightingBase(ITreeNode treeNode, string toolTipText) :
            base(treeNode, toolTipText)
        {
        }
    }
}
