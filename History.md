### vNext ###
- ReSharper and Rider 2018.1 support

### 4.3.0 ###
- Rider 2017.3 support

### 4.2.0 ###
- ReSharper 2017.3 support

### 4.1.0 ###
- Added support for Rider 2017.2

### 4.0.1 ###
- Fixed unnecessary nullability attribute generation in c'tor generation and "Check parameter for null" actions [ReSharper 2017.2.2+]

### 4.0.0 ###
- Added support for properties (by default configured for getter-only properties) => Implicit Nullability now finally supports all members where ReSharper provides nullability analysis :)
- ReSharper 2017.2 support

### 3.7.0 ###
- Changed default of "Restrict to readonly fields" setting to "on"
- Added exclusion of generated code, configurable by a new setting (enabled by default)
- Added exclusion of "XAML fields" (fixes issue #11)

### 3.6.0 ###
- Changed "overrides unknown base member" warnings to include also non-external (solution) code with unknown nullability (=> new highlighting IDs for ImplicitNotNullOverridesUnknownBaseMemberNullability and ImplicitNotNullResultOverridesUnknownBaseMemberNullability)
- Added support for named delegates with (async) Task<T> results
- Added cache for configuration (attribute) parsing (performance improvement)
- ReSharper 2017.1 support

### 3.5.1 ###
- Implicit nullable fields: Added exemption for property backing fields (issue #10)

### 3.5.0 ###
- Added support for fields (including the option to restrict to `readonly` fields / fields in reference types)
- Extended "type highlighting" for fields and properties
- Fixed wrong highlighting of async void method results (issue #8)

### 3.3.0 ###
- Explicit or implicit [NotNull] element types are now highlighted with a dotted underline (can be enabled/disabled on the Implicit Nullability options page)
- Ignore methods with [ContractAnnotation] attribute

### 3.2.0 ###
- ReSharper 2016.3 support

### 3.1.0 ###
- ReSharper 2016.2 support

### 3.0.0 ###
- Implicit Nullability support for compiled assemblies with [AssemblyMetadata("ImplicitNullability.AppliesTo", "...")] configuration attribute
- New "Implicit NotNull element cannot override CanBeNull in base type, nullability should be explicit" warning for return values and out parameters
- New "Implicit NotNull result or out parameter overrides unknown nullability of external code" hint
- Added suppression of "Base declaration has the same annotation" highlighting on code elements with enabled implicit nullability
- Added exclusion of delegate BeginInvoke() method parameters and Invoke() / EndInvoke() results because their implicit nullability cannot be overridden with explicit annotations
- ReSharper 2016.1 support

### 2.2.0 ###
- ReSharper 10.0 support
- Added configuration option in ReSharper's "Products & Features" settings
- Dropped ReSharper 8.2 support

### 2.1.0 ###
- Support for Task<T> method return types (big thanks to Ivan Serduk) [ReSharper 9.2+]
- Fixed extension meta data to enable running Implicit Nullability in InspectCode (ReSharper Command Line Tools)

### 2.0.0 ###
- Added support for method / delegate results and out parameters (can be enabled / disabled via a new option)
- Split option for method / delegate / indexer input and ref parameters into two separate options
- ReSharper 9.2 support
