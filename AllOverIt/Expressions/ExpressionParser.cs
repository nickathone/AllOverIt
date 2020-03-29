using AllOverIt.Expressions.Exceptions;
using AllOverIt.Expressions.Info;
using AllOverIt.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AllOverIt.Expressions
{
  public sealed class ExpressionParser : IExpressionParser
  {
    private sealed class ResolvedExpressionInfo
    {
      public ExpressionInfoType InfoType { get; }
      public Expression Expression { get; }
      public bool IsNegated { get; }

      public ResolvedExpressionInfo(ExpressionInfoType infoType, Expression expression, bool isNegated)
      {
        InfoType = infoType;
        Expression = expression;
        IsNegated = isNegated;
      }
    }

    private static readonly IDictionary<ExpressionInfoType, Func<ResolvedExpressionInfo, IExpressionInfo>> ExpressionInfoResolvers =
      new Dictionary<ExpressionInfoType, Func<ResolvedExpressionInfo, IExpressionInfo>>
      {
        {
          ExpressionInfoType.Constant, resolvedInfo => ParseConstantExpression(resolvedInfo.Expression)
        },
        {
          ExpressionInfoType.Field, resolvedInfo => ParseFieldExpression(resolvedInfo.Expression as MemberExpression, resolvedInfo.IsNegated)
        },
        {
          ExpressionInfoType.Property, resolvedInfo => ParsePropertyExpression(resolvedInfo.Expression as MemberExpression, resolvedInfo.IsNegated)
        },
        {
          ExpressionInfoType.BinaryComparison, resolvedInfo => ParseBinaryComparisonExpression(resolvedInfo.Expression as BinaryExpression)
        },
        {
          ExpressionInfoType.MethodCall, resolvedInfo => ParseMethodCallExpression(resolvedInfo.Expression as MethodCallExpression, resolvedInfo.IsNegated)
        },
        {
          ExpressionInfoType.Conditional, resolvedInfo => ParseConditionalExpression(resolvedInfo.Expression as ConditionalExpression)
        },
        {
          ExpressionInfoType.MemberInit, resolvedInfo => ParseMemberInitExpression(resolvedInfo.Expression as MemberInitExpression)
        },
        {
          ExpressionInfoType.New, resolvedInfo => ParseNewExpression(resolvedInfo.Expression as NewExpression)
        },
        {
          ExpressionInfoType.Parameter, resolvedInfo => ParseParameterExpression(resolvedInfo.Expression as ParameterExpression)
        }
      };

    public IExpressionInfo Parse(Expression expression)
    {
      _ = expression ?? throw new ArgumentNullException(nameof(expression));

      return Parse(expression, type => true);
    }

    public IExpressionInfo Parse(Expression expression, ExpressionInfoType filter)
    {
      _ = expression ?? throw new ArgumentNullException(nameof(expression));

      return Parse(expression, type => filter.HasFlag(type));
    }

    private static IExpressionInfo Parse(Expression root, Func<ExpressionInfoType, bool> filter)
    {
      var expression = root;

      // If we have a lambda expression, it is the body we want to parse
      // So for a call to item => item.Method(); we want to parse item.Method();
      if (expression is LambdaExpression lambdaExpression)
      {
        expression = lambdaExpression.Body;
      }

      return GetExpressionInfo(expression, filter);
    }

    private static IExpressionInfo GetExpressionInfo(Expression expression) => GetExpressionInfo(expression, type => true);

    private static IExpressionInfo GetExpressionInfo(Expression expression, Func<ExpressionInfoType, bool> filter)
    {
      // expression cannot be null here

      // Evaluate the passed expression and identify the expression type
      var resolvedInfo = ResolveExpression(expression, null);

      var expressionTypeAllowed = filter.Invoke(resolvedInfo.InfoType);

      if (!expressionTypeAllowed)
      {
        throw new ExpressionParserException($"The expression type '{resolvedInfo.InfoType}' is not permitted.");
      }

      var expressionInfoResolver = ExpressionInfoResolvers.GetValueOrDefault(resolvedInfo.InfoType);

      // no test for this condition since it should never happen - if it does then ExpressionInfoResolvers needs extending
      _ = expressionInfoResolver ?? throw new NotSupportedException($"The ExpressionInfoType '{resolvedInfo.InfoType}' could not be resolved.");

      return expressionInfoResolver.Invoke(resolvedInfo);
    }

    private static ResolvedExpressionInfo ResolveExpression(Expression expression, bool? isNegated)
    {
      if (!isNegated.HasValue)
      {
        // capture the parent expression
        isNegated = expression.NodeType == ExpressionType.Not || expression.NodeType == ExpressionType.Negate;
      }

      if (expression is UnaryExpression)
      {
        return ResolveExpression(expression.RemoveUnary(), isNegated);
      }

      static ExpressionInfoType GetMemberExpressionInfoType(MemberExpression memberExpression)
      {
        return memberExpression.Member switch
        {
          FieldInfo _ => ExpressionInfoType.Field,
          PropertyInfo _ => ExpressionInfoType.Property,
          _ => throw new ExpressionParserException("Unexpected member expression type.")
        };
      }

      var expressionInfoType = expression switch
      {
        ConstantExpression _ => ExpressionInfoType.Constant,
        ParameterExpression _ => ExpressionInfoType.Parameter,
        NewArrayExpression _ => ExpressionInfoType.Constant,
        MemberExpression memberExpression => GetMemberExpressionInfoType(memberExpression),
        BinaryExpression _ => ExpressionInfoType.BinaryComparison,
        MethodCallExpression _ => ExpressionInfoType.MethodCall,
        ConditionalExpression _ => ExpressionInfoType.Conditional,
        MemberInitExpression _ => ExpressionInfoType.MemberInit,
        NewExpression _ => ExpressionInfoType.New,
        _ => throw new ExpressionParserException("Unexpected expression type.")
      };

      return new ResolvedExpressionInfo(expressionInfoType, expression, isNegated.Value);
    }

    private static ConstantExpressionInfo ParseConstantExpression(Expression expression)
    {
      // get the single constant or array of values 
      var value = expression switch
      {
        // resolve the expression of each array element (limited to constant, field, and property expressions)
        // value =
        //   (from itemExpression in arrayExpression.Expressions
        //     let info = GetExpressionInfo(itemExpression,
        //       type => type == ExpressionInfoType.Constant ||
        //               type == ExpressionInfoType.Field ||
        //               type == ExpressionInfoType.Property)
        //     select ((IExpressionValue) info).Value)
        //   .ToArray();

        // This approach allows for the following scenarios using:
        //   static int f() { return 2; }
        //
        //   Scenario 1:
        //     Expression<Func<int[]>> exp = () => new [] {1, f() + 2, 3};
        //
        //   Scenario 2:
        //     Expression<Func<object[]>> exp = () => new object[] {1, f() + 2, 3};
        //
        //   In the first scenario, the first and last elements are resolved as constant expressions whereas
        //   the second element is compiled and invoked.
        //
        //   In the second scenario, all elements are compiled and invoked due to object casting
        NewArrayExpression arrayExpression => arrayExpression.Expressions.GetValues().ToArray(),

        ConstantExpression constantExpression => constantExpression.Value,

        _ => throw new ExpressionParserException("The constant expression could not be parsed.")
      };

      return new ConstantExpressionInfo(expression, value);
    }

    private static FieldExpressionInfo ParseFieldExpression(MemberExpression expression, bool isNegated)
    {
      var field = expression.Member as FieldInfo;

      var value = field != null 
        ? expression.GetValue() 
        : null;

      return new FieldExpressionInfo(expression, field, value, isNegated);

    }

    private static PropertyExpressionInfo ParsePropertyExpression(MemberExpression expression, bool isNegated)
    {
      var property = expression.Member as PropertyInfo;

      var value = property != null
        ? expression.GetValue()
        : null;

      return new PropertyExpressionInfo(expression, property, value, isNegated);
    }

    private static BinaryComparisonExpressionInfo ParseBinaryComparisonExpression(BinaryExpression expression)
    {
      // null expressions are handled by GetExpressionInfo()
      var left = GetExpressionInfo(expression.Left);
      var right = GetExpressionInfo(expression.Right);

      return new BinaryComparisonExpressionInfo(expression, left, right, expression.NodeType);
    }

    private static MethodCallExpressionInfo ParseMethodCallExpression(MethodCallExpression expression, bool isNegated)
    {
      var @object = GetExpressionInfo(expression.Object);
      var method = expression.Method;

      // Each parameter is itself an expression
      var parameters = expression.Arguments.Select(GetExpressionInfo);

      return new MethodCallExpressionInfo(expression, @object, method, parameters, isNegated);
    }

    private static ConditionalExpressionInfo ParseConditionalExpression(ConditionalExpression expression)
    {
      var test = GetExpressionInfo(expression.Test);
      var ifTrue = GetExpressionInfo(expression.IfTrue);
      var ifFalse = GetExpressionInfo(expression.IfFalse);

      return new ConditionalExpressionInfo(expression, test, ifTrue, ifFalse);
    }

    private static MemberInitExpressionInfo ParseMemberInitExpression(MemberInitExpression expression)
    {
      var bindings = GetMemberBindingExpressionInfo(expression.Bindings);
      var arguments = expression.NewExpression.Arguments.Select(GetExpressionInfo);

      return new MemberInitExpressionInfo(
        expression,
        bindings.Select(item => item.ExpressionInfo),
        arguments);
    }

    private static NewExpressionInfo ParseNewExpression(NewExpression expression)
    {
      var fieldExpressions = expression.Arguments.Select(GetExpressionInfo);

      return new NewExpressionInfo(expression, fieldExpressions);
    }

    private static ParameterExpressionInfo ParseParameterExpression(ParameterExpression expression)
    {
      return new ParameterExpressionInfo(expression);
    }

    private static IEnumerable<(MemberBindingType BindingType, IExpressionInfo ExpressionInfo)> GetMemberBindingExpressionInfo(IEnumerable<MemberBinding> memberBindings)
    {
      return memberBindings.SelectMany(GetMemberBindingExpressionInfo);
    }

    private static IEnumerable<(MemberBindingType BindingType, IExpressionInfo ExpressionInfo)> GetMemberBindingExpressionInfo(MemberBinding binding)
    {
      switch (binding)
      {
        case MemberAssignment memberAssignment:
          yield return (binding.BindingType, GetExpressionInfo(memberAssignment.Expression));
          break;

        case MemberMemberBinding memberMemberBinding:
        {
          var bindingExpressions = GetMemberBindingExpressionInfo(memberMemberBinding.Bindings);

          foreach (var expressionInfo in bindingExpressions)
          {
            yield return expressionInfo;
          }

          break;
        }

        case MemberListBinding memberListBinding:
        {
          var argumentExpressions = memberListBinding.Initializers
            .SelectMany(init => init.Arguments)
            .Select(GetExpressionInfo);

          foreach (var expressionInfo in argumentExpressions)
          {
            yield return (binding.BindingType, expressionInfo);
          }

          break;
        }

        default:
          throw new ExpressionParserException("Unexpected member binding.");
      }
    }
  }
}