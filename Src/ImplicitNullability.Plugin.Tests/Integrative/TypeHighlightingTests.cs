using System.IO;
using ImplicitNullability.Plugin.Configuration;
using ImplicitNullability.Plugin.Tests.Infrastructure;
using ImplicitNullability.Plugin.TypeHighlighting;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;

namespace ImplicitNullability.Plugin.Tests.Integrative
{
    [Category("Type Highlighting")]
    public abstract class TypeHighlightingTests : CSharpHighlightingTestBase
    {
        // Use the ReSharper SDK's gold-file approach instead of InspectionExpectationCommentsTestBase because
        // atm. it doesn't support "static highlightings" or VISIBLE_DOCUMENT-only highlightings.

        protected override string RelativeTestDataPath => Path.Combine(base.RelativeTestDataPath, "..", "TypeHighlightingTests");

        public class TypeHighlightingTestsDefault : TypeHighlightingTests
        {
            [Test]
            public void TestTypeHighlightingMethodsSample() => DoNamedTest2();

            [Test]
            public void TestTypeHighlightingAsyncMethodsSample() => DoNamedTest2("Async.cs");

            [Test]
            public void TestTypeHighlightingTaskResultDelegatesSample() => DoNamedTest2();

            [Test]
            public void TestTypeHighlightingOtherElementsSample() => DoNamedTest2();

            [Test]
            public void TestTypeHighlightingWithGeneratedCode_Generated() =>
                DoTestSolution(TestName2.Replace('_', '.'), "TypeHighlightingWithGeneratedCode.partial.cs", "Async.cs");

            [Test]
            public void TestTypeHighlightingWithGeneratedCode_partial() =>
                DoTestSolution(TestName2.Replace('_', '.'), "TypeHighlightingWithGeneratedCode.Generated.cs", "Async.cs");
        }

        public class TypeHighlightingTestsWithDisabledSetting : TypeHighlightingTests
        {
            [Test]
            public void TestTypeHighlightingMethodsSample() => DoNamedTest2();

            protected override void ChangeSettings(IContextBoundSettingsStore store)
            {
                base.ChangeSettings(store);
                store.SetValue((ImplicitNullabilitySettings s) => s.EnableTypeHighlighting, false);
            }

            protected override string GetGoldTestDataPath([NotNull] string fileName) =>
                base.GetGoldTestDataPath(fileName + ".DisabledSetting");
        }

        public class TypeHighlightingTestsWithInvalidDeclarations : TypeHighlightingTests
        {
            [Test]
            public void TestTypeHighlightingInvalidDeclarationsSample() => DoNamedTest2("Async.cs");

            protected override bool HighlightingPredicate(IHighlighting highlighting, [CanBeNull] IPsiSourceFile sourceFile) =>
                highlighting is StaticNullabilityTypeHighlightingBase; // Do not render errors
        }

        protected override bool HighlightingPredicate([NotNull] IHighlighting highlighting, [CanBeNull] IPsiSourceFile sourceFile) =>
            highlighting is StaticNullabilityTypeHighlightingBase || base.HighlightingPredicate(highlighting, sourceFile);

        protected override void DoTestSolution([NotNull] params string[] fileSet)
        {
            ExecuteWithinSettingsTransaction(store =>
            {
                RunGuarded(() => ChangeSettings(store));
                base.DoTestSolution(fileSet);
            });
        }

        protected virtual void ChangeSettings(IContextBoundSettingsStore store)
        {
            store.EnableImplicitNullabilityForAllCodeElements();
        }
    }
}
