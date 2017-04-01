using System.Windows.Controls;

namespace ImplicitNullability.Samples.CodeWithIN.Special.NullabilityAnalysis.Wpf
{
    public partial class SomeControlWithUninitializedField
    {
        // This is a counter-example to "SomeControlWithInitializedField"

        public Button SomeUserCodeControl;

        public SomeControlWithUninitializedField /*Expect:NotNullMemberIsNotInitialized[Flds && !RtRo]*/()
        {
            InitializeComponent();
        }

        public void Consume()
        {
            ReSharper.SuppressUnusedWarning(ControlInXaml);
        }
    }
}
