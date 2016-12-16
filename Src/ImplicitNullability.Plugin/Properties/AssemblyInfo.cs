using System.Reflection;

[assembly: AssemblyTitle(AssemblyConsts.Title)]
[assembly: AssemblyDescription(AssemblyConsts.Description)]
//

[assembly: AssemblyVersion("0.0.1.0")]
[assembly: AssemblyFileVersion("0.0.1.0")]
[assembly: AssemblyInformationalVersion("0.0.1.0-dev")]
//

[assembly: AssemblyMetadata("ImplicitNullability.AppliesTo", "InputParameters, RefParameters, OutParametersAndResult")]

// ReSharper disable once CheckNamespace
internal static class AssemblyConsts
{
    public const string Title =
        "Implicit Nullability"
#if DEBUG
        + " (Debug Build)"
#endif
        ;

    // Note: Text is duplicated in the nuspec.
    public const string Description =
        "Extends ReSharper's static analysis by changing specific, configurable elements to be [NotNull] by default";
}