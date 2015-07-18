using System;
using JetBrains.Annotations;

namespace ImplicitNullability.Sample.Highlighting.ImplicitNotNullConflictInHierarchy
{
    public class NonPolymorphicMethods
    {
        public interface IInterface
        {
            void SomeMethod([CanBeNull] string a);
        }

        public class Base
        {
            public virtual void SomeMethod([CanBeNull] string a)
            {
            }
        }

        public class MethodHiding : Base
        {
            public new void SomeMethod(string a)
            {
            }
        }

        public class MethodHidingButInterfaceImplementation : Base, IInterface
        {
            public new void SomeMethod(string a /*Expect:ImplicitNotNullConflictInHierarchy*/)
            {
            }
        }

        public class Overload : Base, IInterface
        {
            public void SomeMethod(string a, string b)
            {
            }
        }

        public class GenericParameter : Base, IInterface
        {
            public void SomeMethod<T>(string a)
            {
            }
        }
    }
}