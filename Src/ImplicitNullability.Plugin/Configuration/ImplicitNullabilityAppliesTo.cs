using System;

namespace ImplicitNullability.Plugin.Configuration
{
    /// <summary>
    /// A flags enum representing the selected / configured code elements where Implicit Nullability should apply to.
    /// </summary>
    [Flags]
    public enum ImplicitNullabilityAppliesTo
    {
        None = 0,
        InputParameters = 1,
        RefParameters = 2,
        OutParametersAndResult = 4,
        Fields = 8,
    }
}
