using System;
using System.Linq.Expressions;

namespace AllOverIt.Expressions.Info
{
  // An info class for a binary comparison expression.
  public sealed class BinaryComparisonExpressionInfo : ExpressionInfoBase
  {
    public IExpressionInfo Left { get; }
    public IExpressionInfo Right { get; }
    public ExpressionType OperatorType { get; }

    public BinaryComparisonExpressionInfo(Expression expression, IExpressionInfo left, IExpressionInfo right, ExpressionType operatorType)
      : base(expression, ExpressionInfoType.BinaryComparison)
    {
      Left = left ?? throw new ArgumentNullException(nameof(left));
      Right = right ?? throw new ArgumentNullException(nameof(right));
      OperatorType = operatorType;
    }
  }
}