#  Version 7.0.0
## \<Date>
---

# General Notes
* Multiple dependency package updates across most packages. These are not noted below. Refer to the dependency
  diagram available in the `/Docs/Dependencies` folder.


## AllOverIt
* Added interceptor support to cater for Aspected Oriented Programming (AOP)
* Added string extension `IsEmpty()`
* `IAsyncEnumerable` extension updates
* Added async version of `ChainOfResponsibilityHandler` and `ChainOfResponsibilityComposer`
* Added a pipeline pattern implementation
* Added `AllOverIt.Plugin.PluginLoadContext` to dynamically load assemblies (for .NET Core 3.1 and above)
* Fixed some `Breadcrumbs` overloads that were not recording the caller details
* Updates to string comparison utils (expressions) to detect null constant values and convert / throw as required
* General tidy up of all exception classes
* Deprecated `AllOverIt.Process`, replacing it with `ProcessExecutor`
* Added Task extensions `FireAndForget()`
* Removed unnecessary 'continueOnCapturedContext' argument from `TaskHelper.WhenAll()`
* Added FileUtils `CreateFileWithContentAsync()`
* Added commandline argument helper to escape strings
* Added `IComparer` extension methods `Reverse()` and `Then()`. The latter allows them to be composed like Chain of Responsibility.
* Added object extension `GetObjectElements()`
* Unseal `ReadOnlyCollection`, `ReadOnlyList`, `ReadOnlyDictionary`
* Changed `RepeatingTask.Start()` to use overloads rather than a default initial delay (previously after the `CancellationToken`)


## AllOverIt.AspNetCore
* Unchanged


## AllOverIt.Assertion
* Unchanged


## AllOverIt.Aws.Appsync.Client
* Unchanged


## AllOverIt.Aws.Cdk.Appsync
* Unchanged


## AllOverIt.Csv
* Unchanged


## AllOverIt.DependencyInjection
* Added support for registering named services


## AllOverIt.EntityFrameworkCore
* Unchanged


## AllOverIt.EntityFrameworkCore.Diagrams :new:
* Adds support for generating D2 entity relationship diagrams and exporting to text, svg, png, pdf


## AllOverIt.EntityFrameworkCore.Pagination
* Unchanged


## AllOverIt.Evaluator
* Unchanged


## AllOverIt.Filtering
* Unchanged


## AllOverIt.Fixture
* Added exception assertion helpers


## AllOverIt.Fixture.FakeItEasy
* Unchanged


## AllOverIt.GenericHost
* Unchanged


## AllOverIt.Mapping :new:
* Moved `ObjectMapper` from `AllOverIt` into this dedicated package
* Updated support `ArrayList`


## AllOverIt.Pagination
* Added `GetPageResults()` extension method for `IQueryPaginator<TResult>`. This is intended ONLY for memory based queries.
  It Performs the same job as `GetPageResultsAsync()` found in `AllOverIt.EntityFrameworkCore.Pagination`.


## AllOverIt.Reactive
* Unchanged


## AllOverIt.ReactiveUI
* Added support for a `ReactiveCommand` based pipeline (with support for chaining other sync / async pipeline steps)


## AllOverIt.Serialization.Binary :new:
* Moved the binary serialization from `AllOverIt` into this dedicated package


## AllOverIt.Serialization.Json.Abstractions
* Renamed from `AllOverIt.Serialization.Abstractions`. If upgrading from a previous version then the package reference
  and usages of the `AllOverIt.Serialization.Abstractions` namespace will require manual updates.


## AllOverIt.Serialization.Json.Newtonsoft
* Renamed from `AllOverIt.Serialization.NewtonsoftJson`. If upgrading from a previous version then the package reference
  and usages of the `AllOverIt.Serialization.NewtonsoftJson` namespace will require manual updates.


## AllOverIt.Serialization.Json.SystemText
* Renamed from `AllOverIt.Serialization.SystemTextJson`. If upgrading from a previous version then the package reference
  and usages of the `AllOverIt.Serialization.SystemTextJson` namespace will require manual updates.
* Added an option to `NestedDictionaryConverter` so floating values can be read as double or decimal.


## AllOverIt.Validation
* Unchanged

## AllOverIt.Validation.Options :new:
* Provides support for Configuration Options validation using `FluentValidation` and `AllOverIt.Validation` via the
  `OptionsBuilder<TOptions>` extension method called `UseFluentValidation()`.


## AllOverIt.Wpf :new:
* Includes a `PreviewTextBox` control
