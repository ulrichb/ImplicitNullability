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
        public void GetSomeActionToAnonymousMethod()
        {
            Action act = () => DelegatesSample.GetSomeActionToAnonymousMethod()(null);

            act.ShouldNotThrow("because the delegate *method* is an anonymous method");
        }

        [Test]
        public void GetSomeDelegateToLambda()
        {
            Action act = () => DelegatesSample.GetSomeDelegate()(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.ShouldNotThrow("because the delegate *method* is an anonymous method");
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
                ReSharper.TestValueAnalysis(result, result == null /* REPORTED false negative https://youtrack.jetbrains.com/issue/RSRP-446852 */);
            };

            act.ShouldNotThrow("because the delegate *method* is an anonymous method");
        }

        [Test]
        public void SomeFunctionDelegate()
        {
            Action act = () =>
            {
                DelegatesSample.SomeFunctionDelegate @delegate = DelegatesSample.GetSomeFunctionDelegate();
                var result = @delegate();
                // This false negative is analog to SomeFunctionDelegateWithNotNull and even requires an exemption for the delegate Invoke() method,
                // but it is necessary because this implicit annotation wouldn't be invertible with [CanBeNull]:
                ReSharper.TestValueAnalysis(result, result == null /* REPORTED false negative https://youtrack.jetbrains.com/issue/RSRP-446852 */);
            };

            act.ShouldNotThrow("because the delegate *method* is an anonymous method");
        }

        [Test]
        public void SomeFunctionDelegateWithCanBeNull()
        {
            Action act = () =>
            {
                DelegatesSample.SomeFunctionDelegateWithCanBeNull @delegate = DelegatesSample.GetSomeFunctionDelegateWithCanBeNull();
                var result = @delegate();
                ReSharper.TestValueAnalysis(result /* REPORTED false negative https://youtrack.jetbrains.com/issue/RSRP-446852 */, result == null);
            };

            act.ShouldNotThrow();
        }

        [Test]
        public void SomeNotNullDelegateOfExternalCode()
        {
            Action act = () => DelegatesSample.GetSomeNotNullDelegateOfExternalCode()(null /*Expect:AssignNullToNotNullAttribute*/);

            act.ShouldNotThrow("because the delegate *method* is an anonymous method");
        }

        [Test]
        public void SomeDelegateOfExternalCode()
        {
            Action act = () => DelegatesSample.GetSomeDelegateOfExternalCode()(null /* no warning because the delegate is external */);

            act.ShouldNotThrow("because the delegate *method* is an anonymous method");
        }

        [Test]
        public void SomeDelegateWithRefAndOutWithNullValueForRefArgument()
        {
            Action act = () =>
            {
                string refParam = null;
                string outParam;

                DelegatesSample.GetSomeDelegateWithRefAndOut()(ref refParam, out outParam);

                ReSharper.TestValueAnalysis(refParam, refParam == null /*Expect:ConditionIsAlwaysTrueOrFalse[MRef]*/);
                ReSharper.TestValueAnalysis(outParam, outParam == null /*Expect:ConditionIsAlwaysTrueOrFalse[MOut]*/);
            };

            act.ShouldNotThrow("because the delegate *method* is an anonymous method");
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
                ReSharper.TestValueAnalysis(asyncResult, asyncResult == null /*Expect:ConditionIsAlwaysTrueOrFalse*/);
                asyncResult.AsyncWaitHandle.WaitOne();
            };

            act.ShouldNotThrow();
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
                ReSharper.TestValueAnalysis(result, result == null /* REPORTED false negative https://youtrack.jetbrains.com/issue/RSRP-446852 */);

                var endInvokeResult = @delegate.EndInvoke(@delegate.BeginInvoke(null, null));
                ReSharper.TestValueAnalysis(endInvokeResult, endInvokeResult == null);
            };

            act.ShouldNotThrow();
        }
    }
}