using System;
using System.Reflection;

#if RESHARPER8
using JetBrains.Application.PluginSupport;

#endif

[assembly: AssemblyTitle(AssemblyConsts.Title)]
[assembly: AssemblyVersion("0.0.0.1")]
[assembly: AssemblyFileVersion("0.0.0.1")]

#if RESHARPER8
// The following information is displayed by ReSharper in the Plugins dialog

[assembly: PluginTitle(AssemblyConsts.Title)]
[assembly: PluginDescription(AssemblyConsts.Title)]
#endif

// ReSharper disable once CheckNamespace
internal static class AssemblyConsts
{
    public const string Title =
        "Implicit Nullability ReSharper Plugin"
#if DEBUG
        + " (Debug Build)"
#endif
        ;
}