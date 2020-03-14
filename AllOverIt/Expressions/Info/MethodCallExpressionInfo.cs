using AllOverIt.Extensions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace AllOverIt.Expressions.Info
{
  // An info class for a method call expression.
  public sealed class MethodCallExpressionInfo : ExpressionInfoBase, INegatableExpressionInfo
  {
    // The object the method call was invoked on.
    public IExpressionInfo Object { get; }

    // The method call info.
    public MethodInfo MethodInfo { get; }

    // The parameters used for the method call.
    public IEnumerable<IExpressionInfo> Parameters { get; }

    // indicates if the result from the call should be negated.
    public bool IsNegated { get; }

    public MethodCallExpressionInfo(Expression expression, IExpressionInfo @object, MethodInfo methodInfo, IEnumerable<IExpressionInfo> parameters, bool isNegated)
      : base(expression, ExpressionInfoType.MethodCall)
    {
      Object = @object ?? throw new ArgumentNullException(nameof(@object));
      MethodInfo = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));
      Parameters = parameters?.AsReadOnlyList() ?? throw new ArgumentNullException(nameof(parameters));
      IsNegated = isNegated;
    }
  }
}