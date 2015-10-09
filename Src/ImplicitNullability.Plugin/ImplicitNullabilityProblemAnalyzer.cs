using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ImplicitNullability.Plugin.Highlighting;
using ImplicitNullability.Plugin.Infrastructure;
using JetBrains.Annotations;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Daemon.Stages.Utils;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.ControlFlow;
using JetBrains.ReSharper.Psi.CSharp.ControlFlow;
using JetBrains.ReSharper.Psi.CSharp.Impl.ControlFlow;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using ReSharperExtensionsShared.Debugging;
#if RESHARPER8
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.Stages;

#else
using JetBrains.ReSharper.Feature.Services.Daemon;

#endif

namespace ImplicitNullability.Plugin
{
    [ElementProblemAnalyzer(
        typeof (IParameterDeclaration), typeof (IMethodDeclaration), typeof (IOperatorDeclaration), typeof (IDelegateDeclaration),
        HighlightingTypes =
            new[]
            {
                typeof (NotNullOnImplicitCanBeNullHighlighting),
                typeof (ImplicitNotNullConflictInHierarchyHighlighting),
                typeof (ImplicitNotNullOverridesUnknownExternalMemberHighlighting)
            })]
    public class ImplicitNullabilityProblemAnalyzer : ElementProblemAnalyzer<IDeclaration>
    {
        private static readonly ILogger Logger = JetBrains.Util.Logging.Logger.GetLogger(typeof (ImplicitNullabilityProblemAnalyzer));

        private readonly CodeAnnotationsCache _codeAnnotationsCache;
        private readonly ImplicitNullabilityProvider _implicitNullabilityProvider;

        public ImplicitNullabilityProblemAnalyzer(
            CodeAnnotationsCache codeAnnotationsCache,
            ImplicitNullabilityProvider implicitNullabilityProvider)
        {
            Logger.Verbose(".ctor");

            _codeAnnotationsCache = codeAnnotationsCache;
            _implicitNullabilityProvider = implicitNullabilityProvider;
        }

        protected override void Run(IDeclaration element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
#if DEBUG
            var stopwatch = Stopwatch.StartNew();
#endif

            var highlightingList = new List<IHighlighting>();

            var parameterDeclaration = element as IParameterDeclaration;
            if (parameterDeclaration?.DeclaredElement != null)
                highlightingList.AddRange(HandleParameter(parameterDeclaration, parameterDeclaration.DeclaredElement));

            var methodDeclaration = element as IMethodDeclaration;
            if (methodDeclaration?.DeclaredElement != null)
                highlightingList.AddRange(HandleFunction(methodDeclaration.NameIdentifier, methodDeclaration.DeclaredElement, methodDeclaration, data));

            var operatorDeclaration = element as IOperatorDeclaration;
            if (operatorDeclaration?.DeclaredElement != null)
                highlightingList.AddRange(HandleFunction(operatorDeclaration.OperatorKeyword, operatorDeclaration.DeclaredElement, operatorDeclaration, data));

            var delegateDeclaration = element as IDelegateDeclaration;
            if (delegateDeclaration?.DeclaredElement != null)
                highlightingList.AddRange(HandleDelegate(delegateDeclaration, delegateDeclaration.DeclaredElement));

            highlightingList.ForEach(x => consumer.AddHighlighting(x));

#if DEBUG

            var message = DebugUtility.FormatIncludingContext(element.DeclaredElement) +
                          " => [" + string.Join(", ", highlightingList.Select(x => x.GetType().Name)) + "]";

            Logger.Verbose(DebugUtility.FormatWithElapsed(message, stopwatch));
#endif
        }

        private IEnumerable<IHighlighting> HandleParameter([NotNull] IParameterDeclaration parameterDeclaration, [NotNull] IParameter parameter)
        {
            var nullableAttributeMarks = GetNullableAttributeMarks(parameter).ToArray();

            if (nullableAttributeMarks.Any(x => x == CodeAnnotationNullableValue.NOT_NULL)
                && _implicitNullabilityProvider.AnalyzeParameter(parameter) == CodeAnnotationNullableValue.CAN_BE_NULL)
            {
                yield return new NotNullOnImplicitCanBeNullHighlighting(parameterDeclaration);
            }

            var isImplicitlyNotNull = !nullableAttributeMarks.Any() &&
                                      _implicitNullabilityProvider.AnalyzeParameter(parameter) == CodeAnnotationNullableValue.NOT_NULL;

            if (isImplicitlyNotNull && parameter.IsInputOrRef())
            {
                var superMembersNullability = GetImmediateSuperMembersNullability(parameter).ToArray();

                if (superMembersNullability.Any(x => x.NullableAttribute == CodeAnnotationNullableValue.CAN_BE_NULL))
                    yield return new ImplicitNotNullConflictInHierarchyHighlighting(parameterDeclaration);

                if (superMembersNullability.Any(x => !x.SuperMember.IsPartOfSolutionCode() && x.NullableAttribute.IsUnknown()))
                    yield return new ImplicitNotNullOverridesUnknownExternalMemberHighlighting(parameterDeclaration);
            }
        }

