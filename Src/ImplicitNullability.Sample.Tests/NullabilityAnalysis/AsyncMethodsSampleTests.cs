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
        public void AsyncMethodWithNonNullArgument()
        {
            Func<Task> act = async () => await _instance.AsyncMethod("");

            act.ShouldNotThrow();
        }

        [Test]
        public void AsyncMethodWithNullArgument()
        {
            // Note that AsyncMethod() throws immediately => no await necessary
            Action act = () => _instance.AsyncMethod(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.ShouldThrow<ArgumentNullException>("not an AggregateException because the outermost (rewritten) async method throwed")
                .And.ParamName.Should().Be("a");
        }

        [Test]
        public void AsyncMethodWithManualNullCheck()
        {
            Func<Task> act = async () => await _instance.AsyncMethodWithManualNullCheck(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.ShouldThrow<AggregateException>("the outermost (non rewritten) method throws *within* the async state machine")
                .And.InnerException.Should().BeOfType<ArgumentNullException>()
                .Which.ParamName.Should().Be("a");
        }

        [Test]
        public void CallAsyncMethodWithNullArgument()
        {
            Func<Task> act = async () => await _instance.CallAsyncMethodWithNullArgument();

            act.ShouldThrow<AggregateException>()
                .And.InnerException.Should().BeOfType<ArgumentNullException>()
                .Which.ParamName.Should().Be("a");
        }

        [Test]
        public void AsyncFunctionWithNonNullValueReturnValue()
        {
            Func<Task> act = async () =>
            {
                var result = await _instance.AsyncFunction(returnValue: "");
                // REPORTED http://youtrack.jetbrains.com/issue/RSRP-376091, requires an extension point for [ItemNotNull]:
                ReSharper.TestValueAnalysis(result, result == null);
            };

            act.ShouldNotThrow();
        }

        [Test]
        public void AsyncFunctionWithNullValueReturnValue()
        {
            Func<Task> act = async () => await _instance.AsyncFunction(returnValue: null);

            act.ShouldThrow<AggregateException>()
                .And.InnerException.Should().BeOfType<InvalidOperationException>()
                .Which.Message.Should().Match("[NullGuard] Return value * is null.");
        }
    }
}