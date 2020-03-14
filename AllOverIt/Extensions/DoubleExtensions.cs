using System;

namespace AllOverIt.Extensions
{
  public static class DoubleExtensions
  {
    private const double Epsilon = 1E-07;

    public static bool IsZero(this double value)
    {
      return value.IsEqualTo(0.0d);
    }

    public static bool IsEqualTo(this double lhs, double rhs)
    {
      return Math.Abs(lhs - rhs) < Epsilon;
    }
  }
}