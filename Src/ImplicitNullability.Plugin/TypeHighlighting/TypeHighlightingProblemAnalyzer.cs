using System.Linq;
using ImplicitNullability.Plugin.Configuration;
using ImplicitNullability.Plugin.Infrastructure;
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
            var processKind = data.GetDaemonProcessKind();

            if (processKind == DaemonProcessKind.VISIBLE_DOCUMENT && IsTypeHighlightingEnabled(data.SettingsStore))
                RunForVisibleDocument(declaration, consumer);
        }

        private void RunForVisibleDocument(IDeclaration declaration, IHighlightingConsumer consumer)
        {
            if (declaration is IRegularParameterDeclaration parameterDeclaration)
            {
                HandleElement(consumer, parameterDeclaration.DeclaredElement, parameterDeclaration.TypeUsage);
            }

            var methodDeclaration = declaration as IMethodDeclaration;
            if (methodDeclaration?.DeclaredElement != null)
            {
                HandleMethod(consumer, methodDeclaration, methodDeclaration.DeclaredElement);
            }

            if (declaration is IOperatorDeclaration operatorDeclaration)
            {
                HandleElement(consumer, operatorDeclaration.DeclaredElement, operatorDeclaration.TypeUsage);
            }

            var delegateDeclaration = declaration as IDelegateDeclaration;
            if (delegateDeclaration?.DeclaredElement != null)
            {
                HandleDelegate(consumer, delegateDeclaration, delegateDeclaration.DeclaredElement);
            }

            if (declaration is IFieldDeclaration fieldDeclaration)
            {
                HandleElement(consumer, fieldDeclaration.DeclaredElement, fieldDeclaration.TypeUsage);
            }

            if (declaration is IPropertyDeclaration propertyDeclaration)
            {
                HandleElement(consumer, propertyDeclaration.DeclaredElement, propertyDeclaration.TypeUsage);
            }

            if (declaration is IIndexerDeclaration indexerDeclaration)
            {
                HandleElement(consumer, indexerDeclaration.DeclaredElement, indexerDeclaration.TypeUsage);
            }
        }

        private void HandleMethod(IHighlightingConsumer consumer, IMethodDeclaration methodDeclaration, IMethod method)
        {
            if (method.ReturnType.IsGenericTask())
            {
                var typeUsageOfTask = (IUserTypeUsage) methodDeclaration.TypeUsage;
                HandleGenericTaskTypes(consumer, method, typeUsageOfTask);
            }
            else
            {
                // The following check is a workaround for R#'s CSharpCodeAnnotationProvider returning NOT_NULL also for async void methods
                if (!method.IsAsyncVoid())
                {
                    HandleElement(consumer, method, methodDeclaration.TypeUsage);
                }
            }
        }

        private void HandleDelegate(IHighlightingConsumer consumer, IDelegateDeclaration delegateDeclaration, IDelegate @delegate)
        {
            if (@delegate.InvokeMethod.ReturnType.IsGenericTask())
            {
                var typeUsageOfTask = (IUserTypeUsage) delegateDeclaration.TypeUsage;
                HandleGenericTaskTypes(consumer, @delegate, typeUsageOfTask);
            }
            else
            {
                HandleElement(consumer, @delegate, delegateDeclaration.TypeUsage);
            }
        }

        private void HandleGenericTaskTypes(IHighlightingConsumer consumer, ITypeMember @delegate, IUserTypeUsage typeUsageOfTask)
        {
            var typeIdentifier = typeUsageOfTask.ScalarTypeName.NameIdentifier;
            var innerTypeUsage = typeUsageOfTask.ScalarTypeName.TypeArgumentList.TypeArgumentNodes.FirstOrDefault();

            HandleElement(consumer, @delegate, typeIdentifier);
            HandleContainerElement(consumer, @delegate, innerTypeUsage);
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
