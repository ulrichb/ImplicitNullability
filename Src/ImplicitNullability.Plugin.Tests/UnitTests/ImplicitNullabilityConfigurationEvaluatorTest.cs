using System;
using System.Linq;
using ImplicitNullability.Plugin.Settings;
using JetBrains.Application.Settings;
using JetBrains.Application.Settings.Store.Implementation;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.TestFramework;
using JetBrains.Util;
using NUnit.Framework;

namespace ImplicitNullability.Plugin.Tests.UnitTests
{
    [TestFixture]
    [TestNetFramework45 /*necessary for AssemblyMetadataAttribute reference*/]
    public class ImplicitNullabilityConfigurationEvaluatorTest : BaseTestWithSingleProject
    {
        private const bool Dis = false; // disable
        private const bool Ena = true; // enable
        private const bool Dnc = false; // don't care

        [Test]
        // 1. Enabled=false in the settings overrules everything:
        [TestCase( /*solution:*/ Dis, /*project:*/ Dis, /*options:*/ Ena, Ena, Ena, "AttributeWithAllOptions", /*expected:*/ Dis, Dis, Dis)]
        //
        // 2. Project settings overrule solution settings:
        [TestCase( /*solution:*/ Dis, /*project:*/ Ena, /*options:*/ Dnc, Dnc, Dnc, "AttributeWithAllOptions", /*expected:*/ Ena, Ena, Ena)]
        [TestCase( /*solution:*/ Ena, /*project:*/ Dis, /*options:*/ Dnc, Dnc, Dnc, "AttributeWithAllOptions", /*expected:*/ Dis, Dis, Dis)]
        //
        // 3. Attributes overrule individual options in the settings:
        [TestCase( /*solution:*/ Dnc, /*project:*/ Ena, /*options:*/ Ena, Ena, Ena, "AttributeWithAllOptions", /*expected:*/ Ena, Ena, Ena)]
        [TestCase( /*solution:*/ Dnc, /*project:*/ Ena, /*options:*/ Ena, Ena, Ena, "AttributeWithNoOption", /*expected:*/ Dis, Dis, Dis)]
        //
        // 4. No attribute configuration => take the individual options in the settings:
        [TestCase( /*solution:*/ Dnc, /*project:*/ Ena, /*options:*/ Ena, Ena, Ena, "NoConfigurationAttribute", /*expected:*/ Ena, Ena, Ena)]
        [TestCase( /*solution:*/ Dnc, /*project:*/ Ena, /*options:*/ Dis, Dis, Dis, "NoConfigurationAttribute", /*expected:*/ Dis, Dis, Dis)]
        public void TestInheritance(
            bool enabledInSolution,
            bool enabledInProject,
            bool enableInputParameters,
            bool enableRefParameters,
            bool enableOutParametersAndResult,
            string testInput,
            bool expectedEnableInputParameters,
            bool expectedEnableRefParameters,
            bool expectedEnableOutParametersAndResult)
        {
            Action<IContextBoundSettingsStore> changeSolutionSettings = settingsStore =>
                settingsStore.SetValue((ImplicitNullabilitySettings x) => x.Enabled, enabledInSolution);

            Action<IContextBoundSettingsStore> changeProjectSettings = settingsStore =>
            {
                settingsStore.SetValue((ImplicitNullabilitySettings x) => x.Enabled, enabledInProject);
                settingsStore.SetValue((ImplicitNullabilitySettings x) => x.EnableInputParameters, enableInputParameters);
                settingsStore.SetValue((ImplicitNullabilitySettings x) => x.EnableRefParameters, enableRefParameters);
                settingsStore.SetValue((ImplicitNullabilitySettings x) => x.EnableOutParametersAndResult, enableOutParametersAndResult);
            };

            //

            var configuration = GetImplicitNullabilityConfigurationFor(testInput, changeSolutionSettings, changeProjectSettings);

            //

            AssertImplicitNullabilityConfiguration(
                configuration,
                expectedEnableInputParameters,
                expectedEnableRefParameters,
                expectedEnableOutParametersAndResult);
        }

        [Test]
        // The following attributes override the settings:
        [TestCase("AttributeWithAllOptions", /*expected:*/ Ena, Ena, Ena)]
        [TestCase("AttributeWithOnlyInputParametersOption", /*expected:*/ Ena, Dis, Dis)]
        [TestCase("AttributeWithNoOption", /*expected:*/ Dis, Dis, Dis)]
        [TestCase("AttributeWithInvalidValue", /*expected:*/ Dis, Dis, Dis)]
        [TestCase("AttributeWithValueContainingInvalidOption", /*expected:*/ Ena, Dis, Ena)]
        [TestCase("MultipleAttributes", /*expected:*/ Ena, Ena, Ena)]
        //
        // The following non-existing / invalid attributes are overridden by the settings:
        [TestCase("NoConfigurationAttribute", /*expected:*/ Ena, Dis, Ena)]
        [TestCase("AttributeWithNullKey", /*expected:*/ Ena, Dis, Ena)]
        [TestCase("AttributeWithNullValue", /*expected:*/ Ena, Dis, Ena)]
        [TestCase("NonCompilingAttribute1", /*expected:*/ Ena, Dis, Ena)]
        [TestCase("NonCompilingAttribute2", /*expected:*/ Ena, Dis, Ena)]
        [TestCase("NonCompilingAttribute3", /*expected:*/ Ena, Dis, Ena)]
        [TestCase("NonCompilingAttribute4", /*expected:*/ Ena, Dis, Ena)]
        public void TestAssemblyMetadataAttributeConfiguration(
            string testInput,
            bool expectedEnableInputParameters,
            bool expectedEnableRefParameters,
            bool expectedEnableOutParametersAndResult)
        {
            Action<IContextBoundSettingsStore> changeSolutionSettings = settingsStore =>
            {
                settingsStore.SetValue((ImplicitNullabilitySettings x) => x.Enabled, Ena);
                settingsStore.SetValue((ImplicitNullabilitySettings x) => x.EnableInputParameters, Ena);
                settingsStore.SetValue((ImplicitNullabilitySettings x) => x.EnableRefParameters, Dis);
                settingsStore.SetValue((ImplicitNullabilitySettings x) => x.EnableOutParametersAndResult, Ena);
            };

            var configuration = GetImplicitNullabilityConfigurationFor(testInput, changeSolutionSettings);

            AssertImplicitNullabilityConfiguration(
                configuration,
                expectedEnableInputParameters,
                expectedEnableRefParameters,
                expectedEnableOutParametersAndResult);
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

        private static void AssertImplicitNullabilityConfiguration(
            ImplicitNullabilityConfiguration actual,
            bool expectedEnableInputParameters,
            bool expectedEnableRefParameters,
            bool expectedEnableOutParametersAndResult)
        {
            Assert.That(new
            {
                actual.EnableInputParameters,
                actual.EnableRefParameters,
                actual.EnableOutParametersAndResult
            }, Is.EqualTo(new
            {
                EnableInputParameters = expectedEnableInputParameters,
                EnableRefParameters = expectedEnableRefParameters,
                EnableOutParametersAndResult = expectedEnableOutParametersAndResult
            }));
        }
    }
}
