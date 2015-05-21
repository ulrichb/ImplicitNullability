using System;
using JetBrains.Annotations;

namespace ImplicitNullability.Sample.NullabilityAnalysis
{
  public static class OperatorOverloadsSample
  {
    public class Simple
    {
      public static int operator + (Simple left, Simple right)
      {
        ReSharper.TestValueAnalysis (left, left == null /*Expect:ConditionIsAlwaysTrueOrFalse*/);
        ReSharper.TestValueAnalysis (right, right == null /*Expect:ConditionIsAlwaysTrueOrFalse*/);
        return 0;
      }

      public static Simple operator ++ (Simple value)
      {
        ReSharper.TestValueAnalysis (value, value == null /*Expect:ConditionIsAlwaysTrueOrFalse*/);
        return null;
      }
    }

    public class CanBeNull
    {
      public static int operator + ([CanBeNull] CanBeNull left, [CanBeNull] CanBeNull right)
      {
        ReSharper.TestValueAnalysis (left /*Expect:AssignNullToNotNullAttribute*/, left == null);
        ReSharper.TestValueAnalysis (right /*Expect:AssignNullToNotNullAttribute*/, right == null);
        return 0;
      }

      public static CanBeNull operator ++ ([CanBeNull] CanBeNull value)
      {
        ReSharper.TestValueAnalysis (value /*Expect:AssignNullToNotNullAttribute*/, value == null);
        return null;
      }
    }
  }
}