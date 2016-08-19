using System;
using JetBrains.Annotations;

// ReSharper disable UnusedParameter.Local, MemberCanBeMadeStatic.Local, UnusedMember.Global

public class Sample
{
    public void Foo()
    {
        var str = Bar(null, null);

        Console.WriteLine(str ?? "");
    }


    public string Bar(string a, [CanBeNull] string b, string c = null)
    {
        return null;
    }
}