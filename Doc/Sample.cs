using System;
using JetBrains.Annotations;

// ReSharper disable CheckNamespace, UnusedParameter.Local, MemberCanBeMadeStatic.Local, UnusedMember.Global

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