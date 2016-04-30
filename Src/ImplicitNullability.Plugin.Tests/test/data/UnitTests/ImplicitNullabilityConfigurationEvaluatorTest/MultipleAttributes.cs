using System.Reflection;

[assembly: AssemblyMetadata("ImplicitNullability.AppliesTo", "InputParameters, RefParameters, OutParametersAndResult")]

// There attributes are ignored:
[assembly: AssemblyMetadata("ImplicitNullability.AppliesTo", "OutParametersAndResult")]
[assembly: AssemblyMetadata("ImplicitNullability.AppliesTo", "InputParameters")]
