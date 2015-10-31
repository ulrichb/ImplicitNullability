using System;
using FluentAssertions;
using ImplicitNullability.Samples.CodeWithIN.Highlighting.ImplicitNotNullConflictInHierarchy;
using NUnit.Framework;

namespace ImplicitNullability.Samples.Consumer.Highlighting.ImplicitNotNullConflictInHierarchy
{
    [TestFixture]
    public class HierarchyWithPreconditionsStrongerInDerivedTests
    {
        private HierarchyWithPreconditionsStrongerInDerived.IInterface _interface;

        [SetUp]
        public void SetUp()
        {
            _interface = new HierarchyWithPreconditionsStrongerInDerived.Implementation();
        }

        [Test]
        public void CanBeNullParameterInInterfaceExplicitNotNullInDerived()
        {
            Action act = () => _interface.CanBeNullParameterInInterfaceExplicitNotNullInDerived(null);

            act.ShouldThrow<ArgumentNullException>(
                "this throws although the base interface has a CanBeNull-annotation (but the implementation does not)")
                .And.ParamName.Should().Be("a");
        }
    }
}