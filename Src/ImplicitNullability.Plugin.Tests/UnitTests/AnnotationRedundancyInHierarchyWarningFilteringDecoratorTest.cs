using System.Collections.Generic;
using FakeItEasy;
using JetBrains.Application.Settings;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.Daemon;
using NUnit.Framework;

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
            _decorated = A.Fake<IHighlightingConsumer>();
            _sut = new AnnotationRedundancyInHierarchyWarningFilteringDecorator(_decorated);
        }

        [Test]
        public void ConsumeHighlighting_ShouldPassCall()
        {
            var highlighting = A.Fake<IHighlighting>();

            _sut.ConsumeHighlighting(
                DocumentRange.InvalidRange,
                highlighting,
                Severity.INFO,
                "overriddenHighlightingAttributeId",
                OverlapResolveKind.ERROR,
                overriddenOverloadResolvePriority: 42);

            A.CallTo(() => _decorated.ConsumeHighlighting(
                DocumentRange.InvalidRange,
                highlighting,
                Severity.INFO,
                "overriddenHighlightingAttributeId",
                OverlapResolveKind.ERROR,
                42)).MustHaveHappened();
        }

        [Test]
        public void ConsumeHighlighting_WithSomeHighlighting_ShouldPassCall()
        {
            var highlightingInfo = CreateHighlightingInfo(A.Dummy<IHighlighting>());

            _sut.ConsumeHighlighting(highlightingInfo);

            A.CallTo(() => _decorated.ConsumeHighlighting(highlightingInfo)).MustHaveHappened();
        }

        [Test]
        public void ConsumeHighlighting_WithAnnotationRedundancyInHierarchyWarning_ShouldSwallow()
        {
            var highlightingInfo = CreateHighlightingInfo(A.Dummy<AnnotationRedundancyInHierarchyWarning>());

            _sut.ConsumeHighlighting(highlightingInfo);

            A.CallTo(_decorated).MustNotHaveHappened();
        }

        [Test]
        public void Process_ShouldPassCall()
        {
            var decoratedValue = A.Fake<IDaemonStageProcess>();
            A.CallTo(() => _decorated.Process).Returns(decoratedValue);

            var result = _sut.Process;

            Assert.That(result, Is.SameAs(decoratedValue));
        }

        [Test]
        public void IsNonUserFile_ShouldPassCall()
        {
            A.CallTo(() => _decorated.IsNonUserFile).Returns(true);

            var result = _sut.IsNonUserFile;

            Assert.That(result, Is.EqualTo(true));
        }

        [Test]
        public void IsGeneratedFile_ShouldPassCall()
        {
            A.CallTo(() => _decorated.IsGeneratedFile).Returns(true);

            var result = _sut.IsGeneratedFile;

            Assert.That(result, Is.EqualTo(true));
        }

        [Test]
        public void HighlightingSettingsManager_ShouldPassCall()
        {
            var decoratedValue = A.Fake<IHighlightingSettingsManager>();
            A.CallTo(() => _decorated.HighlightingSettingsManager).Returns(decoratedValue);

            var result = _sut.HighlightingSettingsManager;

            Assert.That(result, Is.SameAs(decoratedValue));
        }

        [Test]
        public void SettingsStore_ShouldPassCall()
        {
            var decoratedValue = A.Fake<IContextBoundSettingsStore>();
            A.CallTo(() => _decorated.SettingsStore).Returns(decoratedValue);

            var result = _sut.SettingsStore;

            Assert.That(result, Is.SameAs(decoratedValue));
        }

        [Test]
        public void Highlightings_ShouldPassCall()
        {
            var decoratedValue = new List<HighlightingInfo>();
            A.CallTo(() => _decorated.Highlightings).Returns(decoratedValue);

            var result = _sut.Highlightings;

            Assert.That(result, Is.SameAs(decoratedValue));
        }

        private static HighlightingInfo CreateHighlightingInfo(IHighlighting highlighting)
        {
            var documentRange = new DocumentRange(A.Fake<IDocument>(), offset: 0);
            return new HighlightingInfo(documentRange, highlighting);
        }
    }
}