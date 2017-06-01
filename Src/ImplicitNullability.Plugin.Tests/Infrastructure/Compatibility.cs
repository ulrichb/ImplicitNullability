#if RESHARPER20163
using System;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Tree;

namespace ImplicitNullability.Plugin.Tests.Infrastructure
{
    public static class Compatibility
    {
        [CanBeNull]
        public static ITreeNode FindPreviousNode([NotNull] this ITreeNode node, [NotNull] Func<ITreeNode, TreeNodeActionType> predicate)
        {
            return node.FindPrevNode(predicate);
        }
    }
}

#endif
