using System.Collections.Generic;
using System.Linq;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.SolutionAnalysis;
using JetBrains.ReSharper.Psi;

namespace ImplicitNullability.Plugin.Tests.Infrastructure
{
    public static class MiscTestExtensions
    {
        public static IEnumerable<IProjectFile> GetAllProjectFilesWithPathPrefix(this ISolution solution, string prefix)
        {
            return
                (from project in solution.GetAllProjects()
                    from file in project.GetAllProjectFiles()
                    where ProjectUtil.GetRelativePresentableProjectPath(file, project).StartsWith(prefix)
                    select file).ToList();
        }

        public static IPsiSourceFile GetSourceFile(this IIssue issue)
        {
            return issue.File.File;
        }
    }
}