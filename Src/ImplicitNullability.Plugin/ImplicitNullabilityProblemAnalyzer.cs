using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ImplicitNullability.Plugin.Highlighting;
using ImplicitNullability.Plugin.Infrastructure;
using JetBrains.Application.Components;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Daemon.CSharp.Stages.Analysis;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using ReSharperExtensionsShared.Debugging;

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
        private static readonly ILogger Logger = JetBrains.Util.Logging.Logger.GetLogger(typeof(ImplicitNullabilityProblemAnalyzer));

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
            Logger.Verbose(".ctor");

            _annotationAttributesChecker = annotationAttributesChecker;
            _nullnessProvider = codeAnnotationsCache.GetProvider<NullnessProvider>();
            _containerElementNullnessProvider = codeAnnotationsCache.GetProvider<ContainerElementNullnessProvider>();
            _implicitNullabilityProvider = implicitNullabilityProvider;

            _incorrectNullableAttributeUsageAnalyzer = new IncorrectNullableAttributeUsageAnalyzer(codeAnnotationsCache);
        }

        protected override void Run(IDeclaration declaration, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
#if DEBUG
            var stopwatch = Stopwatch.StartNew();
#endif

            var declaredElement = declaration.DeclaredElement;
            var attributesSet = declaredElement as IAttributesSet;

            var hasOveriddenImplicitNullability = false;

            var highlightingList = new List<IHighlighting>();

            if (attributesSet != null)
            {
                Assertion.Assert(declaredElement != null, "declaredElement != null");

                var attributeInstances = attributesSet.GetAttributeInstances(inherit: false);

                switch (declaration)
                {
                    case IParameterDeclaration parameterDeclaration:
                        var parameter = parameterDeclaration.DeclaredElement.NotNull();
                        CheckForNotNullOnImplicitCanBeNull(parameter, attributeInstances, declaration, highlightingList);
                        CheckForParameterSuperMemberConflicts(parameter, attributeInstances, declaration, highlightingList);
                        break;

                    case IMethodDeclaration methodDeclaration:
                        var method = methodDeclaration.DeclaredElement.NotNull();
                        CheckForNotNullOnImplicitCanBeNull(method, attributeInstances, methodDeclaration.NameIdentifier, highlightingList);
                        CheckForMethodSuperMemberConflicts(method, attributeInstances, methodDeclaration.NameIdentifier, highlightingList);
                        break;

                    case IOperatorDeclaration operatorDeclaration:
                        var operatorKeyword = operatorDeclaration.OperatorKeyword;
                        CheckForNotNullOnImplicitCanBeNull(declaredElement, attributeInstances, operatorKeyword, highlightingList);
                        break;

                    case IDelegateDeclaration delegateDeclaration:
                        var delegateName = delegateDeclaration.NameIdentifier;
                        CheckForNotNullOnImplicitCanBeNull(declaredElement, attributeInstances, delegateName, highlightingList);
                        break;

                    case IFieldDeclaration fieldDeclaration:
                        var fieldName = fieldDeclaration.NameIdentifier;
                        CheckForNotNullOnImplicitCanBeNull(declaredElement, attributeInstances, fieldName, highlightingList);
                        break;
                }

                highlightingList.ForEach(x => consumer.AddHighlighting(x));

                hasOveriddenImplicitNullability |=
                    _annotationAttributesChecker.ContainsAnyExplicitNullabilityAttributes(attributeInstances) &&
                    _implicitNullabilityProvider.AnalyzeDeclaredElement(declaredElement) != null;

                hasOveriddenImplicitNullability |=
                    _annotationAttributesChecker.ContainsAnyExplicitItemNullabilityAttributes(attributeInstances) &&
                    _implicitNullabilityProvider.AnalyzeDeclaredElementContainerElement(declaredElement) != null;
            }

#if DEBUG
            var message = DebugUtility.FormatIncludingContext(declaredElement) +
                          " => [" + string.Join(", ", highlightingList.Select(x => x.GetType().Name)) + "]";

            Logger.Verbose(DebugUtility.FormatWithElapsed(message, stopwatch));
#endif

            DelegateToIncorrectNullableAttributeUsageAnalyzer(declaration, data, consumer, hasOveriddenImplicitNullability);
        }

        private void CheckForParameterSuperMemberConflicts(
            IParameter parameter,
            IList<IAttributeInstance> attributeInstances,
            ITreeNode highlightingNode,
            ICollection<IHighlighting> highlightingList)
        {
            if (IsImplicitlyNotNull(parameter, attributeInstances))
            {
                var superMembersNullability = GetImmediateSuperMembersNullability(parameter).ToArray();

                if (parameter.IsInput() || parameter.IsRef())
                    CheckForInputOrRefElementSuperMemberConflicts(superMembersNullability, highlightingNode, highlightingList);

                if (parameter.IsOut())
                    CheckForOutputElementSuperMemberConflicts(superMembersNullability, highlightingNode, highlightingList);
            }
        }

        private void CheckForMethodSuperMemberConflicts(
            IMethod method,
            IList<IAttributeInstance> attributeInstances,
            ITreeNode highlightingNode,
            ICollection<IHighlighting> highlightingList)
        {
            // Handle async methods like explicit [NotNull] methods to avoid unnecessary false positives.
            // See Derived.CanBeNull_WithOverridingAsyncMethod() test case.

            if (IsImplicitlyNotNull(method, attributeInstances) && !method.IsAsync)
            {
                var superMembersNullability = GetImmediateSuperMembersNullability(method).ToArray();
                CheckForOutputElementSuperMemberConflicts(superMembersNullability, highlightingNode, highlightingList);
            }


            if (IsContainerElementImplicitlyNotNull(method, attributeInstances))
            {
                // Implicitly nullable Task<T> / async methods

                var superMembersNullability = GetImmediateSuperMembersContainerElementNullability(method).ToArray();
                CheckForOutputElementSuperMemberConflicts(superMembersNullability, highlightingNode, highlightingList);
            }
        }

        private static void CheckForInputOrRefElementSuperMemberConflicts(
            SuperMemberNullability[] superMembersNullability,
            ITreeNode highlightingNode,
            ICollection<IHighlighting> highlightingList)
        {
            if (ContainsCanBeNull(superMembersNullability))
                highlightingList.Add(new ImplicitNotNullConflictInHierarchyHighlighting(highlightingNode));

            if (ContainsUnknownNullability(superMembersNullability))
                highlightingList.Add(new ImplicitNotNullOverridesUnknownBaseMemberNullabilityHighlighting(highlightingNode));
        }

        private static void CheckForOutputElementSuperMemberConflicts(
            SuperMemberNullability[] superMembersNullability,
            ITreeNode highlightingNode,
            ICollection<IHighlighting> highlightingList)
        {
            if (ContainsCanBeNull(superMembersNullability))
                highlightingList.Add(new ImplicitNotNullElementCannotOverrideCanBeNullHighlighting(highlightingNode));

            if (ContainsUnknownNullability(superMembersNullability))
                highlightingList.Add(new ImplicitNotNullResultOverridesUnknownBaseMemberNullabilityHighlighting(highlightingNode));
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

        private static bool ContainsCanBeNull(IEnumerable<SuperMemberNullability> superMembersNullability)
        {
            return superMembersNullability.Any(x => x.NullableAttribute == CodeAnnotationNullableValue.CAN_BE_NULL);
        }

        private static bool ContainsUnknownNullability(IEnumerable<SuperMemberNullability> superMembersNullability)
        {
            return superMembersNullability.Any(x => x.NullableAttribute.IsUnknown());
        }

        private void CheckForNotNullOnImplicitCanBeNull(
            IDeclaredElement element,
            IList<IAttributeInstance> attributeInstances,
            ITreeNode highlightingNode,
            ICollection<IHighlighting> highlightingList)
        {
            if (_annotationAttributesChecker.ContainsExplicitNotNullNullabilityAttribute(attributeInstances) &&
                _implicitNullabilityProvider.AnalyzeDeclaredElement(element) == CodeAnnotationNullableValue.CAN_BE_NULL)
            {
                highlightingList.Add(new NotNullOnImplicitCanBeNullHighlighting(highlightingNode));
            }

            if (_annotationAttributesChecker.ContainsExplicitItemNotNullNullabilityAttribute(attributeInstances) &&
                _implicitNullabilityProvider.AnalyzeDeclaredElementContainerElement(element) == CodeAnnotationNullableValue.CAN_BE_NULL)
            {
                highlightingList.Add(new NotNullOnImplicitCanBeNullHighlighting(highlightingNode));
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

        private IEnumerable<SuperMemberNullability> GetImmediateSuperMembersNullability(IParameter parameter)
        {
            var overridableMember = parameter.ContainingParametersOwner as IOverridableMember;

            if (overridableMember == null)
                return EmptyList<SuperMemberNullability>.InstanceList;

            var superMembers = overridableMember.GetImmediateSuperMembers();

            var parameterIndex = parameter.IndexOf();

            var result =
                superMembers.Select(
                    x => new SuperMemberNullability
                    {
                        SuperMember = x.Element,
                        // We assume that the super members of a parameter owner are also parameter owners with the same number of parameters:
                        NullableAttribute = _nullnessProvider.GetInfo(((IParametersOwner) x.Element).Parameters[parameterIndex])
                    });

            return result;
        }

        private IEnumerable<SuperMemberNullability> GetImmediateSuperMembersNullability(IOverridableMember method)
        {
            return method.GetImmediateSuperMembers().Select(x => new SuperMemberNullability
            {
                SuperMember = x.Element,
                NullableAttribute = _nullnessProvider.GetInfo(x.Member)
            });
        }

        private IEnumerable<SuperMemberNullability> GetImmediateSuperMembersContainerElementNullability(IOverridableMember method)
        {
            return method.GetImmediateSuperMembers().Select(x => new SuperMemberNullability
            {
                SuperMember = x.Element,
                NullableAttribute = _containerElementNullnessProvider.GetInfo(x.Member)
            });
        }

        private struct SuperMemberNullability
        {
            public IOverridableMember SuperMember;
            public CodeAnnotationNullableValue? NullableAttribute;
        }
    }
}
