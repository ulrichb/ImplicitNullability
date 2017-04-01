using System;

namespace ImplicitNullability.Plugin.Configuration
{
    /// <summary>
    /// A flags enum representing the field options (detailed configuration for fields).
    /// </summary>
    [Flags]
    public enum ImplicitNullabilityFieldOptions
    {
        None = 0,
        RestrictToReadonly = 1,
        RestrictToReferenceTypes = 2,
    }
}
