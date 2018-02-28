using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.SolutionAnalysis;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
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
            var filesToAnalyze =
                (from projectFile in projectFilesToAnalyze
                 let sourceFile = projectFile.ToSourceFiles().Single()
                 from rootNode in sourceFile.GetPsiFiles<KnownLanguage>()
                 where !IsExcludedByComment(rootNode)
                 select (SourceFile: sourceFile, RootNode: rootNode)).ToList();

            Assert.That(filesToAnalyze, Is.Not.Empty);

            var expectedWarningComments =
                (from fileToAnalyze in filesToAnalyze
                 from commentNode in fileToAnalyze.RootNode.ThisAndDescendants().OfType<IComment>().ToEnumerable()
                 let expectedWarningId = ExtractExpectedWarningId(commentNode.CommentText, definedExpectedWarningSymbols)
                 where expectedWarningId != null
                 let documentRange = FindPreviousNonWhiteSpaceNode(commentNode).NotNull().GetDocumentRange()
                 select new
                 {
                     ExpectedWarningId = expectedWarningId,
                     File = fileToAnalyze.SourceFile,
                     Coords = documentRange.Document.GetCoordsByOffset(documentRange.TextRange.StartOffset),
                     Range = documentRange.TextRange,
                 }).ToList();

            var issues = RunInspections(solution, filesToAnalyze.Select(x => x.SourceFile));

            // Assert

            var highlightingConfigurableSeverityIds = highlightingTypesToAnalyze
                .Select(x => x.GetCustomAttribute<ConfigurableSeverityHighlightingAttribute>(inherit: false).ConfigurableSeverityId)
                .ToSet();
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
#if RS20173 || RD20173
                             issue.Range.NotNull().Intersects(x.Range)
#else
                             issue.Range.NotNull().IntersectsOrContacts(x.Range)
#endif
                    )
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

        private static bool IsExcludedByComment(IFile fileNode)
        {
            var firstComment = fileNode.FirstChild as IComment;

            if (firstComment != null)
            {
                var match = Regex.Match(firstComment.CommentText, @"^\s*Exclude" + ConditionRegex + @"\s*$");

                return EvaluateConditionExpression(match, additionalSymbols: new string[0], conditionIsPresent: out var _);
            }

            return false;
        }

        [CanBeNull]
        private static string ExtractExpectedWarningId(string commentText, string[] definedExpectedWarningSymbols)
        {
            var match = Regex.Match(commentText, @"^\s*Expect:(?<Id>.+?)(" + ConditionRegex + @")?$");

            if (!match.Success)
                return null;

            var conditionResult = EvaluateConditionExpression(match, definedExpectedWarningSymbols, out var conditionIsPresent);

            if (conditionIsPresent && conditionResult == false)
                return null;

            return match.Groups["Id"].Value;
        }

        // language=REGEXP
        private const string ConditionRegex = @"\[(?<Condition>[^\]]+)*\]";

        private static bool EvaluateConditionExpression(Match match, string[] additionalSymbols, out bool conditionIsPresent)
        {
            conditionIsPresent = match.Groups["Condition"].Success;
            if (conditionIsPresent)
            {
                var expression = new Expression(match.Groups["Condition"].Value);

                var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
                expression.Parameters["RS"] = int.Parse(Regex.Match(assemblyName, @"\d+$").Value);

                expression.EvaluateParameter += (name, args) => { args.Result = additionalSymbols.Contains(name); };

                return (bool) expression.Evaluate();
            }

            return false;
        }

        [CanBeNull]
        private ITreeNode FindPreviousNonWhiteSpaceNode(ITreeNode currentNode)
        {
            return currentNode.FindPreviousNode(x => x is IWhitespaceNode ? TreeNodeActionType.CONTINUE : TreeNodeActionType.ACCEPT);
        }

        private static IReadOnlyCollection<IIssue> RunInspections(ISolution solution, IEnumerable<IPsiSourceFile> sourceFiles)
        {
            var issues = new List<IIssue>();

            using (var lifetime = Lifetimes.Define(solution.GetLifetime()))
            using (var nullProgressIndicator = NullProgressIndicator.Create())
            {
                var collectInspectionResults = new CollectInspectionResults(solution, lifetime, nullProgressIndicator);

                collectInspectionResults.RunLocalInspections(
                    new Stack<IPsiSourceFile>(sourceFiles),
                    issuePointers => issues.AddRange(issuePointers));
            }

            return issues;
        }
    }
}
