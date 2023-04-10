# Overview
**AllOverIt** began as a general purpose library in 2015 and has since evolved into a suite of libraries
aimed at providing a simplified, consistent, approach to cross-cutting and functional concerns such as
caching, serialization, threading, reflection, conversions, mapping, event messaging, validation,
AWS AppSync, data (`IEnumable<T>` and `IQueryable<T>`) filtering and pagination, and much more.

The suite has an ever growing list of behavioural and functional unit tests. The coverage details can
be found below.

| Line Coverage                                     | Branch Coverage                                     | Method Coverage                                     |
| --------------------------------------------------|-----------------------------------------------------|---------------------------------------------------- |
| ![](/Docs/Code%20Coverage/badge_linecoverage.png) | ![](/Docs/Code%20Coverage/badge_branchcoverage.png) | ![](/Docs/Code%20Coverage/badge_methodcoverage.png) |


[Refer to this link](/Docs/Code%20Coverage/summary.link) for a summary of line and branch test code coverage.


# Packages

---

## AllOverIt

A general purpose library containing a variety of classes and helper utilities.

| netstandard2.1     | netcoreapp3.1      | net5.0             | net6.0             | net7.0             |
| -------------------|--------------------|--------------------|--------------------|------------------- |
| :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |


Refer to the online [Documentation](https://mjfreelancing.github.io/AllOverIt/) for usage information.

[![NuGet](https://img.shields.io/nuget/v/AllOverIt?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt/)
[![NuGet](https://img.shields.io/nuget/dt/AllOverIt?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt/)


---


## AllOverIt.AspNetCore

[![NuGet](https://img.shields.io/nuget/v/AllOverIt.AspNetCore?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.AspNetCore/)
[![NuGet](https://img.shields.io/nuget/dt/AllOverIt.AspNetCore?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.AspNetCore/)

A library containing ASP.NET Core utilities.

| netstandard2.1           | netcoreapp3.1      | net5.0             | net6.0             | net7.0             |
| ------------------------------------------------------------------------------------------------------------ |
| :heavy_multiplication_x: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |


---


## AllOverIt.Assertion
[![NuGet](https://img.shields.io/nuget/v/AllOverIt.Assertion?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.Assertion/)
[![NuGet](https://img.shields.io/nuget/dt/AllOverIt.Assertion?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.Assertion/)

A library containing pre and post condition assertion helper methods.

| netstandard2.1     | netcoreapp3.1      | net5.0             | net6.0             | net7.0             |
| ------------------------------------------------------------------------------------------------------ |
| :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |


---


## AllOverIt.Aws.AppSync.Client
[![NuGet](https://img.shields.io/nuget/v/AllOverIt.Aws.AppSync.Client?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.Aws.AppSync.Client/)
[![NuGet](https://img.shields.io/nuget/dt/AllOverIt.Aws.AppSync.Client?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.Aws.AppSync.Client/)

A library containing AppSync GraphQL and Subscription clients.

| netstandard2.1     | netcoreapp3.1      | net5.0             | net6.0             | net7.0             |
| ------------------------------------------------------------------------------------------------------ |
| :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |


---


## AllOverIt.Aws.Cdk.AppSync
[![NuGet](https://img.shields.io/nuget/v/AllOverIt.Aws.Cdk.AppSync?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.Aws.Cdk.AppSync/)
[![NuGet](https://img.shields.io/nuget/dt/AllOverIt.Aws.Cdk.AppSync?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.Aws.Cdk.AppSync/)

A library to help build AWS Graphql schemas using a code-first approach.

| netstandard2.1        | netcoreapp3.1      | net5.0                | net6.0                | net7.0                |
| ------------------------------------------------------------------------------------------------------------------ |
| :heavy_check_mark: \* | :heavy_check_mark: | :heavy_check_mark: \* | :heavy_check_mark: \* | :heavy_check_mark: \* |

\* Although not explicitly built for all platforms, the AWS CDK supports Net Core 3.1 and above.


---


## AllOverIt.Csv
[![NuGet](https://img.shields.io/nuget/v/AllOverIt.Csv?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.Csv/)
[![NuGet](https://img.shields.io/nuget/dt/AllOverIt.Csv?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.Csv/)

A library to assist with CSV export using CsvHelper.

| netstandard2.1     | netcoreapp3.1      | net5.0             | net6.0             | net7.0             |
| ------------------------------------------------------------------------------------------------------ |
| :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |


---


## AllOverIt.DependencyInjection
[![NuGet](https://img.shields.io/nuget/v/AllOverIt.DependencyInjection?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.DependencyInjection/)
[![NuGet](https://img.shields.io/nuget/dt/AllOverIt.DependencyInjection?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.DependencyInjection/)

A library containing useful dependency injection related utilities.

| netstandard2.1     | netcoreapp3.1      | net5.0             | net6.0             | net7.0             |
| ------------------------------------------------------------------------------------------------------ |
| :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |


---


## AllOverIt.EntityFrameworkCore
[![NuGet](https://img.shields.io/nuget/v/AllOverIt.EntityFrameworkCore?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.EntityFrameworkCore/)
[![NuGet](https://img.shields.io/nuget/dt/AllOverIt.EntityFrameworkCore?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.EntityFrameworkCore/)

A library providing utilities for use with EntityFramework Core.

| netstandard2.1     | netcoreapp3.1      | net5.0             | net6.0             | net7.0             |
| ------------------------------------------------------------------------------------------------------ |
| :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |


---


## AllOverIt.EntityFrameworkCore.Diagrams
[![NuGet](https://img.shields.io/nuget/v/AllOverIt.EntityFrameworkCore.Diagrams?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.EntityFrameworkCore.Diagrams/)
[![NuGet](https://img.shields.io/nuget/dt/AllOverIt.EntityFrameworkCore.Diagrams?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.EntityFrameworkCore.Diagrams/)

A library providing formatters to generate ERD diagrams using EntityFramework Core.

| netstandard2.1     | netcoreapp3.1      | net5.0             | net6.0             | net7.0             |
| ------------------------------------------------------------------------------------------------------ |
| :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |


---


## AllOverIt.EntityFrameworkCore.Pagination
[![NuGet](https://img.shields.io/nuget/v/AllOverIt.EntityFrameworkCore.Pagination?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.EntityFrameworkCore.Pagination/)
[![NuGet](https://img.shields.io/nuget/dt/AllOverIt.EntityFrameworkCore.Pagination?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.EntityFrameworkCore.Pagination/)

A library providing keyset-based pagination for use with EntityFramework Core.

| netstandard2.1     | netcoreapp3.1      | net5.0             | net6.0             | net7.0             |
| ------------------------------------------------------------------------------------------------------ |
| :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |


---


## AllOverIt.Evaluator
[![NuGet](https://img.shields.io/nuget/v/AllOverIt.Evaluator?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.Evaluator/)
[![NuGet](https://img.shields.io/nuget/dt/AllOverIt.Evaluator?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.Evaluator/)

A library containing an extendable expression compiler and evaluator.

| netstandard2.1     | netcoreapp3.1      | net5.0             | net6.0             | net7.0             |
| ------------------------------------------------------------------------------------------------------ |
| :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |


---


## AllOverIt.Filtering
[![NuGet](https://img.shields.io/nuget/v/AllOverIt.Filtering?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.Filtering/)
[![NuGet](https://img.shields.io/nuget/dt/AllOverIt.Filtering?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.Filtering/)

A library providing queryable filtering utilities.

| netstandard2.1     | netcoreapp3.1      | net5.0             | net6.0             | net7.0             |
| ------------------------------------------------------------------------------------------------------ |
| :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |


---


## AllOverIt.Fixture
[![NuGet](https://img.shields.io/nuget/v/AllOverIt.Fixture?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.Fixture/)
[![NuGet](https://img.shields.io/nuget/dt/AllOverIt.Fixture?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.Fixture/)

A library containing a base fixture class with numerous helper methods to assist with creating unit test scaffolding. Utilizes AutoFixture to do most of the hard work.

| netstandard2.1     | netcoreapp3.1      | net5.0             | net6.0             | net7.0             |
| ------------------------------------------------------------------------------------------------------ |
| :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |


---


## AllOverIt.Fixture.FakeItEasy
[![NuGet](https://img.shields.io/nuget/v/AllOverIt.Fixture.FakeItEasy?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.Fixture.FakeItEasy/)
[![NuGet](https://img.shields.io/nuget/dt/AllOverIt.Fixture.FakeItEasy?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.Fixture.FakeItEasy/)

A library extending **AllOverIt.Fixture** to support FakeItEasy integration.

| netstandard2.1     | netcoreapp3.1      | net5.0             | net6.0             | net7.0             |
| ------------------------------------------------------------------------------------------------------ |
| :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |


---


## AllOverIt.GenericHost
[![NuGet](https://img.shields.io/nuget/v/AllOverIt.GenericHost?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.GenericHost/)
[![NuGet](https://img.shields.io/nuget/dt/AllOverIt.GenericHost?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.GenericHost/)

A library containing a convenient wrapper for building console applications that support dependency injection.

| netstandard2.1     | netcoreapp3.1      | net5.0             | net6.0             | net7.0             |
| ------------------------------------------------------------------------------------------------------ |
| :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |


---


## AllOverIt.Mapping
[![NuGet](https://img.shields.io/nuget/v/AllOverIt.Mapping?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.Mapping/)
[![NuGet](https://img.shields.io/nuget/dt/AllOverIt.Mapping?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.Mapping/)

A library containing an object mapper that is mostly configuration free.

| netstandard2.1     | netcoreapp3.1      | net5.0             | net6.0             | net7.0             |
| ------------------------------------------------------------------------------------------------------ |
| :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |


---


## AllOverIt.Pagination
[![NuGet](https://img.shields.io/nuget/v/AllOverIt.Pagination?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.Pagination/)
[![NuGet](https://img.shields.io/nuget/dt/AllOverIt.Pagination?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.Pagination/)

A library providing queryable keyset-based pagination utilities.

| netstandard2.1     | netcoreapp3.1      | net5.0             | net6.0             | net7.0             |
| ------------------------------------------------------------------------------------------------------ |
| :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |


---


## AllOverIt.Reactive
[![NuGet](https://img.shields.io/nuget/v/AllOverIt.Reactive?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.Reactive/)
[![NuGet](https://img.shields.io/nuget/dt/AllOverIt.Reactive?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.Reactive/)

A library containing utility extensions for use with System.Reactive.

| netstandard2.1     | netcoreapp3.1      | net5.0             | net6.0             | net7.0             |
| ------------------------------------------------------------------------------------------------------ |
| :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |


---


## AllOverIt.ReactiveUI
[![NuGet](https://img.shields.io/nuget/v/AllOverItUI.Reactive?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.ReactiveUI/)
[![NuGet](https://img.shields.io/nuget/dt/AllOverItUI.Reactive?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.ReactiveUI/)

A library containing utility extensions for use with ReactiveUI.

| netstandard2.1     | netcoreapp3.1      | net5.0             | net6.0             | net7.0             |
| ------------------------------------------------------------------------------------------------------ |
| :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |


---


## AllOverIt.Serialization.Binary
[![NuGet](https://img.shields.io/nuget/v/AllOverIt.Serialization.Binary?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.Serialization.Binary/)
[![NuGet](https://img.shields.io/nuget/dt/AllOverIt.Serialization.Binary?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.Serialization.Binary/)

A library providing support for binary serialization.

| netstandard2.1     | netcoreapp3.1      | net5.0             | net6.0             | net7.0             |
| ------------------------------------------------------------------------------------------------------ |
| :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |


---


## AllOverIt.Serialization.Json.Abstractions
[![NuGet](https://img.shields.io/nuget/v/AllOverIt.Serialization.Json.Abstractions?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.Serialization.Json.Abstractions/)
[![NuGet](https://img.shields.io/nuget/dt/AllOverIt.Serialization.Json.Abstractions?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.Serialization.Json.Abstractions/)

A library containing JSON serialization abstractions.

| netstandard2.1     | netcoreapp3.1      | net5.0             | net6.0             | net7.0             |
| ------------------------------------------------------------------------------------------------------ |
| :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |


---


## AllOverIt.Serialization.Json.NewtonSoft
[![NuGet](https://img.shields.io/nuget/v/AllOverIt.Serialization.Json.NewtonSoft?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.Serialization.Json.NewtonSoft/)
[![NuGet](https://img.shields.io/nuget/dt/AllOverIt.Serialization.Json.NewtonSoft?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.Serialization.Json.NewtonSoft/)

A library containing a wrapper for Newtonsoft.Json serialization based on AllOverIt.Serialization.Json.Abstractions.

| netstandard2.1     | netcoreapp3.1      | net5.0             | net6.0             | net7.0             |
| ------------------------------------------------------------------------------------------------------ |
| :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |


---


## AllOverIt.Serialization.Json.SystemText
[![NuGet](https://img.shields.io/nuget/v/AllOverIt.Serialization.Json.SystemText?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.Serialization.Json.SystemText/)
[![NuGet](https://img.shields.io/nuget/dt/AllOverIt.Serialization.Json.SystemText?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.Serialization.Json.SystemText/)

A library containing a wrapper for System.Text.Json serialization based on AllOverIt.Serialization.Json.Abstractions.

| netstandard2.1     | netcoreapp3.1      | net5.0             | net6.0             | net7.0             |
| ------------------------------------------------------------------------------------------------------ |
| :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |


---


## AllOverIt.Validation
[![NuGet](https://img.shields.io/nuget/v/AllOverIt.Validation?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.Validation/)
[![NuGet](https://img.shields.io/nuget/dt/AllOverIt.Validation?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.Validation/)

A library containing additional validators and extensions for use with FluentValidation.

| netstandard2.1     | netcoreapp3.1      | net5.0             | net6.0             | net7.0             |
| ------------------------------------------------------------------------------------------------------ |
| :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |


---


## AllOverIt.Validation.Options
[![NuGet](https://img.shields.io/nuget/v/AllOverIt.Validation.Options?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.Validation.Options/)
[![NuGet](https://img.shields.io/nuget/dt/AllOverIt.Validation.Options?style=for-the-badge)](https://www.nuget.org/packages/AllOverIt.Validation.Options/)

A library containing Options validation using FluentValidation.

| netstandard2.1     | netcoreapp3.1      | net5.0             | net6.0             | net7.0             |
| ------------------------------------------------------------------------------------------------------ |
| :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |


# Dependencies
The following diagram shows the explicit dependencies used across the entire AllOverIt suite.
[![AllOverIt Dependencies](/Docs/Dependencies/AllOverIt.png)](/Docs/Dependencies/AllOverIt.png)
