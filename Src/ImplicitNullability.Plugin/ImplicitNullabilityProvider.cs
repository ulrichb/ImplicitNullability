using ImplicitNullability.Plugin.Infrastructure;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Asp.Impl.Html;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.Util;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;
using static JetBrains.ReSharper.Psi.DeclaredElementConstants;

#if !(RESHARPER20162 || RESHARPER20163)
using JetBrains.ReSharper.Psi.CSharp;

#endif

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
            switch (declaredElement)
            {
                case IParameter parameter:
                    return AnalyzeParameter(parameter);
                case IFunction function /* methods, constructors, and operators */:
                    return AnalyzeFunction(function);
                case IDelegate @delegate:
                    return AnalyzeDelegate(@delegate);
                case IField field:
                    return AnalyzeField(field);
            }

            return null;
        }

        public CodeAnnotationNullableValue? AnalyzeDeclaredElementContainerElement([CanBeNull] IDeclaredElement declaredElement)
        {
            switch (declaredElement)
            {
                case IMethod method:
                    return AnalyzeFunction(method, useTaskUnderlyingType: true);
                case IDelegate @delegate:
                    return AnalyzeDelegate(@delegate, useTaskUnderlyingType: true);
            }

            return null;
        }

        private CodeAnnotationNullableValue? AnalyzeParameter(IParameter parameter)
        {
            CodeAnnotationNullableValue? result = null;

            var containingParametersOwner = parameter.ContainingParametersOwner;

            if (IsImplicitNullabilityApplicableToParameterOwner(containingParametersOwner))
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

        private CodeAnnotationNullableValue? AnalyzeFunction(IFunction function, bool useTaskUnderlyingType = false)
        {
            CodeAnnotationNullableValue? result = null;

            if (!IsDelegateInvokeOrEndInvokeFunction(function))
            {
                if (_configurationEvaluator.EvaluateFor(function.Module).EnableOutParametersAndResult)
                {
                    if (!ContainsContractAnnotationAttribute(function))
                    {
                        result = GetNullabilityForTypeOrTaskUnderlyingType(function.ReturnType, useTaskUnderlyingType);
                    }
                }
            }

            return result;
        }

        private CodeAnnotationNullableValue? AnalyzeDelegate(IDelegate @delegate, bool useTaskUnderlyingType = false)
        {
            CodeAnnotationNullableValue? result = null;

            if (_configurationEvaluator.EvaluateFor(@delegate.Module).EnableOutParametersAndResult)
            {
                result = GetNullabilityForTypeOrTaskUnderlyingType(@delegate.InvokeMethod.ReturnType, useTaskUnderlyingType);
            }

            return result;
        }

        private CodeAnnotationNullableValue? AnalyzeField(IField field)
        {
            CodeAnnotationNullableValue? result = null;

            if (!field.IsAutoPropertyBackingField())
            {
                var configuration = _configurationEvaluator.EvaluateFor(field.Module);

                if (configuration.EnableFields && IsFieldMatchingConfigurationOptions(field, configuration))
                {
                    result = GetNullabilityForType(field.Type);
                }
            }

            return result;
        }

        private static CodeAnnotationNullableValue? GetNullabilityForTypeOrTaskUnderlyingType(IType type, bool useTaskUnderlyingType)
        {
            if (useTaskUnderlyingType)
            {
                var taskUnderlyingType = GetTaskUnderlyingType(type);

                if (taskUnderlyingType == null)
                    return null; // 'type' is not a 'Task<T>' type => no implicit nullability

                return GetNullabilityForType(taskUnderlyingType);
            }

            return GetNullabilityForType(type);
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

        [CanBeNull]
        private static IType GetTaskUnderlyingType(IType type)
        {
#if RESHARPER20162 || RESHARPER20163
            return type.GetTaskUnderlyingType();
#else
            // Use "latest" language level because this just includes _more_ types (C# 7 "task-like" types) and the nullability
            // value we return here also effects _callers_ (whose C# language level we do not know).

            return type.GetTasklikeUnderlyingType(CSharpLanguageLevel.Latest);
#endif
        }

        private static bool IsImplicitNullabilityApplicableToParameterOwner([CanBeNull] IParametersOwner parametersOwner)
        {
            // IFunction includes methods, constructors, operator overloads, delegate (methods), but also implicitly
            // defined ASP.NET methods, e.g. Bind()  which we want to exclude, because the developer cannot
            // override the implicit annotation.
            // IProperty includes indexer parameters.

            switch (parametersOwner)
            {
                case IFunction function:
                    return !(parametersOwner is AspImplicitTypeMember) &&
                           !IsDelegateBeginInvokeFunction(function) &&
                           IsParametersOwnerNotSynthetic(parametersOwner);
                case IProperty _:
                    return true;
            }

            return false;
        }

        private static bool IsDelegateBeginInvokeFunction(IFunction function)
        {
            // Delegate BeginInvoke() methods must be excluded for *parameters*, because ReSharper doesn't pass the parameter attributes to
            // the DelegateBeginInvokeMethod => implicit nullability could not be overridden with explicit annotations.
            // We can't use R#'s DelegateMethod subtypes to implement this predicate because they don't work for compiled code.

            if (function.ShortName != DELEGATE_BEGIN_INVOKE_METHOD_NAME)
                return false;

            return function.GetContainingType() is IDelegate;
        }

        private static bool IsDelegateInvokeOrEndInvokeFunction(IFunction function)
        {
            // Delegate Invoke() and EndInvoke() methods must be excluded for *result values*, because of 
            // an R# issue, see DelegatesSampleTests.SomeFunctionDelegate.
            // We can't use R#'s DelegateMethod subtypes to implement this predicate because they don't work for compiled code.

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

        private static bool IsFieldMatchingConfigurationOptions(IField field, ImplicitNullabilityConfiguration configuration)
        {
            return (!configuration.FieldsRestrictToReadonly || field.IsReadonly) &&
                   (!configuration.FieldsRestrictToReferenceTypes || field.IsMemberOfReferenceType());
        }
    }
}
