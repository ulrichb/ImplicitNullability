using System;
using FluentAssertions;
using ImplicitNullability.Sample.ExternalCode;
using ImplicitNullability.Sample.Highlighting.ImplicitNotNullOverridesUnknownExternalMember;
using NUnit.Framework;

namespace ImplicitNullability.Sample.Tests.Highlighting.ImplicitNotNullOverridesUnknownExternalMember
{
    [TestFixture]
    public class OverrideExternalCodeTests
    {
        private External.Class _externalClass;
        private External.Class _derivedClassInOwnCodeAsExternalClass;
        private OverrideExternalCode.DerivedClassInOwnCode _derivedClassInOwnCode;

        [SetUp]
        public void SetUp()
        {
            _externalClass = new External.Class();
            _derivedClassInOwnCodeAsExternalClass = new OverrideExternalCode.DerivedClassInOwnCode();
            _derivedClassInOwnCode = new OverrideExternalCode.DerivedClassInOwnCode();
        }

        [Test]
        public void MethodOnExternalClass()
        {
            Action act = () => _externalClass.Method(null);

            act.ShouldNotThrow();
        }

        [Test]
        public void MethodOnExternalClassOverriddenInInOwnCode()
        {
            Action act = () => _derivedClassInOwnCodeAsExternalClass.Method(null /* no warning */);

            act.ShouldThrow<ArgumentNullException>("the derived method has been rewritten altough External.Class base method is nullable")
                .And.ParamName.Should().Be("a");
        }

        [Test]
        public void MethodOnDerivedClassInOwnCode()
        {
            Action act = () => _derivedClassInOwnCode.Method(null /*Expect:AssignNullToNotNullAttribute*/);

            act.ShouldThrow<ArgumentNullException>("the derived method has been rewritten altough External.Class base method is nullable")
                .And.ParamName.Should().Be("a");
        }
    }
}