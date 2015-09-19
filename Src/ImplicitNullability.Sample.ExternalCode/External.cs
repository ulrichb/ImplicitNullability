using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ImplicitNullability.Sample.ExternalCode
{
    public static class External
    {
        public delegate void SomeDelegate(string a);

        public delegate void SomeNotNullDelegate([NotNull] string a);

        public delegate string SomeFunctionDelegate();

        public class Class
        {
            public virtual string this[string a]
            {
                get { return null; }
            }

            public virtual void Method(string a)
            {
            }

            public virtual string Function()
            {
                return null;
            }

            public virtual async Task<string> AsyncFunction()
            {
                await Task.Delay(0);
                return null;
            }
        }

        public interface IInterfaceWithMethod<in T>
        {
            void Method(T a);
        }

        public interface IFunctionWithMethod<out T>
        {
            T Function();
        }

        public interface IInterfaceWithCanBeNullMethod
        {
            void Method([CanBeNull] string a);
        }

        public interface IInterfaceWithNotNullMethod
        {
            void Method([NotNull] string a);
        }

        public interface IInterfaceWithRefAndOutParameterMethod
        {
            void Method(ref string refParam, out string outParam);
        }
    }
}