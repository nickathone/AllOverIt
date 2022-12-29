using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.Pipeline;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Tests.Patterns.Pipeline
{
    public class PipelineBuilderAsync_TIn_TOut_Fixture : FixtureBase
    {
        public class Constructor : PipelineBuilderAsync_TIn_TOut_Fixture
        {
            [Fact]
            public void Should_Throw_When_Step_Null()
            {
                Invoking(() =>
                {
                    Func<int, Task<double>> step = null;

                    _ = new PipelineBuilderAsync<int, double>(step);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }
        }

        public class Build : PipelineBuilderAsync_TIn_TOut_Fixture
        {
            [Fact]
            public void Should_Return_Func()
            {
                Func<int, Task<double>> step = value => Task.FromResult((double) value);

                var builder = new PipelineBuilderAsync<int, double>(step);

                var actual = builder.Build();

                actual.Should().BeSameAs(step);
            }
        }
    }
}