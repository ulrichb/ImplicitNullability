using System.Collections.Generic;
using System.Linq;
using ImplicitNullability.Plugin.Highlighting;
using ImplicitNullability.Plugin.Infrastructure;
using JetBrains.Application.Components;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Daemon.CSharp.Stages.Analysis;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace ImplicitNullability.Plugin
{
    [ElementProblemAnalyzer(
        // From IncorrectNullableAttributeUsageAnalyzer:
        typeof(IMethodDeclaration), typeof(IParameterDeclaration), typeof(IFieldDeclaration),
        typeof(IPropertyDeclaration), typeof(IIndexerDeclaration), typeof(IOperatorDeclaration),
        // Extra:
        typeof(IDelegateDeclaration),
        HighlightingTypes =
            new[]
            {
                // From IncorrectNullableAttributeUsageAnalyzer:
                typeof(AnnotationRedundancyInHierarchyWarning),
                typeof(AnnotationConflictInHierarchyWarning),
                typeof(AnnotationRedundancyAtValueTypeWarning),
                typeof(ContainerAnnotationRedundancyWarning),
                typeof(MultipleNullableAttributesUsageWarning),
                // The own ones:
                typeof(NotNullOnImplicitCanBeNullHighlighting),
                typeof(ImplicitNotNullConflictInHierarchyHighlighting),
                typeof(ImplicitNotNullElementCannotOverrideCanBeNullHighlighting),
                typeof(ImplicitNotNullOverridesUnknownBaseMemberNullabilityHighlighting),
                typeof(ImplicitNotNullResultOverridesUnknownBaseMemberNullabilityHighlighting)
            })]
    public class ImplicitNullabilityProblemAnalyzer : ElementProblemAnalyzer<IDeclaration>,
        IHideImplementation<IncorrectNullableAttributeUsageAnalyzer>
    {
        private readonly CodeAnnotationAttributesChecker _annotationAttributesChecker;
        private readonly NullnessProvider _nullnessProvider;
        private readonly ContainerElementNullnessProvider _containerElementNullnessProvider;
        private readonly ImplicitNullabilityProvider _implicitNullabilityProvider;
        private readonly IncorrectNullableAttributeUsageAnalyzer _incorrectNullableAttributeUsageAnalyzer;

        public ImplicitNullabilityProblemAnalyzer(
            CodeAnnotationAttributesChecker annotationAttributesChecker,
            CodeAnnotationsCache codeAnnotationsCache,
            ImplicitNullabilityProvider implicitNullabilityProvider)
        {
            _annotationAttributesChecker = annotationAttributesChecker;
            _nullnessProvider = codeAnnotationsCache.GetProvider<NullnessProvider>();
            _containerElementNullnessProvider = codeAnnotationsCache.GetProvider<ContainerElementNullnessProvider>();
            _implicitNullabilityProvider = implicitNullabilityProvider;

            _incorrectNullableAttributeUsageAnalyzer = new IncorrectNullableAttributeUsageAnalyzer(codeAnnotationsCache);
        }

        protected override void Run(IDeclaration declaration, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            var declaredElement = declaration.DeclaredElement;
            var attributesSet = declaredElement as IAttributesSet;

            var hasOveriddenImplicitNullability = false;

            if (attributesSet != null)
            {
                Assertion.Assert(declaredElement != null, "declaredElement != null");

                var attributeInstances = attributesSet.GetAttributeInstances(inherit: false);

                switch (declaration)
                {
                    case IParameterDeclaration parameterDeclaration:
                        var parameter = parameterDeclaration.DeclaredElement.NotNull();
                        CheckForNotNullOnImplicitCanBeNull(consumer, parameter, attributeInstances, declaration);
                        CheckForParameterSuperMemberConflicts(consumer, parameter, attributeInstances, declaration);
                        break;

                    case IMethodDeclaration methodDeclaration:
                        var method = methodDeclaration.DeclaredElement.NotNull();
                        CheckForNotNullOnImplicitCanBeNull(consumer, method, attributeInstances, methodDeclaration.NameIdentifier);
                        CheckForMethodSuperMemberConflicts(consumer, method, attributeInstances, methodDeclaration.NameIdentifier);
                        break;

                    case IOperatorDeclaration operatorDeclaration:
                        var operatorKeyword = operatorDeclaration.OperatorKeyword;
                        CheckForNotNullOnImplicitCanBeNull(consumer, declaredElement, attributeInstances, operatorKeyword);
                        break;

                    case IDelegateDeclaration delegateDeclaration:
                        var delegateName = delegateDeclaration.NameIdentifier;
                        CheckForNotNullOnImplicitCanBeNull(consumer, declaredElement, attributeInstances, delegateName);
                        break;

                    case IFieldDeclaration fieldDeclaration:
                        var fieldName = fieldDeclaration.NameIdentifier;
                        CheckForNotNullOnImplicitCanBeNull(consumer, declaredElement, attributeInstances, fieldName);
                        break;

                    case IPropertyDeclaration propertyDeclaration:
                        var property = propertyDeclaration.DeclaredElement.NotNull();
                        var propertyName = propertyDeclaration.NameIdentifier;
                        CheckForNotNullOnImplicitCanBeNull(consumer, declaredElement, attributeInstances, propertyName);
                        CheckForPropertySuperMemberConflicts(consumer, property, attributeInstances, propertyName);
                        break;

                    case IIndexerDeclaration indexerDeclaration:
                        var indexerProperty = indexerDeclaration.DeclaredElement.NotNull();
                        var indexerThisKeyword = indexerDeclaration.ThisKeyword;
                        CheckForNotNullOnImplicitCanBeNull(consumer, declaredElement, attributeInstances, indexerThisKeyword);
                        CheckForPropertySuperMemberConflicts(consumer, indexerProperty, attributeInstances, indexerThisKeyword);
                        break;
                }

                hasOveriddenImplicitNullability |=
                    _annotationAttributesChecker.ContainsAnyExplicitNullabilityAttributes(attributeInstances) &&
                    _implicitNullabilityProvider.AnalyzeDeclaredElement(declaredElement) != null;

                hasOveriddenImplicitNullability |=
                    _annotationAttributesChecker.ContainsAnyExplicitItemNullabilityAttributes(attributeInstances) &&
                    _implicitNullabilityProvider.AnalyzeDeclaredElementContainerElement(declaredElement) != null;
            }

            DelegateToIncorrectNullableAttributeUsageAnalyzer(declaration, data, consumer, hasOveriddenImplicitNullability);
        }

        private void CheckForParameterSuperMemberConflicts(
            IHighlightingConsumer consumer,
            IParameter parameter,
            IList<IAttributeInstance> attributeInstances,
            ITreeNode highlightingNode)
        {
            if (IsImplicitlyNotNull(parameter, attributeInstances))
            {
                var superMembersNullability = GetImmediateSuperMembersNullability(parameter).ToArray();

                if (parameter.IsInput() || parameter.IsRef())
                    CheckForInputOrRefElementSuperMemberConflicts(consumer, superMembersNullability, highlightingNode);

                if (parameter.IsOut())
                    CheckForOutputElementSuperMemberConflicts(consumer, superMembersNullability, highlightingNode);
            }
        }

        private void CheckForMethodSuperMemberConflicts(
            IHighlightingConsumer consumer,
            IMethod method,
            IList<IAttributeInstance> attributeInstances,
            ITreeNode highlightingNode)
        {
            // Handle async methods like explicit [NotNull] methods to avoid unnecessary false positives.
            // See Derived.CanBeNull_WithOverridingAsyncMethod() test case.

            if (IsImplicitlyNotNull(method, attributeInstances) && !method.IsAsync)
            {
                var superMembersNullability = GetImmediateSuperMembersNullability(method).ToArray();
                CheckForOutputElementSuperMemberConflicts(consumer, superMembersNullability, highlightingNode);
            }


            if (IsContainerElementImplicitlyNotNull(method, attributeInstances))
            {
                // Implicitly nullable Task<T> / async methods

                var superMembersNullability = GetImmediateSuperMembersContainerElementNullability(method).ToArray();
                CheckForOutputElementSuperMemberConflicts(consumer, superMembersNullability, highlightingNode);
            }
        }

        private void CheckForPropertySuperMemberConflicts(
            IHighlightingConsumer consumer,
            IProperty property,
            IList<IAttributeInstance> attributeInstances,
            ITreeNode highlightingNode)
        {
            if (IsImplicitlyNotNull(property, attributeInstances))
            {
                if (property.Setter != null)
                {
                    var superMembersNullability = GetImmediateSuperMembersSetterNullability(property).ToArray();
                    CheckForInputOrRefElementSuperMemberConflicts(consumer, superMembersNullability, highlightingNode);
                }

                if (property.Getter != null)
                {
                    var superMembersNullability = GetImmediateSuperMembersGetterNullability(property).ToArray();
                    CheckForOutputElementSuperMemberConflicts(consumer, superMembersNullability, highlightingNode);
                }
            }
        }

        private static void CheckForInputOrRefElementSuperMemberConflicts(
            IHighlightingConsumer consumer,
            CodeAnnotationNullableValue?[] superMembersNullability,
            ITreeNode highlightingNode)
        {
            if (ContainsCanBeNull(superMembersNullability))
                consumer.AddHighlighting(new ImplicitNotNullConflictInHierarchyHighlighting(highlightingNode));

            if (ContainsUnknownNullability(superMembersNullability))
                consumer.AddHighlighting(new ImplicitNotNullOverridesUnknownBaseMemberNullabilityHighlighting(highlightingNode));
        }

        private static void CheckForOutputElementSuperMemberConflicts(
            IHighlightingConsumer consumer,
            CodeAnnotationNullableValue?[] superMembersNullability,
            ITreeNode highlightingNode)
        {
            if (ContainsCanBeNull(superMembersNullability))
                consumer.AddHighlighting(new ImplicitNotNullElementCannotOverrideCanBeNullHighlighting(highlightingNode));

            if (ContainsUnknownNullability(superMembersNullability))
                consumer.AddHighlighting(new ImplicitNotNullResultOverridesUnknownBaseMemberNullabilityHighlighting(highlightingNode));
        }

        private bool IsImplicitlyNotNull(IDeclaredElement declaredElement, IEnumerable<IAttributeInstance> attributeInstances)
        {
            return !_annotationAttributesChecker.ContainsAnyExplicitNullabilityAttributes(attributeInstances) &&
                   _implicitNullabilityProvider.AnalyzeDeclaredElement(declaredElement) == CodeAnnotationNullableValue.NOT_NULL;
        }

        private bool IsContainerElementImplicitlyNotNull(IDeclaredElement declaredElement, IEnumerable<IAttributeInstance> attributeInstances)
        {
            return !_annotationAttributesChecker.ContainsAnyExplicitItemNullabilityAttributes(attributeInstances) &&
                   _implicitNullabilityProvider.AnalyzeDeclaredElementContainerElement(declaredElement) == CodeAnnotationNullableValue.NOT_NULL;
        }

        private static bool ContainsCanBeNull(IEnumerable<CodeAnnotationNullableValue?> superMembersNullability)
        {
            return superMembersNullability.Any(x => x == CodeAnnotationNullableValue.CAN_BE_NULL);
        }

        private static bool ContainsUnknownNullability(IEnumerable<CodeAnnotationNullableValue?> superMembersNullability)
        {
            return superMembersNullability.Any(x => x.IsUnknown());
        }

        private void CheckForNotNullOnImplicitCanBeNull(
            IHighlightingConsumer consumer,
            IDeclaredElement element,
            IList<IAttributeInstance> attributeInstances,
            ITreeNode highlightingNode)
        {
            if (_annotationAttributesChecker.ContainsExplicitNotNullNullabilityAttribute(attributeInstances) &&
                _implicitNullabilityProvider.AnalyzeDeclaredElement(element) == CodeAnnotationNullableValue.CAN_BE_NULL)
            {
                consumer.AddHighlighting(new NotNullOnImplicitCanBeNullHighlighting(highlightingNode));
            }

            if (_annotationAttributesChecker.ContainsExplicitItemNotNullNullabilityAttribute(attributeInstances) &&
                _implicitNullabilityProvider.AnalyzeDeclaredElementContainerElement(element) == CodeAnnotationNullableValue.CAN_BE_NULL)
            {
                consumer.AddHighlighting(new NotNullOnImplicitCanBeNullHighlighting(highlightingNode));
            }
        }

        private void DelegateToIncorrectNullableAttributeUsageAnalyzer(
            ITreeNode declaration,
            ElementProblemAnalyzerData data,
            IHighlightingConsumer consumer,
            bool hasOveriddenImplicitNullability)
        {
            if (hasOveriddenImplicitNullability)
            {
                var consumerDecorator = new AnnotationRedundancyInHierarchyWarningFilteringDecorator(consumer);
                _incorrectNullableAttributeUsageAnalyzer.Run(declaration, data, consumerDecorator);
            }
            else
            {
                _incorrectNullableAttributeUsageAnalyzer.Run(declaration, data, consumer);
            }
        }

        private IEnumerable<CodeAnnotationNullableValue?> GetImmediateSuperMembersNullability(IParameter parameter)
        {
            var overridableMember = parameter.ContainingParametersOwner as IOverridableMember;

            if (overridableMember == null)
                return EmptyList<CodeAnnotationNullableValue?>.InstanceList;

            var superMembers = overridableMember.GetImmediateSuperMembers();

            var parameterIndex = parameter.IndexOf();

            var result = superMembers
                // We assume that the super members of a parameter owner are also parameter owners with the same number of parameters:
                .Select(x => _nullnessProvider.GetInfo(((IParametersOwner) x.Element).Parameters[parameterIndex]));

            return result;
        }

        private IEnumerable<CodeAnnotationNullableValue?> GetImmediateSuperMembersNullability(IMethod method)
        {
            return method.GetImmediateSuperMembers().Select(x => _nullnessProvider.GetInfo(x.Member));
        }

        private IEnumerable<CodeAnnotationNullableValue?> GetImmediateSuperMembersSetterNullability(IProperty property)
        {
            var immediateSuperMembers = property.GetImmediateSuperMembers();
            return immediateSuperMembers.Where(x => ((IProperty) x.Member).Setter != null).Select(x => _nullnessProvider.GetInfo(x.Member));
        }

        private IEnumerable<CodeAnnotationNullableValue?> GetImmediateSuperMembersGetterNullability(IProperty property)
        {
            var immediateSuperMembers = property.GetImmediateSuperMembers();
            return immediateSuperMembers.Where(x => ((IProperty) x.Member).Getter != null).Select(x => _nullnessProvider.GetInfo(x.Member));
        }

        private IEnumerable<CodeAnnotationNullableValue?> GetImmediateSuperMembersContainerElementNullability(IMethod method)
        {
            return method.GetImmediateSuperMembers().Select(x => _containerElementNullnessProvider.GetInfo(x.Member));
        }
    }
}
