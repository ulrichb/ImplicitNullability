using System;
using System.Linq;
using ImplicitNullability.Plugin.Configuration;
using ImplicitNullability.Plugin.Tests.Infrastructure;
using JetBrains.Application.Settings;
using JetBrains.Application.Settings.Implementation;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework.Utils;
using NUnit.Framework;

namespace ImplicitNullability.Plugin.Tests.UnitTests.Configuration
{
    [TestFixture]
    [TestNetFramework45 /*necessary for AssemblyMetadataAttribute reference*/]
    public class ImplicitNullabilityConfigurationEvaluatorTest : BaseTestWithSingleProject
    {
        private const bool Dis = false; // disable
        private const bool Ena = true; // enable
        private const object Dnc = null; // don't care

        const ImplicitNullabilityAppliesTo AppliesToNone = ImplicitNullabilityAppliesTo.None;

        const ImplicitNullabilityAppliesTo AppliesToInputAndFields = ImplicitNullabilityAppliesTo.InputParameters |
                                                                     ImplicitNullabilityAppliesTo.Fields;

        const ImplicitNullabilityAppliesTo AppliesToInputOutputAndProperties = ImplicitNullabilityAppliesTo.InputParameters |
                                                                               ImplicitNullabilityAppliesTo.OutParametersAndResult |
                                                                               ImplicitNullabilityAppliesTo.Properties;

        const ImplicitNullabilityAppliesTo AppliesToAll = ImplicitNullabilityAppliesTo.InputParameters |
                                                          ImplicitNullabilityAppliesTo.RefParameters |
                                                          ImplicitNullabilityAppliesTo.OutParametersAndResult |
                                                          ImplicitNullabilityAppliesTo.Fields |
                                                          ImplicitNullabilityAppliesTo.Properties;

        private static readonly TestRandom TestRandom = TestRandom.CreateWithRandomSeed();

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
        public void TestInheritance(
            bool? enabledInSolution,
            bool enabledInProject,
            bool? enableOptionInProject,
            string testInput,
            ImplicitNullabilityAppliesTo expectedAppliesTo)
        {
            void ChangeSolutionSettings(IContextBoundSettingsStore settingsStore) =>
                settingsStore.SetValue((ImplicitNullabilitySettings x) => x.Enabled, enabledInSolution ?? TestRandom.NextBool());

            void ChangeProjectSettings(IContextBoundSettingsStore settingsStore)
            {
                settingsStore.SetValue((ImplicitNullabilitySettings x) => x.Enabled, enabledInProject);

                enableOptionInProject = enableOptionInProject ?? TestRandom.NextBool();
                settingsStore.SetValue((ImplicitNullabilitySettings x) => x.EnableInputParameters, enableOptionInProject);
                settingsStore.SetValue((ImplicitNullabilitySettings x) => x.EnableRefParameters, enableOptionInProject);
                settingsStore.SetValue((ImplicitNullabilitySettings x) => x.EnableOutParametersAndResult, enableOptionInProject);
                settingsStore.SetValue((ImplicitNullabilitySettings x) => x.EnableFields, enableOptionInProject);
                settingsStore.SetValue((ImplicitNullabilitySettings x) => x.EnableProperties, enableOptionInProject);
            }

            //

            var configuration = GetImplicitNullabilityConfigurationFor(testInput, ChangeSolutionSettings, ChangeProjectSettings);

            //

            Assert.That(configuration.AppliesTo, Is.EqualTo(expectedAppliesTo));
        }

        [Test]
        public void TestInheritance_AppliesToAttributeOverridesAlsoOtherOptions()
        {
            void ChangeSolutionSettings(IContextBoundSettingsStore settingsStore) =>
                settingsStore.EnableImplicitNullability(
                    enableFields: true,
                    fieldsRestrictToReadonly: true,
                    fieldsRestrictToReferenceTypes: true,
                    generatedCode: GeneratedCodeOptions.Exclude);

            var configuration = GetImplicitNullabilityConfigurationFor("AttributeWithAllOptions", ChangeSolutionSettings);

            Assert.That(configuration.AppliesTo, Is.EqualTo(AppliesToAll));
            Assert.That(configuration.FieldOptions, Is.EqualTo(ImplicitNullabilityFieldOptions.NoOption));
            Assert.That(configuration.PropertyOptions, Is.EqualTo(ImplicitNullabilityPropertyOptions.NoOption));
            Assert.That(configuration.GeneratedCode, Is.EqualTo(GeneratedCodeOptions.Include));
        }

