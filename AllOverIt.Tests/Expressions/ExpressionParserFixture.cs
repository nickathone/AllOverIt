using AllOverIt.Expressions;
using AllOverIt.Expressions.Exceptions;
using AllOverIt.Expressions.Info;
using AllOverIt.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace AllOverIt.Tests.Expressions
{
  public class ExpressionParserFixture : AllOverItFixtureBase
  {
    private class DummyPropertyClass
    {
      public string Value { get; set; }
      public int Number { get; set; }

      public DummyPropertyClass()
      {
      }

      public DummyPropertyClass(string value)
      {
        Value = value;
      }
    }

    private class DummyNestedClass
    {
      public DummyPropertyClass PropertyClass { get; set; }
    }

    private class DummyNodeClass
    {
      // initialise non-null so MemberMemberBinding is invoked
      public DummyPropertyClass Property { get; set; } = new DummyPropertyClass("SomeValue");

      // initialise with one element so MemberListBind is invoked
      // Note: cannot set the element to 'new DummyNodeClass()' as this will stack overflow
      //public IList<DummyNodeClass> Children { get; set; } = new List<DummyNodeClass>{ null };

      // initialise with one element so MemberListBind is invoked
      public IList<DummyPropertyClass> Properties { get; set; } = new List<DummyPropertyClass> { new DummyPropertyClass() };
    }

    private readonly ExpressionParser _parser;

    public ExpressionParserFixture()
    {
      _parser = new ExpressionParser();
    }

    public class Parse_Expression : ExpressionParserFixture
    {
      private readonly int _knownValue;

      public Parse_Expression()
      {
        _knownValue = Create<int>();
      }

      [Fact]
      public void Should_Throw_When_Expression_Null()
      {
        Invoking(
            () => _parser.Parse(null))
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage("expression"));
      }

      [Fact]
      public void Should_Parse_Constant_Expression()
      {
        var value = Create<int>();
        var expression = Expression.Constant(value);

        var actual = _parser.Parse(expression);

        actual.Should().BeOfType<ConstantExpressionInfo>();

        var info = actual as ConstantExpressionInfo;

        info.Should().BeEquivalentTo(new
        {
          Expression = expression,
          InfoType = ExpressionInfoType.Constant,
          Value = value
        });
      }

      [Fact]
      public void Should_Parse_Field_Expression()
      {
        var value = Create<double>();
        Expression<Func<double>> expression = () => value;

        var actual = _parser.Parse(expression);

        actual.Should().BeOfType<FieldExpressionInfo>();

        var info = actual as FieldExpressionInfo;

        info.Should().BeEquivalentTo(new
        {
          Expression = expression.Body,
          FieldInfo = (expression.Body as MemberExpression).Member as FieldInfo,
          Value = value,
          InfoType = ExpressionInfoType.Field,
          IsNegated = false
        });
      }

      [Fact]
      public void Should_Parse_Property_Expression()
      {
        var dummy = new DummyPropertyClass { Value = Create<string>() };

        Expression<Func<string>> expression = () => dummy.Value;

        var actual = _parser.Parse(expression);

        actual.Should().BeOfType<PropertyExpressionInfo>();

        var info = actual as PropertyExpressionInfo;

        info.Should().BeEquivalentTo(new
        {
          Expression = expression.Body,
          PropertyInfo = (expression.Body as MemberExpression).Member as PropertyInfo,
          Value = dummy.Value,
          InfoType = ExpressionInfoType.Property,
          IsNegated = false
        });
      }

      [Fact]
      public void Should_Parse_Negative_Property_Expression()
      {
        var dummy = new DummyPropertyClass { Number = Create<int>() };

        Expression<Func<int>> expression = () => -dummy.Number;

        var actual = _parser.Parse(expression);

        actual.Should().BeOfType<PropertyExpressionInfo>();

        var info = actual as PropertyExpressionInfo;

        var unaryExpression = (expression.Body as UnaryExpression).Operand;

        info.Should().BeEquivalentTo(new
        {
          Expression = unaryExpression,
          PropertyInfo = (unaryExpression as MemberExpression).Member as PropertyInfo,
          Value = dummy.Number,
          InfoType = ExpressionInfoType.Property,
          IsNegated = true
        });
      }

      [Fact]
      public void Should_Parse_Binary_Comparison()
      {
        var dummy1 = new DummyPropertyClass { Value = Create<string>() };
        var value2 = Create<string>();

        Expression<Func<bool>> expression = () => dummy1.Value == value2;

        var actual = _parser.Parse(expression);

        actual.Should().BeOfType<BinaryComparisonExpressionInfo>();

        var info = actual as BinaryComparisonExpressionInfo;

        var leftExpression = (expression.Body as BinaryExpression).Left;
        var rightExpression = (expression.Body as BinaryExpression).Right;

        info.Should().BeEquivalentTo(new
        {
          Expression = expression.Body,
          InfoType = ExpressionInfoType.BinaryComparison,
          OperatorType = ExpressionType.Equal,
          Left = new
          {
            Expression = leftExpression,
            PropertyInfo = (leftExpression as MemberExpression).Member as PropertyInfo,
            InfoType = ExpressionInfoType.Property,
            IsNegated = false,
            Value = dummy1.Value
          },
          Right = new
          {
            Expression = rightExpression,
            FieldInfo = (rightExpression as MemberExpression).Member as FieldInfo,
            InfoType = ExpressionInfoType.Field,
            IsNegated = false,
            Value = value2
          }
        });
      }

      [Fact]
      public void Should_Parse_MethodCall_Expression_With_No_Arguments()
      {
        Expression<Func<int>> expression = () => DummyCallWithNoArguments();

        var actual = _parser.Parse(expression);

        actual.Should().BeOfType<MethodCallExpressionInfo>();

        var info = actual as MethodCallExpressionInfo;

        var methodCallExpression = expression.Body as MethodCallExpression;

        // the method call resolves to a constant expression
        var constantExpressionInfo = _parser.Parse(methodCallExpression.Object);

        constantExpressionInfo.Should().BeOfType<ConstantExpressionInfo>();

        info.Should().BeEquivalentTo(new
        {
          Expression = expression.Body,
          IsNegated = false,
          MethodInfo = methodCallExpression.Method,
          Object = constantExpressionInfo,
          Parameters = Enumerable.Empty<IExpressionInfo>(),
          InfoType = ExpressionInfoType.MethodCall
        });
      }

      [Fact]
      public void Should_Parse_MethodCall_Expression_With_Constant_And_MethodCall_Arguments()
      {
        var multiplier1 = Create<int>() % 100;
        Expression<Func<int>> expression = () => DummyCallWithArguments(multiplier1, DummyCallWithNoArguments());

        var actual = _parser.Parse(expression);

        actual.Should().BeOfType<MethodCallExpressionInfo>();

        var info = actual as MethodCallExpressionInfo;

        var methodCallExpression = expression.Body as MethodCallExpression;

        // make sure the method call in the lambda expression is resolved to DummyCallWithArguments()
        var callMethodInfo = methodCallExpression.Method;
        var expectedCallMethodInfo = GetType().GetMethod(nameof(DummyCallWithArguments), BindingFlags.Instance | BindingFlags.NonPublic);
        callMethodInfo.Should().BeSameAs(expectedCallMethodInfo);

        // get the parameters pass to the method call
        var parameters = methodCallExpression.Arguments
          .Select(parameter => _parser.Parse(parameter))
          .AsReadOnlyList();

        parameters.Should().HaveCount(2);

        parameters[0].Should().BeOfType<FieldExpressionInfo>();     // local variable is seen as a field
        parameters[1].Should().BeOfType<MethodCallExpressionInfo>();

        // make sure the call in parameters[1] resolves to DummyCallWithNoArguments()
        var parameterMethodInfo = (parameters[1] as MethodCallExpressionInfo).MethodInfo;
        var expectedParameterMethodInfo = GetType().GetMethod(nameof(DummyCallWithNoArguments), BindingFlags.Instance | BindingFlags.NonPublic);
        parameterMethodInfo.Should().BeSameAs(expectedParameterMethodInfo);

        // the method call resolves to a constant expression
        var constantExpressionInfo = _parser.Parse(methodCallExpression.Object);

        constantExpressionInfo.Should().BeOfType<ConstantExpressionInfo>();

        info.Should().BeEquivalentTo(new
        {
          Expression = expression.Body,
          IsNegated = false,
          MethodInfo = methodCallExpression.Method,
          Object = constantExpressionInfo,
          Parameters = parameters,
          InfoType = ExpressionInfoType.MethodCall
        });
      }

      [Fact]
      public void Should_Parse_Unary_Expression()
      {
        int[] values = { 1, 2, 3 };
        Expression<Func<int>> expression = () => values.Length;

        var actual = _parser.Parse(expression);

        actual.Should().BeOfType<FieldExpressionInfo>();

        var info = actual as FieldExpressionInfo;

        // for a unary expression, the body itself is recursively parsed
        var unaryExpression = (expression.Body as UnaryExpression).Operand;

        info.Should().BeEquivalentTo(new
        {
          Expression = unaryExpression,
          FieldInfo = (unaryExpression as MemberExpression).Member as FieldInfo,
          InfoType = ExpressionInfoType.Field,
          IsNegated = false,
          Value = values
        });
      }

      [Fact]
      public void Should_Parse_Array_Expression()
      {
        // The first and last elements are resolved as constant expressions whereas the second element is
        // compiled and invoked (it needs to be evaluated to obtain the value for the array element).
        Expression<Func<int[]>> expression = () => new[] { 1, DummyCallWithNoArguments() + 2, 3 };

        var actual = _parser.Parse(expression);

        actual.Should().BeOfType<ConstantExpressionInfo>();

        var info = actual as ConstantExpressionInfo;

        var expectedValues = new[] { 1, _knownValue + 2, 3 };

        info.Should().BeEquivalentTo(new
        {
          Expression = expression.Body,
          Value = expectedValues,
          InfoType = ExpressionInfoType.Constant
        });
      }

      [Fact]
      public void Should_Parse_Casted_Array_Expression()
      {
        // all elements are compiled and invoked due to object casting
        Expression<Func<object[]>> expression = () => new object[] { 1, DummyCallWithNoArguments() + 2, 3 };

        var actual = _parser.Parse(expression);

        actual.Should().BeOfType<ConstantExpressionInfo>();

        var info = actual as ConstantExpressionInfo;

        var expectedValues = new[] { 1, _knownValue + 2, 3 };

        info.Should().BeEquivalentTo(new
        {
          Expression = expression.Body,
          Value = expectedValues,
          InfoType = ExpressionInfoType.Constant
        });
      }

      [Fact]
      public void Should_Parse_Negated_Expression()
      {
        var value = Create<bool>();

        Expression<Func<bool>> expression = () => !value;

        var actual = _parser.Parse(expression);

        actual.Should().BeOfType<FieldExpressionInfo>();

        var info = actual as FieldExpressionInfo;

        info.Should().BeEquivalentTo(new
        {
          Expression = (expression.Body as UnaryExpression).Operand,
          FieldInfo = ((expression.Body as UnaryExpression).Operand as MemberExpression).Member as FieldInfo,
          Value = value,
          InfoType = ExpressionInfoType.Field,
          IsNegated = true
        });
      }

      [Fact]
      public void Should_Parse_Negative_Field_Expression()
      {
        var value = Create<int>();

        Expression<Func<int>> expression = () => -value;

        var actual = _parser.Parse(expression);

        actual.Should().BeOfType<FieldExpressionInfo>();

        var info = actual as FieldExpressionInfo;

        info.Should().BeEquivalentTo(new
        {
          Expression = (expression.Body as UnaryExpression).Operand,
          FieldInfo = ((expression.Body as UnaryExpression).Operand as MemberExpression).Member as FieldInfo,
          InfoType = ExpressionInfoType.Field,
          IsNegated = true,
          Value = value
        });
      }

      [Fact]
      public void Should_Parse_Logical_Field_And_Property_Comparison_Expression()
      {
        var lhs = Create<bool>();
        var rhs = Create<DummyPropertyClass>();

        Expression<Func<bool>> expression = () => lhs && rhs.Number % 2 == 0;

        var actual = _parser.Parse(expression);

        actual.Should().BeOfType<BinaryComparisonExpressionInfo>();

        var info = actual as BinaryComparisonExpressionInfo;

        var leftExpression = (expression.Body as BinaryExpression).Left;
        var rightExpression = (expression.Body as BinaryExpression).Right;

        var rightLeftExpression = (rightExpression as BinaryExpression).Left;
        var rightLeftLeftExpression = (rightLeftExpression as BinaryExpression).Left;

        info.Should().BeEquivalentTo(new
        {
          Expression = expression.Body,
          InfoType = ExpressionInfoType.BinaryComparison,
          OperatorType = ExpressionType.AndAlso,
          Left = new
          {
            Expression = leftExpression,
            FieldInfo = (leftExpression as MemberExpression).Member as FieldInfo,
            InfoType = ExpressionInfoType.Field
          },
          Right = new
          {
            Expression = rightExpression,
            InfoType = ExpressionInfoType.BinaryComparison,
            Left = new
            {
              Expression = rightLeftExpression,
              InfoType = ExpressionInfoType.BinaryComparison,
              Left = new
              {
                Expression = rightLeftLeftExpression,
                InfoType = ExpressionInfoType.Property,
                PropertyInfo = (rightLeftLeftExpression as MemberExpression).Member as PropertyInfo,
                Value = rhs.Number,
                IsNegated = false
              },
              Right = new
              {
                Expression = (rightLeftExpression as BinaryExpression).Right,
                InfoType = ExpressionInfoType.Constant,
                Value = 2
              }
            },
            Right = new
            {
              Expression = (rightExpression as BinaryExpression).Right,
              InfoType = ExpressionInfoType.Constant,
              Value = 0
            }
          }
        });
      }

      [Fact]
      public void Should_Parse_Coalesce_Expression()
      {
        int? lhs = null;
        int? rhs = Create<int>();

        // ReSharper disable once ConstantNullCoalescingCondition
        Expression<Func<int?>> expression = () => lhs ?? rhs;

        var actual = _parser.Parse(expression);

        actual.Should().BeOfType<BinaryComparisonExpressionInfo>();

        var info = actual as BinaryComparisonExpressionInfo;

        var leftExpression = (expression.Body as BinaryExpression).Left;
        var rightExpression = (expression.Body as BinaryExpression).Right;

        info.Should().BeEquivalentTo(new
        {
          Expression = expression.Body,
          InfoType = ExpressionInfoType.BinaryComparison,
          OperatorType = ExpressionType.Coalesce,
          Left = new
          {
            Expression = leftExpression,
            FieldInfo = (leftExpression as MemberExpression).Member as FieldInfo,
            InfoType = ExpressionInfoType.Field,
            IsNegated = false,
            Value = lhs
          },
          Right = new
          {
            Expression = rightExpression,
            FieldInfo = (rightExpression as MemberExpression).Member as FieldInfo,
            InfoType = ExpressionInfoType.Field,
            IsNegated = false,
            Value = rhs
          }
        });
      }

      [Fact]
      public void Should_Parse_Conditional_Expression()
      {
        var predicate = Create<bool>();
        var lhs = Create<int>();
        var rhs = Create<int>();

        Expression<Func<int?>> expression = () => predicate ? lhs : rhs;

        var actual = _parser.Parse(expression);

        actual.Should().BeOfType<ConditionalExpressionInfo>();

        var info = actual as ConditionalExpressionInfo;

        var unaryExpression = (expression.Body as UnaryExpression).Operand;

        var testExpression = (unaryExpression as ConditionalExpression).Test;
        var ifTrueExpression = (unaryExpression as ConditionalExpression).IfTrue;
        var ifFalseExpression = (unaryExpression as ConditionalExpression).IfFalse;

        info.Should().BeEquivalentTo(new
        {
          Expression = unaryExpression,
          InfoType = ExpressionInfoType.Conditional,
          Test = new
          {
            Expression = testExpression,
            FieldInfo = (testExpression as MemberExpression).Member as FieldInfo,
            InfoType = ExpressionInfoType.Field,
            IsNegated = false,
            Value = predicate
          },
          IfTrue = new
          {
            Expression = ifTrueExpression,
            FieldInfo = (ifTrueExpression as MemberExpression).Member as FieldInfo,
            InfoType = ExpressionInfoType.Field,
            IsNegated = false,
            Value = lhs
          },
          IfFalse = new
          {
            Expression = ifFalseExpression,
            FieldInfo = (ifFalseExpression as MemberExpression).Member as FieldInfo,
            InfoType = ExpressionInfoType.Field,
            IsNegated = false,
            Value = rhs
          }
        });
      }

      [Fact]
      public void Should_Parse_Conversion_Expression()
      {
        object value = Create<int>();

        Expression<Func<int>> expression = () => (int)value;

        var actual = _parser.Parse(expression);

        actual.Should().BeOfType<FieldExpressionInfo>();

        var info = actual as FieldExpressionInfo;

        var unaryExpression = (expression.Body as UnaryExpression).Operand;

        info.Should().BeEquivalentTo(new
        {
          Expression = unaryExpression,
          FieldInfo = (unaryExpression as MemberExpression).Member as FieldInfo,
          Value = value,
          InfoType = ExpressionInfoType.Field,
          IsNegated = false
        });
      }

      [Fact]
      public void Should_Parse_MemberAssignment_Expression()
      {
        // MemberAssignment is described quite well at
        // https://stackoverflow.com/questions/2917448/what-are-some-examples-of-memberbinding-linq-expressions
        //
        // Summary: Represents assignment operation for a field or property of an object.

        var number = Create<int>();

        Expression<Func<DummyPropertyClass>> expression = ()
          => new DummyPropertyClass("FixedValue")
          {
            Number = number
          };

        var actual = _parser.Parse(expression);

        actual.Should().BeOfType<MemberInitExpressionInfo>();

        var bodyExpression = expression.Body as MemberInitExpression;
        var binding1Expression = (bodyExpression.Bindings.Single() as MemberAssignment).Expression as MemberExpression;
        var argument1Expression = bodyExpression.NewExpression.Arguments.Single() as ConstantExpression;

        var info = actual as MemberInitExpressionInfo;

        info.Should().BeEquivalentTo(new
        {
          Expression = bodyExpression,
          Bindings = new[]
          {
            new
            {
              Expression = binding1Expression,
              Value = number,
              InfoType = ExpressionInfoType.Field,
              IsNegated = false
            }
          },
          Arguments = new[]
          {
            new
            {
              Expression = argument1Expression,
              InfoType = ExpressionInfoType.Constant,
              Value = "FixedValue"
            }
          },
          InfoType = ExpressionInfoType.MemberInit
        });
      }

      [Fact]
      public void Should_Parse_Nested_MemberAssignment_Expression()
      {
        var number = Create<int>();

        Expression<Func<DummyNestedClass>> expression = ()
          => new DummyNestedClass
          {
            PropertyClass = new DummyPropertyClass("FixedValue")
            {
              Number = number
            }
          };

        var actual = _parser.Parse(expression);

        actual.Should().BeOfType<MemberInitExpressionInfo>();

        // new DummyMemberClass { new DummyPropertyClass("FixedValue") { Number = number } }
        var bodyExpression = expression.Body as MemberInitExpression;

        // new DummyPropertyClass("FixedValue") { Number = number }
        var binding1OuterExpression = (bodyExpression.Bindings.Single() as MemberAssignment).Expression as MemberInitExpression;

        // Constant 'number'
        var binding1Expression = (binding1OuterExpression.Bindings.Single() as MemberAssignment).Expression as MemberExpression;

        var argument1Expression = binding1OuterExpression.NewExpression.Arguments.Single() as ConstantExpression;

        var info = actual as MemberInitExpressionInfo;

        info.Should().BeEquivalentTo(new
        {
          Expression = bodyExpression,
          Bindings = new[]
          {
            new
            {
              Expression = binding1OuterExpression,
              Bindings = new[]
              {
                new
                {
                  Expression = binding1Expression,

                  Value = number,
                  InfoType = ExpressionInfoType.Field,
                  IsNegated = false
                }
              },
              Arguments = new[]
              {
                new
                {
                  Expression = argument1Expression,
                  InfoType = ExpressionInfoType.Constant,
                  Value = "FixedValue"
                }
              }
            }
          },
          Arguments = new object[]
          {
          },
          InfoType = ExpressionInfoType.MemberInit
        });
      }

      [Fact]
      public void Should_Parse_MemberMemberBinding_Expression()
      {
        // MemberMemberBinding is described quite well at
        // https://stackoverflow.com/questions/2917448/what-are-some-examples-of-memberbinding-linq-expressions
        //
        // Summary: Represents initializing members of a member of a newly created object.

        var number = Create<int>();

        Expression<Func<DummyNodeClass>> expression = ()
          => new DummyNodeClass
          {
            Property =          // this is new'd during construction of DummyNodeClass
            {
              Number = number   // this would throw a null reference exception if Property was not instantiated
            }
          };

        var actual = _parser.Parse(expression);

        actual.Should().BeOfType<MemberInitExpressionInfo>();

        var bodyExpression = expression.Body as MemberInitExpression;
        var binding1Expression = (bodyExpression.Bindings.Single() as MemberMemberBinding);
        var binding1Assignment = binding1Expression.Bindings.Single() as MemberAssignment;

        var info = actual as MemberInitExpressionInfo;

        info.Should().BeEquivalentTo(new
        {
          Expression = bodyExpression,
          Bindings = new[]
          {
            new       // is a FieldExpressionInfo
            {
              Expression = binding1Assignment.Expression as MemberExpression,
              FieldInfo = (binding1Assignment.Expression as MemberExpression).Member as FieldInfo,
              Value = number,
              InfoType = ExpressionInfoType.Field,
              IsNegated = false
            }
          },
          Arguments = new object[]
          {
          },
          InfoType = ExpressionInfoType.MemberInit
        });
      }

      [Fact]
      public void Should_Parse_MemberListBinding_Expression()
      {
        // MemberListBinding is described quite well at
        // https://stackoverflow.com/questions/2917448/what-are-some-examples-of-memberbinding-linq-expressions
        //
        // Summary: Represents initializing the elements of a collection member of a newly created object.

        var value1 = Create<string>();
        var value2 = Create<string>();

        Expression<Func<DummyNodeClass>> expression = ()
          => new DummyNodeClass
          {
            Properties =
            {
              new DummyPropertyClass(value1),
              new DummyPropertyClass(value2)
            }
          };

        var actual = _parser.Parse(expression);

        actual.Should().BeOfType<MemberInitExpressionInfo>();

        var bodyExpression = expression.Body as MemberInitExpression;
        var binding1Expression = (bodyExpression.Bindings.Single() as MemberListBinding);

        var binding1Initializer1 = binding1Expression.Initializers.ElementAt(0).Arguments.Single();
        var binding1Initializer2 = binding1Expression.Initializers.ElementAt(1).Arguments.Single();

        var binding1Argument1 = (binding1Initializer1 as NewExpression).Arguments.ElementAt(0);
        var binding1Argument2 = (binding1Initializer2 as NewExpression).Arguments.ElementAt(0);

        var info = actual as MemberInitExpressionInfo;

        info.Should().BeEquivalentTo(new
        {
          Expression = bodyExpression,
          Bindings = new object[]
          {
            new           // is a NewExpressionInfo
            {
              Expression = binding1Initializer1,
              InfoType = ExpressionInfoType.New,
              Arguments = new[]
              {
                new       // is a FieldExpressionInfo
                {
                  Expression = binding1Argument1 as MemberExpression,
                  FieldInfo = (binding1Argument1 as MemberExpression).Member as FieldInfo,
                  Value = value1,
                  InfoType = ExpressionInfoType.Field,
                  IsNegated = false
                }
              }
            },
            new           // is a NewExpressionInfo
            {
              Expression = binding1Initializer2,
              InfoType = ExpressionInfoType.New,
              Arguments = new[]
              {
                new       // is a FieldExpressionInfo
                {
                  Expression = binding1Argument2 as MemberExpression,
                  FieldInfo = (binding1Argument2 as MemberExpression).Member as FieldInfo,
                  Value = value2,
                  InfoType = ExpressionInfoType.Field,
                  IsNegated = false
                }
              }
            }
          },
          Arguments = new object[]
          {
          },
          InfoType = ExpressionInfoType.MemberInit
        });
      }

      private int DummyCallWithNoArguments()
      {
        return _knownValue;
      }

      private int DummyCallWithArguments(int multiplier1, int multiplier2)
      {
        return multiplier1 * multiplier2 * Create<int>();
      }
    }

    public class Parse_Expression_Filter : ExpressionParserFixture
    {
      [Fact]
      public void Should_Throw_When_Expression_Null()
      {
        Invoking(
            () => _parser.Parse(null, Create<ExpressionInfoType>()))
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage("expression"));
      }

      [Fact]
      public void Should_Throw_When_Filter_Excludes_Expression_Type()
      {
        var expression = Expression.Constant(Create<int>());

        Invoking(
            () => _parser.Parse(expression, ExpressionInfoType.MethodCall))
          .Should()
          .Throw<ExpressionParserException>()
          .WithMessage(GetNonPermittedExpressionTypeError(ExpressionInfoType.Constant));
      }

      [Fact]
      public void Should_Not_Throw_When_Filter_Includes_Expression_Type()
      {
        var expression = Expression.Constant(Create<int>());

        Invoking(
            () => _parser.Parse(expression, ExpressionInfoType.Constant))
          .Should()
          .NotThrow();
      }

      [Fact]
      public void Should_Not_Throw_When_Body_Expression_Matches_Filter_Expression_Type()
      {
        Expression<Func<int[]>> expression = () => new[] { 1, 2, 3 };

        Invoking(
            () => _parser.Parse(expression, ExpressionInfoType.Constant))
          .Should()
          .NotThrow();
      }

      private static string GetNonPermittedExpressionTypeError(ExpressionInfoType infoType)
      {
        return $"The expression type '{infoType}' is not permitted.";
      }
    }
  }
}