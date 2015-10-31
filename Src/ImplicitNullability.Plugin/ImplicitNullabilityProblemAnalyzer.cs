using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ImplicitNullability.Plugin.Highlighting;
using ImplicitNullability.Plugin.Infrastructure;
using JetBrains.Annotations;
using JetBrains.Application.Components;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
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
        typeof (IMethodDeclaration), typeof (IParameterDeclaration), typeof (IFieldDeclaration),
        typeof (IPropertyDeclaration), typeof (IIndexerDeclaration), typeof (IOperatorDeclaration),
        // Extra:
        typeof (IDelegateDeclaration),
        HighlightingTypes =
            new[]
            {
                // From IncorrectNullableAttributeUsageAnalyzer:
                typeof (AnnotationRedundancyInHierarchyWarning),
                typeof (AnnotationConflictInHierarchyWarning),
                typeof (AnnotationRedundancyAtValueTypeWarning),
                typeof (MultipleNullableAttributesUsageWarning),
                // The own ones:
                typeof (NotNullOnImplicitCanBeNullHighlighting),
                typeof (ImplicitNotNullConflictInHierarchyHighlighting),
                typeof (ImplicitNotNullElementCannotOverrideCanBeNullHighlighting),
                typeof (ImplicitNotNullOverridesUnknownExternalMemberHighlighting),
                typeof (ImplicitNotNullResultOverridesUnknownExternalMemberHighlighting)
            })]
    public class ImplicitNullabilityProblemAnalyzer : ElementProblemAnalyzer<IDeclaration>,
        IHideImplementation<IncorrectNullableAttributeUsageAnalyzer>
    {
        private static readonly ILogger Logger = JetBrains.Util.Logging.Logger.GetLogger(typeof (ImplicitNullabilityProblemAnalyzer));

        private readonly CodeAnnotationsCache _codeAnnotationsCache;
        private readonly ImplicitNullabilityProvider _implicitNullabilityProvider;
        private readonly IncorrectNullableAttributeUsageAnalyzer _incorrectNullableAttributeUsageAnalyzer;

        public ImplicitNullabilityProblemAnalyzer(
            CodeAnnotationsCache codeAnnotationsCache,
            ImplicitNullabilityProvider implicitNullabilityProvider)
        {
            Logger.Verbose(".ctor");

            _codeAnnotationsCache = codeAnnotationsCache;
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

            var filterAnnotationRedundancyInHierarchyWarning = false;

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

                if (ContainsAnyExplicitNullabilityAttributes(attributeInstances) &&
                    _implicitNullabilityProvider.AnalyzeDeclaredElement(declaredElement) != null)
                {
                    filterAnnotationRedundancyInHierarchyWarning = true;
                }
            }

#if DEBUG
            var message = DebugUtility.FormatIncludingContext(declaredElement) +
                          " => [" + string.Join(", ", highlightingList.Select(x => x.GetType().Name)) + "]";

            Logger.Verbose(DebugUtility.FormatWithElapsed(message, stopwatch));
#endif

            DelegateToIncorrectNullableAttributeUsageAnalyzer(declaration, data, consumer, filterAnnotationRedundancyInHierarchyWarning);
        }

        private void CheckForParameterSuperMemberConflicts(
            [NotNull] IParameter parameter,
            [NotNull] IList<IAttributeInstance> attributeInstances,
            [NotNull] ITreeNode highlightingNode,
            [NotNull] ICollection<IHighlighting> highlightingList)
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
            [NotNull] IMethod method,
            [NotNull] IList<IAttributeInstance> attributeInstances,
            [NotNull] ITreeNode highlightingNode,
            [NotNull] ICollection<IHighlighting> highlightingList)
        {
            // Handle async methods like explicit [NotNull] methods to avoid unnecessary false positives.
            // See Derived.CanBeNull_WithOverridingAsyncMethod() test case.

            if (IsImplicitlyNotNull(method, attributeInstances) && !method.IsAsync)
            {
                var superMembersNullability = GetImmediateSuperMembersNullability(method).ToArray();
                CheckForOutputElementSuperMemberConflicts(superMembersNullability, highlightingNode, highlightingList);
            }

#if !RESHARPER91

            if (IsContainerElementImplicitlyNotNull(method, attributeInstances))
            {
                // Implicitly nullable Task<T> / async methods

                var superMembersNullability = GetImmediateSuperMembersContainerElementNullability(method).ToArray();
                CheckForOutputElementSuperMemberConflicts(superMembersNullability, highlightingNode, highlightingList);
            }
#endif
        }

        private static void CheckForInputOrRefElementSuperMemberConflicts(
            [NotNull] SuperMemberNullability[] superMembersNullability,
            [NotNull] ITreeNode highlightingNode,
            [NotNull] ICollection<IHighlighting> highlightingList)
        {
            if (ContainsCanBeNull(superMembersNullability))
                highlightingList.Add(new ImplicitNotNullConflictInHierarchyHighlighting(highlightingNode));

            if (ContainsExternalUnknownNullability(superMembersNullability))
                highlightingList.Add(new ImplicitNotNullOverridesUnknownExternalMemberHighlighting(highlightingNode));
        }

        private static void CheckForOutputElementSuperMemberConflicts(
            [NotNull] SuperMemberNullability[] superMembersNullability,
            [NotNull] ITreeNode highlightingNode,
            [NotNull] ICollection<IHighlighting> highlightingList)
        {
            if (ContainsCanBeNull(superMembersNullability))
                highlightingList.Add(new ImplicitNotNullElementCannotOverrideCanBeNullHighlighting(highlightingNode));

            if (ContainsExternalUnknownNullability(superMembersNullability))
                highlightingList.Add(new ImplicitNotNullResultOverridesUnknownExternalMemberHighlighting(highlightingNode));
        }

        private bool IsImplicitlyNotNull(
            [NotNull] IDeclaredElement declaredElement,
            [NotNull] IEnumerable<IAttributeInstance> attributeInstances)
        {
            return !ContainsAnyExplicitNullabilityAttributes(attributeInstances) &&
                   _implicitNullabilityProvider.AnalyzeDeclaredElement(declaredElement) == CodeAnnotationNullableValue.NOT_NULL;
        }

