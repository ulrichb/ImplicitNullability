using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework;
using JetBrains.TestFramework.Application.Zones;
using NUnit.Framework;
using ImplicitNullability.Plugin.Tests;

namespace ImplicitNullability.Plugin.Tests
{
    [ZoneDefinition]
    public interface IImplicitNullabilityTestEnvironmentZone : ITestsZone, IRequire<PsiFeatureTestZone>
    {
    }

    [ZoneMarker]
    public class ZoneMarker : IRequire<IImplicitNullabilityTestEnvironmentZone>
    {
    }
}

// ReSharper disable once CheckNamespace
[SetUpFixture]
public class TestEnvironmentSetUpFixture : ExtensionTestEnvironmentAssembly<IImplicitNullabilityTestEnvironmentZone>
{
}