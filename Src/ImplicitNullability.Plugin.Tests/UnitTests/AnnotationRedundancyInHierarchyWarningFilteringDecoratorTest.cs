using System.Collections.Generic;
using FakeItEasy;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.Daemon;
using NUnit.Framework;
using A_ = FakeItEasy.A; // Fix the conflict with the "A" namespace in "JetBrains.ReSharper.Diagramming.Features"

namespace ImplicitNullability.Plugin.Tests.UnitTests
{
    [TestFixture]
    public class AnnotationRedundancyInHierarchyWarningFilteringDecoratorTest
    {
        private AnnotationRedundancyInHierarchyWarningFilteringDecorator _sut;
        private IHighlightingConsumer _decorated;

        [SetUp]
        public void SetUp()
        {
            _decorated = A_.Fake<IHighlightingConsumer>();
            _sut = new AnnotationRedundancyInHierarchyWarningFilteringDecorator(_decorated);
        }

        [Test]
        public void ConsumeHighlighting_WithSomeHighlighting_ShouldPassCall()
        {
            var highlightingInfo = CreateHighlightingInfo(A_.Dummy<IHighlighting>());

            _sut.ConsumeHighlighting(highlightingInfo);

            A_.CallTo(() => _decorated.ConsumeHighlighting(highlightingInfo)).MustHaveHappened();
        }

        [Test]
        public void ConsumeHighlighting_WithAnnotationRedundancyInHierarchyWarning_ShouldSwallow()
        {
            var highlightingInfo = CreateHighlightingInfo(A_.Dummy<AnnotationRedundancyInHierarchyWarning>());

            _sut.ConsumeHighlighting(highlightingInfo);

            A_.CallTo(_decorated).MustNotHaveHappened();
        }

        [Test]
        public void Highlightings_ShouldPassCall()
        {
            var decoratedValue = new List<HighlightingInfo>();
            A_.CallTo(() => _decorated.Highlightings).Returns(decoratedValue);

            var result = _sut.Highlightings;

            Assert.That(result, Is.SameAs(decoratedValue));
        }

        private static HighlightingInfo CreateHighlightingInfo(IHighlighting highlighting)
        {
            var documentRange = new DocumentRange(A_.Fake<IDocument>(), offset: 0);
            return new HighlightingInfo(documentRange, highlighting);
        }
    }
}
