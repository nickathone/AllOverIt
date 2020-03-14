using AllOverIt.Extensions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AllOverIt.Expressions.Info
{
  public sealed class NewExpressionInfo : ExpressionInfoBase
  {
    public IReadOnlyList<IExpressionInfo> Arguments { get; }

    public NewExpressionInfo(Expression expression, IEnumerable<IExpressionInfo> arguments)
      : base(expression, ExpressionInfoType.New)
    {
      Arguments = arguments?.AsReadOnlyList() ?? throw new ArgumentNullException(nameof(arguments));
    }
  }
}