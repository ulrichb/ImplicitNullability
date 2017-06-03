using System;
using FluentAssertions;
using ImplicitNullability.Samples.CodeWithIN;
using ImplicitNullability.Samples.CodeWithIN.Highlighting.ImplicitNotNullOverridesUnknownBaseMemberNullability;
using ImplicitNullability.Samples.CodeWithoutIN;
using NUnit.Framework;

namespace ImplicitNullability.Samples.Consumer.Highlighting.ImplicitNotNullOverridesUnknownBaseMemberNullability
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

            act.ShouldThrow<ArgumentNullException>("derived method has been rewritten altough external base method is (unannotated) nullable")
                .And.ParamName.Should().Be("a");
        }

        [Test]
        public void MethodOnDerivedClassInOwnCode()
        {
            Action act = () => _derivedClassInOwnCode.Method(null /*Expect:AssignNullToNotNullAttribute[Implicit]*/);

            act.ShouldThrow<ArgumentNullException>("derived method has been rewritten altough external base method is (unannotated) nullable")
                .And.ParamName.Should().Be("a");
        }

        //

        [Test]
        public void FunctionOnExternalClass()
        {
            Action act = () =>
            {
                var result = _externalClass.Function();
                ReSharper.TestValueAnalysis(result, result == null); // unknown nullability
            };

            act.ShouldNotThrow();
        }

        [Test]
        public void FunctionOnExternalClassOverriddenInInOwnCode()
        {
            Action act = () =>
            {
                var result = _derivedClassInOwnCodeAsExternalClass.Function();
                ReSharper.TestValueAnalysis(result, result == null); // unknown nullability
            };

            act.ShouldThrow<InvalidOperationException>("derived method has been rewritten although external base method is (unannotated) nullable")
                .WithMessage("[NullGuard] Return value * is null.");
        }

        [Test]
        public void FunctionOnDerivedClassInOwnCode()
        {
            Action act = () =>
            {
                var result = _derivedClassInOwnCode.Function();
                ReSharper.TestValueAnalysis(result, result == null /*Expect:ConditionIsAlwaysTrueOrFalse[Implicit]*/);
            };

            act.ShouldThrow<InvalidOperationException>("derived method has been rewritten although external base method is (unannotated) nullable")
                .WithMessage("[NullGuard] Return value * is null.");
        }
    }
}
