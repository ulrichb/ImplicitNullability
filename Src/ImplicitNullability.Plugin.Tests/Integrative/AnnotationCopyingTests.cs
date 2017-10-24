using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using ImplicitNullability.Plugin.Tests.Infrastructure;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Intentions.CSharp.ContextActions;
using JetBrains.ReSharper.Intentions.CSharp.QuickFixes;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework;
using NUnit.Framework;

namespace ImplicitNullability.Plugin.Tests.Integrative
{
    public static class AnnotationCopyingTests
    {
        // This fixture proves that with enabled Implicit Nullability the [NotNull] annotations are *not* copied.

        private static readonly string CommonRelativeTestDataPath = Path.Combine(nameof(Integrative), nameof(AnnotationCopyingTests));

        [TestFixture]
        [TestNetFramework4]
        public class CheckParamNullActionTest : CSharpContextActionExecuteTestBase<CheckParamNullAction>
        {
            protected override string RelativeTestDataPath => CommonRelativeTestDataPath;

            [ExcludeFromCodeCoverage]
            protected override string ExtraPath => throw new NotSupportedException();

            [Test]
            public void CheckParamNullAction() => DoNamedTest();

            [Test]
            public void CheckParamNullActionWithEnabledImplicitNullability() => WithEnabledImplicitNullability(this, () => DoNamedTest());
        }

        [TestFixture]
        [TestNetFramework4]
        public class GenerateConstructorFixTest : CSharpQuickFixTestBase<GenerateConstructorFix>
        {
            protected override string RelativeTestDataPath => CommonRelativeTestDataPath;

            [Test]
            public void GenerateConstructor() => DoNamedTest();

            [Test]
            public void GenerateConstructorWithEnabledImplicitNullability() => WithEnabledImplicitNullability(this, () => DoNamedTest());
        }

        private static void WithEnabledImplicitNullability(BaseTest baseTest, Action action)
        {
            baseTest.ExecuteWithinSettingsTransaction(settings =>
            {
                baseTest.RunGuarded(() => settings.EnableImplicitNullabilityForAllCodeElements());

                action();
            });
        }
    }
}
