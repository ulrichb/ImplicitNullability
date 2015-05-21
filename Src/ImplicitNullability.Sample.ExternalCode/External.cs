using System;
using JetBrains.Annotations;

namespace ImplicitNullability.Sample.ExternalCode
{
  public static class External
  {
    public delegate void SomeDelegate (string a);

    public delegate void SomeNotNullDelegate ([NotNull] string a);

    public abstract class Class
    {
      public virtual string this [string a]
      {
        get { return null; }
      }

      public virtual void SomeMethod (string a, string b)
      {
      }
    }

    public interface IInterface<in T>
    {
      void SomeMethod (T a);
    }

    public interface IInterfaceWithCanBeNullParameter
    {
      void SomeMethod ([CanBeNull] string a);
    }

    public interface IInterfaceWithNotNullParameter
    {
      void SomeMethod ([NotNull] string a);
    }

    public interface IInterfaceWithRefAndOutParameter
    {
      void SomeMethod (ref string a, out string b);
    }
  }
}