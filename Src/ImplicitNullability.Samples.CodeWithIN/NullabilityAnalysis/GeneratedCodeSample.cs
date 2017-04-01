using System.CodeDom.Compiler;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis
{
    // Note: NullGuard already excludes [GeneratedCode].

    public static partial class GeneratedCodeSample
    {
        [GeneratedCode("tool", "version")]
        public partial class GeneratedCodeOnType
        {
            public GeneratedCodeOnType /*Expect:NotNullMemberIsNotInitialized[InclGenCode]*/(string a)
            {
                ReSharper.SuppressUnusedWarning(a);
            }

            public string Field;

            public void Method(string a)
            {
            }

            public void MethodExplicit([NotNull] string a)
            {
            }

            partial void PartialMethodWithImplementation(string a);

            public string Function() => null /*Expect:AssignNullToNotNullAttribute[InclGenCode]*/;

            public delegate string SomeDelegate(string s);

            public async Task<string> AsyncFunction() => await Async.CanBeNullResult<string>() /*Expect:AssignNullToNotNullAttribute[InclGenCode]*/;

            public void Consume()
            {
                // Note: Partial methods are excluded also when they have an implementation because the GeneratedCodeAttribute
                // semantically applies to the whole declared element, not just to the declaration. See also the partial
                // method examples in SomeT4GeneratedClass.
                PartialMethodWithImplementation(null /*Expect:AssignNullToNotNullAttribute[InclGenCode]*/);
            }
        }

        public partial class GeneratedCodeOnMember
        {
            [GeneratedCode("tool", "version")]
            public GeneratedCodeOnMember /*Expect:NotNullMemberIsNotInitialized[InclGenCode]*/(string a)
            {
                ReSharper.SuppressUnusedWarning(a);
            }

            [GeneratedCode("tool", "version")]
            public string Field;

            [GeneratedCode("tool", "version")]
            public void Method(string a)
            {
            }

            [GeneratedCode("tool", "version")]
            public void MethodExplicit([NotNull] string a)
            {
            }

            [GeneratedCode("tool", "version")]
            partial void PartialMethodWithImplementationAndAttribute(string a);

            partial void PartialMethodWithImplementationAndNoAttribute(string a);

            [GeneratedCode("tool", "version")]
            public string Function() => null /*Expect:AssignNullToNotNullAttribute[InclGenCode]*/;

            [GeneratedCode("tool", "version")]
            public delegate string SomeDelegate(string s);

            [GeneratedCode("tool", "version")]
            public async Task<string> AsyncFunction() => await Async.CanBeNullResult<string>() /*Expect:AssignNullToNotNullAttribute[InclGenCode]*/;

            public void Consume()
            {
                PartialMethodWithImplementationAndAttribute(null /*Expect:AssignNullToNotNullAttribute[InclGenCode]*/);
                PartialMethodWithImplementationAndNoAttribute(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);
            }
        }
    }

    // "Top level" (edge case) is important because of the GetContainingType() access for delegates (which is null in this case).

    [GeneratedCode("tool", "version")]
    public delegate string SomeTopLevelGeneratedCodeDelegate(string s);

    public delegate string SomeTopLevelNonGeneratedCodeDelegate(string s);
}
