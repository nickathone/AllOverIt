using System;
using System.Linq.Expressions;

namespace AllOverIt.Expressions.Info
{
  public abstract class ExpressionInfoBase : IExpressionInfo
  {
    public Expression Expression { get; }
    public ExpressionInfoType InfoType { get; }

    protected ExpressionInfoBase(Expression expression, ExpressionInfoType infoType)
    {
      Expression = expression ?? throw new ArgumentNullException(nameof(expression));
      InfoType = infoType;
    }
  }
}