using ImplicitNullability.Plugin.Highlighting;
using System;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Tree;
#if RESHARPER8
using JetBrains.ReSharper.Daemon;
#else
#endif

[assembly: RegisterConfigurableSeverity (
    NotNullOnImplicitCanBeNullHighlighting.SeverityId,
    null,
    HighlightingGroupIds.CodeSmell,
    NotNullOnImplicitCanBeNullHighlighting.Message,
    NotNullOnImplicitCanBeNullHighlighting.Description,
    Severity.WARNING,
    solutionAnalysisRequired: false)]

namespace ImplicitNullability.Plugin.Highlighting
{
  [ConfigurableSeverityHighlighting (
      SeverityId,
      "CSHARP",
      OverlapResolve = OverlapResolveKind.WARNING,
      ToolTipFormatString = Message)]
  public class NotNullOnImplicitCanBeNullHighlighting : ImplicitNullabilityHighlightingBase
  {
    public const string SeverityId = "NotNullOnImplicitCanBeNull";

    public const string Message = "Implicit CanBeNull parameter has an explicit NotNull annotation";

    public const string Description =
        "Warns about explicit NotNull annotations on implicit CanBeBull elements (like parameters with a null default value). " +
        "This warning is useful when using tools like Fody NullGuard, which do not process explicit NotNull annotations. " +
        NeedsSettingNoteText;

    public NotNullOnImplicitCanBeNullHighlighting (ITreeNode treeNode)
        : base (treeNode, Message)
    {
    }
  }
}