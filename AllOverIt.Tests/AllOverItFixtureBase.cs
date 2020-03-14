using AllOverIt.Fixture;

namespace AllOverIt.Tests
{
  public class AllOverItFixtureBase : AoiFixtureBase
  {
    internal static string GetExpectedArgumentNullExceptionMessage(string name)
    {
      return $"Value cannot be null. (Parameter '{name}')";
    }
  }
}