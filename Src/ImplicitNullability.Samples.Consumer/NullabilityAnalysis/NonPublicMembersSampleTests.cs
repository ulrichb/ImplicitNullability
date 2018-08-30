using System;
using System.Reflection;
using FluentAssertions;
using ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis;
using NUnit.Framework;

namespace ImplicitNullability.Samples.Consumer.NullabilityAnalysis
{
    [TestFixture]
    public class NonPublicMembersSampleTests
    {
        private NonPublicMembersSample _instance;

        [SetUp]
        public void SetUp()
        {
            _instance = new NonPublicMembersSample();
        }

        [Test]
        [TestCase("InternalMethod", "internalMethodParameter")]
        [TestCase("ProtectedMethod", "protectedMethodParameter")]
        [TestCase("PrivateMethod", "privateMethodParameter")]
        public void MethodWithNullArgument(string methodName, string parameterName)
        {
            Action act = () => GetNonPublicMethod(methodName).Invoke(_instance, new object[] { null });

            act.Should().Throw<TargetInvocationException>()
                .And.InnerException.Should().BeOfType<ArgumentNullException>().Which.ParamName.Should().Be(parameterName);
        }

        [Test]
        [TestCase("InternalFunction")]
        [TestCase("ProtectedFunction")]
        [TestCase("PrivateFunction")]
        public void Function(string name)
        {
            Action act = () => GetNonPublicMethod(name).Invoke(_instance, new object[0]);

            act.Should().Throw<TargetInvocationException>()
                .And.InnerException.Should().BeOfType<InvalidOperationException>()
                .Which.Message.Should().Match("[NullGuard] Return value of method *" + name + "* is null.");
        }

        [Test]
        [TestCase("InternalField")]
        [TestCase("ProtectedField")]
        [TestCase("_privateField")]
        public void Field(string name)
        {
            var value = GetNonPublicField(name).GetValue(_instance);

            value.Should().BeNull();
        }

        [Test]
        [TestCase("InternalProperty")]
        [TestCase("ProtectedProperty")]
        [TestCase("PrivateProperty")]
        public void Property(string name)
        {
            Action act = () => GetNonPublicProperty(name).GetValue(_instance);

            act.Should().Throw<TargetInvocationException>()
                .And.InnerException.Should().BeOfType<InvalidOperationException>()
                .Which.Message.Should().Match("[NullGuard] Return value of property *" + name + "* is null.");
        }

        private MethodInfo GetNonPublicMethod(string name) =>
            _instance.GetType().GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance);

        private FieldInfo GetNonPublicField(string name) =>
            _instance.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);

        private PropertyInfo GetNonPublicProperty(string name) =>
            _instance.GetType().GetProperty(name, BindingFlags.NonPublic | BindingFlags.Instance);
    }
}
