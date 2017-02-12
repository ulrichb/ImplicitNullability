using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Modules;

namespace ImplicitNullability.Plugin.Infrastructure
{
    public static class PsiExtensions
    {
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

        public static bool IsMemberOfReferenceType(this ITypeMember typeMember)
        {
            return !(typeMember.GetContainingType() is IStruct);
        }

        public static bool IsAsyncVoid(this IMethod method)
        {
            return method.IsAsync && method.ReturnType.IsVoid();
        }
    }
}
