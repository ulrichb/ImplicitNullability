using JetBrains.Annotations;
using static ImplicitNullability.Samples.CodeWithIN.ReSharper;

namespace ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis
{
    public class MethodsRefParameterSample
    {
        public void MethodWithRefParameter(ref string refParam)
        {
            TestValueAnalysis(refParam, refParam == null /* REPORTED false negative https://youtrack.jetbrains.com/issue/RSRP-427414 */);

            // Note that the implicit not null argument applies also to the outgoing value of 'refParam'
            refParam = null /*Expect:AssignNullToNotNullAttribute[MRef]*/;
        }

        public void MethodWithExplicitNotNullRefParameter([NotNull] ref string refParam)
        {
            TestValueAnalysis(refParam, refParam == null /* REPORTED false negative https://youtrack.jetbrains.com/issue/RSRP-427414 */);
            refParam = null /*Expect:AssignNullToNotNullAttribute*/;
        }

        public void MethodWithCanBeNullRefParameter([CanBeNull] ref string refParam)
        {
            TestValueAnalysis(refParam /* REPORTED false negative https://youtrack.jetbrains.com/issue/RSRP-427414 */, refParam == null);
            refParam = null;
        }
    }
}