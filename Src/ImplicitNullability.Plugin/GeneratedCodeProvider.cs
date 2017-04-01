using System.Linq;
using ImplicitNullability.Plugin.Infrastructure;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.GeneratedCode;

namespace ImplicitNullability.Plugin
{
    [PsiComponent]
    public class GeneratedCodeProvider
    {
        public bool IsGeneratedOrSynthetic(ITypeMember typeMember)
        {
            // When calling this on a delegate _method_ (e.g. the 'Invoke' delegate method) we have to direct to
            // the containing delegate _type_ instead because the delegate method is only a "synthetic" element without
            // declarations (see DelegateMethod.GetDeclarations(), resp. the subtypes of DelegateMethod).

            var containingType = typeMember.GetContainingType();

            if (containingType is IDelegate @delegate)
            {
                if (IsGeneratedOrSynthetic(@delegate, @delegate.GetContainingType()))
                    return true;
            }
            else
            {
                if (IsGeneratedOrSynthetic(typeMember, containingType))
                    return true;
            }


            return false;
        }

        private static bool IsGeneratedOrSynthetic(ITypeMember typeMember, [CanBeNull] ITypeElement containingType)
        {
            if (typeMember.HasGeneratedCodeAttribute())
                return true;

            // Can be null for "top level" delegate types:
            if (containingType != null && containingType.HasGeneratedCodeAttribute())
                return true;

            if (typeMember.Module.IsPartOfSolutionCode())
            {
                var declarations = typeMember.GetDeclarations();

                // Note that the declarations list can be empty e.g. for "synthetic" ASP.NET methods like Bind().
                // In this case we also return true (=> "is generated"), which is OK for all tested scenarios.
                // If this becomes a problem for specific elements, we need exemptions like we have for delegate methods (see above).

                // Note that 'IsGeneratedDeclaration' also contains a 'IsSynthetic()' and a 'HasGeneratedCodeAttribute()' check:
                return declarations.All(GeneratedUtils.IsGeneratedDeclaration);
            }

            return false;
        }
    }
}
