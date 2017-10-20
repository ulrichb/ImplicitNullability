using System;
using System.Runtime.Remoting;
using FluentAssertions;
using ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis;
using NUnit.Framework;
using static ImplicitNullability.Samples.CodeWithIN.ReSharper;

namespace ImplicitNullability.Samples.Consumer.NullabilityAnalysis
{
    [TestFixture]
    public class DelegatesSampleTests
    {
        private string BecauseDelegateMethodIsAnonymous = "because the delegate *method* is an anonymous method";

        [Test]
        public void SomeAction()
        {
            Action act = () => DelegatesSample.GetSomeAction()(null);

            act.ShouldNotThrow(BecauseDelegateMethodIsAnonymous);
        }

        [Test]
        public void SomeActionWithClosedValues()
        {
            Action act = () => DelegatesSample.GetSomeActionWithClosedValues()(null);

            act.ShouldNotThrow(BecauseDelegateMethodIsAnonymous + " (implemented using a display class)");
        }

        [Test]
        public void GetSomeActionToAnonymousMethod()
        {
            Action act = () => DelegatesSample.GetSomeActionToAnonymousMethod()(null);

            act.ShouldNotThrow(BecauseDelegateMethodIsAnonymous);
        }

        [Test]
        public void GetSomeDelegateToLambda()
        {
            Action act = () => DelegatesSample.GetSomeDelegate()(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.ShouldNotThrow(BecauseDelegateMethodIsAnonymous);
        }

        [Test]
        public void SomeDelegateToNamedMethod()
        {
            Action act = () => DelegatesSample.GetSomeDelegateToNamedMethod()(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.ShouldThrow<ArgumentNullException>("because the delegate *method* parameter is implicitly NotNull")
                .And.ParamName.Should().Be("a");
        }

        [Test]
        public void SomeDelegateToNamedMethodWithCanBeNull()
        {
            Action act = () => DelegatesSample.GetSomeDelegateToNamedMethodWithCanBeNull()(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.ShouldNotThrow();
        }

        [Test]
        public void SomeDelegateWithCanBeNullToNamedMethod()
        {
            Action act = () => DelegatesSample.GetSomeDelegateWithCanBeNullToNamedMethod()(null);

            act.ShouldThrow<ArgumentNullException>("because the delegate *method* parameter is (implicitly) NotNull")
                .And.ParamName.Should().Be("a");
        }

        [Test]
        public void SomeDelegateWithCanBeNullToNamedMethodWithCanBeNull()
        {
            Action act = () => DelegatesSample.GetSomeDelegateWithCanBeNullToNamedMethodWithCanBeNull()(null);

            act.ShouldNotThrow();
        }

        [Test]
        public void SomeFunctionDelegateWithNotNull()
        {
            Action act = () =>
            {
                DelegatesSample.SomeFunctionDelegateWithNotNull @delegate = DelegatesSample.GetSomeFunctionDelegateWithNotNull();
                var result = @delegate();
                TestValueAnalysis(result, result == null /* REPORTED false negative https://youtrack.jetbrains.com/issue/RSRP-446852 */);
            };

            act.ShouldNotThrow(BecauseDelegateMethodIsAnonymous);
        }

        [Test]
        public void SomeFunctionDelegate()
        {
            Action act = () =>
            {
                DelegatesSample.SomeFunctionDelegate @delegate = DelegatesSample.GetSomeFunctionDelegate();
                var result = @delegate();
                // This false negative is analog to SomeFunctionDelegateWithNotNull and even requires an exemption for the delegate Invoke() method,
                // but it is necessary because the developer can't opt out of the implicit annotation with [CanBeNull]:
                TestValueAnalysis(result, result == null);
            };

            act.ShouldNotThrow(BecauseDelegateMethodIsAnonymous);
        }

        [Test]
        public void SomeFunctionDelegateWithCanBeNull()
        {
            Action act = () =>
            {
                DelegatesSample.SomeFunctionDelegateWithCanBeNull @delegate = DelegatesSample.GetSomeFunctionDelegateWithCanBeNull();
                var result = @delegate();
                TestValueAnalysis(result /* REPORTED false negative https://youtrack.jetbrains.com/issue/RSRP-446852 */, result == null);
            };

            act.ShouldNotThrow();
        }

        [Test]
        public void SomeNotNullDelegateOfExternalCode()
        {
            Action act = () => DelegatesSample.GetSomeNotNullDelegateOfExternalCode()(null /*Expect:AssignNullToNotNullAttribute*/);

            act.ShouldNotThrow(BecauseDelegateMethodIsAnonymous);
        }

        [Test]
        public void SomeDelegateOfExternalCode()
        {
            Action act = () => DelegatesSample.GetSomeDelegateOfExternalCode()(null /* no warning because the delegate is external */);

            act.ShouldNotThrow(BecauseDelegateMethodIsAnonymous);
        }

        [Test]
        public void SomeAsyncFunctionDelegateWithNotNull()
        {
            Action act = () =>
            {
                DelegatesSample.SomeAsyncFunctionDelegateWithNotNull @delegate = DelegatesSample.GetSomeAsyncFunctionDelegateWithNotNull();
                var result = @delegate();
                TestValueAnalysis(result, result == null /* REPORTED false negative https://youtrack.jetbrains.com/issue/RSRP-446852 */);
            };

            act.ShouldNotThrow(BecauseDelegateMethodIsAnonymous);
        }

        [Test]
        public void SomeAsyncFunctionDelegate()
        {
            Action act = () =>
            {
                DelegatesSample.SomeAsyncFunctionDelegate @delegate = DelegatesSample.GetSomeAsyncFunctionDelegate();
                var result = @delegate();
                // This false negative is analog to SomeAsyncFunctionDelegateWithNotNull and even requires an exemption for the delegate Invoke() method,
                // but it is necessary because the developer can't opt out of the implicit annotation with [CanBeNull]:
                TestValueAnalysis(result, result == null);
            };

            act.ShouldNotThrow(BecauseDelegateMethodIsAnonymous);
        }

        [Test]
        public void SomeAsyncFunctionDelegateWithCanBeNull()
        {
            Action act = () =>
            {
                DelegatesSample.SomeAsyncFunctionDelegateWithCanBeNull @delegate = DelegatesSample.GetSomeAsyncFunctionDelegateWithCanBeNull();
                var result = @delegate();
                TestValueAnalysis(result /* REPORTED false negative https://youtrack.jetbrains.com/issue/RSRP-446852 */, result == null);
            };

            act.ShouldNotThrow();
        }

        [Test]
        public void SomeDelegateWithRefAndOutWithNullValueForRefArgument()
        {
            Action act = () =>
            {
                string refParam = null;

                DelegatesSample.GetSomeDelegateWithRefAndOut()(ref refParam, out var outParam);

                TestValueAnalysis(refParam, refParam == null /*Expect:ConditionIsAlwaysTrueOrFalse[MRef]*/);
                TestValueAnalysis(outParam, outParam == null /*Expect:ConditionIsAlwaysTrueOrFalse[MOut]*/);
            };

            act.ShouldNotThrow(BecauseDelegateMethodIsAnonymous);
        }

        [Test]
        public void SomeDelegateWithInvokeAndBeginInvokeCalls()
        {
            Action act = () =>
            {
                var someDelegate = DelegatesSample.GetSomeDelegate();
                someDelegate.Invoke(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

                var asyncResult = someDelegate.BeginInvoke(null, null, null);
                // The BeginInvoke() result is made implicitly NotNull by ReSharper's CodeAnnotationsCache:
                TestValueAnalysis(asyncResult, asyncResult == null /*Expect:ConditionIsAlwaysTrueOrFalse*/);
                asyncResult.AsyncWaitHandle.WaitOne();
            };

            act.ShouldNotThrow(BecauseDelegateMethodIsAnonymous);
        }

        [Test]
        public void GetSomeDelegateWithCanBeNullWithInvokeAndBeginInvokeCalls()
        {
            Action act = () =>
            {
                var someDelegate = DelegatesSample.GetSomeDelegateWithCanBeNull();
                someDelegate.Invoke(null);
                someDelegate.BeginInvoke(null, null, null).AsyncWaitHandle.WaitOne();
            };

            act.ShouldNotThrow();
        }

        [Test]
        public void SomeFunctionDelegateWithInvokeAndBeginInvokeCalls()
        {
            Action act = () =>
            {
                var @delegate = DelegatesSample.GetSomeFunctionDelegate();
                var result = @delegate.Invoke();
                TestValueAnalysis(result, result == null /* REPORTED false negative https://youtrack.jetbrains.com/issue/RSRP-446852 */);

                var asyncResult = @delegate.BeginInvoke(null, null);
                var endInvokeResult = @delegate.EndInvoke(asyncResult);
                TestValueAnalysis(endInvokeResult, endInvokeResult == null);
            };

            act.ShouldNotThrow(BecauseDelegateMethodIsAnonymous);
        }

        [Test]
        public void SomeFunctionDelegateWithEndInvokeWithNullArgument()
        {
            var @delegate = DelegatesSample.GetSomeFunctionDelegate();

            Action act = () => @delegate.EndInvoke(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.ShouldThrow<RemotingException>("because the IAsyncResult argument is null");
        }
    }
}
