using System.Reflection;
using FakeItEasy;
using ImplicitNullability.Plugin.Highlighting;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Stages.Analysis;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.TestFramework;
using JetBrains.Util;
using NUnit.Framework;
using A_ = FakeItEasy.A; // Fix the conflict with the "A" namespace in "JetBrains.ReSharper.Diagramming.Features"

namespace ImplicitNullability.Plugin.Tests.UnitTests
{
    [TestFixture]
    [TestNetFramework4]
    public class ImplicitNullabilityProblemAnalyzerTest : BaseTestWithSolution
    {
        [Test]
        public void ImplicitNullabilityProblemAnalyzer_ShouldBeDecoratedWithCorrectElementProblemAnalyzerAttribute()
        {
            var problemAnalyzerAttribute = typeof(ImplicitNullabilityProblemAnalyzer).GetCustomAttribute<ElementProblemAnalyzerAttribute>();

            var hiddenProblemAnalyzerAttribute =
                typeof(IncorrectNullableAttributeUsageAnalyzer).GetCustomAttribute<ElementProblemAnalyzerAttribute>();

            Assert.That(
                problemAnalyzerAttribute.ElementTypes,
                Is.EqualTo(hiddenProblemAnalyzerAttribute.ElementTypes.Concat(typeof(IDelegateDeclaration))));

            Assert.That(
                problemAnalyzerAttribute.HighlightingTypes,
                Is.EqualTo(hiddenProblemAnalyzerAttribute.HighlightingTypes.Concat(
                    typeof(NotNullOnImplicitCanBeNullHighlighting),
                    typeof(ImplicitNotNullConflictInHierarchyHighlighting),
                    typeof(ImplicitNotNullElementCannotOverrideCanBeNullHighlighting),
                    typeof(ImplicitNotNullOverridesUnknownBaseMemberNullabilityHighlighting),
                    typeof(ImplicitNotNullResultOverridesUnknownBaseMemberNullabilityHighlighting))));
        }

        [Test]
        public void Run_WithNullDeclaredElement_ShouldReturnNothing()
        {
            // This scenario (DeclaredElement = null) is tested in isolation because it's difficult to produce it in the integrative tests.

            DoTestSolution((_, solution) =>
            {
                var implicitNullabilityProblemAnalyzer = (IElementProblemAnalyzer) solution.GetComponent<ImplicitNullabilityProblemAnalyzer>();

                var declaration = A_.Fake<IDeclaration>();
                A_.CallTo(() => declaration.DeclaredElement).Returns(null);

                var highlightingConsumer = A_.Fake<IHighlightingConsumer>();

                //

                implicitNullabilityProblemAnalyzer.Run(declaration, A_.Dummy<ElementProblemAnalyzerData>(), highlightingConsumer);

                //

                A_.CallTo(highlightingConsumer).MustNotHaveHappened();
            });
        }
    }
}
