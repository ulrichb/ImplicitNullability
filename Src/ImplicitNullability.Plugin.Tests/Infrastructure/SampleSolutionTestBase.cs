using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ImplicitNullability.Plugin.Settings;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.Application.Settings;
using JetBrains.Application.Settings.Store.Implementation;
using JetBrains.Extension;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Daemon.SolutionAnalysis;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.TestFramework;
using JetBrains.Util;
using NUnit.Framework;
#if RESHARPER8
using JetBrains.Application;
using JetBrains.ReSharper.Daemon;

#else
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Resources.Shell;

#endif

namespace ImplicitNullability.Plugin.Tests.Infrastructure
{
    public abstract class SampleSolutionTestBase : BaseTestWithExistingSolution
    {
        protected void UseSampleSolution(Action<ISolution> action)
        {
            var solutionFilePath = FileSystemPath.Parse(TestDataPathUtility.GetPathRelativeToSolution("ImplicitNullability.Sample.sln"));

            DoTestSolution(
                solutionFilePath,
                (lifetime, solution) => { action(solution); });
        }

        protected IList<IIssue> TestExpectedInspectionComments(
            ISolution solution,
            IEnumerable<IProjectFile> projectFilesToAnalyze,
            IEnumerable<Type> highlightingTypesToAnalyze)
        {
            var sourceFilesToAnalyze = projectFilesToAnalyze.Select(x => x.ToSourceFiles().Single()).ToList();

            // IDEA: Support also warning message Expect:"some text"
            // IDEA: Support predicates (e.g. using "Dynamic Linq"): Expect:SomeId[ReSharper9UP && X == "x"]
            var expectedWarningComments =
                (from sourceFile in sourceFilesToAnalyze
                    from commentNode in sourceFile.GetPsiFiles<CSharpLanguage>().Single().GetAllCommentNodes()
                    let match = Regex.Match(commentNode.CommentText, @"^\s*Expect:(.*)$")
                    where match.Success
                    select
                        new
                        {
                            ExpectedWarningId = match.Groups[1].Value.Trim(),
                            File = sourceFile,
                            Range = FindPreviousNonWhiteSpaceNode(commentNode).NotNull().GetDocumentRange().TextRange
                        }).ToList();

            var issues = RunInspection(solution, sourceFilesToAnalyze);

            // Assert

            var highlightingConfigurableSeverityIds = highlightingTypesToAnalyze
                .Select(x => x.GetCustomAttribute<ConfigurableSeverityHighlightingAttribute>(inherit: false).ConfigurableSeverityId)
                .ToHashSet();
            var actualIssues = issues.Where(x => highlightingConfigurableSeverityIds.Contains(x.IssueType.ConfigurableSeverityId)).ToList();

            var actualIssuesThatMatchExpected = actualIssues.Select(
                issue => new
                {
                    Issue = new {issue.Message, File = issue.GetSourceFile(), issue.Range}, // Info for the assertion message
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
            return new[]
            {
                typeof (AssignNullToNotNullAttributeWarning),
                typeof (ConditionIsAlwaysTrueOrFalseWarning),
                typeof (PossibleNullReferenceExceptionWarning),
                typeof (PossibleInvalidOperationExceptionWarning)
            };
        }

        protected static void EnableImplicitNullabilitySetting([NotNull] IProject project)
        {
            // Enable the setting on project level to test the correct "ToDataContext" binding of the settings. Unfortunately settings stored in
            // the ".csproj.DotSettings" file aren't evaluated (see https://devnet.jetbrains.com/message/5527647).

            // Note that as we bind the settings changes to a project within the solution, this seems to isolate the changes => no reset 
            // mechanism necessary.
            var settingsStore = Shell.Instance.GetComponent<SettingsStore>()
                .BindToContextTransient(ContextRange.ManuallyRestrictWritesToOneContext(project.ToDataContext()));

            Assert.That(settingsStore.GetValue((ImplicitNullabilitySettings s) => s.Enabled), Is.False, "fixate default value");
            settingsStore.SetValue((ImplicitNullabilitySettings s) => s.Enabled, true);
            settingsStore.SetValue((ImplicitNullabilitySettings s) => s.EnableOutParametersAndResult, false); // TODO
        }

        [CanBeNull]
        private ITreeNode FindPreviousNonWhiteSpaceNode([NotNull] ITreeNode currentNode)
        {
            return currentNode.FindPrevNode(x => x is IWhitespaceNode ? TreeNodeActionType.CONTINUE : TreeNodeActionType.ACCEPT);
        }

        private static List<IIssue> RunInspection(ISolution solution, ICollection<IPsiSourceFile> sourceFiles)
        {
            var issues = new List<IIssue>();

            Assert.IsTrue(
                CollectInspectionResults.Do(
                    solution,
# if RESHARPER8
                    sourceFiles.Select(x => x.ToProjectFile().NotNull()).ToList(),
#else
                    sourceFiles,
#endif
                    SimpleTaskExecutor.Instance,
                    x =>
                    {
                        lock (issues)
                            issues.Add(x);
                    }));

            return issues;
        }
    }
}