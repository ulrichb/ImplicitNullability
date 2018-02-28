using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.Platform.VisualStudio.SinceVs10.Shell.Zones;

namespace ImplicitNullability.Plugin.VsFormatDefinitions
{
    [ZoneMarker]
    public class ZoneMarker : IRequire<ISinceVs10Zone>
    {
    }
}
