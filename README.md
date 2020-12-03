:warning: **End-of-life note:** C# 8+ (VS 2019+) supports [_nullable reference types_](https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/nullable-reference-types) which [supersedes](#c-nullable-reference-types) - and perfectly aligns with - _Implicit Nullability_. Therefore ReSharper/Rider 2020.2 is the last version supported by _Implicit Nullability_.

## Implicit Nullability ReSharper Extension

[![Build status](https://teamcity.jetbrains.com/app/rest/builds/buildType:(id:OpenSourceProjects_ImplicitNullability_Ci),branch:master,running:any/statusIcon.svg)](https://teamcity.jetbrains.com/viewType.html?buildTypeId=OpenSourceProjects_ImplicitNullability_Ci)
[![Gitter](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/ulrichb/ImplicitNullability?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
<a href="https://teamcity.jetbrains.com/viewType.html?buildTypeId=OpenSourceProjects_ImplicitNullability_Ci&branch=master"><img src="https://dl.dropbox.com/s/tz8rhb0ucv3lruo/master-linecoverage.svg" alt="Line Coverage" title="Line Coverage"></a>
<a href="https://teamcity.jetbrains.com/viewType.html?buildTypeId=OpenSourceProjects_ImplicitNullability_Ci&branch=master"><img src="https://dl.dropbox.com/s/4pmdllotpgd46kj/master-branchcoverage.svg" alt="Branch Coverage" title="Branch Coverage"></a>

[ReSharper Gallery Page](https://resharper-plugins.jetbrains.com/packages/ReSharper.ImplicitNullability/)

[History of changes](History.md)

## Idea

The basic idea of this extension is to change the behavior of ReSharper's [static nullability analysis](https://www.jetbrains.com/resharper/help/Code_Analysis__Code_Annotations.html) so that specific code elements get a default nullability annotation without specifying an explicit `[NotNull]` or `[CanBeNull]` attribute. For example, reference types in method parameters are by default `[NotNull]` (→ they need an explicit `[CanBeNull]` to become nullable).

<img src="/Doc/Sample.png" alt="Code Sample" width="647" />

With enabled _Implicit Nullability_ for specific, [configurable](#configuration), syntax elements, the following rules apply.

<!-- duplicated in the options page -->
 * Reference types are by default implicitly `[NotNull]`.
 * Their nullability can be overridden with an explicit `[CanBeNull]` attribute.
 * Optional method parameters with a `null` default value are implicitly `[CanBeNull]`.

In a nutshell, the code showed in the picture above  ...
```C#
public string Bar(string a, [CanBeNull] string b, string c = null)
{
    // ...
}
```
... implicitly becomes ...
```C#
[NotNull]
public string Bar([NotNull] string a, [CanBeNull] string b, [CanBeNull] string c = null)
{
    // ...
}
```

### `[NotNull]` as default

Without this extension the default nullability value is "unknown" which means that ReSharper excludes these elements for its nullability analysis. As a result of the changed default nullability of this extension we have to place `[CanBeNull]` annotations only for _specific_ code elements (e.g. a reference type parameter where it should be allowed to pass `null` as argument) and don't need explicit annotations for the majority of cases (in code bases which try to reduce passing `null` references to a minimum).

### Async method return types (requires ReSharper 9.2)

ReSharper 9.2 [introduced](https://youtrack.jetbrains.com/issue/RSRP-376091) support for nullability analysis for `async` (`Task<T>` return typed) methods. With enabled _Implicit Nullability_ for method return values, `Task<T>` return typed methods also become implicitly `[ItemNotNull]` (ReSharper uses this attribute to refer to the _value_ of `Task<T>`). For nullable `Task<T>` return values, this can be overridden with `[ItemCanBeNull]`.

### Improves static analysis quality

The following example program contains a potential `NullReferenceException` in `command​.Equals​("Hello")` because the programmer missed that `GetCommand()` could also return `null`.

```C#
public static void Main(string[] args)
{
    string command = GetCommand(args);

    if (command.Equals("Hello"))
        Console.WriteLine("Hello World!");
}

private static string GetCommand(string[] args)
{
    if (args.Length < 1)
        return null;

    return args[0];
}
```

With enabled _Implicit Nullability_ this bug would have been detected by ReSharper's static analysis.

 1. ReSharper would warn about returning `null` in `GetCommand()` because this method would be implicitly annotated as `[NotNull]`.
 2. This warning would be solved by the programmer by adding `[CanBeNull]` to `GetCommand()`.
 3. As a consequence of the `[CanBeNull]` attribute, ReSharper would now warn about the potential `NullReferenceException` in the `command​.Equals​("Hello")` call in `Main()`.

### Improves documentation

In the example above _Implicit Nullability_ forces the programmer to fix the missing `[CanBeNull]` attribute on `GetCommand()`. This shows how the number of `[CanBeNull]` annotations will be increased in the code base and therefore doesn't only improve ReSharper's static analysis but also the documentation of method signatures (contracts).

### Fody NullGuard

Another goal of this extension is to bring ReSharper's static analysis in sync with the implicit null checks of [Fody NullGuard](https://github.com/Fody/NullGuard#readme). For example, this [Fody](https://github.com/Fody/Fody#readme) weaver injects `throw new ArgumentNullException​(/*...*/)` statements for method parameters into method bodies using the same rules as _Implicit Nullability_. In other words this weaver adds _runtime checks_ for nullability to ReSharper's _static_ analysis.

### C# Nullable reference types

C# 8 will get [_Nullable reference types_](https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/nullable-reference-types) which brings option types into the C# language. This matches the semantics of _Implicit Nullability_ and goes even further because it extends it to locals, generic parameters, ... because the nullability annotation works on type level.

```C#
[CanBeNull]
string Bar([CanBeNull] string a, string b)
{
    string result = a;
    return result;
}
```
... then corresponds to ...
```C#
string? Bar(string? a, string b)
{
    string? result = a;
    return result;
}
```

This will make _Implicit Nullability_ unnecessary in the future and at the same time a perfect "upgrade path" because a good `[CanBeNull]`-annotated code base will be easily portable to C# 8.

### Differences to ReSharper's "Implicit [NotNull] Value Analysis Mode"

Since version 2016.1 ReSharper supports (in the "internal mode") a comparable feature (see *Code Inspection | Settings*) with the following differences.

_Implicit Nullability_ ...
* ... rules can be enabled / disabled for _specific_ code elements like parameters or (readonly) fields.
* ... supports (project specific) [configuration by code](#code-configuration).
* ... adds additional [warnings](#code-inspection-warnings).
* ... adds [type highlighting](#type-highlighting) for explicit or implicit `[NotNull]` elements.

## Type highlighting

Explicit or implicit `[NotNull]` element types are highlighted with a dotted underline. (See the pink underlines in the `Bar`-method in the sample screenshot [above](#idea).) This helps to recognize all `[NotNull]` elements, especially inferred `[NotNull]` elements from a base class and code elements which are configured as implicitly `[NotNull]`.

The highlighting can be enabled/disabled on the _Implicit Nullability_ options page, and the colors can be configured in Visual Studio's "Fonts and Colors" options.

## Configuration

Implicit nullability can be enabled or disabled for specific syntax elements in the *Code Inspection | Implicit Nullability* options page.

<img src="/Doc/OptionsPage.png" alt="Options Page" />

### Code configuration

_Implicit Nullability_ can also be configured by code using an [`AssemblyMetadataAttribute`](https://msdn.microsoft.com/en-us/library/system.reflection.assemblymetadataattribute.aspx). This has the advantage that the configuration gets compiled into the assembly so that consumers of the assembly with installed _Implicit Nullability_ get the same implicit nullability annotations of the compiled code elements as _within_ the library's solution.

Example:
```C#
[assembly: AssemblyMetadata("ImplicitNullability.AppliesTo",
                            "InputParameters, RefParameters, OutParametersAndResult, Fields, Properties")]
[assembly: AssemblyMetadata("ImplicitNullability.Fields", "RestrictToReadonly")]
[assembly: AssemblyMetadata("ImplicitNullability.Properties", "RestrictToGetterOnly")]
[assembly: AssemblyMetadata("ImplicitNullability.GeneratedCode", "Exclude")]
```

:warning: After changing the settings (either by code or in the options page), [cleaning the solution cache](https://www.jetbrains.com/help/resharper/Configuring_Caches_Location.html#cleanup) is necessary to update already analyzed code.

## Code inspection warnings

In addition to the behavior change of the nullability analysis the following code inspection warnings are provided by this extension.

For more information about these warnings (and their motivation) see their description in the ReSharper *Code Inspection | Inspection Severity* options page.

### "Implicit NotNull conflicts with nullability in base type"
Id: `ImplicitNotNullConflictInHierarchy`

<!-- Doc/Sample.cs -->
```C#
abstract class Base
{
    public abstract void Method([CanBeNull] string a);
}

class Derived : Base
{
    // "Implicit NotNull conflicts with nullability in base type":
    public override void Method(string a)
    {
    }
}
```

### "Implicit NotNull element cannot override CanBeNull in base type, nullability should be explicit"
Id: `ImplicitNotNullElementCannotOverrideCanBeNull`

<!-- Doc/Sample.cs -->
```C#
abstract class Base
{
    [CanBeNull]
    public abstract string Method();
}

class Derived : Base
{
    // "Implicit NotNull element cannot override CanBeNull in base type, nullability should be explicit":
    public override string Method() => "";
}
```

### "Implicit NotNull overrides unknown nullability of base member, nullability should be explicit" resp. hint "Implicit NotNull result or out parameter overrides unknown nullability of base member, nullability should be explicit"
Ids: `ImplicitNotNullOverridesUnknownBaseMemberNullability` resp. `ImplicitNotNullResultOverridesUnknownBaseMemberNullability`

<!-- Doc/Sample.cs -->
```C#
class Derived : External.Class /* (external code with unannotated 'Method' and 'Function') */
{
    // "Implicit NotNull overrides unknown nullability of base member, nullability should be explicit":
    public override void Method(string a)
    {
    }

    // "Implicit NotNull result or out parameter overrides unknown nullability of base member,
    // nullability should be explicit":
    public override string Function() => "";
}
```

### "Implicit CanBeNull element has an explicit NotNull annotation"
Id: `NotNullOnImplicitCanBeNull`

<!-- Doc/Sample.cs -->
```C#
// "Implicit CanBeNull element has an explicit NotNull annotation":
void Foo([NotNull] int? a)
{
}
```

## Credits

Big thanks to [Fabian Schmied](https://github.com/fschmied) for supporting the design and conception of _Implicit Nullability_.
