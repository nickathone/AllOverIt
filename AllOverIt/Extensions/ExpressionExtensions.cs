using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AllOverIt.Extensions
{
  public static class ExpressionExtensions
  {
    public static object GetValue(this Expression expression)
    {
      try
      {
        return expression switch
        {
          null => null,
          ConstantExpression constantExpression => constantExpression.Value,
          MemberExpression memberExpression => GetValue(memberExpression),
          MethodCallExpression methodCallExpression => GetValue(methodCallExpression),
          _ => GetDynamicInvocationResult(expression)
        };
      }
      catch (TargetInvocationException exception)
      {
        // The InnerException will never be null - it holds the underlying exception thrown by the invoked method
        // ReSharper disable once PossibleNullReferenceException
        throw exception.InnerException;
      }
    }

    public static IEnumerable<object> GetValues(this IEnumerable<Expression> expressions)
    {
      return expressions.Select(GetValue);
    }

    public static Expression RemoveUnary(this Expression expression)
    {
      if (expression is UnaryExpression unary)
      {
        return unary.Operand;
      }

      return expression;
    }

    private static object GetValue(MemberExpression memberExpression)
    {
      var value = GetValue(memberExpression.Expression);

      var member = memberExpression.Member;

      return member switch
      {
        FieldInfo fieldInfo => fieldInfo.GetValue(value),
        PropertyInfo propertyInfo => propertyInfo.GetValue(value),
        _ => throw new InvalidOperationException($"Unsupported member type '{member.GetType()}'")
      };
    }

    private static object GetValue(MethodCallExpression methodCallExpression)
    {
      var obj = GetValue(methodCallExpression.Object);
      var parameters = GetValues(methodCallExpression.Arguments).ToArray();

      return methodCallExpression.Method.Invoke(obj, parameters);
    }

    private static object GetDynamicInvocationResult(Expression expression)
    {
      var lambdaExpression = Expression.Lambda(expression);
      var func = lambdaExpression.Compile();

      return func.DynamicInvoke();
    }
  }
}