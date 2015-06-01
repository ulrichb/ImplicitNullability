using System.Collections.Generic;
using System.Linq;
using JetBrains.ProjectModel;

namespace ImplicitNullability.Plugin.Tests.Infrastructure
{
    public static class SolutionExtensions
    {
        public static IEnumerable<IProjectFile> GetAllProjectFilesWithPathPrefix(this ISolution solution, string prefix)
        {
            return
                (from project in solution.GetAllProjects()
                    from file in project.GetAllProjectFiles()
                    where ProjectUtil.GetRelativePresentableProjectPath(file, project).StartsWith(prefix)
                    select file).ToList();
        }
    }
}