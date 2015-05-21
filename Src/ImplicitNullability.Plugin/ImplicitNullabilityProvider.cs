using System;
using System.Linq.Expressions;
using ImplicitNullability.Plugin.Infrastructure;
using ImplicitNullability.Plugin.Settings;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Asp.Impl.Html;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;
using JetBrains.Util.Logging;

namespace ImplicitNullability.Plugin
{
  [PsiComponent]
  public class ImplicitNullabilityProvider
  {
    private static readonly ILogger s_logger = Logger.GetLogger (typeof (ImplicitNullabilityProvider));

    private readonly ISettingsStore _settingsStore;

    public ImplicitNullabilityProvider (ISettingsStore settingsStore)
    {
      _settingsStore = settingsStore;
    }

    public CodeAnnotationNullableValue? AnalyzeParameter ([NotNull] IParameter parameter)
    {
      CodeAnnotationNullableValue? result = null;

      if (parameter.IsPartOfSolutionCode() && IsOptionEnabled (parameter, s => s.EnableInputAndRefParameters))
      {
        if (IsInputOrRefParameterOfRegularMethodOrIndexer (parameter))
        {
          if (parameter.Type.IsValueType())
          {
            // Nullable or non-nullable value type

            if (parameter.Type.IsNullable())
              result = CodeAnnotationNullableValue.CAN_BE_NULL;
          }
          else
          {
            // Reference/pointer type, generic method without value type/reference type constraint

            if (IsOptionalArgumentWithNullDefaultValue (parameter))
              result = CodeAnnotationNullableValue.CAN_BE_NULL;
            else
              result = CodeAnnotationNullableValue.NOT_NULL;
          }
        }
      }

      return result;
    }

    private bool IsInputOrRefParameterOfRegularMethodOrIndexer ([NotNull] IParameter parameter)
    {
      return IsInputOrRefParameter (parameter) && IsImplicitNullabilityApplicableToParameter (parameter);
    }

    private bool IsImplicitNullabilityApplicableToParameter ([NotNull] IParameter parameter)
    {
      var parametersOwner = parameter.ContainingParametersOwner;

      //Using declarations:
      ////return parameter.GetDeclarations().SingleOrDefault() is IRegularParameterDeclaration &&
      ////       (parametersOwner != null && IsParametersOwnerNotSynthetic (parametersOwner));

      // IFunction includes methods, constructors, operator overloads, delegates, but also implicitly defined ASP.NET methods, e.g. Bind(), which
      // we want to exclude, because the developer cannot override the implicit annotation.
      var isParametersOwnerRegularFunctionOrIndexer = (parametersOwner is IFunction && !(parametersOwner is AspImplicitTypeMember)) ||
                                                      parametersOwner is IProperty;

      return isParametersOwnerRegularFunctionOrIndexer && IsParametersOwnerNotSynthetic (parametersOwner);
    }

    private bool IsParametersOwnerNotSynthetic ([NotNull] IParametersOwner containingParametersOwner)
    {
      // Exclude ReSharper's fake (called "synthetic") parameter owners (methods), like ASP.NET WebForms' Render-method, or Razor's Write-methods,
      // because these look like regular project methods but should be excluded from the implicit nullability annotations because the developer
      // cannot override the result of the implicit nullability:

      return !containingParametersOwner.IsSynthetic();
    }

    private static bool IsInputOrRefParameter ([NotNull] IParameter parameter)
    {
      return parameter.Kind != ParameterKind.OUTPUT;
    }

    private bool IsOptionEnabled ([NotNull] IParameter element, [NotNull] Expression<Func<ImplicitNullabilitySettings, bool>> optionExpression)
    {
      var contextRange = ContextRange.Smart (element.Module.ToDataContext());

      var enabled = _settingsStore.BindToContextTransient (contextRange).GetValue ((ImplicitNullabilitySettings s) => s.Enabled);

      return enabled && _settingsStore.BindToContextTransient (contextRange).GetValue (optionExpression);
    }

    private static bool IsOptionalArgumentWithNullDefaultValue ([NotNull] IParameter parameter)
    {
#if DEBUG
      if (parameter.IsOptional)
      {
        var optionalParameterText = "OptionalParameter IsConstant: " + parameter.GetDefaultValue().IsConstant +
                                    ", ConstantValue.Value: " + (parameter.GetDefaultValue().ConstantValue.Value ?? "NULL") +
                                    ", IsDefaultType: " + parameter.GetDefaultValue().IsDefaultType +
                                    ", DefaultTypeValue.IsValueType(): " + parameter.GetDefaultValue().DefaultTypeValue.IsValueType();

        s_logger.LogMessage (LoggingLevel.VERBOSE, optionalParameterText);
      }
#endif

      return parameter.IsOptional && IsNullDefaultValue (parameter.GetDefaultValue());
    }

    private static bool IsNullDefaultValue ([NotNull] DefaultValue defaultValue)
    {
      // Note that for "param = null" and "param = default(string)", the ConstantValue cannot be trusted; we therefore must check IsDefaultType:
      var isNullDefaultExpression = defaultValue.IsDefaultType && !defaultValue.DefaultTypeValue.IsValueType();

      // For "param = StringConstantMember" this check is necessary:
      var isNullConstantExpression = defaultValue.IsConstant && defaultValue.ConstantValue.Value == null;

      return isNullDefaultExpression || isNullConstantExpression;
    }
  }
}