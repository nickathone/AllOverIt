using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.Pipeline;
using AllOverIt.Patterns.Pipeline.Extensions;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Tests.Patterns.Pipeline
{
    public class PipelineBuilderFixture : FixtureBase
    {
        public class Pipe_Func : PipelineBuilderFixture
        {
            [Fact]
            public void Should_Throw_When_Func_Null()
            {
                Func<int, double> step = null;

                Invoking(() =>
                {
                    _ = PipelineBuilder.Pipe(step);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Return_Pipeline_Builder()
            {
                Func<int, double> step = value => (double) value;

                var actual = PipelineBuilder.Pipe(step);

                actual.Should().BeOfType<PipelineBuilder<int, double>>();
            }

            [Fact]
            public void Should_Create_Pipeline_Step()
            {
                var invoked = false;

                Func<int, double> step = value =>
                {
                    invoked = true;
                    return (double) value;
                };

                var actual = PipelineBuilder.Pipe(step);

                actual.Build().Invoke(Create<int>());

                invoked.Should().BeTrue();
            }

            [Fact]
            public void Should_Create_Pipeline_Sequence()
            {
                var counter = 0;
                var invoked1 = 0;
                var invoked2 = 0;

                Func<int, double> step1 = value =>
                {
                    counter++;
                    invoked1 = counter;
                    return (double) value;
                };

                Func<double, string> step2 = value =>
                {
                    counter++;
                    invoked2 = counter;

                    return value.ToString();
                };

                var actual = PipelineBuilder.Pipe(step1).Pipe(step2);

                actual.Build().Invoke(Create<int>());

                invoked1.Should().Be(1);
                invoked2.Should().Be(2);
            }
        }

        public class Pipe_PipelineStep_Instance : PipelineBuilderFixture
        {
            [Fact]
            public void Should_()
            {

            }
        }

        public class Pipe_PipelineStep_Type : PipelineBuilderFixture
        {
            [Fact]
            public void Should_()
            {

            }
        }

        public class PipeAsync_Func : PipelineBuilderFixture
        {
            [Fact]
            public void Should_Throw_When_Func_Null()
            {
                Func<int, Task<double>> step = null;

                Invoking(() =>
                {
                    _ = PipelineBuilder.PipeAsync(step);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Return_Pipeline_Builder()
            {
                Func<int, Task<double>> step = value => Task.FromResult((double) value);

                var actual = PipelineBuilder.PipeAsync(step);

                actual.Should().BeOfType<PipelineBuilderAsync<int, double>>();
            }

            [Fact]
            public async Task Should_Create_Pipeline_Step()
            {
                var invoked = false;

                Func<int, Task<double>> step = value =>
                {
                    invoked = true;
                    return Task.FromResult((double) value);
                };

                var actual = PipelineBuilder.PipeAsync(step);

                await actual.Build().Invoke(Create<int>());

                invoked.Should().BeTrue();
            }

            [Fact]
            public void Should_Create_Pipeline_Sequence()
            {
                var counter = 0;
                var invoked1 = 0;
                var invoked2 = 0;

                Func<int, Task<double>> step1 = value =>
                {
                    counter++;
                    invoked1 = counter;
                    return Task.FromResult((double) value);
                };

                Func<double, Task<string>> step2 = value =>
                {
                    counter++;
                    invoked2 = counter;
                    return Task.FromResult(value.ToString());
                };

                var actual = PipelineBuilder.PipeAsync(step1).PipeAsync(step2);

                actual.Build().Invoke(Create<int>());

                invoked1.Should().Be(1);
                invoked2.Should().Be(2);
            }
        }

        public class PipeAsync_PipelineStep_Instance : PipelineBuilderFixture
        {
            [Fact]
            public void Should_()
            {

            }
        }

        public class PipeAsync_PipelineStep_Type : PipelineBuilderFixture
        {
            [Fact]
            public void Should_()
            {

            }
        }
    }
}