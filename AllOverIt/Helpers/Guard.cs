using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AllOverIt.Helpers
{
  public static class Guard
  {
    public static TType WhenNotNull<TType>(Expression<Func<TType>> expression, string errorMessage = default)
      where TType : class
    {
      _ = expression ?? ThrowArgumentNullException<Expression<Func<TType>>>(nameof(expression), errorMessage);

      switch (expression)
      {
        case LambdaExpression lambdaExpression when lambdaExpression.Body is MemberExpression memberExpression:
        {
          var value = expression.Compile().Invoke();

          return value.WhenNotNull(memberExpression.Member.Name, errorMessage);
        }

        default:
          throw new ArgumentException($"{nameof(expression)} must be a LambdaExpression containing a MemberExpression");
      }
    }

    public static IEnumerable<TType> WhenNotNullOrEmpty<TType>(Expression<Func<IEnumerable<TType>>> expression, string errorMessage = default)
    {
      _ = expression ?? ThrowArgumentNullException<Expression<Func<IEnumerable<TType>>>>(nameof(expression), errorMessage);

      switch (expression)
      {
        case LambdaExpression lambdaExpression when lambdaExpression.Body is MemberExpression memberExpression:
        {
          var value = expression.Compile().Invoke();

          return value.WhenNotNullOrEmpty(memberExpression.Member.Name, errorMessage);
        }

        default:
          throw new ArgumentException($"{nameof(expression)} must be a LambdaExpression containing a MemberExpression");
      }
    }

    public static string WhenNotNullOrEmpty(Expression<Func<string>> expression, string errorMessage = default)
    {
      _ = expression ?? ThrowArgumentNullException<Expression<Func<string>>>(nameof(expression), errorMessage);

      switch (expression)
      {
        case LambdaExpression lambdaExpression when lambdaExpression.Body is MemberExpression memberExpression:
        {
          var value = expression.Compile().Invoke();

          return value.WhenNotNullOrEmpty(memberExpression.Member.Name, errorMessage);
        }

        default:
          throw new ArgumentException($"{nameof(expression)} must be a LambdaExpression containing a MemberExpression");
      }
    }
    
    #region object extensions

    // returns @object if not null, otherwise throws ArgumentNullException
    public static TType WhenNotNull<TType>(this TType argument, string name, string errorMessage = default)
      where TType : class
    {
      return argument ?? ThrowArgumentNullException<TType>(name, errorMessage);
    }

    // returns @object if not null or empty, otherwise throws ArgumentNullException / ArgumentException
    public static IEnumerable<TType> WhenNotNullOrEmpty<TType>(this IEnumerable<TType> argument, string name, string errorMessage = default)
    {
      _ = argument ?? ThrowArgumentNullException<IEnumerable<TType>>(name, errorMessage);

      // ReSharper disable once PossibleMultipleEnumeration
      if (!argument.Any())
      {
        throw new ArgumentException(errorMessage ?? "The argument cannot be empty", name);
      }

      return argument;
    }

    // returns @object if not null or empty, otherwise throws ArgumentNullException / ArgumentException
    public static string WhenNotNullOrEmpty(this string argument, string name, string errorMessage = default)
    {
      _ = argument ?? ThrowArgumentNullException(name, errorMessage);

      if (!string.IsNullOrWhiteSpace(argument))
      {
        return argument;
      }

      throw new ArgumentException(errorMessage ?? "The argument cannot be empty", name);
    }

    #endregion

    private static string ThrowArgumentNullException(string name, string errorMessage)
    {
      return ThrowArgumentNullException<String>(name, errorMessage);
    }

    private static TType ThrowArgumentNullException<TType>(string name, string errorMessage)
    {
      if (errorMessage == default)
      {
        throw new ArgumentNullException(name);
      }

      throw new ArgumentNullException(name, errorMessage);
    }
  }
}
