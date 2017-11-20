using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;

namespace ImplicitNullability.Plugin
{
    internal static class PluginConsts
    {
        public const string Title =
                "Implicit Nullability"
#if DEBUG
                + " (Debug Build)"
#endif
            ;

        // Note: Text is duplicated in the nuspec.
        public const string Description =
            "Extends ReSharper's static nullability analysis by changing specific, configurable elements to be [NotNull] by default";
    }


    [ZoneDefinition]
    [ZoneDefinitionConfigurableFeature(PluginConsts.Title, PluginConsts.Description, IsInProductSection: false)]
    public interface IImplicitNullabilityZone : IPsiLanguageZone,
        IRequire<ILanguageCSharpZone>,
        IRequire<DaemonZone>
    {
    }

    [ZoneMarker]
    public class ZoneMarker : IRequire<IImplicitNullabilityZone>
    {
    }
}
