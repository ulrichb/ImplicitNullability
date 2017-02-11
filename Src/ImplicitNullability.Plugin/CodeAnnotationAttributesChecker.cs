using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;

namespace ImplicitNullability.Plugin
{
    /// <summary>
    /// A service which checks for specific code annotations attributes.
    /// </summary>
    [PsiComponent]
    public class CodeAnnotationAttributesChecker
    {
        private readonly CodeAnnotationsConfiguration _codeAnnotationsConfiguration;

        public CodeAnnotationAttributesChecker(CodeAnnotationsConfiguration codeAnnotationsConfiguration)
        {
            _codeAnnotationsConfiguration = codeAnnotationsConfiguration;
        }


        public bool ContainsAnyExplicitNullabilityAttributes(IEnumerable<IAttributeInstance> attributeInstances)
        {
            return attributeInstances.Any(x =>
                _codeAnnotationsConfiguration.IsAnnotationAttribute(x, NullnessProvider.NotNullAttributeShortName) ||
                _codeAnnotationsConfiguration.IsAnnotationAttribute(x, NullnessProvider.CanBeNullAttributeShortName));
        }

        public bool ContainsExplicitNotNullNullabilityAttribute(IEnumerable<IAttributeInstance> attributeInstances)
        {
            return attributeInstances.Any(x =>
                _codeAnnotationsConfiguration.IsAnnotationAttribute(x, NullnessProvider.NotNullAttributeShortName));
        }

        public bool ContainsAnyExplicitItemNullabilityAttributes(IEnumerable<IAttributeInstance> attributeInstances)
        {
            return attributeInstances.Any(x =>
                _codeAnnotationsConfiguration.IsAnnotationAttribute(x, ContainerElementNullnessProvider.ItemNotNullAttributeShortName) ||
                _codeAnnotationsConfiguration.IsAnnotationAttribute(x, ContainerElementNullnessProvider.ItemCanBeNullAttributeShortName));
        }

        public bool ContainsExplicitItemNotNullNullabilityAttribute(IEnumerable<IAttributeInstance> attributeInstances)
        {
            return attributeInstances.Any(x =>
                _codeAnnotationsConfiguration.IsAnnotationAttribute(x, ContainerElementNullnessProvider.ItemNotNullAttributeShortName));
        }

        public bool ContainsContractAnnotationAttribute(IEnumerable<IAttributeInstance> attributeInstances)
        {
            return attributeInstances.Any(x =>
                _codeAnnotationsConfiguration.IsAnnotationAttribute(x, ContractAnnotationProvider.ContractAnnotationAttributeShortName));
        }
    }
}
