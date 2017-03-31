using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis
{
    public class ContractAnnotationSample
    {
        // This sample proves that IN doesn't override ContractAnnotation attributes.

        [ContractAnnotation("=> notnull")]
        public string FunctionWithNotNull() => null /*Expect no warning because the "notnull" is ony respected at call site*/;

        [ContractAnnotation("=> canbenull")]
        public string FunctionWithCanBeNull() => null /*Expect no warning*/;

        public string FunctionWithoutContract() => null /*Expect:AssignNullToNotNullAttribute[MOut]*/;

        public void Consume()
        {
            var functionWithNotNull = FunctionWithNotNull();
            ReSharper.TestValueAnalysis(functionWithNotNull, functionWithNotNull == null /*Expect:ConditionIsAlwaysTrueOrFalse*/);

            var functionWithCanBeNull = FunctionWithCanBeNull();
            ReSharper.TestValueAnalysis(functionWithCanBeNull /*Expect:AssignNullToNotNullAttribute*/, functionWithCanBeNull == null);

            var functionWithoutContract = FunctionWithoutContract();
            ReSharper.TestValueAnalysis(functionWithoutContract, functionWithoutContract == null /*Expect:ConditionIsAlwaysTrueOrFalse[MOut]*/);
        }

        [ContractAnnotation("=> notnull")]
        public async Task<string> FunctionWithNotNullAsync()
        {
            return await Async.CanBeNullResult<string>();
        }

        public async Task<string> FunctionWithoutContractAsync()
        {
            return await Async.CanBeNullResult<string>() /*Expect:AssignNullToNotNullAttribute[MOut]*/;
        }

        public async Task ConsumeAsnc()
        {
            // Atm. "item..." is not supported. See https://youtrack.jetbrains.com/issue/RSRP-462282.
            var functionWithNotNull = await FunctionWithNotNullAsync();
            ReSharper.TestValueAnalysis(functionWithNotNull, functionWithNotNull == null);

            var functionWithoutContract = await FunctionWithoutContractAsync();
            ReSharper.TestValueAnalysis(functionWithoutContract, functionWithoutContract == null /*Expect:ConditionIsAlwaysTrueOrFalse[MOut]*/);
        }
    }
}
