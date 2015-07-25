using ImplicitNullability.Plugin.Highlighting;
using System;
using JetBrains.ReSharper.Psi.Tree;
#if RESHARPER8
using JetBrains.ReSharper.Daemon;

#else
using JetBrains.ReSharper.Feature.Services.Daemon;

#endif

[assembly: RegisterConfigurableSeverity(
    ImplicitNotNullConflictInHierarchyHighlighting.SeverityId,
    null,
    HighlightingGroupIds.CodeSmell,
    ImplicitNotNullConflictInHierarchyHighlighting.Message,
    ImplicitNotNullConflictInHierarchyHighlighting.Description,
    Severity.WARNING,
    solutionAnalysisRequired: false)]

namespace ImplicitNullability.Plugin.Highlighting
{
    [ConfigurableSeverityHighlighting(
        SeverityId,
        "CSHARP",
        OverlapResolve = OverlapResolveKind.WARNING,
        ToolTipFormatString = Message)]
    public class ImplicitNotNullConflictInHierarchyHighlighting : ImplicitNullabilityHighlightingBase
    {
        public const string SeverityId = "ImplicitNotNullConflictInHierarchy";
        public const string Message = "Implicit NotNull conflicts with nullability in super type";

        public const string Description =
            "Warns about substitutability violations, e.g. implicit NotNull parameter overrides base member with a corresponding CanBeNull parameter. " +
            "This is the equivalent to \"Annotation conflict in hierarchy\" for explicit NotNull annotations. " +
            NeedsSettingNoteText;

        public ImplicitNotNullConflictInHierarchyHighlighting(ITreeNode treeNode)
            : base(treeNode, Message)
        {
        }
    }
}