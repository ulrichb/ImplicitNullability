using System;

namespace ImplicitNullability.Plugin.Configuration
{
    /// <summary>
    /// A flags enum representing the property options (detailed configuration for properties).
    /// </summary>
    [Flags]
    public enum ImplicitNullabilityPropertyOptions
    {
        NoOption = 0,
        RestrictToGetterOnly = 1,
        RestrictToReferenceTypes = 2,
    }
}
