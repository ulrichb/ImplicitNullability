using ImplicitNullability.Plugin.Configuration;
using ImplicitNullability.Plugin.Infrastructure;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Util;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.Psi.Xaml.DeclaredElements;
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
        private readonly GeneratedCodeProvider _generatedCodeProvider;
        private readonly CodeAnnotationAttributesChecker _codeAnnotationAttributesChecker;

        public ImplicitNullabilityProvider(
            ImplicitNullabilityConfigurationEvaluator configurationEvaluator,
            GeneratedCodeProvider generatedCodeProvider,
            CodeAnnotationAttributesChecker codeAnnotationAttributesChecker
        )
        {
            _configurationEvaluator = configurationEvaluator;
            _generatedCodeProvider = generatedCodeProvider;
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
                case IProperty property:
                    return AnalyzeProperty(property);
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
                Assertion.Assert(
                    containingParametersOwner != null && (containingParametersOwner is IFunction || containingParametersOwner is IProperty),
                    "containingParametersOwner is function or property");

                var configuration = _configurationEvaluator.EvaluateFor(parameter.Module);

                if (!IsExcludedGeneratedCode(configuration, (ITypeMember) containingParametersOwner))
                {
                    if (parameter.IsInput() && configuration.HasAppliesTo(ImplicitNullabilityAppliesTo.InputParameters))
                    {
                        if (IsOptionalArgumentWithNullDefaultValue(parameter))
                            result = CodeAnnotationNullableValue.CAN_BE_NULL;
                        else
                            result = GetNullabilityForType(parameter.Type);
                    }

                    if (parameter.IsRef() && configuration.HasAppliesTo(ImplicitNullabilityAppliesTo.RefParameters))
                        result = GetNullabilityForType(parameter.Type);

                    if (parameter.IsOut() && configuration.HasAppliesTo(ImplicitNullabilityAppliesTo.OutParametersAndResult))
                        result = GetNullabilityForType(parameter.Type);
                }
            }

            return result;
        }

        private CodeAnnotationNullableValue? AnalyzeFunction(IFunction function, bool useTaskUnderlyingType = false)
        {
            CodeAnnotationNullableValue? result = null;

            if (!IsDelegateInvokeOrEndInvokeFunction(function))
            {
                var configuration = _configurationEvaluator.EvaluateFor(function.Module);

                if (configuration.HasAppliesTo(ImplicitNullabilityAppliesTo.OutParametersAndResult))
                {
                    if (!(ContainsContractAnnotationAttribute(function) || IsExcludedGeneratedCode(configuration, function)))
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

            var configuration = _configurationEvaluator.EvaluateFor(@delegate.Module);

            if (configuration.HasAppliesTo(ImplicitNullabilityAppliesTo.OutParametersAndResult))
            {
                if (!IsExcludedGeneratedCode(configuration, @delegate))
                {
                    result = GetNullabilityForTypeOrTaskUnderlyingType(@delegate.InvokeMethod.ReturnType, useTaskUnderlyingType);
                }
            }

            return result;
        }

        private CodeAnnotationNullableValue? AnalyzeField(IField field)
        {
            CodeAnnotationNullableValue? result = null;

            if (!(field is IXamlField))
            {
                var configuration = _configurationEvaluator.EvaluateFor(field.Module);

                if (configuration.HasAppliesTo(ImplicitNullabilityAppliesTo.Fields) &&
                    IsFieldMatchingConfigurationOptions(field, configuration))
                {
                    if (!IsExcludedGeneratedCode(configuration, field))
                    {
                        result = GetNullabilityForType(field.Type);
                    }
                }
            }

            return result;
        }

        private CodeAnnotationNullableValue? AnalyzeProperty(IProperty property)
        {
            CodeAnnotationNullableValue? result = null;

            var configuration = _configurationEvaluator.EvaluateFor(property.Module);

            if (configuration.HasAppliesTo(ImplicitNullabilityAppliesTo.Properties) &&
                IsPropertyMatchingConfigurationOptions(property, configuration))
            {
                if (!IsExcludedGeneratedCode(configuration, property))
                {
                    result = GetNullabilityForType(property.Type);
                }
            }

            return result;
        }

        private bool IsExcludedGeneratedCode(ImplicitNullabilityConfiguration configuration, ITypeMember typeMember)
        {
            if (configuration.GeneratedCode == GeneratedCodeOptions.Exclude)
                return _generatedCodeProvider.IsGeneratedOrSynthetic(typeMember);

            return false;
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
            // Use "latest" language level because this just includes _more_ types (C# 7 "task-like" types) and the nullability
            // value we return here also effects _callers_ (whose C# language level we do not know).

            return type.GetTasklikeUnderlyingType(CSharpLanguageLevel.Latest);
        }

        private static bool IsImplicitNullabilityApplicableToParameterOwner([CanBeNull] IParametersOwner parametersOwner)
        {
            // IFunction includes methods, constructors, operator overloads, delegate (methods), but also implicitly
            // defined ("synthetic") methods, which we want to exclude because the developer cannot
            // override the implicit annotation.
            // IProperty includes indexer parameters.

            switch (parametersOwner)
            {
                case IFunction function:
                    return !IsDelegateBeginInvokeFunction(function) &&
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
            return
                (!configuration.HasFieldOption(ImplicitNullabilityFieldOptions.RestrictToReadonly) || field.IsReadonly) &&
                (!configuration.HasFieldOption(ImplicitNullabilityFieldOptions.RestrictToReferenceTypes) || field.IsMemberOfReferenceType());
        }

        private static bool IsPropertyMatchingConfigurationOptions(IProperty property, ImplicitNullabilityConfiguration configuration)
        {
            return
                // Note that `IsWritable` also searches for "polymorphic setters" (see `PartiallyOveriddenProperties` test data)
                (!configuration.HasPropertyOption(ImplicitNullabilityPropertyOptions.RestrictToGetterOnly) || !property.IsWritable) &&
                (!configuration.HasPropertyOption(ImplicitNullabilityPropertyOptions.RestrictToReferenceTypes) || property.IsMemberOfReferenceType());
        }
    }
}
