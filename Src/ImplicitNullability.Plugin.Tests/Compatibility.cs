using System.Collections.Generic;
using JetBrains.ReSharper.Daemon.SolutionAnalysis;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

#if RESHARPER8
using System.Linq;
using JetBrains.Util;

#endif

namespace ImplicitNullability.Plugin.Tests
{
  public static class Compatibility
  {
    public static IPsiSourceFile GetSourceFile (this IIssue issue)
    {
# if RESHARPER8
      return issue.File.ToSourceFile().NotNull();
#else
      return issue.File.File;
#endif
    }

    public static IEnumerable<IComment> GetAllCommentNodes (this IFile file)
    {
# if RESHARPER8
      return file.EnumerateSubTree().OfType<IComment>();
#else
      return file.ThisAndDescendants().OfType<IComment>().ToEnumerable();
#endif
    }
  }
}