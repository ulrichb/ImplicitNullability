using JetBrains.Annotations;

namespace ImplicitNullability.Samples.CodeWithIN.Highlighting.ImplicitNotNullConflictInHierarchy
{
    public class NonPolymorphicMethods
    {
        public interface IInterface
        {
            void Method([CanBeNull] string a);
        }

        public class Base
        {
            public virtual void Method([CanBeNull] string a)
            {
            }
        }

        public class MethodHiding : Base
        {
            public new void Method(string a /* no warning */)
            {
            }
        }

        public class MethodHidingButInterfaceImplementation : Base, IInterface
        {
            public new void Method(string a /*Expect:ImplicitNotNullConflictInHierarchy[Implicit]*/)
            {
            }
        }

        public class Overload : Base, IInterface
        {
            public void Method(string a /* no warning */, string b)
            {
            }
        }

        public class GenericParameter : Base, IInterface
        {
            public void Method<T>(string a /* no warning */)
            {
                ReSharper.SuppressUnusedWarning(typeof(T));
            }
        }
    }
}
