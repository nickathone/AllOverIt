using AllOverIt.Expressions.Info;
using System.Linq.Expressions;

namespace AllOverIt.Expressions
{
  // An interface for an expression parser.
  public interface IExpressionParser
  {
    IExpressionInfo Parse(Expression expression);
    IExpressionInfo Parse(Expression expression, ExpressionInfoType filter);
  }
}