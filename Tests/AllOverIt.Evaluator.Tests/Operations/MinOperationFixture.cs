using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using System;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class MinOperationFixture : BinaryOperationFixtureBase<MinOperation>
    {
        protected override Type OperatorType => typeof(MinOperator);
    }
}