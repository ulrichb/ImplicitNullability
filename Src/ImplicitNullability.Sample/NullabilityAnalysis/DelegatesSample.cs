using System;
using ImplicitNullability.Sample.ExternalCode;
using JetBrains.Annotations;

namespace ImplicitNullability.Sample.NullabilityAnalysis
{
    public static class DelegatesSample
    {
        public static Action<string> GetSomeAction()
        {
            return s => ReSharper.TestValueAnalysis(s, s == null);
        }

        public static Action<string> GetSomeActionWithClosedValues()
        {
            var captured = Guid.NewGuid();
            return s =>
            {
                ReSharper.TestValueAnalysis(s, s == null);
                ReSharper.SuppressUnusedWarning(captured);
            };
        }

        public static Action<string> GetSomeActionToAnonymousMethod()
        {
            return delegate(string s) { ReSharper.TestValueAnalysis(s, s == null); };
        }

        public delegate void SomeDelegate(string s);

        public static SomeDelegate GetSomeDelegate()
        {
            return s => ReSharper.TestValueAnalysis(s, s == null /* REPORTED false negative https://youtrack.jetbrains.com/issue/RSRP-446852 */);
        }

        public static SomeDelegate GetSomeDelegateToNamedMethod()
        {
            return NamedMethod;
        }

        public static SomeDelegate GetSomeDelegateToNamedMethodWithCanBeNull()
        {
            return NamedMethodWithCanBeNull;
        }

        public delegate void SomeDelegateWithCanBeNull([CanBeNull] string s);

        public static SomeDelegateWithCanBeNull GetSomeDelegateWithCanBeNullToNamedMethod()
        {
            // REPORT? Here R# could warn about the precondition substitutability issue
            return NamedMethod;
        }

        public static SomeDelegateWithCanBeNull GetSomeDelegateWithCanBeNullToNamedMethodWithCanBeNull()
        {
            return NamedMethodWithCanBeNull;
        }

        [NotNull]
        public delegate string SomeFunctionDelegateWithNotNull();

        public static SomeFunctionDelegateWithNotNull GetSomeFunctionDelegateWithNotNull()
        {
            return () => null /*Expect:AssignNullToNotNullAttribute*/;
        }

        public delegate string SomeFunctionDelegate();

        public static SomeFunctionDelegate GetSomeFunctionDelegate()
        {
            return () => null /*Expect:AssignNullToNotNullAttribute[MOut]*/;
        }

        public static SomeFunctionDelegate GetSomeFunctionDelegateToNamedMethod()
        {
            // REPORT? Here R# could warn about the postcondition substitutability issue
            return NamedFunctionWithCanBeNull;
        }

        [CanBeNull]
        public delegate string SomeFunctionDelegateWithCanBeNull();

        public static SomeFunctionDelegateWithCanBeNull GetSomeFunctionDelegateWithCanBeNull()
        {
            return () => null;
        }

        public static External.SomeNotNullDelegate GetSomeNotNullDelegateOfExternalCode()
        {
            return s => ReSharper.TestValueAnalysis(s, s == null);
        }

        public static External.SomeDelegate GetSomeDelegateOfExternalCode()
        {
            return s => ReSharper.TestValueAnalysis(s, s == null);
        }

        public delegate void SomeDelegateWithRefAndOut(ref string refParam, out string outParam);

        public static SomeDelegateWithRefAndOut GetSomeDelegateWithRefAndOut()
        {
            return delegate(ref string refParam, out string outParam)
            {
                ReSharper.TestValueAnalysis(refParam, refParam == null);

                // For ref and out-parameters (in contrast to the return value), R# doesn't seem to use the delegate annotations:
                refParam = null;
                outParam = null;
            };
        }

        private static void NamedMethod(string a)
        {
        }

        private static void NamedMethodWithCanBeNull([CanBeNull] string a)
        {
        }

        [CanBeNull]
        private static string NamedFunctionWithCanBeNull()
        {
            return null;
        }

        public static void ActionDelegateConstructor()
        {
            Action nullAction = null;

            // Here the target parameter is a Special.Parameter, which is an IParameter. Note that this is an "builtin" null ref warning. Nullability 
            // annotations are not involved for this error, although the annotation provider is being called (observed in R# 8.2).
            var action = new Action(nullAction /*Expect:PossibleNullReferenceException*/);
            ReSharper.SuppressUnusedWarning(action);
        }
    }
}