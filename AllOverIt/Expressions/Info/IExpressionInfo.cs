using System.Linq.Expressions;

namespace AllOverIt.Expressions.Info
{
  public interface IExpressionInfo
  {
    Expression Expression { get; }
    ExpressionInfoType InfoType { get; }
  }
}