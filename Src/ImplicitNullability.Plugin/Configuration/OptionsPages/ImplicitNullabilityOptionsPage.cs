using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq.Expressions;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.Application.UI.Commands;
using JetBrains.Application.UI.Components;
using JetBrains.Application.UI.Icons.CommonThemedIcons;
using JetBrains.Application.UI.Options;
using JetBrains.Application.UI.Options.OptionsDialog;
using JetBrains.Application.UI.Options.OptionsDialog.SimpleOptions;
using JetBrains.Application.UI.Options.OptionsDialog.SimpleOptions.ViewModel;
using JetBrains.DataFlow;
using JetBrains.ReSharper.Feature.Services.Daemon.OptionPages;
using JetBrains.UI.RichText;
using JetBrains.Util;
using JetBrains.Lifetimes;

namespace ImplicitNullability.Plugin.Configuration.OptionsPages
{
    [ExcludeFromCodeCoverage /* options page user interface is tested manually */]
    [OptionsPage(PageId, PageTitle, typeof(CommonThemedIcons.Bulb), ParentId = CodeInspectionPage.PID)]
#pragma warning disable 618
    // TODO: Refactor to BeSimpleOptionsPage
    public class ImplicitNullabilityOptionsPage : SimpleOptionsPage
#pragma warning restore 618
    {
        public const string PageTitle = "Implicit Nullability";
        private const string PageId = nameof(ImplicitNullabilityOptionsPage);

        private static readonly TextStyle Bold = new TextStyle(FontStyle.Bold);
        private static readonly TextStyle Italic = new TextStyle(FontStyle.Italic);

        private readonly ISettingsOptimization _settingsOptimization;
        private readonly Clipboard _clipboard;

        public ImplicitNullabilityOptionsPage(
            Lifetime lifetime,
            OptionsSettingsSmartContext optionsSettingsSmartContext,
            ISettingsOptimization settingsOptimization,
            Clipboard clipboard)
            : base(lifetime, optionsSettingsSmartContext)
        {
            _settingsOptimization = settingsOptimization;
            _clipboard = clipboard;

            var enabledOption = AddBoolOption((ImplicitNullabilitySettings s) => s.Enabled, "Enabled");

            // Note: Text duplicated in README
            SetIndent(AddText(
                "With enabled Implicit Nullability, reference types are by default implicitly [NotNull] for " +
                "specific, configurable elements (see below). " +
                "Their nullability can be overridden with an explicit [CanBeNull] attribute. " +
                "Optional method parameters with a null default value are implicitly [CanBeNull]."), 1);
            AddEmptyLine();
            SetIndent(AddText(
                "Code elements which should be affected by Implicit Nullability can be configured in two ways."), 1);
            AddEmptyLine();
            SetIndent(AddText(
                "1. Recommended for application authors: Using the following settings."), 1);


            AddNullabilityBoolOption(
                s => s.EnableInputParameters,
                "Input parameters of methods, delegates, and indexers",
                additionalText: null,
                parentOption: enabledOption,
                indent: 2);

            AddNullabilityBoolOption(
                s => s.EnableRefParameters,
                "Ref parameters of methods and delegates",
                additionalText: null,
                parentOption: enabledOption,
                indent: 2);

            AddNullabilityBoolOption(
                s => s.EnableOutParametersAndResult,
                "Return values and out parameters of methods and delegates",
                additionalText: null,
                parentOption: enabledOption,
                indent: 2);

            var enableFieldsOption = AddNullabilityBoolOption(
                s => s.EnableFields,
                "Fields",
                additionalText: null,
                parentOption: enabledOption,
                indent: 2);

            AddNullabilityBoolOption(
                s => s.FieldsRestrictToReadonly,
                new RichText("Restrict to ") + new RichText("readonly", Italic) +
                new RichText(" fields (because ") + new RichText("readonly", Italic) + new RichText(" fields can be guarded "),
                new RichText("by invariants / constructor checks)"),
                parentOption: enableFieldsOption,
                indent: 3);

            AddNullabilityBoolOption(
                s => s.FieldsRestrictToReferenceTypes,
                new RichText("Restrict to fields contained in reference types (because due to the implicit default "),
                new RichText("constructor ") + new RichText("struct", Italic) +
                new RichText("s cannot guarantee non-null initialized fields)"),
                parentOption: enableFieldsOption,
                indent: 3);

            var enablePropertiesOption = AddNullabilityBoolOption(
                s => s.EnableProperties,
                "Properties",
                additionalText: null,
                parentOption: enabledOption,
                indent: 2);

            AddNullabilityBoolOption(
                s => s.PropertiesRestrictToGetterOnly,
                "Restrict to getter-only properties (including C# 6+ readonly auto-properties)",
                additionalText: null,
                parentOption: enablePropertiesOption,
                indent: 3);

            AddNullabilityBoolOption(
                s => s.PropertiesRestrictToReferenceTypes,
                new RichText("Restrict to properties contained in reference types (because due to the implicit default "),
                new RichText("constructor ") + new RichText("struct", Italic) +
                new RichText("s cannot guarantee non-null initialized auto-properties)"),
                parentOption: enablePropertiesOption,
                indent: 3);

            AddText("");
            AddNullabilityBoolOption(
                s => s.GeneratedCode,
                GeneratedCodeOptions.Exclude,
                GeneratedCodeOptions.Include,
                new RichText("Exclude generated code (decorated with ") + new RichText("[GeneratedCode]", Italic),
                new RichText(" attribute or configured on the \"Code Inspection | Generated Code\" options page)"),
                parentOption: enabledOption,
                indent: 2);

            AddEmptyLine();
            SetIndent(AddRichText(
                    new RichText("2. Recommended for library authors: By defining ") + new RichText("[AssemblyMetadata]", Italic) +
                    new RichText(" attributes in all concerned assemblies. " +
                                 "This has the advantage of the Implicit Nullability configuration also getting compiled into the assemblies.")),
                indent: 1);

            var copyButtonText = "Copy [AssemblyMetadata] attributes to clipboard (corresponding to the options above)";
            var copyButton = AddButton(copyButtonText, new DelegateCommand(CopyAssemblyAttributeCode));
#if RIDER
            AddEmptyLine();
            SetIndent(AddText("TODO: Add 'Copy [AssemblyMetadata] attributes to clipboard' button here"), 1); // TODO RiderSupport: Button
            AddEmptyLine();
#endif
            SetIndent(copyButton, 2);
            enabledOption.CheckedProperty.FlowInto(myLifetime, copyButton.GetIsEnabledProperty());

            SetIndent(AddRichText(
                    new RichText("Implicit Nullability normally ignores referenced assemblies. " +
                                 "It can take referenced libraries into account only if they contain embedded ") +
                    new RichText("[AssemblyMetadata]", Italic) + new RichText(" -based configuration.")),
                indent: 1);
            AddEmptyLine();
            SetIndent(AddRichText(
                    new RichText("The options in ") + new RichText("[AssemblyMetadata]", Italic) +
                    new RichText(" attributes override the options selected above.")),
                indent: 1);

            AddEmptyLine();
            AddRichText(
                new RichText("Warning: ", Bold) + new RichText("After changing these settings respectively changing ") +
                new RichText("the [AssemblyMetadata] attribute value, cleaning the solution cache (see \"General\" options page) " +
                             "is necessary to update already analyzed code."));

            AddEmptyLine();
            AddBoolOption(
                (ImplicitNullabilitySettings s) => s.EnableTypeHighlighting,
                "Enable type highlighting of explicit or implicit [NotNull] elements (to adapt the color, look ",
                "for \"Implicit Nullability\" in Visual Studio's \"Fonts and Colors\" options)",
                indent: 0);
        }

        private BoolOptionViewModel AddNullabilityBoolOption(
            Expression<Func<ImplicitNullabilitySettings, bool>> settingExpression,
            RichText text,
            [CanBeNull] RichText additionalText,
            BoolOptionViewModel parentOption,
            int indent)
        {
            var result = AddBoolOption(settingExpression, text, additionalText, indent);
            SetUpDependentOption(result, parentOption);
            return result;
        }

        private void AddNullabilityBoolOption<TEntryMemberType>(
            Expression<Func<ImplicitNullabilitySettings, TEntryMemberType>> settingExpression,
            TEntryMemberType trueValue,
            TEntryMemberType falseValue,
            RichText text,
            [CanBeNull] RichText additionalText,
            BoolOptionViewModel parentOption,
            int indent)
        {
            var result = AddBoolOption(settingExpression, trueValue, falseValue, text, additionalText, indent);
            SetUpDependentOption(result, parentOption);
        }

        private void SetUpDependentOption(IOptionEntity option, BoolOptionViewModel parentOption)
        {
            // Note: The `Compose(parentOption.IsEnabledProperty)` starts to become important at the second "level".
            parentOption.CheckedProperty.Compose(myLifetime, parentOption.IsEnabledProperty)
                .Select(myLifetime, "checked and enabled", pair => pair.First && pair.Second)
                .FlowInto(myLifetime, option.GetIsEnabledProperty());
        }

        private BoolOptionViewModel AddBoolOption<T>(
            Expression<Func<T, bool>> settingExpression,
            RichText text,
            [CanBeNull] RichText additionalText,
            int indent)
        {
            // Rider doesn't support line-wrapping for check boxes atm.

#if RESHARPER
            var boolOption = AddBoolOption(settingExpression, text + additionalText);
#else // RIDER
            var boolOption = AddBoolOption(settingExpression, text);

            if (additionalText != null)
                AddAdditionalText(boolOption, additionalText, indent);
#endif
            SetIndent(boolOption, indent);
            return boolOption;
        }

        private IOptionEntity AddBoolOption<T>(
            Expression<Func<ImplicitNullabilitySettings, T>> settingExpression,
            T trueValue,
            T falseValue,
            RichText text,
            [CanBeNull] RichText additionalText,
            int indent)
        {
#if RESHARPER
            var boolOption = AddBoolOption(settingExpression, trueValue, falseValue, text + additionalText);
#else // RIDER
            var boolOption = AddBoolOption(settingExpression, trueValue, falseValue, text);

            if (additionalText != null)
                AddAdditionalText(boolOption, additionalText, indent);
#endif
            SetIndent(boolOption, indent);
            return boolOption;
        }

#if RIDER
        private void AddAdditionalText(BoolOptionViewModel boolOption, RichText additionalText, int indent)
        {
            var textOptionEntity = AddRichText(additionalText);
            boolOption.IsEnabledProperty.FlowInto(myLifetime, textOptionEntity.GetIsEnabledProperty());
            SetIndent(textOptionEntity, indent + 1);
        }
#endif

        private void CopyAssemblyAttributeCode()
        {
            var currentImplicitNullabilitySettings =
                OptionsSettingsSmartContext.GetKey<ImplicitNullabilitySettings>(_settingsOptimization);

            var assemblyMetadataCode = AssemblyAttributeConfigurationTranslator.GenerateAttributeCode(
                ImplicitNullabilityConfiguration.CreateFromSettings(currentImplicitNullabilitySettings));

            _clipboard.SetText(assemblyMetadataCode);
            MessageBox.ShowInfo("The following code has been copied to your clipboard:\n\n\n" + assemblyMetadataCode);
        }
    }
}
