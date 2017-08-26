using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.Util;

namespace ImplicitNullability.Plugin
{
    /// <summary>
    /// An implementation of ReSharper's <see cref="ICustomCodeAnnotationProvider"/> extension point which uses
    /// the <see cref="ImplicitNullabilityProvider"/> to implement the Implicit Nullability rules.
    /// 
    /// Note that these "custom" providers are called by R# at last (basically if there are no (inherited) nullability attributes).
    /// </summary>
    [PsiComponent]
    public class ImplicitNullabilityCustomCodeAnnotationProvider : ICustomCodeAnnotationProvider
    {
        private static readonly ILogger Logger = JetBrains.Util.Logging.Logger.GetLogger(typeof(ImplicitNullabilityCustomCodeAnnotationProvider));

        private readonly ImplicitNullabilityProvider _implicitNullabilityProvider;

        public ImplicitNullabilityCustomCodeAnnotationProvider(ImplicitNullabilityProvider implicitNullabilityProvider)
        {
            Logger.Verbose(".ctor");
            _implicitNullabilityProvider = implicitNullabilityProvider;
        }

        public CodeAnnotationNullableValue? GetNullableAttribute([CanBeNull] IDeclaredElement element)
        {
            return _implicitNullabilityProvider.AnalyzeDeclaredElement(element);
        }

        public CodeAnnotationNullableValue? GetContainerElementNullableAttribute([CanBeNull] IDeclaredElement element)
        {
            return _implicitNullabilityProvider.AnalyzeDeclaredElementContainerElement(element);
        }

        public ICollection<IAttributeInstance> GetSpecialAttributeInstances([CanBeNull] IClrDeclaredElement element)
        {
            return EmptyList<IAttributeInstance>.InstanceList;
        }
    }
}
