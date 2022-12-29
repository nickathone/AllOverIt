using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.Pipeline;
using AllOverIt.Patterns.Pipeline.Extensions;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Tests.Patterns.ChainOfResponsibility.Extensions
{
    public class PipelineBuilderExtensionsFixture : FixtureBase
    {
        internal class PipelineBuilderDummy : IPipelineBuilder<int, double>
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

        internal class PipelineBuilderAsyncDummy : IPipelineBuilderAsync<int, double>
        {
            public Func<int, Task<double>> Build()
            {
                throw new NotImplementedException();
            }
        }

        internal class StepAsyncDummy : IPipelineStepAsync<double, string>
        {
            public Task<string> ExecuteAsync(double input)
            {
                throw new NotImplementedException();
            }
        }

        private readonly PipelineBuilderDummy _builderDummy = new();
        private readonly Func<double, string> _funcStep = value => value.ToString();
        private readonly StepDummy _stepDummy = new();

        private readonly PipelineBuilderAsyncDummy _builderAsyncDummy = new();
        private readonly Func<double, Task<string>> _funcStepAsync = value => Task.FromResult(value.ToString());
        private readonly StepAsyncDummy _stepAsyncDummy = new();

        public class Pipe_Func : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = PipelineStepBuilderExtensions.Pipe<int, double, string>((IPipelineBuilder<int, double>) null, _funcStep);
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
                    _ = PipelineStepBuilderExtensions.Pipe<int, double, string>(_builderDummy, (Func<double, string>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = PipelineStepBuilderExtensions.Pipe<int, double, string>(_builderDummy, _funcStep);

                actual.Should().BeOfType<PipelineBuilder<int, double, string>>();
            }
        }

        public class Pipe_Step_Instance : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = PipelineStepBuilderExtensions.Pipe<int, double, string>((IPipelineBuilder<int, double>) null, _stepDummy);
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
                    _ = PipelineStepBuilderExtensions.Pipe<int, double, string>(_builderDummy, (IPipelineStep<double, string>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = PipelineStepBuilderExtensions.Pipe<int, double, string>(_builderDummy, _stepDummy);

                actual.Should().BeOfType<PipelineBuilder<int, double, string>>();
            }
        }

        public class Pipe_Step_Type : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = PipelineStepBuilderExtensions.Pipe<StepDummy, int, double, string>((IPipelineBuilder<int, double>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("prevStep");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = PipelineStepBuilderExtensions.Pipe<StepDummy, int, double, string>(_builderDummy);

                actual.Should().BeOfType<PipelineBuilder<int, double, string>>();
            }
        }

        public class PipeAsync_FuncAsync : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = PipelineStepBuilderExtensions.PipeAsync<int, double, string>((IPipelineBuilderAsync<int, double>) null, _funcStepAsync);
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
                    _ = PipelineStepBuilderExtensions.PipeAsync<int, double, string>(_builderAsyncDummy, (Func<double, Task<string>>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = PipelineStepBuilderExtensions.PipeAsync<int, double, string>(_builderAsyncDummy, _funcStepAsync);

                actual.Should().BeOfType<PipelineBuilderAsync<int, double, string>>();
            }
        }

        public class PipeAsync_StepAsync_Instance : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = PipelineStepBuilderExtensions.PipeAsync<int, double, string>((IPipelineBuilderAsync<int, double>) null, _stepAsyncDummy);
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
                    _ = PipelineStepBuilderExtensions.PipeAsync<int, double, string>(_builderAsyncDummy, (IPipelineStepAsync<double, string>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = PipelineStepBuilderExtensions.PipeAsync<int, double, string>(_builderAsyncDummy, _stepAsyncDummy);

                actual.Should().BeOfType<PipelineBuilderAsync<int, double, string>>();
            }
        }

        public class PipeAsync_StepAsync_Type : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = PipelineStepBuilderExtensions.PipeAsync<StepAsyncDummy, int, double, string>((IPipelineBuilderAsync<int, double>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("prevStep");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = PipelineStepBuilderExtensions.PipeAsync<StepAsyncDummy, int, double, string>(_builderAsyncDummy);

                actual.Should().BeOfType<PipelineBuilderAsync<int, double, string>>();
            }
        }

        public class PipeAsync_Func : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = PipelineStepBuilderExtensions.Pipe<int, double, string>((IPipelineBuilderAsync<int, double>) null, _funcStep);
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
                    _ = PipelineStepBuilderExtensions.Pipe<int, double, string>(_builderAsyncDummy, (Func<double, string>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = PipelineStepBuilderExtensions.Pipe<int, double, string>(_builderAsyncDummy, _funcStep);

                actual.Should().BeOfType<PipelineBuilderAsync<int, double, string>>();
            }
        }

        public class PipeAsync_Step_Instance : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = PipelineStepBuilderExtensions.Pipe<int, double, string>((IPipelineBuilderAsync<int, double>) null, _stepDummy);
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
                    _ = PipelineStepBuilderExtensions.Pipe<int, double, string>(_builderAsyncDummy, (IPipelineStep<double, string>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = PipelineStepBuilderExtensions.Pipe<int, double, string>(_builderAsyncDummy, _stepDummy);

                actual.Should().BeOfType<PipelineBuilderAsync<int, double, string>>();
            }
        }

        public class PipeAsync_Step_Type : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = PipelineStepBuilderExtensions.Pipe<StepDummy, int, double, string>((IPipelineBuilderAsync<int, double>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("prevStep");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = PipelineStepBuilderExtensions.Pipe<StepDummy, int, double, string>(_builderAsyncDummy);

                actual.Should().BeOfType<PipelineBuilderAsync<int, double, string>>();
            }
        }
    }
}