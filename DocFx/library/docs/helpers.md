# Helpers
---

## EnumHelper

```csharp
public static IReadOnlyCollection<TType> GetEnumValues<TType>()
  where TType : struct, Enum;
```

Provides an easy way to obtain a list of all possible values for a given enum type. For example:

```csharp
// returns Static, Instance, Abstract, Virtual, NonVirtual, Internal, Private,
//         Protected, Public, DefaultScope, DefaultAccessor, DefaultVisibility,
//         Default, AllScope, AllAccessor, AllVisibility, All
var enumValues = EnumHelper.GetEnumValues<BindingOptions>();
```

## Raii
The `Raii` and `Raii<TType>` classes implement the Resource Acquisition Is Initialization idiom. During construction a resource is acquired or initialized and during disposal there is an opportunity to perform custom last-minute cleanup. These classes should ideally be used within a `using()` block to ensure the disposal is deterministic.

While the `Raii` class is often used for things such as creating and disposing of database connections, it's also useful for automating actions that need to be performed when a unit of work has been completed. The following example represents a _logger_ that sends a message to an `Action`. The action is a simplified representation of an injected dependency (this implementation is for illustration purposes only).

```csharp
public class Logger
{
  private class ProfilerContext
  {
    public Stopwatch Stopwatch { get; }
    public string Title { get; }

    public ProfilerContext(string title)
    {
      Stopwatch = Stopwatch.StartNew();
      Title = title;
    }
  }

  // use the to emulate logging to some external output
  private readonly Action<string> _action;

  public Logger(Action<string> action)
  {
    _action = action;
  }

  public void LogMessage(string message)
  {
    _action.Invoke(message);;
  }

  public IDisposable GetProfiler(string title)
  {
    return new Raii<ProfilerContext>(
      () => new ProfilerContext(title),
      context => StopProfiling(context)
    );
  }

  private void StopProfiling(ProfilerContext context)
  {
    var message = $"{context.Title} took {context.Stopwatch.ElapsedMilliseconds}ms";

    LogMessage(message);
  }
}
```

The `Logger` contains a `GetProfiler()` method that returns an `IDisposable` in the form of a `Raii<ProfilerContext>`. During construction of this object a `Stopwatch` is created and immediately starts running. When the `IDisposable` is diposed of, the cleanup action is executed, resulting in an elapsed log message being processed via the `StopProfiling()` method.

The profiler would be used like so:

```csharp
using (logger.GetProfiler(title))
{
  // the code to be profiled goes here(false);
}
```
