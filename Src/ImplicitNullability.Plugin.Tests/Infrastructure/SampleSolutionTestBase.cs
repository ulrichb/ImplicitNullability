using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ImplicitNullability.Plugin.Settings;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.Application.Settings;
using JetBrains.Application.Settings.Store.Implementation;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Daemon.SolutionAnalysis;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.TestFramework;
using JetBrains.Util;
using NCalc;
using NUnit.Framework;

#if !(RESHARPER92 || RESHARPER100)
using FakeItEasy;
using JetBrains.DataFlow;
#endif

namespace ImplicitNullability.Plugin.Tests.Infrastructure
{
    public abstract class SampleSolutionTestBase : BaseTestWithExistingSolution
    {
        protected void UseSampleSolution([NotNull] Action<ISolution> testAction)
        {
            var solutionFilePath = FileSystemPath.Parse(TestDataPathUtility.GetPathRelativeToSolution("ImplicitNullability.Sample.sln"));

            DoTestSolution(solutionFilePath, (lifetime, solution) => { testAction(solution); });

            // Close the solution to isolate all solution-level settings changes (this has a small performance hit because normally the
            // solution is only "cleaned" at the end of DoTestSolution())
            RunGuarded(() => CloseSolution());
        }

        protected IList<IIssue> TestExpectedInspectionComments(
            [NotNull] ISolution solution,
            [NotNull] IEnumerable<IProjectFile> projectFilesToAnalyze,
            [NotNull] IEnumerable<Type> highlightingTypesToAnalyze,
            [NotNull] params string[] definedExpectedWarningSymbols)
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
                        Range = documentRange.TextRange,
                        Coords = documentRange.Document.GetCoordsByOffset(documentRange.TextRange.StartOffset)
                    }).ToList();

            var issues = RunInspections(solution, sourceFilesToAnalyze);

            // Assert

            var highlightingConfigurableSeverityIds = highlightingTypesToAnalyze
                .Select(x => x.GetCustomAttribute<ConfigurableSeverityHighlightingAttribute>(inherit: false).ConfigurableSeverityId)
                .ToHashSet();
            var actualIssues = issues.Where(x => highlightingConfigurableSeverityIds.Contains(x.IssueType.ConfigurableSeverityId)).ToList();

            var actualIssuesThatMatchExpected = actualIssues.Select(
                issue => new
                {
                    // Info for the assertion message:
                    IssueInfo = new
                    {
                        issue.Message,
                        File = issue.GetSourceFile(),
                        issue.Range,
                        Coords = issue.File.File.Document.GetCoordsByOffset(issue.Range.NotNull().StartOffset)
                    },
                    Match = expectedWarningComments.SingleOrDefault(
                        x => x.ExpectedWarningId == issue.IssueType.ConfigurableSeverityId &&
                             issue.GetSourceFile().Equals(x.File) &&
                             issue.Range.NotNull().Intersects(x.Range))
                }).ToList();

            Assert.That(issues.Where(x => x.GetSeverity() >= Severity.ERROR), Is.Empty, "no errors should happen during analysis");

            Assert.That(actualIssuesThatMatchExpected.Where(x => x.Match == null), Is.Empty, "there should be no unexpected issues");

            var unmatchedExpectedWarningComments = expectedWarningComments.Except(actualIssuesThatMatchExpected.Select(x => x.Match));
            Assert.That(unmatchedExpectedWarningComments, Is.Empty, "there should not exist any unmatched expected warnings");

            Console.WriteLine("totalIssuesCount: " + actualIssues.Count);
            return actualIssues;
        }

        protected IEnumerable<Type> GetNullabilityAnalysisHighlightingTypes()
        {
            var implicitNullabilityProblemAnalyzerHighlightingTypes =
                typeof(ImplicitNullabilityProblemAnalyzer).GetCustomAttribute<ElementProblemAnalyzerAttribute>(false).HighlightingTypes;

            return new[]
            {
                typeof(AssignNullToNotNullAttributeWarning),
                typeof(ConditionIsAlwaysTrueOrFalseWarning),
                typeof(PossibleNullReferenceExceptionWarning),
                typeof(PossibleInvalidOperationExceptionWarning)
            }
                .Concat(implicitNullabilityProblemAnalyzerHighlightingTypes);
        }

        protected static void EnableImplicitNullability(
            [NotNull] ISolution sampleSolution,
            bool enableInputParameters = false,
            bool enableRefParameters = false,
            bool enableOutParametersAndResult = false)
        {
            // We need to change the settings here by code, because the settings stored in the .DotSettings files aren't 
            // evaluated (see https://resharper-support.jetbrains.com/hc/en-us/community/posts/206628865).
            // Note that the settings changes are cleaned in UseSampleSolution() => no reset mechanism necessary.

            var solutionSettings = sampleSolution.GetComponent<SettingsStore>()
                .BindToContextTransient(ContextRange.ManuallyRestrictWritesToOneContext(sampleSolution.ToDataContext()));

            // Fixate default values:
            Assert.That(solutionSettings.GetValue((ImplicitNullabilitySettings s) => s.Enabled), Is.False);
            Assert.That(solutionSettings.GetValue((ImplicitNullabilitySettings s) => s.EnableInputParameters), Is.True);
            Assert.That(solutionSettings.GetValue((ImplicitNullabilitySettings s) => s.EnableRefParameters), Is.True);
            Assert.That(solutionSettings.GetValue((ImplicitNullabilitySettings s) => s.EnableOutParametersAndResult), Is.True);

            solutionSettings.SetValue((ImplicitNullabilitySettings s) => s.Enabled, true);
            solutionSettings.SetValue((ImplicitNullabilitySettings s) => s.EnableInputParameters, enableInputParameters);
            solutionSettings.SetValue((ImplicitNullabilitySettings s) => s.EnableRefParameters, enableRefParameters);
            solutionSettings.SetValue((ImplicitNullabilitySettings s) => s.EnableOutParametersAndResult, enableOutParametersAndResult);
        }

        protected static void EnableImplicitNullabilityWithAllOptions([NotNull] ISolution sampleSolution)
        {
            EnableImplicitNullability(sampleSolution, true, true, true);
        }

        [CanBeNull]
        private string ExtractExpectedWarningId([NotNull] string commentText, [NotNull] string[] definedExpectedWarningSymbols)
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
        private ITreeNode FindPreviousNonWhiteSpaceNode([NotNull] ITreeNode currentNode)
        {
            return currentNode.FindPrevNode(x => x is IWhitespaceNode ? TreeNodeActionType.CONTINUE : TreeNodeActionType.ACCEPT);
        }

        private static IReadOnlyCollection<IIssue> RunInspections(ISolution solution, ICollection<IPsiSourceFile> sourceFiles)
        {
            var issues = new List<IIssue>();

#if RESHARPER92 || RESHARPER100
            Assert.IsTrue(
                CollectInspectionResults.Do(
                    solution,
                    sourceFiles,
                    SimpleTaskExecutor.Instance,
                    x =>
                    {
                        lock (issues)
                            issues.Add(x);
                    }));
#else
            using (var lifetime = Lifetimes.Define(solution.GetLifetime()))
            {
                var collectInspectionResults = new CollectInspectionResults(solution, lifetime, A.Dummy<IProgressIndicator>());

                collectInspectionResults.RunLocalInspections(new Stack<IPsiSourceFile>(sourceFiles),
                    issuePointers => issues.AddRange(issuePointers));
            }
#endif

            return issues;
        }
    }
}