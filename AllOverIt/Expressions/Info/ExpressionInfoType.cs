using System;

namespace AllOverIt.Expressions.Info
{
  // A enumeration of available expression info types.
  [Flags]
  public enum ExpressionInfoType
  {
    // A constant expression.
    Constant = 1,

    // A field expression.
    Field = 2,

    // A property expression.
    Property = 4,

    // A binary comparison expression.
    BinaryComparison = 8,

    // A method call expression.
    MethodCall = 16,

    // A conditional expression
    Conditional = 32,

    // a member initialization expression
    MemberInit = 64,

    // a new expression
    New = 128,

    // A parameter expression
    Parameter = 256
  }
}