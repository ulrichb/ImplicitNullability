using System;
using System.Linq.Expressions;
using ImplicitNullability.Plugin.Infrastructure;
using ImplicitNullability.Plugin.Settings;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Asp.Impl.Html;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Impl.Special;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;
using JetBrains.Util.Logging;

namespace ImplicitNullability.Plugin
{
    [PsiComponent]
    public class ImplicitNullabilityProvider
    {
        private static readonly ILogger s_logger = Logger.GetLogger(typeof (ImplicitNullabilityProvider));

        private readonly ISettingsStore _settingsStore;

        public ImplicitNullabilityProvider(ISettingsStore settingsStore)
        {
            _settingsStore = settingsStore;
        }

        public CodeAnnotationNullableValue? AnalyzeParameter([NotNull] IParameter parameter)
        {
            CodeAnnotationNullableValue? result = null;

            if (parameter.IsPartOfSolutionCode() && IsImplicitNullabilityApplicableToParameter(parameter))
            {
                if (parameter.IsInput() && IsOptionEnabled(parameter, s => s.EnableInputParameters))
                {
                    if (IsOptionalArgumentWithNullDefaultValue(parameter))
                        result = CodeAnnotationNullableValue.CAN_BE_NULL;
                    else
                        result = GetNullabilityForType(parameter.Type);
                }

                if (parameter.IsRef() && IsOptionEnabled(parameter, s => s.EnableRefParameters))
                    result = GetNullabilityForType(parameter.Type);

                if (parameter.IsOut() && IsOptionEnabled(parameter, s => s.EnableOutParametersAndResult))
                    result = GetNullabilityForType(parameter.Type);
            }

            return result;
        }

        public CodeAnnotationNullableValue? AnalyzeFunction([NotNull] IFunction method)
        {
            // Methods and operators

            CodeAnnotationNullableValue? result = null;

            if (!(method is DelegateMethod))
            {
                if (method.IsPartOfSolutionCode() && IsOptionEnabled(method, s => s.EnableOutParametersAndResult))
                {
                    result = GetNullabilityForType(method.ReturnType);
                }
            }

            return result;
        }

        public CodeAnnotationNullableValue? AnalyzeDelegate([NotNull] IDelegate @delegate)
        {
            CodeAnnotationNullableValue? result = null;

            if (@delegate.IsPartOfSolutionCode() && IsOptionEnabled(@delegate, s => s.EnableOutParametersAndResult))
            {
                result = GetNullabilityForType(@delegate.InvokeMethod.ReturnType);
            }

            return result;
        }

        private static CodeAnnotationNullableValue? GetNullabilityForType([NotNull] IType type)
        {
            if (type.IsValueType())
            {
                // Nullable or non-nullable value type

                if (type.IsNullable())
                    return CodeAnnotationNullableValue.CAN_BE_NULL;
            }
            else
            {
                // Reference/pointer type, generic method without value type/reference type constraint

                return CodeAnnotationNullableValue.NOT_NULL;
            }

            return null;
        }

        private bool IsImplicitNullabilityApplicableToParameter([NotNull] IParameter parameter)
        {
            var parametersOwner = parameter.ContainingParametersOwner;

            //Using declarations:
            ////return parameter.GetDeclarations().SingleOrDefault() is IRegularParameterDeclaration &&
            ////       (parametersOwner != null && IsParametersOwnerNotSynthetic (parametersOwner));

            // IFunction includes methods, constructors, operator overloads, delegates, but also implicitly defined ASP.NET methods, e.g. Bind(), which
            // we want to exclude, because the developer cannot override the implicit annotation.
            var isParametersOwnerRegularFunctionOrIndexer = (parametersOwner is IFunction && !(parametersOwner is AspImplicitTypeMember)) ||
                                                            parametersOwner is IProperty;

            return isParametersOwnerRegularFunctionOrIndexer && IsParametersOwnerNotSynthetic(parametersOwner);
        }

        private bool IsParametersOwnerNotSynthetic([NotNull] IParametersOwner containingParametersOwner)
        {
            // Exclude ReSharper's fake (called "synthetic") parameter owners (methods), like ASP.NET WebForms' Render-method, or Razor's Write-methods,
            // because these look like regular project methods but should be excluded from the implicit nullability annotations because the developer
            // cannot override the result of the implicit nullability:

            return !containingParametersOwner.IsSynthetic();
        }

        private bool IsOptionEnabled([NotNull] IClrDeclaredElement element,
            [NotNull] Expression<Func<ImplicitNullabilitySettings, bool>> optionExpression)
        {
            var contextRange = ContextRange.Smart(element.Module.ToDataContext());

            var enabled = _settingsStore.BindToContextTransient(contextRange).GetValue((ImplicitNullabilitySettings s) => s.Enabled);

            return enabled && _settingsStore.BindToContextTransient(contextRange).GetValue(optionExpression);
        }

        private static bool IsOptionalArgumentWithNullDefaultValue([NotNull] IParameter parameter)
        {
#if DEBUG
            if (parameter.IsOptional)
            {
                var optionalParameterText = "OptionalParameter IsConstant: " + parameter.GetDefaultValue().IsConstant +
                                            ", ConstantValue.Value: " + (parameter.GetDefaultValue().ConstantValue.Value ?? "NULL") +
                                            ", IsDefaultType: " + parameter.GetDefaultValue().IsDefaultType +
                                            ", DefaultTypeValue.IsValueType(): " + parameter.GetDefaultValue().DefaultTypeValue.IsValueType();

                s_logger.LogMessage(LoggingLevel.VERBOSE, optionalParameterText);
            }
#endif

            return parameter.IsOptional && IsNullDefaultValue(parameter.GetDefaultValue());
        }

        private static bool IsNullDefaultValue([NotNull] DefaultValue defaultValue)
        {
            // Note that for "param = null" and "param = default(string)", the ConstantValue cannot be trusted; we therefore must check IsDefaultType:
            var isNullDefaultExpression = defaultValue.IsDefaultType && !defaultValue.DefaultTypeValue.IsValueType();

            // For "param = StringConstantMember" this check is necessary:
            var isNullConstantExpression = defaultValue.IsConstant && defaultValue.ConstantValue.Value == null;

            return isNullDefaultExpression || isNullConstantExpression;
        }
    }
}