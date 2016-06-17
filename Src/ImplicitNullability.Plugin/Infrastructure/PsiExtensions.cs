using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Modules;

namespace ImplicitNullability.Plugin.Infrastructure
{
    public static class PsiExtensions
    {
        public static bool IsPartOfSolutionCode(this IClrDeclaredElement declaredElement)
        {
            return declaredElement.Module.IsPartOfSolutionCode();
        }

        /// <summary>
        /// Check if this module is part of the solution code (see https://resharper-support.jetbrains.com/hc/en-us/community/posts/205991209).
        /// </summary>
        public static bool IsPartOfSolutionCode(this IPsiModule psiModule)
        {
            return psiModule is IProjectPsiModule;
        }

        public static bool IsUnknown(this CodeAnnotationNullableValue? x)
        {
            return !x.HasValue;
        }

        public static bool IsInput(this IParameter parameter)
        {
            return parameter.Kind == ParameterKind.VALUE;
        }

        public static bool IsRef(this IParameter parameter)
        {
            return parameter.Kind == ParameterKind.REFERENCE;
        }

        public static bool IsOut(this IParameter parameter)
        {
            return parameter.Kind == ParameterKind.OUTPUT;
        }
    }
}