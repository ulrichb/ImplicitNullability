using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using JetBrains.Application.Settings;
using JetBrains.DataFlow;
using JetBrains.ReSharper.Feature.Services.Daemon.OptionPages;
using JetBrains.UI.Application;
using JetBrains.UI.Options;
using JetBrains.UI.Options.Helpers;
using JetBrains.UI.Resources;

namespace ImplicitNullability.Plugin.Settings
{
    [ExcludeFromCodeCoverage /* options page user interface is tested manually */]
    [OptionsPage(
        PageId,
        PageTitle,
        typeof (CommonThemedIcons.Bulb),
        ParentId = CodeInspectionPage.PID)]
    public class ImplicitNullabilityOptionsPage : AStackPanelOptionsPage
    {
        // IDEA: After dropping R# 8.2 support: switch to SimpleOptionsPage (would automatically implement ISearchablePage)

        public const string PageTitle = "Implicit Nullability";
        private const string PageId = "ImplicitNullabilityOptions";

        private const int LeftMargin = 3; // pixels
        private const int VerticalSpace = 8; // pixels

        private readonly Lifetime _lifetime;
        private readonly OptionsSettingsSmartContext _settings;

        public ImplicitNullabilityOptionsPage(Lifetime lifetime, IUIApplication environment, OptionsSettingsSmartContext settings)
            : base(lifetime, environment, PageId)
        {
            _lifetime = lifetime;
            _settings = settings;
            InitControls();
        }

        private void InitControls()
        {
            var enabledCheckBox = new Controls.CheckBox {Text = "Enabled", AutoSize = true};
            enabledCheckBox.Margin += new Padding(LeftMargin, VerticalSpace, 0, 0);
            Controls.Add(enabledCheckBox);

            // Note: Text duplicated in README
            var infoLabel = new Controls.Label(
                "With enabled Implicit Nullability, reference types are by default implicitly [NotNull] for the syntax elements selected below. " +
                "Their nullability can be overridden with an explicit [CanBeNull] attribute. " +
                "Optional method parameters with a null default value are implicitly [CanBeNull].",
                JetBrains.UI.Options.Helpers.Controls.IndentF);

            enabledCheckBox.Checked.Change.Advise(_lifetime, x => infoLabel.Enabled = x.Property.Value);
            Controls.Add(infoLabel);

            var inputParametersCheckBox = CreateOptionCheckBox(enabledCheckBox, "Input parameters of methods, delegates, and indexers");
            Controls.Add(inputParametersCheckBox);

            var refParametersCheckBox = CreateOptionCheckBox(enabledCheckBox, "Ref parameters of methods and delegates");
            Controls.Add(refParametersCheckBox);

            var outParametersAndResultCheckBox = CreateOptionCheckBox(enabledCheckBox, "Return values and out parameters of methods and delegates");
            Controls.Add(outParametersAndResultCheckBox);

            var cacheInfoLabel = new Controls.Label("Note: After changing these settings, cleaning the solution cache (see " +
                                                    "\"General\" options page) is necessary to update already analyzed code.");
            cacheInfoLabel.Margin += new Padding(LeftMargin, VerticalSpace, 0, 0);
            Controls.Add(cacheInfoLabel);

            _settings.SetBinding(_lifetime, (ImplicitNullabilitySettings s) => s.Enabled, enabledCheckBox.Checked);
            _settings.SetBinding(_lifetime, (ImplicitNullabilitySettings s) => s.EnableInputParameters, inputParametersCheckBox.Checked);
            _settings.SetBinding(_lifetime, (ImplicitNullabilitySettings s) => s.EnableRefParameters, refParametersCheckBox.Checked);
            _settings.SetBinding(_lifetime, (ImplicitNullabilitySettings s) => s.EnableOutParametersAndResult, outParametersAndResultCheckBox.Checked);
        }

        private Controls.CheckBox CreateOptionCheckBox(Controls.CheckBox enabledCheckBox, string text)
        {
            var checkBox = new Controls.CheckBox {Text = text, AutoSize = true};
            checkBox.Margin += JetBrains.UI.Options.Helpers.Controls.IndentF;
            enabledCheckBox.Checked.Change.Advise(_lifetime, x => checkBox.Enabled = x.Property.Value);

            return checkBox;
        }
    }
}