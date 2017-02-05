using System.Linq;
using ImplicitNullability.Plugin.Settings;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ImplicitNullability.Plugin.TypeHighlighting
{
    [ElementProblemAnalyzer(
        typeof(IRegularParameterDeclaration),
        typeof(IMethodDeclaration),
        typeof(IOperatorDeclaration),
        typeof(IDelegateDeclaration),
        typeof(IFieldDeclaration),
        typeof(IPropertyDeclaration),
        typeof(IIndexerDeclaration),
        HighlightingTypes = new[] { typeof(StaticNullabilityTypeHighlighting), typeof(StaticNullabilityItemTypeHighlighting) })]
    public sealed class TypeHighlightingProblemAnalyzer : ElementProblemAnalyzer<IDeclaration>
    {
        private readonly NullnessProvider _nullnessProvider;
        private readonly ContainerElementNullnessProvider _containerElementNullnessProvider;

        public TypeHighlightingProblemAnalyzer(CodeAnnotationsCache codeAnnotationsCache)
        {
            _nullnessProvider = codeAnnotationsCache.GetProvider<NullnessProvider>();
            _containerElementNullnessProvider = codeAnnotationsCache.GetProvider<ContainerElementNullnessProvider>();
        }

        protected override void Run(IDeclaration declaration, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            if (data.ProcessKind == DaemonProcessKind.VISIBLE_DOCUMENT && IsTypeHighlightingEnabled(data.SettingsStore))
                RunForVisibleDocument(declaration, consumer);
        }

        private void RunForVisibleDocument(IDeclaration declaration, IHighlightingConsumer consumer)
        {
            var parameterDeclaration = declaration as IRegularParameterDeclaration;
            if (parameterDeclaration?.DeclaredElement != null)
            {
                HandleElement(consumer, parameterDeclaration.DeclaredElement, parameterDeclaration.TypeUsage);
            }

            var methodDeclaration = declaration as IMethodDeclaration;
            if (methodDeclaration?.DeclaredElement != null)
            {
                var method = methodDeclaration.DeclaredElement;

                if (method.ReturnType.IsGenericTask())
                {
                    var userTypeUsage = (IUserTypeUsage) methodDeclaration.TypeUsage;

                    var typeIdentifier = userTypeUsage.ScalarTypeName.NameIdentifier;
                    var innerTypeUsage = userTypeUsage.ScalarTypeName.TypeArgumentList.TypeArgumentNodes.FirstOrDefault();

                    HandleElement(consumer, method, typeIdentifier);
                    HandleContainerElement(consumer, method, innerTypeUsage);
                }
                else
                {
                    HandleElement(consumer, method, methodDeclaration.TypeUsage);
                }
            }

            var operatorDeclaration = declaration as IOperatorDeclaration;
            if (operatorDeclaration != null)
            {
                HandleElement(consumer, operatorDeclaration.DeclaredElement, operatorDeclaration.TypeUsage);
            }

            var delegateDeclaration = declaration as IDelegateDeclaration;
            if (delegateDeclaration != null)
            {
                HandleElement(consumer, delegateDeclaration.DeclaredElement, delegateDeclaration.TypeUsage);
            }

            var fieldDeclaration = declaration as IFieldDeclaration;
            if (fieldDeclaration != null)
            {
                HandleElement(consumer, fieldDeclaration.DeclaredElement, fieldDeclaration.TypeUsage);
            }

            var propertyDeclaration = declaration as IPropertyDeclaration;
            if (propertyDeclaration != null)
            {
                HandleElement(consumer, propertyDeclaration.DeclaredElement, propertyDeclaration.TypeUsage);
            }

            var indexerDeclaration = declaration as IIndexerDeclaration;
            if (indexerDeclaration != null)
            {
                HandleElement(consumer, indexerDeclaration.DeclaredElement, indexerDeclaration.TypeUsage);
            }
        }

        private void HandleElement(
            IHighlightingConsumer consumer,
            [CanBeNull] IAttributesOwner element,
            [CanBeNull] ITreeNode typeNodeForHighlighting)
        {
            if (typeNodeForHighlighting != null && IsNotNull(element))
                consumer.AddHighlighting(new StaticNullabilityTypeHighlighting(typeNodeForHighlighting, "[NotNull]"));
        }

        private void HandleContainerElement(
            IHighlightingConsumer consumer,
            [CanBeNull] IAttributesOwner element,
            [CanBeNull] ITypeUsage typeNodeForHighlighting)
        {
            if (typeNodeForHighlighting != null && IsItemNotNull(element))
                consumer.AddHighlighting(new StaticNullabilityItemTypeHighlighting(typeNodeForHighlighting, "[ItemNotNull]"));
        }

        private bool IsTypeHighlightingEnabled(IContextBoundSettingsStore settingsStore) =>
            settingsStore.GetValue((ImplicitNullabilitySettings s) => s.EnableTypeHighlighting);

        private bool IsNotNull([CanBeNull] IAttributesOwner attributesOwner) =>
            _nullnessProvider.GetInfo(attributesOwner) == CodeAnnotationNullableValue.NOT_NULL;

        private bool IsItemNotNull([CanBeNull] IAttributesOwner attributesOwner) =>
            _containerElementNullnessProvider.GetInfo(attributesOwner) == CodeAnnotationNullableValue.NOT_NULL;
    }
}