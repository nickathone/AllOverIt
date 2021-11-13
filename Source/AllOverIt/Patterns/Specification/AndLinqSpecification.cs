﻿using AllOverIt.Expressions;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Patterns.Specification
{
    /// <summary>A specification that performs a logical AND operation between two expressions.</summary>
    /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
    public sealed class AndLinqSpecification<TType> : BinaryLinqSpecification<TType>
    {
        /// <summary>Constructor.</summary>
        /// <param name="leftSpecification">The left specification applied against a candidate.</param>
        /// <param name="rightSpecification">The right specification applied against a candidate.</param>
        public AndLinqSpecification(ILinqSpecification<TType> leftSpecification, ILinqSpecification<TType> rightSpecification)
            : base(leftSpecification, rightSpecification)
        {
        }

        /// <inheritdoc />
        public override Expression<Func<TType, bool>> AsExpression()
        {
            var leftExpression = LeftSpecification.AsExpression();
            var rightExpression = RightSpecification.AsExpression();

            return leftExpression.And(rightExpression);
        }
    }
}