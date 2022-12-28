using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.Pipeline;
using AllOverIt.Patterns.Pipeline.Extensions;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Tests.Patterns.ChainOfResponsibility.Extensions
{
    public class PipelineStepBuilderExtensionsFixture : FixtureBase
    {
        internal class PipelineStepBuilderDummy : IPipelineStepBuilder<int, double>
        {
            public Func<int, double> Build()
            {
                throw new NotImplementedException();
            }
        }

        internal class StepDummy : IPipelineStep<double, string>
        {
            public string Execute(double input)
            {
                throw new NotImplementedException();
            }
        }

        private readonly PipelineStepBuilderDummy _dummyBuilder = new();
        private readonly Func<double, string> _funcStep = value => value.ToString();
        private readonly StepDummy _stepDummy = new();

        public class Pipe_Func : PipelineStepBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    PipelineStepBuilderExtensions.Pipe<int, double, string>((IPipelineStepBuilder<int, double>) null, _funcStep);
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
                    PipelineStepBuilderExtensions.Pipe<int, double, string>(_dummyBuilder, (Func<double, string>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = PipelineStepBuilderExtensions.Pipe<int, double, string>(_dummyBuilder, _funcStep);

                actual.Should().BeOfType<PipelineStepBuilder<int, double, string>>();
            }
        }

        public class Pipe_Step_Instance : PipelineStepBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    PipelineStepBuilderExtensions.Pipe<int, double, string>((IPipelineStepBuilder<int, double>) null, _stepDummy);
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
                    PipelineStepBuilderExtensions.Pipe<int, double, string>(_dummyBuilder, (IPipelineStep<double, string>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = PipelineStepBuilderExtensions.Pipe<int, double, string>(_dummyBuilder, _stepDummy);

                actual.Should().BeOfType<PipelineStepBuilder<int, double, string>>();
            }
        }

        public class Pipe_Step_Type : PipelineStepBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    PipelineStepBuilderExtensions.Pipe<StepDummy, int, double, string>((IPipelineStepBuilder<int, double>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("prevStep");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = PipelineStepBuilderExtensions.Pipe<StepDummy, int, double, string>(_dummyBuilder);

                actual.Should().BeOfType<PipelineStepBuilder<int, double, string>>();
            }
        }
    }
}