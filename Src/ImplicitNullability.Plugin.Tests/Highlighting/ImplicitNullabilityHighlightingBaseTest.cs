using FakeItEasy;
using ImplicitNullability.Plugin.Highlighting;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Tree;
using NUnit.Framework;

namespace ImplicitNullability.Plugin.Tests.Highlighting
{
    [TestFixture]
    public class ImplicitNullabilityHighlightingBaseTest
    {
        private ITreeNode _fakeTreeNode;
        private ImplicitNullabilityHighlightingBase _sut;

        [SetUp]
        public void SetUp()
        {
            _fakeTreeNode = A.Fake<ITreeNode>();
            _sut = new TestImplicitNullabilityHighlighting(_fakeTreeNode, "ToolTipText");
        }

        [Test]
        public void ToolTip()
        {
            Assert.That(_sut.ToolTip, Is.EqualTo("ToolTipText"));
        }

        [Test]
        public void ErrorStripeToolTip()
        {
            Assert.That(_sut.ErrorStripeToolTip, Is.EqualTo("ToolTipText"));
        }

        [Test]
        public void NavigationOffsetPatch()
        {
            Assert.That(_sut.NavigationOffsetPatch, Is.EqualTo(0));
        }

        [Test]
        public void IsValid_WithInvalidTreeNode()
        {
            A.CallTo(() => _fakeTreeNode.IsValid()).Returns(false);

            Assert.That(_sut.IsValid(), Is.EqualTo(false));
        }

        [Test]
        public void IsValid_WithValidTreeNode()
        {
            A.CallTo(() => _fakeTreeNode.IsValid()).Returns(true);

            Assert.That(_sut.IsValid(), Is.EqualTo(true));
        }

        // NOTE: CalculateRange() is tested in the integrative tests

        private class TestImplicitNullabilityHighlighting : ImplicitNullabilityHighlightingBase
        {
            public TestImplicitNullabilityHighlighting([NotNull] ITreeNode treeNode, [NotNull] string toolTipText)
                : base(treeNode, toolTipText)
            {
            }
        }
    }
}