﻿using AllOverIt.Expressions.Strings;
using AllOverIt.Filtering.Operations;
using AllOverIt.Filtering.Options;
using AllOverIt.Fixture.Extensions;
using FakeItEasy;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Filtering.Tests.Operations
{
    public class StartsWithFixture : OperationsFixtureBase
    {
        [Fact]
        public void Should_Throw_When_PropertyExpression_Null()
        {
            Invoking(() =>
            {
                _ = new StartsWithOperation<DummyClass>(null, Create<string>(), A.Fake<IOperationFilterOptions>());
            })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("propertyExpression");
        }

        [Fact]
        public void Should_Throw_When_Options_Null()
        {
            Invoking(() =>
            {
                _ = new StartsWithOperation<DummyClass>(model => model.Name, Create<string>(), null);
            })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("options");
        }

        [Theory]
        [MemberData(nameof(FilterComparisonOptions))]
        public void Should_Satisfy_Specification(bool useParameterizedQueries, StringComparisonMode stringComparisonMode)
        {
            var options = new OperationFilterOptions
            {
                UseParameterizedQueries = useParameterizedQueries,
                StringComparisonMode = stringComparisonMode
            };

            var operation = new StartsWithOperation<DummyClass>(model => model.Name, Model.Name[..4], options);

            operation.IsSatisfiedBy(Model).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(FilterComparisonOptions))]
        public void Should_Not_Satisfy_Specification(bool useParameterizedQueries, StringComparisonMode stringComparisonMode)
        {
            var options = new OperationFilterOptions
            {
                UseParameterizedQueries = useParameterizedQueries,
                StringComparisonMode = stringComparisonMode
            };

            var operation = new StartsWithOperation<DummyClass>(model => model.Name, Create<string>(), options);

            operation.IsSatisfiedBy(Model).Should().BeFalse();
        }
    }
}
