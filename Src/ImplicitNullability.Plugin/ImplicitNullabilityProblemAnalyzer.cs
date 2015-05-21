using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ImplicitNullability.Plugin.Highlighting;
using ImplicitNullability.Plugin.Infrastructure;
using JetBrains.Annotations;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using JetBrains.Util.Logging;
#if RESHARPER8
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.Stages;
#else
using JetBrains.ReSharper.Feature.Services.Daemon;

#endif

namespace ImplicitNullability.Plugin
{
  [ElementProblemAnalyzer (
      typeof (IParameterDeclaration),
      HighlightingTypes =
          new[]
          {
              typeof (NotNullOnImplicitCanBeNullHighlighting),
              typeof (ImplicitNotNullConflictInHierarchyHighlighting),
              typeof (ImplicitNotNullOverridesUnknownExternalMemberHighlighting)
          })]
  public class ImplicitNullabilityProblemAnalyzer : IElementProblemAnalyzer
  {
    private static readonly ILogger s_logger = Logger.GetLogger (typeof (ImplicitNullabilityProblemAnalyzer));

    private readonly CodeAnnotationsCache _codeAnnotationsCache;
    private readonly ImplicitNullabilityProvider _implicitNullabilityProvider;

    public ImplicitNullabilityProblemAnalyzer (
        CodeAnnotationsCache codeAnnotationsCache,
        ImplicitNullabilityProvider implicitNullabilityProvider)
    {
      s_logger.LogMessage (LoggingLevel.INFO, ".ctor");

      _codeAnnotationsCache = codeAnnotationsCache;
      _implicitNullabilityProvider = implicitNullabilityProvider;
    }

    public void Run (ITreeNode element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
      var parameterDeclaration = element as IParameterDeclaration;

      if (parameterDeclaration != null && parameterDeclaration.DeclaredElement != null)
      {
#if DEBUG
        var stopwatch = Stopwatch.StartNew();
#endif
        var parameter = parameterDeclaration.DeclaredElement;

        var highlightingList = HandleParameter (parameterDeclaration, parameter).ToList();

        highlightingList.ForEach (x => consumer.AddHighlighting (x));

#if DEBUG
        string message = DebugUtilities.FormatIncludingContext (parameter) + " => ["
                         + string.Join (", ", highlightingList.Select (x => x.GetType().Name)) + "]";

        s_logger.LogMessage (LoggingLevel.VERBOSE, DebugUtilities.FormatWithElapsed (message, stopwatch));
#endif
      }
    }

    private IEnumerable<IHighlighting> HandleParameter ([NotNull] IParameterDeclaration parameterDeclaration, [NotNull] IParameter parameter)
    {
      var nullableAttributeMarks = GetNullableAttributeMarks (parameter).ToList();

      if (nullableAttributeMarks.Any (x => x == CodeAnnotationNullableValue.NOT_NULL)
          && _implicitNullabilityProvider.AnalyzeParameter (parameter) == CodeAnnotationNullableValue.CAN_BE_NULL)
      {
        yield return new NotNullOnImplicitCanBeNullHighlighting (parameterDeclaration);
      }

      var isImplicitlyNotNull = !nullableAttributeMarks.Any() &&
                                _implicitNullabilityProvider.AnalyzeParameter (parameter) == CodeAnnotationNullableValue.NOT_NULL;

      if (isImplicitlyNotNull)
      {
        var superMembersNullability = GetImmediateSuperMembersNullability (parameter).ToList();

        if (superMembersNullability.Any (x => x.NullableAttribute == CodeAnnotationNullableValue.CAN_BE_NULL))
          yield return new ImplicitNotNullConflictInHierarchyHighlighting (parameterDeclaration);

        if (superMembersNullability.Any (x => !x.SuperMember.IsPartOfSolutionCode() && x.NullableAttribute.IsUnknown()))
          yield return new ImplicitNotNullOverridesUnknownExternalMemberHighlighting (parameterDeclaration);
      }
    }

    /// <summary>
    /// Returns the direct (non-inherited) nullability attribute values of <paramref name="parameter"/>.
    /// </summary>
    private IEnumerable<CodeAnnotationNullableValue> GetNullableAttributeMarks ([NotNull] IParameter parameter)
    {
      return parameter.GetAttributeInstances (inherit: false)
          .Select (x => _codeAnnotationsCache.GetNullableAttributeMark (x)).Where (x => x != null).Select (x => x.Value);
    }

    private IEnumerable<SuperMemberNullability> GetImmediateSuperMembersNullability ([NotNull] IParameter parameter)
    {
      var overridableMember = parameter.ContainingParametersOwner as IOverridableMember;

      if (overridableMember == null)
        return new SuperMemberNullability[0];

      var superMembers = overridableMember.GetImmediateSuperMembers();

      var parameterIndex = parameter.IndexOf();

      var result =
          superMembers.Select (
              x => new SuperMemberNullability
                   {
                       SuperMember = x.Element,
                       // We assume that the super members of a parameter owner are also parameter owners with the same number of parameters:
                       NullableAttribute = _codeAnnotationsCache.GetNullableAttribute (((IParametersOwner) x.Element).Parameters[parameterIndex])
                   });

      return result;
    }

    private class SuperMemberNullability
    {
      public IOverridableMember SuperMember;
      public CodeAnnotationNullableValue? NullableAttribute;
    }
  }
}