#if !RESHARPER91
        private bool IsContainerElementImplicitlyNotNull(
            [NotNull] IDeclaredElement declaredElement,
            [NotNull] IEnumerable<IAttributeInstance> attributeInstances)
        {
            return !ContainsAnyExplicitItemNullabilityAttributes(attributeInstances) &&
                   _implicitNullabilityProvider.AnalyzeDeclaredElementContainerElement(declaredElement) == CodeAnnotationNullableValue.NOT_NULL;
        }
#endif

        private static bool ContainsCanBeNull(IEnumerable<SuperMemberNullability> superMembersNullability)
        {
            return superMembersNullability.Any(x => x.NullableAttribute == CodeAnnotationNullableValue.CAN_BE_NULL);
        }

        private static bool ContainsExternalUnknownNullability(IEnumerable<SuperMemberNullability> superMembersNullability)
        {
            return superMembersNullability.Any(x => !x.SuperMember.IsPartOfSolutionCode() && x.NullableAttribute.IsUnknown());
        }

        private void CheckForNotNullOnImplicitCanBeNull(
            [NotNull] IDeclaredElement element,
            [NotNull] IList<IAttributeInstance> attributeInstances,
            [NotNull] ITreeNode highlightingNode,
            [NotNull] ICollection<IHighlighting> highlightingList)
        {
            if (ContainsExplicitNotNullNullabilityAttribute(attributeInstances) &&
                _implicitNullabilityProvider.AnalyzeDeclaredElement(element) == CodeAnnotationNullableValue.CAN_BE_NULL)
            {
                highlightingList.Add(new NotNullOnImplicitCanBeNullHighlighting(highlightingNode));
            }

#if !RESHARPER91
            if (ContainsExplicitItemNotNullNullabilityAttribute(attributeInstances) &&
                _implicitNullabilityProvider.AnalyzeDeclaredElementContainerElement(element) == CodeAnnotationNullableValue.CAN_BE_NULL)
            {
                highlightingList.Add(new NotNullOnImplicitCanBeNullHighlighting(highlightingNode));
            }
#endif
        }

        private void DelegateToIncorrectNullableAttributeUsageAnalyzer(
            [NotNull] ITreeNode declaration,
            [NotNull] ElementProblemAnalyzerData data,
            [NotNull] IHighlightingConsumer consumer,
            bool filterAnnotationRedundancyInHierarchyWarning)
        {
            if (filterAnnotationRedundancyInHierarchyWarning)
            {
                var consumerDecorator = new AnnotationRedundancyInHierarchyWarningFilteringDecorator(consumer);
                _incorrectNullableAttributeUsageAnalyzer.Run(declaration, data, consumerDecorator);
            }
            else
            {
                _incorrectNullableAttributeUsageAnalyzer.Run(declaration, data, consumer);
            }
        }

        private bool ContainsAnyExplicitNullabilityAttributes(IEnumerable<IAttributeInstance> attributeInstances)
        {
            return attributeInstances.Any(x => _codeAnnotationsCache.IsAnnotationAttribute(x, CodeAnnotationsCache.NotNullAttributeShortName) ||
                                               _codeAnnotationsCache.IsAnnotationAttribute(x, CodeAnnotationsCache.CanBeNullAttributeShortName));
        }

        private bool ContainsExplicitNotNullNullabilityAttribute(IEnumerable<IAttributeInstance> attributeInstances)
        {
            return attributeInstances.Any(x => _codeAnnotationsCache.IsAnnotationAttribute(x, CodeAnnotationsCache.NotNullAttributeShortName));
        }

