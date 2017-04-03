using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using FakeItEasy;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.SolutionAnalysis;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.TestFramework;
using JetBrains.Util;
using NCalc;
using NUnit.Framework;

namespace ImplicitNullability.Plugin.Tests.Infrastructure
{
    public abstract class InspectionExpectationCommentsTestBase : BaseTestWithExistingSolution
    {
        protected IList<IIssue> TestExpectedInspectionComments(
            ISolution solution,
            IEnumerable<IProjectFile> projectFilesToAnalyze,
            IEnumerable<Type> highlightingTypesToAnalyze,
            params string[] definedExpectedWarningSymbols)
        {
            var sourceFilesToAnalyze = projectFilesToAnalyze.Select(x => x.ToSourceFiles().Single()).ToList();
            Assert.That(sourceFilesToAnalyze, Is.Not.Empty);

            var expectedWarningComments =
                (from sourceFile in sourceFilesToAnalyze
                 let rootNode = sourceFile.GetPsiFiles<CSharpLanguage>().Single()
                 from commentNode in rootNode.ThisAndDescendants().OfType<IComment>().ToEnumerable()
                 let expectedWarningId = ExtractExpectedWarningId(commentNode.CommentText, definedExpectedWarningSymbols)
                 where expectedWarningId != null
                 let documentRange = FindPreviousNonWhiteSpaceNode(commentNode).NotNull().GetDocumentRange()
                 select new
                 {
                     ExpectedWarningId = expectedWarningId,
                     File = sourceFile,
                     Coords = documentRange.Document.GetCoordsByOffset(documentRange.TextRange.StartOffset),
                     Range = documentRange.TextRange,
                 }).ToList();

            var issues = RunInspections(solution, sourceFilesToAnalyze);

            // Assert

            var highlightingConfigurableSeverityIds = highlightingTypesToAnalyze
                .Select(x => x.GetCustomAttribute<ConfigurableSeverityHighlightingAttribute>(inherit: false).ConfigurableSeverityId)
                .ToHashSet();
            var actualIssues = issues.Where(x => highlightingConfigurableSeverityIds.Contains(x.IssueType.ConfigurableSeverityId)).ToList();

            var actualIssuesAndMatches = actualIssues.Select(
                issue => new
                {
                    // Info for the assertion message:
                    IssueInfo = new
                    {
                        issue.Message,
                        File = issue.GetSourceFile(),
                        Coords = issue.File.File.Document.GetCoordsByOffset(issue.Range.NotNull().StartOffset),
                        issue.Range,
                    },
                    Match = expectedWarningComments.SingleOrDefault(
                        x => x.ExpectedWarningId == issue.IssueType.ConfigurableSeverityId &&
                             issue.GetSourceFile().Equals(x.File) &&
                             issue.Range.NotNull().Intersects(x.Range))
                }).ToList();

            Assert.That(issues.Where(x => x.GetSeverity() >= Severity.ERROR), Is.Empty, "no errors should happen during analysis");

            var unexpectedIssues = actualIssuesAndMatches.Where(x => x.Match == null).ToList();
            var unmatchedExpectedWarningComments = expectedWarningComments.Except(actualIssuesAndMatches.Select(x => x.Match)).ToList();

            var newLine = Environment.NewLine;
            Assert.That(
                unexpectedIssues.Count + unmatchedExpectedWarningComments.Count,
                Is.EqualTo(0),
                "# Unexpected issues #" + newLine + string.Join(newLine, unexpectedIssues) + newLine + newLine +
                "# Unmatched expected warnings #" + newLine + string.Join(newLine, unmatchedExpectedWarningComments) + newLine);

            Console.WriteLine("totalIssuesCount: " + actualIssues.Count);
            return actualIssues;
        }

        [CanBeNull]
        private string ExtractExpectedWarningId(string commentText, string[] definedExpectedWarningSymbols)
        {
            var match = Regex.Match(commentText, @"^\s*Expect:(?<Id>.+?)(\[(?<Condition>[^\]]+)*\])?$");

            if (!match.Success)
                return null;

            if (match.Groups["Condition"].Success)
            {
                var expression = new Expression(match.Groups["Condition"].Value);

                var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
                expression.Parameters["RS"] = int.Parse(Regex.Match(assemblyName, @"\d+$").Value);

                expression.EvaluateParameter += (name, args) => { args.Result = definedExpectedWarningSymbols.Contains(name); };

                if (false.Equals(expression.Evaluate()))
                    return null;
            }

            return match.Groups["Id"].Value;
        }

        [CanBeNull]
        private ITreeNode FindPreviousNonWhiteSpaceNode(ITreeNode currentNode)
        {
            return currentNode.FindPreviousNode(x => x is IWhitespaceNode ? TreeNodeActionType.CONTINUE : TreeNodeActionType.ACCEPT);
        }

        private static IReadOnlyCollection<IIssue> RunInspections(ISolution solution, ICollection<IPsiSourceFile> sourceFiles)
        {
            var issues = new List<IIssue>();

            using (var lifetime = Lifetimes.Define(solution.GetLifetime()))
            {
                var collectInspectionResults = new CollectInspectionResults(solution, lifetime, A.Dummy<IProgressIndicator>());

                collectInspectionResults.RunLocalInspections(new Stack<IPsiSourceFile>(sourceFiles),
                    issuePointers => issues.AddRange(issuePointers));
            }

            return issues;
        }
    }
}
