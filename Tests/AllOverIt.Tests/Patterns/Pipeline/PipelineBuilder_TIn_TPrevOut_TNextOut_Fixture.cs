using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.Pipeline;
using AllOverIt.Patterns.Pipeline.Extensions;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Tests.Patterns.Pipeline
{
    public class PipelineBuilder_TIn_TPrevOut_TNextOut_Fixture : FixtureBase
    {
        public class Constructor : PipelineBuilder_TIn_TPrevOut_TNextOut_Fixture
        {
            [Fact]
            public void Should_Throw_When_PrevStep_Null()
            {
                Invoking(() =>
                {
                    Func<double, string> step = value => value.ToString();

                    _ = new PipelineBuilder<int, double, string>(null, step);
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
                    var prevStep = A.Fake<IPipelineBuilder<int, double>>();

                    _ = new PipelineBuilder<int, double, string>(prevStep, null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }
        }

        public class Build : PipelineBuilder_TIn_TPrevOut_TNextOut_Fixture
        {
            [Fact]
            public void Should_Return_Composed_Func()
            {
                var factor = Create<double>();

                // IPipelineBuilder<int, double>
                var builder1 = PipelineBuilder.Pipe<int, double>(value => value * factor);

                var builder2 = new PipelineBuilder<int, double, string>(builder1, value => $"{value}");

                var func = builder2.Build();

                var input = Create<int>();
                var expected = $"{input * factor}";

                var actual = func.Invoke(input);

                expected.Should().Be(actual);
            }
        }
    }
}