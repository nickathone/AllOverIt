using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AllOverIt.Expressions.Info
{
  // An info class for a property expression.
  public sealed class PropertyExpressionInfo : ExpressionInfoBase, IExpressionValue, INegatableExpressionInfo
  {
    // The property's info.
    public PropertyInfo PropertyInfo { get; }

    // The property's value.
    public object Value { get; }

    // Indicates if the property's value should be negated.
    public bool IsNegated { get; }

    public PropertyExpressionInfo(Expression expression, PropertyInfo propertyInfo, object value, bool isNegated)
      : base(expression, ExpressionInfoType.Property)
    {
      PropertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
      Value = value;
      IsNegated = isNegated;
    }
  }
}