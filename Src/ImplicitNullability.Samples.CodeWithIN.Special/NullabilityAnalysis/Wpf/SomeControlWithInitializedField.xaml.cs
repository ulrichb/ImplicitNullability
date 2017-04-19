using System.Windows.Controls;

namespace ImplicitNullability.Samples.CodeWithIN.Special.NullabilityAnalysis.Wpf
{
    public partial class SomeControlWithInitializedField
    {
        // This is a repro sample for https://github.com/ulrichb/ImplicitNullability/issues/11

        public Button SomeUserCodeControl;

        // Here would be a /*Expect:NotNullMemberIsNotInitialized[InclGenCode]*/ comment (for the tests with disabled GeneratedCode = Include
        // setting) if the R# SDK tests would include the Custom Tool-generated .g.i.cs file in the obj-directory. As this doesn't happen
        // (although the file is present in the project system in VS), the expectation comment is not necessary.

        public SomeControlWithInitializedField /*Expect no NotNullMemberIsNotInitialized (also not for the fields in the generated code)*/()
        {
            InitializeComponent();

            SomeUserCodeControl = new Button();
        }

        public void Consume()
        {
            ReSharper.TestValueAnalysis(ControlInXaml, ControlInXaml == null /*Expect no warning*/);
            ReSharper.TestValueAnalysis(SomeUserCodeControl, SomeUserCodeControl == null /*Expect:ConditionIsAlwaysTrueOrFalse[Flds && !RtRo]*/);
        }
    }
}
