using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.Pipeline;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Tests.Patterns.Pipeline
{
    public class PipelineBuilder_TIn_TOut_Fixture : FixtureBase
    {
        public class Constructor : PipelineBuilder_TIn_TOut_Fixture
        {
            [Fact]
            public void Should_Throw_When_Step_Null()
            {
                Invoking(() =>
                {
                    Func<int, double> step = null;

                    _ = new PipelineBuilder<int, double>(step);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }
        }

        public class Build : PipelineBuilder_TIn_TOut_Fixture
        {
            [Fact]
            public void Should_Return_Func()
            {
                Func<int, double> step = value => (double) value;

                var builder = new PipelineBuilder<int, double>(step);

                var actual = builder.Build();

                actual.Should().BeSameAs(step);
            }
        }
    }
}