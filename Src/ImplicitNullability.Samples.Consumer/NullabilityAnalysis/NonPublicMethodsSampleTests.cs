using System;
using System.Reflection;
using FluentAssertions;
using ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis;
using NUnit.Framework;

namespace ImplicitNullability.Samples.Consumer.NullabilityAnalysis
{
    [TestFixture]
    public class NonPublicMethodsSampleTests
    {
        private NonPublicMethodsSample _instance;

        [SetUp]
        public void SetUp()
        {
            _instance = new NonPublicMethodsSample();
        }

        [Test]
        [TestCase("InternalMethod", "internalMethodParameter")]
        [TestCase("ProtectedMethod", "protectedMethodParameter")]
        [TestCase("PrivateMethod", "privateMethodParameter")]
        public void MethodWithNullArgument(string methodName, string parameterName)
        {
            Action act = () => GetNonPublicMethod(methodName).Invoke(_instance, new object[] {null});

            act.ShouldThrow<TargetInvocationException>()
                .And.InnerException.Should().BeOfType<ArgumentNullException>().Which.ParamName.Should().Be(parameterName);
        }

        [Test]
        [TestCase("InternalFunction")]
        [TestCase("ProtectedFunction")]
        [TestCase("PrivateFunction")]
        public void Function(string methodName)
        {
            Action act = () => GetNonPublicMethod(methodName).Invoke(_instance, new object[0]);

            act.ShouldThrow<TargetInvocationException>()
                .And.InnerException.Should().BeOfType<InvalidOperationException>()
                .Which.Message.Should().Match("[NullGuard] Return value of method *" + methodName + "* is null.");
        }

        private MethodInfo GetNonPublicMethod(string name)
        {
            return _instance.GetType().GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance);
        }
    }
}