using JetBrains.Application.BuildScript.Application.Zones;
#if RS20181
using ISinceVs10EnvZone = JetBrains.Platform.VisualStudio.SinceVs10.Shell.Zones.ISinceVs10Zone;
#else
using JetBrains.Platform.VisualStudio.SinceVs10.Shell.Zones;
#endif

namespace ImplicitNullability.Plugin.VsFormatDefinitions
{
    [ZoneMarker]
    public class ZoneMarker : IRequire<ISinceVs10EnvZone>
    {
    }
}
