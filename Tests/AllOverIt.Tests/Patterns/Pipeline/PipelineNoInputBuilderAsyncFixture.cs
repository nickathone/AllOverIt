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
    public class PipelineNoInputBuilderAsync_TIn_TOut : FixtureBase
    {
        public class Constructor : PipelineNoInputBuilderAsync_TIn_TOut
        {
            [Fact]
            public void Should_Throw_When_Step_Null()
            {
                Invoking(() =>
                {
                    Func<Task<double>> step = null;

                    _ = new PipelineNoInputBuilderAsync<double>(step);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }
        }

        public class Build : PipelineNoInputBuilderAsync_TIn_TOut
        {
            [Fact]
            public void Should_Return_Func()
            {
                Func<Task<double>> step = () => Task.FromResult(Create<double>());

                var builder = new PipelineNoInputBuilderAsync<double>(step);

                var actual = builder.Build();

                actual.Should().BeSameAs(step);
            }
        }
    }

    public class PipelineNoInputBuilderAsync_TIn_TPrevOut_TNextOut : FixtureBase
    {
        public class Constructor : PipelineNoInputBuilderAsync_TIn_TPrevOut_TNextOut
        {
            [Fact]
            public void Should_Throw_When_PrevStep_Null()
            {
                Invoking(() =>
                {
                    Func<double, Task<string>> step = value => Task.FromResult(value.ToString());

                    _ = new PipelineNoInputBuilderAsync<double, string>(null, step);
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
                    var prevStep = A.Fake<IPipelineBuilderAsync<double>>();

                    _ = new PipelineNoInputBuilderAsync<double, string>(prevStep, null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }
        }

        public class Build : PipelineNoInputBuilderAsync_TIn_TPrevOut_TNextOut
        {
            [Fact]
            public async Task Should_Return_Composed_Func()
            {
                var value = Create<double>();

                // IPipelineBuilderAsync<double>
                var builder1 = PipelineBuilder.PipeAsync<double>(() => Task.FromResult(value));

                var builder2 = new PipelineNoInputBuilderAsync<double, string>(builder1, value => Task.FromResult($"{value}"));

                var func = builder2.Build();

                var expected = $"{value}";

                var actual = await func.Invoke();

                expected.Should().Be(actual);
            }
        }
    }
}