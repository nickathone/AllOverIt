using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AllOverIt.Helpers
{
  public static class Guard
  {
    public static TType WhenNotNull<TType>(Expression<Func<TType>> expression)
      where TType : class
    {
      _ = expression ?? throw new ArgumentNullException(nameof(expression));

      switch (expression)
      {
        case LambdaExpression lambdaExpression when lambdaExpression.Body is MemberExpression memberExpression:
        {
          var value = expression.Compile().Invoke();

          return value.WhenNotNull(memberExpression.Member.Name);
        }

        default:
          throw new ArgumentException($"{nameof(expression)} must be a LambdaExpression containing a MemberExpression");
      }
    }

    public static IEnumerable<TType> WhenNotNullOrEmpty<TType>(Expression<Func<IEnumerable<TType>>> expression)
    {
      _ = expression ?? throw new ArgumentNullException(nameof(expression));

      switch (expression)
      {
        case LambdaExpression lambdaExpression when lambdaExpression.Body is MemberExpression memberExpression:
        {
          var value = expression.Compile().Invoke();

          return value.WhenNotNullOrEmpty(memberExpression.Member.Name);
        }

        default:
          throw new ArgumentException($"{nameof(expression)} must be a LambdaExpression containing a MemberExpression");
      }
    }

    public static string WhenNotNullOrEmpty(Expression<Func<string>> expression)
    {
      _ = expression ?? throw new ArgumentNullException(nameof(expression));

      switch (expression)
      {
        case LambdaExpression lambdaExpression when lambdaExpression.Body is MemberExpression memberExpression:
        {
          var value = expression.Compile().Invoke();

          return value.WhenNotNullOrEmpty(memberExpression.Member.Name);
        }

        default:
          throw new ArgumentException($"{nameof(expression)} must be a LambdaExpression containing a MemberExpression");
      }
    }
    
    #region object extensions

    // returns @object if not null, otherwise throws ArgumentNullException
    public static TType WhenNotNull<TType>(this TType argument, string name)
      where TType : class
    {
      return argument ?? throw new ArgumentNullException(name);
    }

    // returns @object if not null or empty, otherwise throws ArgumentNullException / ArgumentException
    public static IEnumerable<TType> WhenNotNullOrEmpty<TType>(this IEnumerable<TType> argument, string name)
    {
      _ = argument ?? throw new ArgumentNullException(name);

      if (!argument.Any())
      {
        throw new ArgumentException("The argument cannot be empty", name);
      }

      return argument;
    }

    // returns @object if not null or empty, otherwise throws ArgumentNullException / ArgumentException
    public static string WhenNotNullOrEmpty(this string argument, string name)
    {
      _ = argument ?? throw new ArgumentNullException(name);

      if (!string.IsNullOrWhiteSpace(argument))
      {
        return argument;
      }

      throw new ArgumentException("The argument cannot be empty", name);
    }

    #endregion
  }
}