using System;
using ImplicitNullability.Plugin.Settings;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Tree;

#if RESHARPER8
using IHighlighting = JetBrains.ReSharper.Daemon.Impl.IHighlightingWithRange;

#else

#endif

namespace ImplicitNullability.Plugin.Highlighting
{
    public abstract class ImplicitNullabilityHighlightingBase : IHighlighting
    {
        protected const string NeedsSettingNoteText =
            "Note that this warning will only be displayed for elements selected in the " +
            ImplicitNullabilityOptionsPage.PageTitle + " options page.";

        private readonly ITreeNode _treeNode;
        private readonly string _toolTipText;

        protected ImplicitNullabilityHighlightingBase(ITreeNode treeNode, string toolTipText)
        {
            _treeNode = treeNode;
            _toolTipText = toolTipText;
        }

        public string ToolTip
        {
            get { return _toolTipText; }
        }

        public string ErrorStripeToolTip
        {
            get { return _toolTipText; }
        }

        public int NavigationOffsetPatch
        {
            get { return 0; }
        }

        public bool IsValid()
        {
            return _treeNode.IsValid();
        }

        public DocumentRange CalculateRange()
        {
            return _treeNode.GetDocumentRange();
        }
    }
}