using System.Linq.Expressions;

namespace AllOverIt.Expressions.Info
{
  public sealed class ParameterExpressionInfo : ExpressionInfoBase
  {
    public string Name { get; set; }

    public ParameterExpressionInfo(ParameterExpression expression)
      : base(expression, ExpressionInfoType.Parameter)
    {
      Name = expression.Name;
    }
  }
}