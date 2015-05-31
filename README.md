# Implicit Nullability ReSharper Extension

[![Build status](https://ci.appveyor.com/api/projects/status/qldjrfvj090h8b0q/branch/master?svg=true)](https://ci.appveyor.com/project/ulrichb/implicitnullability/branch/master)

## Idea
The basic idea of this extension is to change the behavior of ReSharper's [static nullability analysis](https://www.jetbrains.com/resharper/help/Code_Analysis__Code_Annotations.html) so that specific code elements get a default nullability annotation without specifying an explicit `[NotNull]` / `[CanBeNull]` attribute. For example, reference types (in method parameters, ...) are by default `[NotNull]` (→ they need an opt-in `[CanBeNull]` to become nullable).

![code sample](https://github.com/ulrichb/ImplicitNullability/blob/master/Doc/Sample.png)

The following rules are used for the implicit nullability value for input and `ref` parameters of methods, delegates and indexers.

* Reference type parameters (without null default value) → implicitly `[NotNull]`
* Nullable value type parameters and optional parameters with null default value → implicitly `[CanBeNull]`

Without this extension the default nullability value is "unknown" which means that ReSharper excludes these elements for its nullability analysis. As a result of the changed default nullability we have to place a `[CanBeNull]` annotation only for specific code elements (e.g. a reference type parameter which should be allowed to be `null` in a method invocation) and don't need explicit annotations for the majority of cases (in code bases which try to reduce passing `null` references to a minimum).

Another idea of this extension is to bring ReSharper's static analysis in sync with the implicit null checks (weaving of `ArgumentNullException`s into assemblies) of [Fody NullGuard](https://github.com/Fody/NullGuard#readme) which has the same rules for method and indexer parameters. Using this [Fody](https://github.com/Fody/Fody#readme) weaver, you get runtime checks in addition to ReSharper's static analysis.

## Code inspection warnings

In addition to the behavior change of the nullability analysis the following code inspection warnings are provided by this extension.

* "Implicit NotNull conflicts with nullability in super type" (`ImplicitNotNullConflictInHierarchy`)
* "Implicit NotNull overrides unknown nullability of external code" (`ImplicitNotNullOverridesUnknownExternalMember`)
* "Implicit CanBeNull parameter has an explicit NotNull annotation" (`NotNullOnImplicitCanBeNull`)

For more information about these warnings (and their motivation) see their description in the ReSharper *Code Inspection | Inspection Severity* options page.

## Configuration

Implicit nullability can be enabled/disabled for specific syntax elements in the *Code Inspection | Implicit Nullability* options page. This can also be set per project by manually adding a [`csproj.DotSettings` next to the `csproj` file](https://blog.jetbrains.com/dotnet/2012/01/18/per-project-settings-or-how-to-have-different-naming-styles-for-my-test-project/).

:warning: After changing these settings, [cleaning the solution cache](https://www.jetbrains.com/resharper/help/Configuring_Caches_Location.html#dynaProc1) is necessary to update already analyzed code.
