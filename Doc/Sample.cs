using System;
using ImplicitNullability.Samples.CodeWithoutIN;
using JetBrains.Annotations;

// ReSharper disable CheckNamespace, UnusedParameter.Local, MemberCanBeMadeStatic.Local, UnusedMember.Global, UnusedMember.Local, ArrangeTypeModifiers, ArrangeTypeMemberModifiers

public class Sample
{
    public void Foo()
    {
        var str = Bar(null, null);

        Console.WriteLine(str ?? "n/a");
    }


    public string Bar(string a, [CanBeNull] string b, string c = null)
    {
        return null;
    }
}

class ImplicitNotNullConflictInHierarchy
{
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
}

class ImplicitNotNullElementCannotOverrideCanBeNull
{
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
}

class ImplicitNotNullOverridesUnknownBaseMemberNullability
{
    class Derived : External.Class /* (with unannotated 'Method' and 'Function') */
    {
        // "Implicit NotNull overrides unknown nullability of base member, nullability should be explicit":
        public override void Method(string a)
        {
        }

        // "Implicit NotNull result or out parameter overrides unknown nullability of base member,
        // nullability should be explicit":
        public override string Function() => "";
    }
}

class NotNullOnImplicitCanBeNull
{
    // "Implicit CanBeNull element has an explicit NotNull annotation":
    void Foo([NotNull] int? a)
    {
    }
}
