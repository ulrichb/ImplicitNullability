#if DEBUG
using System;
using System.Diagnostics;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;

namespace ImplicitNullability.Plugin.Infrastructure
{
  internal static class DebugUtilities
  {
    public static string FormatIncludingContext ([CanBeNull] IDeclaredElement element)
    {
      if (element == null)
        return "NULL";

      var result = element.GetType().Name + " '" + element.ShortName + "'";

      var clrDeclaredElement = element as IClrDeclaredElement;

      if (clrDeclaredElement != null)
      {
        var containingType = clrDeclaredElement.GetContainingType();
        var containingTypeName = containingType == null ? "null" : containingType.GetClrName().FullName;
        result += " in " + containingTypeName;

        if (clrDeclaredElement is IParameter) // executing GetContainingTypeMember() on e.g. TypeParameters throws in R# 8.2
        {
          var containingTypeMember = clrDeclaredElement.GetContainingTypeMember();
          if (containingTypeMember != null)
            result += "." + containingTypeMember.ShortName + "()";
        }
      }

      return result;
    }

    public static string FormatWithElapsed ([NotNull] string message, [NotNull] Stopwatch stopwatch)
    {
      return message + " took " + Math.Round (stopwatch.Elapsed.TotalMilliseconds * 1000) + " usec";
    }
  }
}

#endif