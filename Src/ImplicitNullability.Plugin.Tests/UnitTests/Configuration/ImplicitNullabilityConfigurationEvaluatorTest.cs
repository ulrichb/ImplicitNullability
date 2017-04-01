using System;
using System.Linq;
using ImplicitNullability.Plugin.Configuration;
using JetBrains.Application.Settings;
using JetBrains.Application.Settings.Store.Implementation;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.TestFramework;
using JetBrains.Util;
using NUnit.Framework;

namespace ImplicitNullability.Plugin.Tests.UnitTests.Configuration
{
    [TestFixture]
    [TestNetFramework45 /*necessary for AssemblyMetadataAttribute reference*/]
    public class ImplicitNullabilityConfigurationEvaluatorTest : BaseTestWithSingleProject
    {
        private const bool Dis = false; // disable
        private const bool Ena = true; // enable
        private const bool Dnc = false; // don't care

        const ImplicitNullabilityAppliesTo AppliesToNone = ImplicitNullabilityAppliesTo.None;

        const ImplicitNullabilityAppliesTo AppliesToInputAndFields = ImplicitNullabilityAppliesTo.InputParameters |
                                                                     ImplicitNullabilityAppliesTo.Fields;

        const ImplicitNullabilityAppliesTo AppliesToInputAndOutput = ImplicitNullabilityAppliesTo.InputParameters |
                                                                     ImplicitNullabilityAppliesTo.OutParametersAndResult;

        const ImplicitNullabilityAppliesTo AppliesToAll = ImplicitNullabilityAppliesTo.InputParameters |
                                                          ImplicitNullabilityAppliesTo.RefParameters |
                                                          ImplicitNullabilityAppliesTo.OutParametersAndResult |
                                                          ImplicitNullabilityAppliesTo.Fields;

        [Test]
        // 1. Enabled=false in the settings overrules everything:
        [TestCase( /*solution:*/ Dis, /*project:*/ Dis, /*options:*/ Ena, "AttributeWithAllOptions", /*expected:*/ AppliesToNone)]
        //
        // 2. Project settings overrule solution settings:
        [TestCase( /*solution:*/ Dis, /*project:*/ Ena, /*options:*/ Dnc, "AttributeWithAllOptions", /*expected:*/ AppliesToAll)]
        [TestCase( /*solution:*/ Ena, /*project:*/ Dis, /*options:*/ Dnc, "AttributeWithAllOptions", /*expected:*/ AppliesToNone)]
        //
        // 3. Attributes overrule individual options in the settings:
        [TestCase( /*solution:*/ Dnc, /*project:*/ Ena, /*options:*/ Ena, "AttributeWithAllOptions", /*expected:*/ AppliesToAll)]
        [TestCase( /*solution:*/ Dnc, /*project:*/ Ena, /*options:*/ Ena, "AttributeWithNoOption", /*expected:*/ AppliesToNone)]
        //
        // 4. No attribute configuration => take the individual options in the settings:
        [TestCase( /*solution:*/ Dnc, /*project:*/ Ena, /*options:*/ Ena, "NoConfigurationAttribute", /*expected:*/ AppliesToAll)]
        [TestCase( /*solution:*/ Dnc, /*project:*/ Ena, /*options:*/ Dis, "NoConfigurationAttribute", /*expected:*/ AppliesToNone)]
        public void TestInheritanceRules(
            bool enabledInSolution,
            bool enabledInProject,
            bool enableOptionInProject,
            string testInput,
            ImplicitNullabilityAppliesTo expectedAppliesTo)
        {
            void ChangeSolutionSettings(IContextBoundSettingsStore settingsStore) =>
                settingsStore.SetValue((ImplicitNullabilitySettings x) => x.Enabled, enabledInSolution);

            void ChangeProjectSettings(IContextBoundSettingsStore settingsStore)
            {
                settingsStore.SetValue((ImplicitNullabilitySettings x) => x.Enabled, enabledInProject);
                settingsStore.SetValue((ImplicitNullabilitySettings x) => x.EnableInputParameters, enableOptionInProject);
                settingsStore.SetValue((ImplicitNullabilitySettings x) => x.EnableRefParameters, enableOptionInProject);
                settingsStore.SetValue((ImplicitNullabilitySettings x) => x.EnableOutParametersAndResult, enableOptionInProject);
                settingsStore.SetValue((ImplicitNullabilitySettings x) => x.EnableFields, enableOptionInProject);
            }

            //

            var configuration = GetImplicitNullabilityConfigurationFor(testInput, ChangeSolutionSettings, ChangeProjectSettings);

            //

            Assert.That(configuration.AppliesTo, Is.EqualTo(expectedAppliesTo));
        }

