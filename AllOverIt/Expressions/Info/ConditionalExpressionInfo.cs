using System;
using System.Linq.Expressions;

namespace AllOverIt.Expressions.Info
{
  // An info class for a conditional expression.
  public sealed class ConditionalExpressionInfo : ExpressionInfoBase
  {
    public IExpressionInfo Test { get; }
    public IExpressionInfo IfTrue { get; }
    public IExpressionInfo IfFalse { get; }

    public ConditionalExpressionInfo(Expression expression, IExpressionInfo test, IExpressionInfo ifTrue, IExpressionInfo ifFalse)
      : base(expression, ExpressionInfoType.Conditional)
    {
      Test = test ?? throw new ArgumentNullException(nameof(test)); ;
      IfTrue = ifTrue ?? throw new ArgumentNullException(nameof(ifTrue)); ;
      IfFalse = ifFalse ?? throw new ArgumentNullException(nameof(ifFalse)); ;
    }
  }
}