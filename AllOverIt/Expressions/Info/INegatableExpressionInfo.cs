namespace AllOverIt.Expressions.Info
{
  public interface INegatableExpressionInfo
  {
    // Indicates if the value of a field, property, or method call should be negated. This includes
    // numerical and boolean values.
    bool IsNegated { get; }
  }
}