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
                typeof(ImplicitNotNullOverridesUnknownExternalMemberHighlighting),
                typeof(ImplicitNotNullResultOverridesUnknownExternalMemberHighlighting)
            })]
    public class ImplicitNullabilityProblemAnalyzer : ElementProblemAnalyzer<IDeclaration>,
        IHideImplementation<IncorrectNullableAttributeUsageAnalyzer>
    {
        private static readonly ILogger Logger = JetBrains.Util.Logging.Logger.GetLogger(typeof(ImplicitNullabilityProblemAnalyzer));

        private readonly ImplicitNullabilityProvider _implicitNullabilityProvider;
        private readonly IncorrectNullableAttributeUsageAnalyzer _incorrectNullableAttributeUsageAnalyzer;
        private readonly NullabilityProvider _nullabilityProvider;

        public ImplicitNullabilityProblemAnalyzer(
            NullabilityProvider nullabilityProvider,
            CodeAnnotationsCache codeAnnotationsCache,
            ImplicitNullabilityProvider implicitNullabilityProvider)
        {
            Logger.Verbose(".ctor");

            _nullabilityProvider = nullabilityProvider;
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
                var attributeInstances = attributesSet.GetAttributeInstances(inherit: false);

                var parameterDeclaration = declaration as IParameterDeclaration;
                if (parameterDeclaration?.DeclaredElement != null)
                {
                    var parameter = parameterDeclaration.DeclaredElement;
                    CheckForNotNullOnImplicitCanBeNull(parameter, attributeInstances, parameterDeclaration, highlightingList);
                    CheckForParameterSuperMemberConflicts(parameter, attributeInstances, parameterDeclaration, highlightingList);
                }

                var methodDeclaration = declaration as IMethodDeclaration;
                if (methodDeclaration?.DeclaredElement != null)
                {
                    var method = methodDeclaration.DeclaredElement;
                    CheckForNotNullOnImplicitCanBeNull(method, attributeInstances, methodDeclaration.NameIdentifier, highlightingList);
                    CheckForMethodSuperMemberConflicts(method, attributeInstances, methodDeclaration.NameIdentifier, highlightingList);
                }

                var operatorDeclaration = declaration as IOperatorDeclaration;
                if (operatorDeclaration != null)
                {
                    var operatorKeyword = operatorDeclaration.OperatorKeyword;
                    CheckForNotNullOnImplicitCanBeNull(declaredElement, attributeInstances, operatorKeyword, highlightingList);
                }

                var delegateDeclaration = declaration as IDelegateDeclaration;
                if (delegateDeclaration != null)
                {
                    var nameIdentifier = delegateDeclaration.NameIdentifier;
                    CheckForNotNullOnImplicitCanBeNull(declaredElement, attributeInstances, nameIdentifier, highlightingList);
                }

                highlightingList.ForEach(x => consumer.AddHighlighting(x));

                hasOveriddenImplicitNullability |=
                    _nullabilityProvider.ContainsAnyExplicitNullabilityAttributes(attributeInstances) &&
                    _implicitNullabilityProvider.AnalyzeDeclaredElement(declaredElement) != null;

                hasOveriddenImplicitNullability |=
                    _nullabilityProvider.ContainsAnyExplicitItemNullabilityAttributes(attributeInstances) &&
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

            if (ContainsExternalUnknownNullability(superMembersNullability))
                highlightingList.Add(new ImplicitNotNullOverridesUnknownExternalMemberHighlighting(highlightingNode));
        }

        private static void CheckForOutputElementSuperMemberConflicts(
            SuperMemberNullability[] superMembersNullability,
            ITreeNode highlightingNode,
            ICollection<IHighlighting> highlightingList)
        {
            if (ContainsCanBeNull(superMembersNullability))
                highlightingList.Add(new ImplicitNotNullElementCannotOverrideCanBeNullHighlighting(highlightingNode));

            if (ContainsExternalUnknownNullability(superMembersNullability))
                highlightingList.Add(new ImplicitNotNullResultOverridesUnknownExternalMemberHighlighting(highlightingNode));
        }

        private bool IsImplicitlyNotNull(IDeclaredElement declaredElement, IEnumerable<IAttributeInstance> attributeInstances)
        {
            return !_nullabilityProvider.ContainsAnyExplicitNullabilityAttributes(attributeInstances) &&
                   _implicitNullabilityProvider.AnalyzeDeclaredElement(declaredElement) == CodeAnnotationNullableValue.NOT_NULL;
        }

        private bool IsContainerElementImplicitlyNotNull(IDeclaredElement declaredElement, IEnumerable<IAttributeInstance> attributeInstances)
        {
            return !_nullabilityProvider.ContainsAnyExplicitItemNullabilityAttributes(attributeInstances) &&
                   _implicitNullabilityProvider.AnalyzeDeclaredElementContainerElement(declaredElement) == CodeAnnotationNullableValue.NOT_NULL;
        }

        private static bool ContainsCanBeNull(IEnumerable<SuperMemberNullability> superMembersNullability)
        {
            return superMembersNullability.Any(x => x.NullableAttribute == CodeAnnotationNullableValue.CAN_BE_NULL);
        }

        private static bool ContainsExternalUnknownNullability(IEnumerable<SuperMemberNullability> superMembersNullability)
        {
            return superMembersNullability.Any(x => !x.SuperMember.IsPartOfSolutionCode() && x.NullableAttribute.IsUnknown());
        }

        private void CheckForNotNullOnImplicitCanBeNull(
            IDeclaredElement element,
            IList<IAttributeInstance> attributeInstances,
            ITreeNode highlightingNode,
            ICollection<IHighlighting> highlightingList)
        {
            if (_nullabilityProvider.ContainsExplicitNotNullNullabilityAttribute(attributeInstances) &&
                _implicitNullabilityProvider.AnalyzeDeclaredElement(element) == CodeAnnotationNullableValue.CAN_BE_NULL)
            {
                highlightingList.Add(new NotNullOnImplicitCanBeNullHighlighting(highlightingNode));
            }

            if (_nullabilityProvider.ContainsExplicitItemNotNullNullabilityAttribute(attributeInstances) &&
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
                        NullableAttribute = _nullabilityProvider.GetElementNullability(((IParametersOwner) x.Element).Parameters[parameterIndex])
                    });

            return result;
        }

        private IEnumerable<SuperMemberNullability> GetImmediateSuperMembersNullability(IOverridableMember method)
        {
            return method.GetImmediateSuperMembers().Select(x => new SuperMemberNullability
            {
                SuperMember = x.Element,
                NullableAttribute = _nullabilityProvider.GetElementNullability(x.Member)
            });
        }

        private IEnumerable<SuperMemberNullability> GetImmediateSuperMembersContainerElementNullability(IOverridableMember method)
        {
            return method.GetImmediateSuperMembers().Select(x => new SuperMemberNullability
            {
                SuperMember = x.Element,
                NullableAttribute = _nullabilityProvider.GetContainerElementNullability(x.Member)
            });
        }

        private struct SuperMemberNullability
        {
            public IOverridableMember SuperMember;
            public CodeAnnotationNullableValue? NullableAttribute;
        }
    }
}