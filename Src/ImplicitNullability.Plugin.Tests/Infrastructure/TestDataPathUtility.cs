using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Diagnostics;

namespace ImplicitNullability.Plugin.Tests.Infrastructure
{
    internal static class TestDataPathUtility
    {
        public static string GetPathRelativeToSolution(params string[] path)
        {
            return Path.Combine(new[] { GetDirectoryOfThisSoureFile(), "..", "..", ".." }.Concat(path).ToArray());
        }

        private static string GetDirectoryOfThisSoureFile([CallerFilePath] string filePath = null)
        {
            return Path.GetDirectoryName(filePath).NotNull();
        }
    }
}
