using AllOverIt.Fixture;

namespace AllOverIt.Tests
{
  public class AllOverItFixtureBase : AoiFixtureBase
  {
    internal static string GetExpectedArgumentNullExceptionMessage(string name, string errorMessage = default)
    {
      return $"{errorMessage ?? "Value cannot be null."} (Parameter '{name}')";
    }

    internal static string GetExpectedArgumentExceptionMessage(string name, string errorMessage = default)
    {
      return $"{errorMessage} (Parameter '{name}')";
    }
  }
}
