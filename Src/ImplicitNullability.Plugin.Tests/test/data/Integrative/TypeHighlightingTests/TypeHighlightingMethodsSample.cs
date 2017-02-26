using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ImplicitNullability.Plugin.Tests.test.data.Integrative.TypeHighlightingTests
{
    public abstract class TypeHighlightingMethodsSampleBase
    {
        [NotNull]
        public abstract string VirtualMethodWithExplicitNullabilityInBase([NotNull] string a, [CanBeNull] string b);
    }

    public class TypeHighlightingMethodsSample : TypeHighlightingMethodsSampleBase
    {
        [NotNull]
        public string MethodExplicit([NotNull] string a)
        {
            return a;
        }

        public string MethodImplicit(string a, ref string refParam, out string outParam, params object[] values)
        {
            Console.WriteLine(a == null);
            outParam = null;
            return null;
        }

        public void NonReferenceTypes(int a, DateTime b)
        {
        }

        [CanBeNull]
        public string Nullable([CanBeNull] string a)
        {
            return a;
        }

        // ReSharper disable once ImplicitNotNullConflictInHierarchy
        public override string VirtualMethodWithExplicitNullabilityInBase(string a, string b)
        {
            return a;
        }

        public static TypeHighlightingMethodsSample operator ++(TypeHighlightingMethodsSample value)
        {
            return new TypeHighlightingMethodsSample();
        }

        [CanBeNull]
        public static TypeHighlightingMethodsSample operator --([CanBeNull] TypeHighlightingMethodsSample value)
        {
            return null;
        }

        public static explicit operator TypeHighlightingMethodsSample(string s)
        {
            return new TypeHighlightingMethodsSample();
        }

        public IEnumerable<string> Iterator()
        {
            yield return "";
        }
    }
}
