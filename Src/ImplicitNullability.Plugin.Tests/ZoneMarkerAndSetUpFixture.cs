using ImplicitNullability.Plugin.Tests;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework;
using NUnit.Framework;
#if RS20181
using ITestsEnvZone = JetBrains.TestFramework.Application.Zones.ITestsZone;
#else
using JetBrains.TestFramework.Application.Zones;
#endif

[assembly: RequiresSTA]

namespace ImplicitNullability.Plugin.Tests
{
    [ZoneDefinition]
    public interface IImplicitNullabilityTestEnvironmentZone : ITestsEnvZone, IRequire<PsiFeatureTestZone>, IRequire<IImplicitNullabilityZone>
    {
    }

    [ZoneMarker]
    public class ZoneMarker : IRequire<IImplicitNullabilityTestEnvironmentZone>
    {
    }
}

// Note: Global namespace to workaround (or hide) https://youtrack.jetbrains.com/issue/RSRP-464493.
[SetUpFixture]
public class TestEnvironmentSetUpFixture : ExtensionTestEnvironmentAssembly<IImplicitNullabilityTestEnvironmentZone>
{
}
