using ImplicitNullability.Plugin.Infrastructure;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Asp.Impl.Html;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;
using static JetBrains.ReSharper.Psi.DeclaredElementConstants;

namespace ImplicitNullability.Plugin
{
    /// <summary>
    /// Calculates the nullability value of the different PSI elements => implements the Implicit Nullability rules.
    /// </summary>
    [PsiComponent]
    public class ImplicitNullabilityProvider
    {
        private static readonly ILogger Logger = JetBrains.Util.Logging.Logger.GetLogger(typeof(ImplicitNullabilityProvider));

        private readonly ImplicitNullabilityConfigurationEvaluator _configurationEvaluator;
        private readonly CodeAnnotationAttributesChecker _codeAnnotationAttributesChecker;

        public ImplicitNullabilityProvider(
            ImplicitNullabilityConfigurationEvaluator configurationEvaluator,
            CodeAnnotationAttributesChecker codeAnnotationAttributesChecker
        )
        {
            _configurationEvaluator = configurationEvaluator;
            _codeAnnotationAttributesChecker = codeAnnotationAttributesChecker;
        }

        public CodeAnnotationNullableValue? AnalyzeDeclaredElement([CanBeNull] IDeclaredElement declaredElement)
        {
            CodeAnnotationNullableValue? result = null;

            var parameter = declaredElement as IParameter;
            if (parameter != null)
                result = AnalyzeParameter(parameter);

            var function = declaredElement as IFunction;
            if (function != null)
                result = AnalyzeFunction(function);

            var @delegate = declaredElement as IDelegate;
            if (@delegate != null)
                result = AnalyzeDelegate(@delegate);

            return result;
        }

        private CodeAnnotationNullableValue? AnalyzeParameter(IParameter parameter)
        {
            CodeAnnotationNullableValue? result = null;

            if (IsImplicitNullabilityApplicableToParameter(parameter))
            {
                var configuration = _configurationEvaluator.EvaluateFor(parameter.Module);

                if (parameter.IsInput() && configuration.EnableInputParameters)
                {
                    if (IsOptionalArgumentWithNullDefaultValue(parameter))
                        result = CodeAnnotationNullableValue.CAN_BE_NULL;
                    else
                        result = GetNullabilityForType(parameter.Type);
                }

                if (parameter.IsRef() && configuration.EnableRefParameters)
                    result = GetNullabilityForType(parameter.Type);

                if (parameter.IsOut() && configuration.EnableOutParametersAndResult)
                    result = GetNullabilityForType(parameter.Type);
            }

            return result;
        }

        private CodeAnnotationNullableValue? AnalyzeFunction(IFunction function)
        {
            // Methods and operators

            CodeAnnotationNullableValue? result = null;

            if (!IsDelegateInvokeOrEndInvokeFunction(function))
            {
                if (_configurationEvaluator.EvaluateFor(function.Module).EnableOutParametersAndResult)
                {
                    if (!ContainsContractAnnotationAttribute(function))
                    {
                        result = GetNullabilityForType(function.ReturnType);
                    }
                }
            }

            return result;
        }

        private CodeAnnotationNullableValue? AnalyzeDelegate(IDelegate @delegate)
        {
            CodeAnnotationNullableValue? result = null;

            if (_configurationEvaluator.EvaluateFor(@delegate.Module).EnableOutParametersAndResult)
            {
                result = GetNullabilityForType(@delegate.InvokeMethod.ReturnType);
            }

            return result;
        }

        public CodeAnnotationNullableValue? AnalyzeDeclaredElementContainerElement([CanBeNull] IDeclaredElement element)
        {
            CodeAnnotationNullableValue? result = null;

            var method = element as IMethod;
            if (method != null)
                result = AnalyzeMethodContainerElement(method);

            return result;
        }

        private CodeAnnotationNullableValue? AnalyzeMethodContainerElement(IMethod method)
        {
            CodeAnnotationNullableValue? result = null;

            if (_configurationEvaluator.EvaluateFor(method.Module).EnableOutParametersAndResult)
            {
                if (!ContainsContractAnnotationAttribute(method))
                {
                    var taskUnderlyingType = method.ReturnType.GetTaskUnderlyingType();

                    if (taskUnderlyingType != null)
                        result = GetNullabilityForType(taskUnderlyingType);
                }
            }

            return result;
        }