        [Test]
        // The following attributes override the settings:
        [TestCase("AttributeWithAllOptions", /*expected:*/ AppliesToAll)]
        [TestCase("AttributeWithOnlyInputParametersOption", /*expected:*/ ImplicitNullabilityAppliesTo.InputParameters)]
        [TestCase("AttributeWithNoOption", /*expected:*/ AppliesToNone)]
        [TestCase("AttributeWithInvalidValue", /*expected:*/ AppliesToNone)]
        [TestCase("AttributeWithValueContainingInvalidOption", /*expected:*/ AppliesToInputAndFields)]
        [TestCase("MultipleAttributes", /*expected:*/ AppliesToAll)]
        //
        // The following non-existing / invalid attributes are overridden by the settings:
        [TestCase("NoConfigurationAttribute", /*expected:*/ AppliesToInputAndOutput)]
        [TestCase("AttributeWithNullKey", /*expected:*/ AppliesToInputAndOutput)]
        [TestCase("AttributeWithNullValue", /*expected:*/ AppliesToInputAndOutput)]
        [TestCase("NonCompilingAttribute1", /*expected:*/ AppliesToInputAndOutput)]
        [TestCase("NonCompilingAttribute2", /*expected:*/ AppliesToInputAndOutput)]
        [TestCase("NonCompilingAttribute3", /*expected:*/ AppliesToInputAndOutput)]
        [TestCase("NonCompilingAttribute4", /*expected:*/ AppliesToInputAndOutput)]
        public void TestAssemblyMetadataAttributeConfiguration(string testInput, ImplicitNullabilityAppliesTo expectedAppliesTo)
        {
            void ChangeSolutionSettings(IContextBoundSettingsStore settingsStore)
            {
                settingsStore.SetValue((ImplicitNullabilitySettings x) => x.Enabled, Ena);
                settingsStore.SetValue((ImplicitNullabilitySettings x) => x.EnableInputParameters, Ena);
                settingsStore.SetValue((ImplicitNullabilitySettings x) => x.EnableRefParameters, Dis);
                settingsStore.SetValue((ImplicitNullabilitySettings x) => x.EnableOutParametersAndResult, Ena);
                settingsStore.SetValue((ImplicitNullabilitySettings x) => x.EnableFields, Dis);
            }

            var configuration = GetImplicitNullabilityConfigurationFor(testInput, ChangeSolutionSettings);

            Assert.That(configuration.AppliesTo, Is.EqualTo(expectedAppliesTo));
        }

        [Test]
        [TestCase("AttributeWithFieldsWithNullValue", ImplicitNullabilityFieldOptions.None)]
        [TestCase("AttributeWithFieldsWithNoOption", ImplicitNullabilityFieldOptions.None)]
        [TestCase("AttributeWithFieldsWithInvalidValue", ImplicitNullabilityFieldOptions.None)]
        [TestCase("AttributeWithFieldsWithAllOptions", ImplicitNullabilityFieldOptions.RestrictToReadonly |
                                                       ImplicitNullabilityFieldOptions.RestrictToReferenceTypes)]
        public void TestAssemblyMetadataAttributeFieldsConfiguration(string testInput, ImplicitNullabilityFieldOptions expected)
        {
            void ChangeSolutionSettings(IContextBoundSettingsStore settingsStore)
            {
                settingsStore.SetValue((ImplicitNullabilitySettings x) => x.Enabled, Ena);
                settingsStore.SetValue((ImplicitNullabilitySettings x) => x.EnableFields, Dis);
            }

            var configuration = GetImplicitNullabilityConfigurationFor(testInput, ChangeSolutionSettings);

            Assert.That(configuration.HasAppliesTo(ImplicitNullabilityAppliesTo.Fields));
            Assert.That(configuration.FieldOptions, Is.EqualTo(expected));
        }

        private ImplicitNullabilityConfiguration GetImplicitNullabilityConfigurationFor(
            string testInput,
            Action<IContextBoundSettingsStore> changeSolutionSettings,
            Action<IContextBoundSettingsStore> changeProjectSettings = null)
        {
            ImplicitNullabilityConfiguration? result = null;

            var testInputFile = GetTestDataFilePath2(testInput + ".cs");
            Assert.That(testInputFile.ExistsFile, Is.True, "fixate that the test input file exists");

            WithSingleProject(
                testInputFile.FullPath,
                (lifetime, solution, project) => RunGuarded(
                    () =>
                    {
                        var sut = solution.GetComponent<ImplicitNullabilityConfigurationEvaluator>();
                        var settingsStore = project.GetComponent<SettingsStore>();

                        changeSolutionSettings.Invoke(
                            settingsStore.BindToContextTransient(ContextRange.ManuallyRestrictWritesToOneContext(solution.ToDataContext())));

                        changeProjectSettings?.Invoke(
                            settingsStore.BindToContextTransient(ContextRange.ManuallyRestrictWritesToOneContext(project.ToDataContext())));

                        result = sut.EvaluateFor(project.GetPsiModules().Single());
                    }));

            // Note that instead of using ExecuteWithinSettingsTransaction(), we change the solution/project settings (to be able to test
            // the usage to the correct DataContext). We close the solution afterwards to isolate the changes (has a small performance hit).
            RunGuarded(() => CloseSolution());

            return result.NotNull();
        }
    }
}
