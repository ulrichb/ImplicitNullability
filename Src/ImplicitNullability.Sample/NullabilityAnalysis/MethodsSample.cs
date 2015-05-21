using System;
using JetBrains.Annotations;

namespace ImplicitNullability.Sample.NullabilityAnalysis
{
  public class MethodsSample
  {
    private const string StringConst = "some string";
    private const string NullStringConst = null;
    private const string DefaultOfStringConst = default(string);

    public void TestProcedure (string a)
    {
      ReSharper.TestValueAnalysis (a, a == null /*Expect:ConditionIsAlwaysTrueOrFalse*/);
    }

    public void TestProcedureWithExplicitCanBeNull ([CanBeNull] string canBeNull)
    {
    }

    public void TestProcedureWithImplicitCanBeNull (int? nullableInt, string optional = null)
    {
      ReSharper.TestValueAnalysis (nullableInt /*Expect:AssignNullToNotNullAttribute*/, nullableInt == null);
      ReSharper.TestValueAnalysis (optional /*Expect:AssignNullToNotNullAttribute*/, optional == null);
    }

    public void TestProcedureWithNotNullAnnotationForNullableInt ([NotNull] int? a)
    {
      // REPORT? Warning, although explicitly NotNull
      ReSharper.TestValueAnalysis (a /*Expect:AssignNullToNotNullAttribute*/, a == null);
    }

    // ReSharper disable AssignNullToNotNullAttribute - because only in R# 9 and not relevant for this test case IDEA: instead add a conditional "Except:[R#9]"?
    public void TestProcedureWithNotNullAnnotationForNullDefaultArgument ([NotNull] string optional = null)
        // ReSharper restore AssignNullToNotNullAttribute
    {
      // REPORT? Warning, although explicitly NotNull
      ReSharper.TestValueAnalysis (optional /*Expect:AssignNullToNotNullAttribute*/, optional == null);
    }

    public void TestProcedureWithOutAndRefParameters (out string outString, ref string refString)
    {
      outString = null; // No warning, because out params shouldn't be implicit NotNull

      ReSharper.TestValueAnalysis (refString, refString == null /* REPORTED false negative https://youtrack.jetbrains.com/issue/RSRP-427414 */);

      // Note that the implicit not null argument applies also to the outgoing value of 'refString'
      refString = null /*Expect:AssignNullToNotNullAttribute*/;
    }

    public void TestProcedureWithExplicitNotNullRefParameter ([NotNull] ref string refString)
    {
      ReSharper.TestValueAnalysis (refString, refString == null /* REPORTED false negative https://youtrack.jetbrains.com/issue/RSRP-427414 */);
      refString = null /*Expect:AssignNullToNotNullAttribute*/;
    }

    public void TestProcedureWithCanBeNullRefParameter ([CanBeNull] ref string refString)
    {
      ReSharper.TestValueAnalysis (refString /* REPORTED false negative https://youtrack.jetbrains.com/issue/RSRP-427414 */, refString == null);
      refString = null;
    }

    public void TestProcedureWithNullDefaultOfStringDefaultArgument (string optional = default(string))
    {
      ReSharper.TestValueAnalysis (optional /*Expect:AssignNullToNotNullAttribute*/, optional == null);
    }

    public void TestProcedureWithNullDefaultArgumentFromConst (string optional = NullStringConst)
    {
      ReSharper.TestValueAnalysis (optional /*Expect:AssignNullToNotNullAttribute*/, optional == null);
    }

    public void TestProcedureWithNullDefaultArgumentFromDefaultOfStringConst (string optional = DefaultOfStringConst)
    {
      ReSharper.TestValueAnalysis (optional /*Expect:AssignNullToNotNullAttribute*/, optional == null);
    }

    public void TestProcedureWithNonNullDefaultArgument (string optional = "default")
    {
      ReSharper.TestValueAnalysis (optional, optional == null /*Expect:ConditionIsAlwaysTrueOrFalse*/);
    }

    public void TestProcedureWithNonNullDefaultArgumentFromConst (string optional = StringConst)
    {
      ReSharper.TestValueAnalysis (optional, optional == null /*Expect:ConditionIsAlwaysTrueOrFalse*/);
    }

    public void TestProcedureWithParams (params string[] a)
    {
      ReSharper.TestValueAnalysis (a, a == null /*Expect:ConditionIsAlwaysTrueOrFalse*/);
    }

    public string TestFunction ([CanBeNull] string returnValue)
    {
      return returnValue;
    }

    [CanBeNull]
    public string TestCanBeNullFunction ([CanBeNull] string returnValue)
    {
      return returnValue;
    }

    [UsedImplicitly]
    internal void InternalMethod (string internalMethodParameter)
    {
      ReSharper.TestValueAnalysis(internalMethodParameter, internalMethodParameter == null /*Expect:ConditionIsAlwaysTrueOrFalse*/);
    }

    [UsedImplicitly]
    protected void ProtectedMethod (string protectedMethodParameter)
    {
      ReSharper.TestValueAnalysis(protectedMethodParameter, protectedMethodParameter == null /*Expect:ConditionIsAlwaysTrueOrFalse*/);
    }

    [UsedImplicitly]
    private void PrivateMethod (string privateMethodParameter)
    {
      ReSharper.TestValueAnalysis(privateMethodParameter, privateMethodParameter == null /*Expect:ConditionIsAlwaysTrueOrFalse*/);
    }

    public static void StaticTestProcedure (string a)
    {
      ReSharper.TestValueAnalysis (a, a == null /*Expect:ConditionIsAlwaysTrueOrFalse*/);
    }

    public void GenericMethod<T> (T a)
    {
      ReSharper.TestValueAnalysis (a, ReferenceEquals (a, null) /*Expect:ConditionIsAlwaysTrueOrFalse*/);
    }
  }
}