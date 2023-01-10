using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.Pipeline;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Tests.Patterns.Pipeline
{
    public class PipelineBuilderAsync_TIn_TOut : FixtureBase
    {
        public class Constructor : PipelineBuilderAsync_TIn_TOut
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

        public class Build : PipelineBuilderAsync_TIn_TOut
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

    public class PipelineBuilderAsync_TIn_TPrevOut_TNextOut : FixtureBase
    {
        public class Constructor : PipelineBuilderAsync_TIn_TPrevOut_TNextOut
        {
            [Fact]
            public void Should_Throw_When_PrevStep_Null()
            {
                Invoking(() =>
                {
                    Func<double, Task<string>> step = value => Task.FromResult(value.ToString());

                    _ = new PipelineBuilderAsync<int, double, string>(null, step);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("prevStep");
            }

            [Fact]
            public void Should_Throw_When_Step_Null()
            {
                Invoking(() =>
                {
                    var prevStep = A.Fake<IPipelineBuilderAsync<int, double>>();

                    _ = new PipelineBuilderAsync<int, double, string>(prevStep, null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }
        }

        public class Build : PipelineBuilderAsync_TIn_TPrevOut_TNextOut
        {
            [Fact]
            public async Task Should_Return_Composed_Func()
            {
                var factor = Create<double>();

                // IPipelineBuilderAsync<int, double>
                var builder1 = PipelineBuilder.PipeAsync<int, double>(value => Task.FromResult(value * factor));

                var builder2 = new PipelineBuilderAsync<int, double, string>(builder1, value => Task.FromResult($"{value}"));

                var func = builder2.Build();

                var input = Create<int>();
                var expected = $"{input * factor}";

                var actual = await func.Invoke(input);

                expected.Should().Be(actual);
            }
        }
    }
}