# Design decisions

## Implicit nullability of delegates and methods in Razor/ASP.NET files (FS, UB on 02.07.2014)

The methods arguments *are* implicitly nullable. -> Principle: "Wherever a CanBeNull/NotNull annotation can be added by the developer, implicit nullability is applicable".

## Disadvantages

* Doesn't match Fody NullGuard plugin. But: Static analysis doesn't need to completely match runtime null checks added by weaver; the main purpose of the rewriter is a good API exception contract paired with fail fast. Developer must know details of NullGuard anyway, and it's clear that NullGuard can help only this far (with realistic effort).
* Delegates: doesn't match anonymous methods/lambdas. But: Delegates are invocable, and it makes sense to make use of static analysis at least at call sites. Additionally, with lambdas/anonymous methods, C# doesn't allow adding CanBeNull/NotNull attributes for parameters, but it does allow on delegate declarations.

## Advantages

* Symmetry with "regular" method parameters
* Simple rule, easy to understand
* For input parameters it makes the calls *safer*, not more unsafe (i.e., safer default than "Unknown" nullability)

Note that ReSharper does not check nullability constraints when binding methods to delegates. For example, the following code does not produce a warning.

```C#
public delegate void SomeDelegate ([CanBeNull] string s);

private static void M (/*[NotNull]*/ string s) // Explicit or implicit NotNull doesn't make a difference
{
   Console.WriteLine (s.Length); // NullReferenceException when called with s == null
}

SomeDelegate d = M;
d(null); // Problem: Static analysis allows calling d with null, although the method M does not
```

Therefore, the user is responsible for checking nullability constraints when creating delegates bound to methods.

# ImplicitNullability.Plugin.Tests project

The ReSharper "integrative" tests don't use the "gold file approach" of the ReSharper SDK, but use annotations directly in the source files for the expectations (e.g. `/*Expect:AssignNullToNotNullAttribute[RS >= 92 && MOut]*/`). These expectations are matched with the actual inspection warnings when running the analysis during test execution.

The advantages of this approach:
* The static analysis expectations are directly next to the "runtime analysis" expectations (see `ImplicitNullability.Samples.Consumer` projects).
* Conditional expectations.
* Expectations also visible during manual tests.
* Better editing experience (no duplication of the test data) and better readability (less indirections).

## ExternalAnnotations directory

Includes specific external annotations, e.g. for TemplateControl.Eval(), because the external annotation files
aren't included in the SDK package.

## Test data

Test data used for the integrative tests (in ImplicitNullability.Plugin.Tests) and for manual tests consists of the following projects:
* Samples.CodeWithoutIN.External: Simulates external code which does not use Implicit Nullability (unannotated  members have default nullability).
* Samples.CodeWithIN.Internal: Simulates a library project which is part of the own solution with configured Implicit Nullability.
* Samples.Consumer.OfInternalCodeWithIN: Code which uses Samples.CodeWithIN.Internal _as project reference_ (simulates a consumer of internal IN code).
* Samples.CodeWithIN.External: Simulates an external library with configured Implicit Nullability. Contains parts of Samples.CodeWithIN.Internal + an `[AssemblyMetadata]` configuration.
* Samples.Consumer.OfExternalCodeWithIN: Code which uses Samples.CodeWithIN.External _as assembly reference_ (simulates a consumer of external IN code).

```plain
                                +----------------------------------+
                                |  Samples.CodeWithoutIN.External  |
                                +----------------+-----------------+
                                                 ^
                                                 |
                         +-----------------------+----------------------+
                         |                                              |
 /-----------------------------------------------\                      |
 |                       |                       |                      |
 |       +---------------+---------------+       |      +---------------+---------------+
 |       |  Samples.CodeWithIN.Internal  |       |      |  Samples.CodeWithIN.External  |
 |       +---------------+---------------+       |      +---------------+---------------+
 |                       ^                       |                      ^
 |                       |                       \----------------------------------------------\
 |                       |                                              |                       |
 |  +--------------------+--------------------+    +--------------------+--------------------+  |
 |  |  Samples.Consumer.OfInternalCodeWithIN  |    |  Samples.Consumer.OfExternalCodeWithIN  |  |
 |  +-----------------------------------------+    +-----------------------------------------+  |
 |                                                                                              |
 | ImplicitNullability.Sample.sln                                                               |
 \----------------------------------------------------------------------------------------------/
```

# Local development / manual testing

1. Install ReSharper into an `Exp` VisualStudio instance.
2. Install the extension package (`Build.ps1` artifact).
3. Copy the following property into `ImplicitNullability.Plugin.RXX.csproj.user`.

```xml
<PropertyGroup>
  <HostFullIdentifier>ReSharperPlatformVs(version of VS)Exp</HostFullIdentifier>
</PropertyGroup>
```
