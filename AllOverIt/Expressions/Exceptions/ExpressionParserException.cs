using System;

namespace AllOverIt.Expressions.Exceptions
{
  // An exception class for errors raised during the parsing of an expression.
  [Serializable]
  public sealed class ExpressionParserException
    : Exception
  {
    internal ExpressionParserException(string message, Exception innerException = null)
      : base(message, innerException)
    { }
  }
}