#if !RESHARPER91
        private bool ContainsAnyExplicitItemNullabilityAttributes(IEnumerable<IAttributeInstance> attributeInstances)
        {
            return attributeInstances.Any(x => _codeAnnotationsCache.IsAnnotationAttribute(x, CodeAnnotationsCache.ItemNotNullAttributeShortName) ||
                                               _codeAnnotationsCache.IsAnnotationAttribute(x, CodeAnnotationsCache.ItemCanBeNullAttributeShortName));
        }

        private bool ContainsExplicitItemNotNullNullabilityAttribute(IEnumerable<IAttributeInstance> attributeInstances)
        {
            return attributeInstances.Any(x => _codeAnnotationsCache.IsAnnotationAttribute(x, CodeAnnotationsCache.ItemNotNullAttributeShortName));
        }
#endif

        private IEnumerable<SuperMemberNullability> GetImmediateSuperMembersNullability([NotNull] IParameter parameter)
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
                        NullableAttribute = _codeAnnotationsCache.GetNullableAttribute(((IParametersOwner) x.Element).Parameters[parameterIndex])
                    });

            return result;
        }

        private IEnumerable<SuperMemberNullability> GetImmediateSuperMembersNullability(IOverridableMember method)
        {
            return method.GetImmediateSuperMembers().Select(x => new SuperMemberNullability
            {
                SuperMember = x.Element,
                NullableAttribute = _codeAnnotationsCache.GetNullableAttribute(x.Member)
            });
        }

#if !RESHARPER91
        private IEnumerable<SuperMemberNullability> GetImmediateSuperMembersContainerElementNullability(IOverridableMember method)
        {
            return method.GetImmediateSuperMembers().Select(x => new SuperMemberNullability
            {
                SuperMember = x.Element,
                NullableAttribute = _codeAnnotationsCache.GetContainerElementNullableAttribute(x.Member)
            });
        }
#endif

        private struct SuperMemberNullability
        {
            public IOverridableMember SuperMember;
            public CodeAnnotationNullableValue? NullableAttribute;
        }
    }
}