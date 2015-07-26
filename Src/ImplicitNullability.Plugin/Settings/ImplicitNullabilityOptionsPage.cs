using System;
using JetBrains.Application.Settings;
using JetBrains.DataFlow;
using JetBrains.UI.Application;
using JetBrains.UI.Options;
using JetBrains.UI.Options.Helpers;
using JetBrains.UI.Resources;
#if RESHARPER8
using JetBrains.ReSharper.Features.Environment.Options.Inspections;

#else
using JetBrains.ReSharper.Feature.Services.Daemon.OptionPages;

#endif

namespace ImplicitNullability.Plugin.Settings
{
    [OptionsPage(
        c_pageId,
        PageTitle,
        typeof (CommonThemedIcons.Bulb),
        ParentId = CodeInspectionPage.PID)]
    public class ImplicitNullabilityOptionsPage : AStackPanelOptionsPage // REVIEW: R#9 ISearchablePage ??
    {
        private readonly Lifetime _lifetime;
        private readonly OptionsSettingsSmartContext _settings;
        public const string PageTitle = "Implicit Nullability";
        private const string c_pageId = "ImplicitNullabilityOptions";

        public ImplicitNullabilityOptionsPage(Lifetime lifetime, IUIApplication environment, OptionsSettingsSmartContext settings)
            : base(lifetime, environment, c_pageId)
        {
            _lifetime = lifetime;
            _settings = settings;
            InitControls();
        }

        private void InitControls()
        {
            var enabledCheckBox = new Controls.CheckBox();
            enabledCheckBox.Text = "Enabled";
            Controls.Add(enabledCheckBox);

            var inputAndRefParametersCheckBox = new Controls.CheckBox();
            inputAndRefParametersCheckBox.Margin += JetBrains.UI.Options.Helpers.Controls.IndentF;
            inputAndRefParametersCheckBox.Text = "Input and ref parameters of methods, delegates and indexers with the following rules";

            inputAndRefParametersCheckBox.AutoSize = true;
            Controls.Add(inputAndRefParametersCheckBox);

            Controls.Add(
                new Controls.Label(
                    "\u2022 Nullable value type parameters and optional parameters with null default value \u2192 implicitly CanBeNull\n" +
                    "\u2022 Reference type parameters (without null default value) \u2192 implicitly NotNull",
                    JetBrains.UI.Options.Helpers.Controls.IndentF + JetBrains.UI.Options.Helpers.Controls.IndentF));

            enabledCheckBox.Checked.Change.Advise(_lifetime, x => inputAndRefParametersCheckBox.Enabled = x.Property.Value);

            _settings.SetBinding(_lifetime, (ImplicitNullabilitySettings s) => s.Enabled, enabledCheckBox.Checked);
            _settings.SetBinding(_lifetime, (ImplicitNullabilitySettings s) => s.EnableInputAndRefParameters, inputAndRefParametersCheckBox.Checked);
        }
    }
}