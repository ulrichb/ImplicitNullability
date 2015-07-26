using System;
using FluentAssertions;
using ImplicitNullability.Sample.NullabilityAnalysis;
using NUnit.Framework;

namespace ImplicitNullability.Sample.Tests.NullabilityAnalysis
{
    [TestFixture]
    public class DelegatesSampleTests
    {
        [Test]
        public void SomeAction()
        {
            Action act = () => DelegatesSample.GetSomeAction()(null);

            act.ShouldNotThrow("because the delegate *method* is an anonymous method");
        }

        [Test]
        public void SomeActionWithClosedValues()
        {
            Action act = () => DelegatesSample.GetSomeActionWithClosedValues()(null);

            act.ShouldNotThrow("because the delegate *method*  is an anonymous method (implemented using a display class)");
        }

        [Test]
        public void SomeDelegateToAnonymousMethod()
        {
            Action act = () => DelegatesSample.GetSomeDelegateToAnonymousMethod()(null /*Expect:AssignNullToNotNullAttribute*/);

            act.ShouldNotThrow("because the delegate *method* is an anonymous method");
        }

        [Test]
        public void SomeDelegateToNamedMethod()
        {
            Action act = () => DelegatesSample.GetSomeDelegateToNamedMethod()(null /*Expect:AssignNullToNotNullAttribute*/);

            act.ShouldThrow<ArgumentNullException>("because the delegate *method* parameter is implicitly NotNull")
                .And.ParamName.Should().Be("a");
        }

        [Test]
        public void SomeDelegateToNamedMethodWithCanBeNull()
        {
            Action act = () => DelegatesSample.GetSomeDelegateToNamedMethodWithCanBeNull()(null /*Expect:AssignNullToNotNullAttribute*/);

            act.ShouldNotThrow("because the delegate *method* parameter is CanBeNull");
        }

        [Test]
        public void SomeDelegateWithCanBeNullToNamedMethod()
        {
            Action act = () => DelegatesSample.GetSomeDelegateWithCanBeNullToNamedMethod()(null);

            act.ShouldThrow<ArgumentNullException>("because the delegate *method* parameter is implicitly NotNull")
                .And.ParamName.Should().Be("a");
        }

        [Test]
        public void SomeDelegateWithCanBeNullToNamedMethodWithCanBeNull()
        {
            Action act = () => DelegatesSample.GetSomeDelegateWithCanBeNullToNamedMethodWithCanBeNull()(null);

            act.ShouldNotThrow("because the delegate *method* parameter is CanBeNull");
        }

        [Test]
        public void SomeDelegateWithRefAndOutWithNullValueForRefArgument()
        {
            string outString;
            string refString = null;
            Action act = () => DelegatesSample.GetSomeDelegateWithRefAndOut()(out outString, ref refString);

            act.ShouldNotThrow("because the delegate *method* is an anonymous method");
        }

        [Test]
        public void SomeDelegateOfExternalCode()
        {
            Action act = () => DelegatesSample.GetSomeDelegateOfExternalCode()(null /* no warning because the delegate is external */);

            act.ShouldNotThrow("because the delegate *method* is an anonymous method");
        }

        [Test]
        public void SomeNotNullDelegateOfExternalCode()
        {
            Action act = () => DelegatesSample.GetSomeNotNullDelegateOfExternalCode()(null /*Expect:AssignNullToNotNullAttribute*/);

            act.ShouldNotThrow("because the delegate *method* is an anonymous method");
        }
    }
}