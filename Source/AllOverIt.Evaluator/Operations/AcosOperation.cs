﻿using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    /// <summary>An operation used to calculate the angle (in radians) of a cosine value.</summary>
    public sealed class AcosOperation : ArithmeticOperationBase
    {
        /// <summary>Constructor.</summary>
        public AcosOperation() 
            : base(1, MakeOperator)
        {
        }

        private static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new AcosOperator(e[0]));
        }
    }
}