        private static CodeAnnotationNullableValue? GetNullabilityForType(IType type)
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

        private bool IsImplicitNullabilityApplicableToParameter(IParameter parameter)
        {
            var parametersOwner = parameter.ContainingParametersOwner;

            // IFunction includes methods, constructors, operator overloads, delegates, but also implicitly defined ASP.NET methods, e.g. Bind()
            // which we want to exclude, because the developer cannot override the implicit annotation.
            // IProperty includes indexer parameters.
            var isParametersOwnerRegularFunctionOrIndexer = (parametersOwner is IFunction && !(parametersOwner is AspImplicitTypeMember)) ||
                                                            parametersOwner is IProperty;

            return isParametersOwnerRegularFunctionOrIndexer &&
                   !IsDelegateBeginInvokeMethod(parametersOwner) &&
                   IsParametersOwnerNotSynthetic(parametersOwner);
        }

        private static bool IsDelegateBeginInvokeMethod(IParametersOwner parametersOwner)
        {
            // Delegate BeginInvoke() methods must be excluded for *parameters*, because ReSharper doesn't pass the parameter attributes to
            // the DelegateBeginInvokeMethod => implicit nullability could not be overridden with explicit annotations.

            if (parametersOwner.ShortName != DELEGATE_BEGIN_INVOKE_METHOD_NAME)
                return false;

            return parametersOwner.GetContainingType() is IDelegate;
        }

        private static bool IsDelegateInvokeOrEndInvokeFunction(IFunction function)
        {
            // Delegate Invoke() and EndInvoke() methods must be excluded for *result values*, because of 
            // an R# issue, see DelegatesSampleTests.SomeFunctionDelegate.

            if (!(function.ShortName == DELEGATE_INVOKE_METHOD_NAME || function.ShortName == DELEGATE_END_INVOKE_METHOD_NAME))
                return false;

            return function.GetContainingType() is IDelegate;
        }

        private static bool IsParametersOwnerNotSynthetic(IParametersOwner containingParametersOwner)
        {
            // Exclude ReSharper's fake (called "synthetic") parameter owners (methods), like ASP.NET WebForms' Render-method, or 
            // Razor's Write-methods, because these look like regular project methods but should be excluded from Implicit
            // Nullability because the developer cannot override the result with explicit annotations.

            return !containingParametersOwner.IsSynthetic();
        }

        private static bool IsOptionalArgumentWithNullDefaultValue(IParameter parameter)
        {
#if DEBUG
            if (parameter.IsOptional)
            {
                var defaultValue = parameter.GetDefaultValue();
                var optionalParameterText = "OptionalParameter IsConstant: " + defaultValue.IsConstant +
                                            ", ConstantValue.Value: " + (defaultValue.ConstantValue.Value ?? "NULL") +
                                            ", IsDefaultType: " + defaultValue.IsDefaultType +
                                            ", DefaultTypeValue.IsValueType(): " + defaultValue.DefaultTypeValue.IsValueType();

                Logger.Verbose(optionalParameterText);
            }
#endif

            return parameter.IsOptional && IsNullDefaultValue(parameter.GetDefaultValue());
        }

        private static bool IsNullDefaultValue(DefaultValue defaultValue)
        {
            // Note that for "param = null" and "param = default(string)", the ConstantValue cannot be trusted; we therefore must check IsDefaultType:
            var isNullDefaultExpression = defaultValue.IsDefaultType && !defaultValue.DefaultTypeValue.IsValueType();

            // For "param = StringConstantMember" this check is necessary:
            var isNullConstantExpression = defaultValue.IsConstant && defaultValue.ConstantValue.Value == null;

            return isNullDefaultExpression || isNullConstantExpression;
        }

        private bool ContainsContractAnnotationAttribute(IFunction function)
        {
            var attributeInstances = function.GetAttributeInstances(inherit: true);

            // Can't use the CodeAnnotationsCache here because we would get an endless recursion:
            return _codeAnnotationAttributesChecker.ContainsContractAnnotationAttribute(attributeInstances);
        }
    }
}
