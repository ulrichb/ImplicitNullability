using System.IO;
using ImplicitNullability.Plugin.Settings;
using ImplicitNullability.Plugin.Tests.Infrastructure;
using ImplicitNullability.Plugin.TypeHighlighting;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;
#if RESHARPER20161
using CSharpHighlightingTestBase = JetBrains.ReSharper.FeaturesTestFramework.Daemon.CSharpHighlightingTestNet45Base;

#else
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;

#endif

namespace ImplicitNullability.Plugin.Tests.Integrative
{
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
            public void TestTypeHighlightingAsyncMethodsSample() => DoNamedTest2();

            [Test]
            public void TestTypeHighlightingOtherElementsSample() => DoNamedTest2();
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

            protected override string GetGoldTestDataPath(string fileName) => base.GetGoldTestDataPath(fileName + ".DisabledSetting");
        }

        public class TypeHighlightingTestsWithInvalidDeclarations : TypeHighlightingTests
        {
            [Test]
            public void TestTypeHighlightingInvalidDeclarationsSample() => DoNamedTest2();

            protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile) =>
                highlighting is StaticNullabilityTypeHighlightingBase; // Do not render errors
        }

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile) =>
            highlighting is StaticNullabilityTypeHighlightingBase || base.HighlightingPredicate(highlighting, sourceFile);

        protected override void DoTestSolution(params string[] fileSet)
        {
            ExecuteWithinSettingsTransaction(store =>
            {
                RunGuarded(() => ChangeSettings(store));
                base.DoTestSolution(fileSet);
            });
        }

        protected virtual void ChangeSettings(IContextBoundSettingsStore store)
        {
            store.EnableImplicitNullabilityWithAllOptions();
        }
    }
}
