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
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.TestFramework;
using JetBrains.Util;
using NCalc;
using NUnit.Framework;
#if RESHARPER91
using JetBrains.Extension;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Resources.Shell;

#else
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Resources.Shell;

#endif

namespace ImplicitNullability.Plugin.Tests.Infrastructure
{
    public abstract class SampleSolutionTestBase : BaseTestWithExistingSolution
    {
        protected void UseSampleSolution(Action<ISolution> testAction)
        {
            var solutionFilePath = FileSystemPath.Parse(TestDataPathUtility.GetPathRelativeToSolution("ImplicitNullability.Sample.sln"));

            DoTestSolution(solutionFilePath, (lifetime, solution) => { testAction(solution); });

            // Close the solution to isolate all solution-level settings changes (this has a small performance hit because normally the
            // solution is only "cleaned" at the end of DoTestSolution())
            RunGuarded(() => CloseSolution());
        }

        protected IList<IIssue> TestExpectedInspectionComments(
            ISolution solution,
            IEnumerable<IProjectFile> projectFilesToAnalyze,
            IEnumerable<Type> highlightingTypesToAnalyze,
            string definedExpectedWarningSymbol = null)
        {
            var sourceFilesToAnalyze = projectFilesToAnalyze.Select(x => x.ToSourceFiles().Single()).ToList();

            var expectedWarningComments =
                (from sourceFile in sourceFilesToAnalyze
                    let rootNode = sourceFile.GetPsiFiles<CSharpLanguage>().Single()
                    from commentNode in rootNode.ThisAndDescendants().OfType<IComment>().ToEnumerable()
                    let expectedWarningId = ExtractExpectedWarningId(commentNode.CommentText, definedExpectedWarningSymbol)
                    where expectedWarningId != null
                    select
                        new
                        {
                            ExpectedWarningId = expectedWarningId,
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
                typeof (PossibleInvalidOperationExceptionWarning),
                typeof (AnnotationRedundancyInHierarchyWarning)
            };
        }

        /// <summary>
        /// Enable implicit nullability for the *whole solution* (otherwise the IsPartOfSolutionCode() checks in 
        /// <see cref="ImplicitNullabilityProvider"/> wouldn't be tested, as project-level settings wouldn't include external assembly
        /// code elements). Further, specially disable implicit nullability for the 'ImplicitNullability.Sample.NonImplicit' project.
        /// </summary>
        protected static void EnableImplicitNullabilitySetting(
            [NotNull] ISolution sampleSolution, Action<IContextBoundSettingsStore> additionalSolutionChanges = null)
        {
            // We need to change the settings here by code, because the settings stored in the .DotSettings files aren't 
            // evaluated (see https://devnet.jetbrains.com/message/5527647).
            // Note that the settings changes are cleaned in UseSampleSolution() => no reset mechanism necessary.

            var solutionSettings = Shell.Instance.GetComponent<SettingsStore>()
                .BindToContextTransient(ContextRange.ManuallyRestrictWritesToOneContext(sampleSolution.ToDataContext()));

            Assert.That(solutionSettings.GetValue((ImplicitNullabilitySettings s) => s.Enabled), Is.False, "fixate default value");
            solutionSettings.SetValue((ImplicitNullabilitySettings s) => s.Enabled, true);

            if (additionalSolutionChanges != null)
                additionalSolutionChanges(solutionSettings);

            var nonImplicitProject = sampleSolution.GetProjectByName("ImplicitNullability.Sample.NonImplicit").NotNull();
            var nonImplicitProjectSettings = Shell.Instance.GetComponent<SettingsStore>()
                .BindToContextTransient(ContextRange.ManuallyRestrictWritesToOneContext(nonImplicitProject.ToDataContext()));

            nonImplicitProjectSettings.SetValue((ImplicitNullabilitySettings s) => s.Enabled, false);
        }

        [CanBeNull]
        private string ExtractExpectedWarningId(string commentText, [CanBeNull] string definedExpectedWarningSymbol)
        {
            var match = Regex.Match(commentText, @"^\s*Expect:(?<Id>.+?)(\[(?<Condition>[^\]]+)*\])?$");

            if (!match.Success)
                return null;

            if (match.Groups["Condition"].Success)
            {
                var expression = new Expression(match.Groups["Condition"].Value);

                var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
                expression.Parameters["RS"] = int.Parse(Regex.Match(assemblyName, @"\d+$").Value);

                expression.EvaluateParameter += (name, args) => { args.Result = name == definedExpectedWarningSymbol; };

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

        private static List<IIssue> RunInspection(ISolution solution, ICollection<IPsiSourceFile> sourceFiles)
        {
            var issues = new List<IIssue>();

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

            return issues;
        }
    }
}