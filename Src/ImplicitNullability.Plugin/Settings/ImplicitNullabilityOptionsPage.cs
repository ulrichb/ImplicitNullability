using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using JetBrains.Annotations;
using JetBrains.DataFlow;
using JetBrains.ReSharper.Feature.Services.Daemon.OptionPages;
using JetBrains.UI;
using JetBrains.UI.Options;
using JetBrains.UI.Options.OptionsDialog2.SimpleOptions;
using JetBrains.UI.Options.OptionsDialog2.SimpleOptions.ViewModel;
using JetBrains.UI.Resources;
using JetBrains.Util;
#if RESHARPER92 || RESHARPER100
using JetBrains.UI.Extensions.Commands;

#else
using JetBrains.Application.UI.Commands;

#endif

namespace ImplicitNullability.Plugin.Settings
{
    [ExcludeFromCodeCoverage /* options page user interface is tested manually */]
    [OptionsPage(PageId, PageTitle, typeof(CommonThemedIcons.Bulb), ParentId = CodeInspectionPage.PID)]
    public class ImplicitNullabilityOptionsPage : SimpleOptionsPage
    {
        private readonly Clipboard _clipboard;
        public const string PageTitle = "Implicit Nullability";
        private const string PageId = "ImplicitNullabilityOptions";

        private readonly BoolOptionViewModel _enableInputParametersOption;
        private readonly BoolOptionViewModel _enableRefParametersOption;
        private readonly BoolOptionViewModel _enableOutParametersAndResultOption;

        public ImplicitNullabilityOptionsPage(Lifetime lifetime, OptionsSettingsSmartContext optionsSettingsSmartContext, Clipboard clipboard)
            : base(lifetime, optionsSettingsSmartContext)
        {
            _clipboard = clipboard;

            var enabledOption = AddBoolOption((ImplicitNullabilitySettings s) => s.Enabled, "Enabled");

            // Note: Text duplicated in README
            var infoText =
                "With enabled Implicit Nullability, reference types are by default implicitly [NotNull] for " +
                "specific, configurable elements (see below). " +
                "Their nullability can be overridden with an explicit [CanBeNull] attribute. " +
                "Optional method parameters with a null default value are implicitly [CanBeNull].\n" +
                "\n" +
                "Code elements which should be affected by Implicit Nullability can be configured in two ways.\n" +
                "\n" +
                "1. Recommended for application authors: Using the following settings.";
            SetIndent(AddText(infoText), 1);


            _enableInputParametersOption = AddNullabilityBoolOption(
                s => s.EnableInputParameters,
                "Input parameters of methods, delegates, and indexers",
                enabledOption);
            SetIndent(_enableInputParametersOption, 2);

            _enableRefParametersOption = AddNullabilityBoolOption(
                s => s.EnableRefParameters,
                "Ref parameters of methods and delegates",
                enabledOption);
            SetIndent(_enableRefParametersOption, 2);

            _enableOutParametersAndResultOption = AddNullabilityBoolOption(
                s => s.EnableOutParametersAndResult,
                "Return values and out parameters of methods and delegates",
                enabledOption);
            SetIndent(_enableOutParametersAndResultOption, 2);

            var assemblyAttributeInfoText1 =
                "\n" +
                "2. Recommended for library authors: By defining an [AssemblyMetadata] attribute in all concerned assemblies. "
                +
                "This has the advantage of the Implicit Nullability configuration also getting compiled into the assemblies.";
            SetIndent(AddText(assemblyAttributeInfoText1), 1);

            var copyButtonText = "Copy [AssemblyMetadata] attribute to clipboard (using above options as a template)";
            var copyButton = AddButton(copyButtonText, new DelegateCommand(CopyAssemblyAttributeCode));
            SetIndent(copyButton, 2);
            enabledOption.CheckedProperty.FlowInto(myLifetime, copyButton.GetIsEnabledProperty());

            var assemblyAttributeInfoText2 =
                "Implicit Nullability normally ignores referenced assemblies. " +
                "It can take referenced libraries into account only if they contain embedded [AssemblyMetadata]-based configuration.\n" +
                "\n" +
                "The options in a [AssemblyMetadata] attribute override the options selected above.";
            SetIndent(AddText(assemblyAttributeInfoText2), 1);

            var cacheInfoText =
                "Warning: After changing these settings respectively changing the [AssemblyMetadata] attribute value, " +
                "cleaning the solution cache (see \"General\" options page) " +
                "is necessary to update already analyzed code.";
            AddText(cacheInfoText);
        }

        private BoolOptionViewModel AddNullabilityBoolOption(
            [NotNull] Expression<Func<ImplicitNullabilitySettings, bool>> settingsExpression,
            [NotNull] string text,
            [NotNull] BoolOptionViewModel enabledOption)
        {
            var result = AddBoolOption(settingsExpression, text);
            enabledOption.CheckedProperty.FlowInto(myLifetime, result.GetIsEnabledProperty());

            return result;
        }

        private void CopyAssemblyAttributeCode()
        {
            var assemblyMetadataCode = AssemblyAttributeConfigurationTranslator.GenerateAttributeCode(
                new ImplicitNullabilityConfiguration(
                    _enableInputParametersOption.CheckedProperty.Value,
                    _enableRefParametersOption.CheckedProperty.Value,
                    _enableOutParametersAndResultOption.CheckedProperty.Value));

            _clipboard.SetText(assemblyMetadataCode);
            MessageBox.ShowInfo("The following code has been copied to your clipboard:\n\n\n" + assemblyMetadataCode);
        }
    }
}