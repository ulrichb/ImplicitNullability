using System;
using System.Threading.Tasks;
using FluentAssertions;
using ImplicitNullability.Sample.NullabilityAnalysis;
using NUnit.Framework;

namespace ImplicitNullability.Sample.Tests.NullabilityAnalysis
{
    [TestFixture]
    public class AsyncMethodsSampleTests
    {
        private AsyncMethodsSample _instance;

        [SetUp]
        public void SetUp()
        {
            _instance = new AsyncMethodsSample();
        }

        [Test]
        public void MethodWithNonNullArgument()
        {
            Func<Task> act = async () => await _instance.Method("");

            act.ShouldNotThrow();
        }

        [Test]
        public void MethodWithNullArgument()
        {
#pragma warning disable CS4014
            // Note that Method() throws immediately => no await necessary
            Action act = () => _instance.Method(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);
#pragma warning restore CS4014

            act.ShouldThrow<ArgumentNullException>("not an AggregateException because the outermost (rewritten) async method throwed")
                .And.ParamName.Should().Be("a");
        }

        [Test]
        public void MethodWithManualNullCheck()
        {
            Func<Task> act = async () => await _instance.MethodWithManualNullCheck(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.ShouldThrow<AggregateException>("the outermost (non rewritten) method throws *within* the async state machine")
                .And.InnerException.Should().BeOfType<ArgumentNullException>()
                .Which.ParamName.Should().Be("a");
        }

        [Test]
        public void CallMethodWithNullArgument()
        {
            Func<Task> act = async () => await _instance.CallMethodWithNullArgument();

            act.ShouldThrow<AggregateException>()
                .And.InnerException.Should().BeOfType<ArgumentNullException>()
                .Which.ParamName.Should().Be("a");
        }

        [Test]
        public void NonVirtualAsyncMethod()
        {
            var task = _instance.NonVirtualAsyncMethod();
            // For async methods ReSharper's CSharpCodeAnnotationProvider makes the result implicitly NotNull:
            ReSharper.TestValueAnalysis(task, task == null /*Expect:ConditionIsAlwaysTrueOrFalse*/);
        }

        [Test]
        public void VirtualAsyncMethod()
        {
            var task = _instance.VirtualAsyncMethod();
            // For overridable async methods, ReSharper's CSharpCodeAnnotationProvider doesn't return NotNull:
            ReSharper.TestValueAnalysis(task, task == null /*Expect:ConditionIsAlwaysTrueOrFalse[MOut]*/);
        }

        [Test]
        public void FunctionWithNonNullValueReturnValue()
        {
            Func<Task> act = async () =>
            {
                var result = await _instance.Function(returnValue: "");
                // REPORTED http://youtrack.jetbrains.com/issue/RSRP-376091, requires an extension point for [ItemNotNull]:
                ReSharper.TestValueAnalysis(result, result == null);
            };

            act.ShouldNotThrow();
        }

        [Test]
        public void FunctionWithNullValueReturnValue()
        {
            Func<Task> act = async () => await _instance.Function(returnValue: null);

            act.ShouldThrow<AggregateException>()
                .And.InnerException.Should().BeOfType<InvalidOperationException>()
                .Which.Message.Should().Match("[NullGuard] Return value * is null.");
        }
    }
}