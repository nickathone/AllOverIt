using AllOverIt.Extensions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AllOverIt.Expressions.Info
{
  public sealed class MemberInitExpressionInfo : ExpressionInfoBase
  {
    public IReadOnlyList<IExpressionInfo> Bindings { get; }
    public IReadOnlyList<IExpressionInfo> Arguments { get; }

    public MemberInitExpressionInfo(Expression expression, IEnumerable<IExpressionInfo> bindings, IEnumerable<IExpressionInfo> arguments)
      : base(expression, ExpressionInfoType.MemberInit)
    {
      Bindings = bindings?.AsReadOnlyList() ?? throw new ArgumentNullException(nameof(bindings));
      Arguments = arguments?.AsReadOnlyList() ?? throw new ArgumentNullException(nameof(arguments));
    }
  }
}