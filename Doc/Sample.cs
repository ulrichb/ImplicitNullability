using JetBrains.Annotations;

// ReSharper disable UnusedParameter.Local, MemberCanBeMadeStatic.Local, UnusedMember.Global

public class Sample
{
    public void Caller()
    {
        MethodWithImplicitNullability(null, null);
    }


    private void MethodWithImplicitNullability(string a, [CanBeNull] string b)
    {
    }
}