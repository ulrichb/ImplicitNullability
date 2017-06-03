using System.Reflection;

// These attributes will be ignored:
[assembly: AssemblyMetadata("ImplicitNullability.AppliesTo", "OutParametersAndResult")]
[assembly: AssemblyMetadata("ImplicitNullability.AppliesTo", "InputParameters")]

// This one will win the fight:
[assembly: AssemblyMetadata("ImplicitNullability.AppliesTo", "InputParameters, Fields")]
