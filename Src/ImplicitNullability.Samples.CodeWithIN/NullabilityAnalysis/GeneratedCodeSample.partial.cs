namespace ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis
{
    public static partial class GeneratedCodeSample
    {
        public partial class GeneratedCodeOnType
        {
            partial void PartialMethodWithImplementation(string a) => ReSharper.SuppressUnusedWarning(a);
        }

        public partial class GeneratedCodeOnMember
        {
            partial void PartialMethodWithImplementationAndAttribute(string a) => ReSharper.SuppressUnusedWarning(a);

            partial void PartialMethodWithImplementationAndNoAttribute(string a) => ReSharper.SuppressUnusedWarning(a);
        }
    }
}
