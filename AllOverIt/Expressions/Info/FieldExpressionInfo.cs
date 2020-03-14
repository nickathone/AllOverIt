using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AllOverIt.Expressions.Info
{
  // An info class for a field expression.
  public sealed class FieldExpressionInfo : ExpressionInfoBase, IExpressionValue, INegatableExpressionInfo
  {
    // The field's FieldInfo.
    public FieldInfo FieldInfo { get; }

    // The field's value.
    public object Value { get; }

    // Indicates if the property's value should be negated.
    public bool IsNegated { get; }

    public FieldExpressionInfo(Expression expression, FieldInfo fieldInfo, object value, bool isNegated)
      : base(expression, ExpressionInfoType.Field)
    {
      FieldInfo = fieldInfo ?? throw new ArgumentNullException(nameof(fieldInfo));
      Value = value;
      IsNegated = isNegated;
    }
  }
}