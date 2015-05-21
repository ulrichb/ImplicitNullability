using System;
using ImplicitNullability.Sample.ExternalCode;
using JetBrains.Annotations;

namespace ImplicitNullability.Sample.NullabilityAnalysis
{
  public static class DelegatesSample
  {
    public static Action<string> GetSomeAction ()
    {
      return s => ReSharper.TestValueAnalysis (s, s == null);
    }

    public static Action<string> GetSomeActionWithClosedValues ()
    {
      var captured = 0;
      return s =>
      {
        ReSharper.TestValueAnalysis (s, s == null);
        var x = captured;
      };
    }

    public delegate void SomeDelegate (string s);
    public delegate void SomeDelegateWithCanBeNull ([CanBeNull] string s);

    public static SomeDelegate GetSomeDelegateToAnonymousMethod ()
    {
      // ReSharper disable once ConvertToLambdaExpression
      return delegate (string s) { ReSharper.TestValueAnalysis (s, s == null); };
    }

    public static SomeDelegate GetSomeDelegateToNamedMethod ()
    {
      return NamedMethod;
    }

    public static SomeDelegate GetSomeDelegateToNamedMethodWithCanBeNull ()
    {
      return NamedMethodWithCanBeNull;
    }

    public static SomeDelegateWithCanBeNull GetSomeDelegateWithCanBeNullToNamedMethod ()
    {
      // REPORT? Here R# could warn about assigning a method with a NotNull parameter to a delegate with CanBeNull on the corresponding parameter
      return NamedMethod;
    }

    public static SomeDelegateWithCanBeNull GetSomeDelegateWithCanBeNullToNamedMethodWithCanBeNull ()
    {
      return NamedMethodWithCanBeNull;
    }

    public static External.SomeDelegate GetSomeDelegateOfExternalCode ()
    {
      return s => ReSharper.TestValueAnalysis (s, s == null);
    }

    public static External.SomeNotNullDelegate GetSomeNotNullDelegateOfExternalCode ()
    {
      return s => ReSharper.TestValueAnalysis (s, s == null);
    }

    public delegate void SomeDelegateWithRefAndOut (out string outString, ref string refString);

    public static SomeDelegateWithRefAndOut GetSomeDelegateWithRefAndOut ()
    {
      return delegate (out string outString, ref string refString)
      {
        outString = null;
        ReSharper.TestValueAnalysis (refString, refString == null);
      };
    }

    private static void NamedMethod (string a)
    {
    }

    private static void NamedMethodWithCanBeNull ([CanBeNull] string a)
    {
    }

    private static void ActionDelegateConstructor ()
    {
      Action nullAction = null;

      // Here the target parameter is a Special.Parameter, which is an IParameter. Note that this is an "builtin" null ref warning. Nullability 
      // annotations are not involved for this error, although the annotation provider is being called (observed in R# 8.2).
      var action = new Action (nullAction /*Expect:PossibleNullReferenceException*/);
    }
  }
}