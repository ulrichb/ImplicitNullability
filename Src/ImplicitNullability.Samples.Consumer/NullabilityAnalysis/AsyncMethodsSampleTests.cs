using System;
using System.Threading.Tasks;
using FluentAssertions;
using ImplicitNullability.Samples.CodeWithIN;
using ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis;
using NUnit.Framework;

namespace ImplicitNullability.Samples.Consumer.NullabilityAnalysis
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

            act.Should().NotThrow();
        }

        [Test]
        public void MethodWithNullArgument()
        {
#pragma warning disable CS4014
            // Note that Method() throws immediately => no await necessary
            Action act = () => _instance.Method(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);
#pragma warning restore CS4014

            act.Should().Throw<ArgumentNullException>("not an AggregateException because the outermost (rewritten) async method throwed")
                .And.ParamName.Should().Be("a");
        }

        [Test]
        public void MethodWithManualNullCheck()
        {
            Func<Task> act = async () => await _instance.MethodWithManualNullCheck(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.Should().Throw<AggregateException>("the outermost (non rewritten) method throws *within* the async state machine")
                .And.InnerException.Should().BeOfType<ArgumentNullException>()
                .Which.ParamName.Should().Be("a");
        }

        [Test]
        public void CallMethodWithNullArgument()
        {
            Func<Task> act = async () => await _instance.CallMethodWithNullArgument();

            act.Should().Throw<AggregateException>()
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
        public void FunctionWithExplicitNotNull()
        {
            Func<Task> act = async () =>
            {
                var result = await _instance.FunctionWithExplicitItemNotNull(returnValue: "");
                ReSharper.TestValueAnalysis(result, result == null /*Expect:ConditionIsAlwaysTrueOrFalse*/);
            };

            act.Should().NotThrow();
        }

        [Test]
        public void FunctionWithNonNullValueReturnValue()
        {
            Func<Task> act = async () =>
            {
                var result = await _instance.Function(returnValue: "");
                ReSharper.TestValueAnalysis(result, result == null /*Expect:ConditionIsAlwaysTrueOrFalse[MOut]*/);
            };

            act.Should().NotThrow();
        }

        [Test]
        public void FunctionWithNullValueReturnValue()
        {
            Func<Task> act = async () => await _instance.Function(returnValue: null);

            act.Should().Throw<AggregateException>()
                .And.InnerException.Should().BeOfType<InvalidOperationException>()
                .Which.Message.Should().Match("[NullGuard] Return value * is null.");
        }

        [Test]
        public void FunctionWithItemCanBeNull()
        {
            Func<Task> act = async () => await _instance.FunctionWithItemCanBeNull(returnValue: null);

            act.Should().NotThrow();
        }

        [Test]
        public void FunctionWithNullableInt()
        {
            Func<Task> act = async () =>
            {
                var result = await _instance.FunctionWithNullableInt(returnValue: null);
                ReSharper.TestValueAnalysis(result /*Expect:AssignNullToNotNullAttribute[MOut]*/, result == null);
            };

            act.Should().NotThrow();
        }

        [Test]
        public async Task NonAsyncTaskResultFunctionWithExplicitNotNull()
        {
            var result = await _instance.NonAsyncTaskResultFunctionWithExplicitNotNull(null);
            ReSharper.TestValueAnalysis(result, result == null /*Expect:ConditionIsAlwaysTrueOrFalse*/);

            result.Should().BeNull("not NullGuard rewritten");
        }

        [Test]
        public async Task NonAsyncTaskResultFunction()
        {
            var result = await _instance.NonAsyncTaskResultFunction(null);
            ReSharper.TestValueAnalysis(result, result == null /*Expect:ConditionIsAlwaysTrueOrFalse[MOut]*/);

            result.Should().BeNull("not NullGuard rewritten");
        }
    }
}
