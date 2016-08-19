using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Tree;
using ReSharperExtensionsShared.Highlighting;

namespace ImplicitNullability.Plugin.TypeHighlighting
{
    public abstract class StaticNullabilityTypeHighlightingBase : SimpleTreeNodeHighlightingBase<ITreeNode>
    {
        protected StaticNullabilityTypeHighlightingBase([NotNull] ITreeNode treeNode, [NotNull] string toolTipText) :
            base(treeNode, toolTipText)
        {
        }
    }
}