# Implicit Nullability ReSharper Extension

## Idea
The basic idea of this plugin is to change the behavior of ReSharper's [static nullability analysis](https://www.jetbrains.com/resharper/help/Code_Analysis__Code_Annotations.html) so that specific code elements get a default nullability annotation without specifying an explicit `[NotNull]`/`[CanBeNull]` attribute.

The following rules are used for the implicit nullability value for input and `ref` parameters of methods, delegates and indexers.

* Reference type parameters (without null default value) → implicitly `NotNull`
* Nullable value type parameters and optional parameters with null default value → implicitly `CanBeNull`

Without this plugin the default nullability value is "unknown" which means that ReSharper excludes these elements for its nullability analysis. As a result of the changed default nullability we have to place a `CanBeNull` annotation only for specific code elements (e.g. a reference type parameter, which should be allowed to be `null` in a method invocation) and don't need explicit annotations for the majority of cases (in code bases, which try to reduce passing `null` references to a minimum).

Another idea of this plugin is to bring ReSharper's static analysis in sync with the implicit null checks (weaving of `ArgumentNullException`s into assemblies) of [Fody NullGuard](https://github.com/Fody/NullGuard#readme), which has the same rules for method and indexer parameters. Using this [Fody](https://github.com/Fody/Fody#readme) weaver, you get runtime checks in addition to ReSharper's static analysis.

## Code inspection warnings

In addition to the behavior change of the nullability analysis the following code inspection warnings are provided by this plugin.

* "Implicit NotNull conflicts with nullability in super type" (Id: `ImplicitNotNullConflictInHierarchy`)
* "Implicit NotNull overrides unknown nullability of external code" (Id: `ImplicitNotNullOverridesUnknownExternalMember`)
* "Implicit CanBeNull parameter has an explicit NotNull annotation" (Id: `NotNullOnImplicitCanBeNull`)

For more information about these warnings (and its motivation) see their description in the ReSharper *Code Inspection | Inspection Severity* options page.

## Configuration

The implicit nullability can be enabled/disabled in the *Code Inspection | Implicit Nullability* options page. This can also be set per project by manually adding a `csproj.DotSettings` next to the `csproj` file.