        [Test]
        // The following attributes override the settings:
        [TestCase("AttributeWithNoOption", /*expected:*/ AppliesToNone)]
        [TestCase("AttributeWithNoneOption", /*expected:*/ AppliesToNone)]
        [TestCase("AttributeWithInvalidValue", /*expected:*/ AppliesToNone)]
        [TestCase("AttributeWithOnlyInputParametersOption", /*expected:*/ ImplicitNullabilityAppliesTo.InputParameters)]
        [TestCase("AttributeWithAllOptions", /*expected:*/ AppliesToAll)]
        [TestCase("AttributeWithValueContainingInvalidOption", /*expected:*/ AppliesToInputAndFields)]
        [TestCase("AttributeWithValueMultipleRedundantValues", /*expected:*/ AppliesToInputAndFields)]
        [TestCase("MultipleAttributes", /*expected:*/ AppliesToInputAndFields)]
        //
        // The following non-existing / invalid attributes are overridden by the settings:
        [TestCase("NoConfigurationAttribute", /*expected:*/ AppliesToInputOutputAndProperties)]
        [TestCase("AttributeWithNullKey", /*expected:*/ AppliesToInputOutputAndProperties)]
        [TestCase("AttributeWithNullValue", /*expected:*/ AppliesToInputOutputAndProperties)]
        [TestCase("NonCompilingAttribute1", /*expected:*/ AppliesToInputOutputAndProperties)]
        [TestCase("NonCompilingAttribute2", /*expected:*/ AppliesToInputOutputAndProperties)]
        [TestCase("NonCompilingAttribute3", /*expected:*/ AppliesToInputOutputAndProperties)]
        [TestCase("NonCompilingAttribute4", /*expected:*/ AppliesToInputOutputAndProperties)]
        public void TestAssemblyMetadataAttributeConfiguration(string testInput, ImplicitNullabilityAppliesTo expected)
        {
            void ChangeSolutionSettings(IContextBoundSettingsStore settingsStore) =>
                settingsStore.EnableImplicitNullability(enableInputParameters: true, enableOutParametersAndResult: true, enableProperties: true);

            var configuration = GetImplicitNullabilityConfigurationFor(testInput, ChangeSolutionSettings);

            Assert.That(configuration.AppliesTo, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("FieldOptions_NoAttribute", /*expected:*/ ImplicitNullabilityFieldOptions.NoOption)]
        [TestCase("FieldOptions_AttributeWithNullValue", /*expected:*/ ImplicitNullabilityFieldOptions.NoOption)]
        [TestCase("FieldOptions_AttributeWithNoOption", /*expected:*/ ImplicitNullabilityFieldOptions.NoOption)]
        [TestCase("FieldOptions_AttributeWithInvalidValue", /*expected:*/ ImplicitNullabilityFieldOptions.NoOption)]
        [TestCase("FieldOptions_AttributeWithAllOptions", /*expected:*/ ImplicitNullabilityFieldOptions.RestrictToReadonly |
                                                                        ImplicitNullabilityFieldOptions.RestrictToReferenceTypes)]
        public void TestAssemblyMetadataAttribute_FieldOptions(string testInput, ImplicitNullabilityFieldOptions expected)
        {
            void ChangeSolutionSettings(IContextBoundSettingsStore settingsStore) =>
                settingsStore.EnableImplicitNullability();

            var configuration = GetImplicitNullabilityConfigurationFor(testInput, ChangeSolutionSettings);

            Assert.That(configuration.HasAppliesTo(ImplicitNullabilityAppliesTo.Fields));
            Assert.That(configuration.FieldOptions, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("PropertyOptions_NoAttribute", /*expected:*/ ImplicitNullabilityPropertyOptions.NoOption)]
        [TestCase("PropertyOptions_AttributeWithAllOptions", /*expected:*/ ImplicitNullabilityPropertyOptions.RestrictToGetterOnly |
                                                                           ImplicitNullabilityPropertyOptions.RestrictToReferenceTypes)]
        public void TestAssemblyMetadataAttribute_PropertyOptions(string testInput, ImplicitNullabilityPropertyOptions expected)
        {
            void ChangeSolutionSettings(IContextBoundSettingsStore settingsStore) =>
                settingsStore.EnableImplicitNullability();

            var configuration = GetImplicitNullabilityConfigurationFor(testInput, ChangeSolutionSettings);

            Assert.That(configuration.HasAppliesTo(ImplicitNullabilityAppliesTo.Properties));
            Assert.That(configuration.PropertyOptions, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("GeneratedCode_NoAttribute", /*expected:*/ GeneratedCodeOptions.Include)]
        [TestCase("GeneratedCode_AttributeWithInclude", /*expected:*/ GeneratedCodeOptions.Include)]
        [TestCase("GeneratedCode_AttributeWithExclude", /*expected:*/ GeneratedCodeOptions.Exclude)]
        public void TestAssemblyMetadataAttribute_GeneratedCode(string testInput, GeneratedCodeOptions expected)
        {
            void ChangeSolutionSettings(IContextBoundSettingsStore settingsStore) =>
                settingsStore.EnableImplicitNullability();

            var configuration = GetImplicitNullabilityConfigurationFor(testInput, ChangeSolutionSettings);

            Assert.That(configuration.HasAppliesTo(ImplicitNullabilityAppliesTo.InputParameters));
            Assert.That(configuration.GeneratedCode, Is.EqualTo(expected));
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
                        var settingsStore = solution.GetComponent<SettingsStore>()
                            .CreateNestedTransaction(lifetime, "transacted test store");

                        changeSolutionSettings.Invoke(
                            settingsStore.BindToContextTransient(ContextRange.ManuallyRestrictWritesToOneContext(solution.ToDataContext())));

                        changeProjectSettings?.Invoke(
                            settingsStore.BindToContextTransient(ContextRange.ManuallyRestrictWritesToOneContext(project.ToDataContext())));

                        var sut = new ImplicitNullabilityConfigurationEvaluator(settingsStore, solution.GetComponent<ISettingsOptimization>());

                        result = sut.EvaluateFor(project.GetPsiModules().Single());
                    }));

            return result.NotNull();
        }
    }
}
