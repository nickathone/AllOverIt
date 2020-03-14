using System.Linq.Expressions;

namespace AllOverIt.Expressions.Info
{
  // An info class for a constant expression.
  public sealed class ConstantExpressionInfo : ExpressionInfoBase, IExpressionValue
  {
    // The value for the constant.
    public object Value { get; }

    public ConstantExpressionInfo(Expression expression, object value)
      : base(expression, ExpressionInfoType.Constant)
    {
      Value = value;
    }
  }
}