        private IEnumerable<IHighlighting> HandleFunction([NotNull] ITreeNode declaration, [NotNull] IFunction function, IFunctionDeclaration methodDeclaration, ElementProblemAnalyzerData data)
        {
            var nullableAttributeMarks = GetNullableAttributeMarks(function);

            if (nullableAttributeMarks.Any(x => x == CodeAnnotationNullableValue.NOT_NULL)
                && _implicitNullabilityProvider.AnalyzeFunction(function) == CodeAnnotationNullableValue.CAN_BE_NULL)
            {
                yield return new NotNullOnImplicitCanBeNullHighlighting(declaration);
            }

            // TODO: handle async methods (ItemNotNull/ItemCanBeNull)
            if (!nullableAttributeMarks.Any() &&
                _implicitNullabilityProvider.AnalyzeFunction(function) == CodeAnnotationNullableValue.NOT_NULL)
            {
                var cSharpControlFlowGraph = ControlFlowDaemonUtil.GetOrBuild(methodDeclaration, data) as ICSharpControlFlowGraph;

                if (cSharpControlFlowGraph != null)
                {
                    var analysisResult = CSharpControlFlowGraphInspector.Inspect(cSharpControlFlowGraph, ValueAnalysisMode.OPTIMISTIC, node => ControlFlowDaemonUtil.GetOrBuild(node, data) as ICSharpControlFlowGraph);

                    //var type = analysisResult.GetType();
                    //var getVariableStateAt = type.GetMethod("GetVariableStateAt",
                    //    BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(IControlFlowElement), typeof(VariableInfo), typeof(bool), typeof(bool) }, null);
                    //var findInfoByExpression = type.GetMethod("FindInfoByExpression", BindingFlags.Instance | BindingFlags.NonPublic);

                    //return this.GetVariableStateAt(


                    //    Enumerable.FirstOrDefault<IControlFlowElement>(


                    //        this.ControlFlowGraph.GetLeafElementsFor((ITreeNode)expression)), 



                    //    this.FindInfoByExpression((ICSharpExpression)expression), 


                    //    false, 

                    //    includeWeakNotNull);

                    foreach (var node in methodDeclaration.ThisAndDescendants<IReturnStatement>().ToEnumerable())
                    {
                        //var cSharpControlFlowNullReferenceState =
                        //    (CSharpControlFlowNullReferenceState)
                        //        getVariableStateAt.Invoke(analysisResult, new object[]
                        //        {
                        //            cSharpControlFlowGraph.GetLeafElementsFor(node.Value).FirstOrDefault(),
                        //            findInfoByExpression.Invoke(analysisResult, new object[ ] {node.Value}),
                        //            false,
                        //            true
                        //        });





                        //CSharpControlFlowNullReferenceState cSharpControlFlowNullReferenceState = analysisResult.GetExpressionNullReferenceState((IReferenceExpression)node.Value, true);
                        if (analysisResult.SuggestReturnValueAnnotationAttribute == CSharpControlFlowNullReferenceState.UNKNOWN)
                        {
                            yield return new LoooolHighlighting(node);
                        }

                    }
                }
            }
        }

        private IEnumerable<IHighlighting> HandleDelegate([NotNull] IDelegateDeclaration delegateDeclaration, [NotNull] IDelegate @delegate)
        {
            var nullableAttributeMarks = GetNullableAttributeMarks(@delegate);

            if (nullableAttributeMarks.Any(x => x == CodeAnnotationNullableValue.NOT_NULL)
                && _implicitNullabilityProvider.AnalyzeDelegate(@delegate) == CodeAnnotationNullableValue.CAN_BE_NULL)
            {
                yield return new NotNullOnImplicitCanBeNullHighlighting(delegateDeclaration.NameIdentifier);
            }
        }

        /// <summary>
        /// Returns the direct (non-inherited) nullability attribute values of <paramref name="owner"/>.
        /// </summary>
        private IEnumerable<CodeAnnotationNullableValue> GetNullableAttributeMarks([NotNull] IAttributesSet owner)
        {
            return owner.GetAttributeInstances(inherit: false)
                .Select(x => _codeAnnotationsCache.GetNullableAttributeMark(x)).Where(x => x != null).Select(x => x.Value);
        }

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

        private struct SuperMemberNullability
        {
            public IOverridableMember SuperMember;
            public CodeAnnotationNullableValue? NullableAttribute;
        }
    }
}