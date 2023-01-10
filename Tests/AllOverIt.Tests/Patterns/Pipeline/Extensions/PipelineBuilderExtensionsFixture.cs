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

        internal class PipelineBuilderNoInputDummy : IPipelineBuilder<double>
        {
            public Func<double> Build()
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

        internal class PipelineBuilderNoInputAsyncDummy : IPipelineBuilderAsync<double>
        {
            public Func<Task<double>> Build()
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
        private readonly PipelineBuilderNoInputDummy _builderNoInputDummy = new();
        private readonly Func<double, string> _funcStep = value => value.ToString();
        private readonly StepDummy _stepDummy = new();

        private readonly PipelineBuilderAsyncDummy _builderAsyncDummy = new();
        private readonly PipelineBuilderNoInputAsyncDummy _builderNoInputAsyncDummy = new();
        private readonly Func<double, Task<string>> _funcStepAsync = value => Task.FromResult(value.ToString());
        private readonly StepAsyncDummy _stepAsyncDummy = new();

        #region Pipe - with input, synchronous, return IPipelineBuilder<TIn, TNextOut>

        public class Pipe_Func_Builder : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = PipelineBuilderExtensions.Pipe<int, double, string>((IPipelineBuilder<int, double>) null, _funcStep);
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
                    _ = PipelineBuilderExtensions.Pipe<int, double, string>(_builderDummy, (Func<double, string>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = PipelineBuilderExtensions.Pipe<int, double, string>(_builderDummy, _funcStep);

                actual.Should().BeOfType<PipelineBuilder<int, double, string>>();
            }
        }

        public class Pipe_Step_Instance_Builder : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = PipelineBuilderExtensions.Pipe<int, double, string>((IPipelineBuilder<int, double>) null, _stepDummy);
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
                    _ = PipelineBuilderExtensions.Pipe<int, double, string>(_builderDummy, (IPipelineStep<double, string>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = PipelineBuilderExtensions.Pipe<int, double, string>(_builderDummy, _stepDummy);

                actual.Should().BeOfType<PipelineBuilder<int, double, string>>();
            }
        }

        public class Pipe_Step_Type_Builder : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = PipelineBuilderExtensions.Pipe<StepDummy, int, double, string>((IPipelineBuilder<int, double>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("prevStep");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = PipelineBuilderExtensions.Pipe<StepDummy, int, double, string>(_builderDummy);

                actual.Should().BeOfType<PipelineBuilder<int, double, string>>();
            }
        }

        #endregion

        #region Pipe - with input, asynchronous, return IPipelineBuilderAsync<TIn, TNextOut>

        public class PipeAsync_FuncAsync_BuilderAsync : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = PipelineBuilderExtensions.PipeAsync<int, double, string>((IPipelineBuilderAsync<int, double>) null, _funcStepAsync);
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
                    _ = PipelineBuilderExtensions.PipeAsync<int, double, string>(_builderAsyncDummy, (Func<double, Task<string>>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = PipelineBuilderExtensions.PipeAsync<int, double, string>(_builderAsyncDummy, _funcStepAsync);

                actual.Should().BeOfType<PipelineBuilderAsync<int, double, string>>();
            }
        }

        public class PipeAsync_StepAsync_Instance_BuilderAsync : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = PipelineBuilderExtensions.PipeAsync<int, double, string>((IPipelineBuilderAsync<int, double>) null, _stepAsyncDummy);
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
                    _ = PipelineBuilderExtensions.PipeAsync<int, double, string>(_builderAsyncDummy, (IPipelineStepAsync<double, string>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = PipelineBuilderExtensions.PipeAsync<int, double, string>(_builderAsyncDummy, _stepAsyncDummy);

                actual.Should().BeOfType<PipelineBuilderAsync<int, double, string>>();
            }
        }

        public class PipeAsync_StepAsync_Type_BuilderAsync : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = PipelineBuilderExtensions.PipeAsync<StepAsyncDummy, int, double, string>((IPipelineBuilderAsync<int, double>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("prevStep");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = PipelineBuilderExtensions.PipeAsync<StepAsyncDummy, int, double, string>(_builderAsyncDummy);

                actual.Should().BeOfType<PipelineBuilderAsync<int, double, string>>();
            }
        }

        #endregion

        #region Pipe - with input, synchronous, return IPipelineBuilderAsync<TIn, TNextOut>

        public class Pipe_Func_BuilderAsync : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = PipelineBuilderExtensions.Pipe<int, double, string>((IPipelineBuilderAsync<int, double>) null, _funcStep);
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
                    _ = PipelineBuilderExtensions.Pipe<int, double, string>(_builderAsyncDummy, (Func<double, string>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = PipelineBuilderExtensions.Pipe<int, double, string>(_builderAsyncDummy, _funcStep);

                actual.Should().BeOfType<PipelineBuilderAsync<int, double, string>>();
            }
        }

        public class Pipe_Step_Instance_BuilderAsync : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = PipelineBuilderExtensions.Pipe<int, double, string>((IPipelineBuilderAsync<int, double>) null, _stepDummy);
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
                    _ = PipelineBuilderExtensions.Pipe<int, double, string>(_builderAsyncDummy, (IPipelineStep<double, string>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = PipelineBuilderExtensions.Pipe<int, double, string>(_builderAsyncDummy, _stepDummy);

                actual.Should().BeOfType<PipelineBuilderAsync<int, double, string>>();
            }
        }

        public class Pipe_Step_Type_BuilderAsync : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = PipelineBuilderExtensions.Pipe<StepDummy, int, double, string>((IPipelineBuilderAsync<int, double>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("prevStep");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = PipelineBuilderExtensions.Pipe<StepDummy, int, double, string>(_builderAsyncDummy);

                actual.Should().BeOfType<PipelineBuilderAsync<int, double, string>>();
            }
        }

        #endregion

        #region Pipe - with no input, synchronous, return IPipelineBuilder<TIn, TNextOut>

        public class Pipe_NoInput_Func_Builder : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = PipelineBuilderExtensions.Pipe<double, string>((IPipelineBuilder<double>) null, _funcStep);
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
                    _ = PipelineBuilderExtensions.Pipe<double, string>(_builderNoInputDummy, (Func<double, string>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = PipelineBuilderExtensions.Pipe<double, string>(_builderNoInputDummy, _funcStep);

                actual.Should().BeOfType<PipelineNoInputBuilder<double, string>>();
            }
        }

        public class Pipe_NoInput_Step_Instance_Builder : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = PipelineBuilderExtensions.Pipe<double, string>((IPipelineBuilder<double>) null, _stepDummy);
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
                    _ = PipelineBuilderExtensions.Pipe<double, string>(_builderNoInputDummy, (IPipelineStep<double, string>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = PipelineBuilderExtensions.Pipe<double, string>(_builderNoInputDummy, _stepDummy);

                actual.Should().BeOfType<PipelineNoInputBuilder<double, string>>();
            }
        }

        public class Pipe_NoInput_Step_Type_Builder : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = PipelineBuilderExtensions.Pipe<StepDummy, double, string>((IPipelineBuilder<double>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("prevStep");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = PipelineBuilderExtensions.Pipe<StepDummy, double, string>(_builderNoInputDummy);

                actual.Should().BeOfType<PipelineNoInputBuilder<double, string>>();
            }
        }

        #endregion

        #region Pipe - with no input, asynchronous, return IPipelineBuilderAsync<TIn, TNextOut>

        public class PipeAsync_NoInput_FuncAsync_BuilderAsync : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = PipelineBuilderExtensions.PipeAsync<double, string>((IPipelineBuilderAsync<double>) null, _funcStepAsync);
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
                    _ = PipelineBuilderExtensions.PipeAsync<double, string>(_builderNoInputAsyncDummy, (Func<double, Task<string>>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = PipelineBuilderExtensions.PipeAsync<double, string>(_builderNoInputAsyncDummy, _funcStepAsync);

                actual.Should().BeOfType<PipelineNoInputBuilderAsync<double, string>>();
            }
        }

        public class PipeAsync_NoInput_StepAsync_Instance_BuilderAsync : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = PipelineBuilderExtensions.PipeAsync<double, string>((IPipelineBuilderAsync<double>) null, _stepAsyncDummy);
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
                    _ = PipelineBuilderExtensions.PipeAsync<double, string>(_builderNoInputAsyncDummy, (IPipelineStepAsync<double, string>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = PipelineBuilderExtensions.PipeAsync<double, string>(_builderNoInputAsyncDummy, _stepAsyncDummy);

                actual.Should().BeOfType<PipelineNoInputBuilderAsync<double, string>>();
            }
        }

        public class PipeAsync_NoInput_StepAsync_Type_BuilderAsync : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = PipelineBuilderExtensions.PipeAsync<StepAsyncDummy, double, string>((IPipelineBuilderAsync<double>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("prevStep");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = PipelineBuilderExtensions.PipeAsync<StepAsyncDummy, double, string>(_builderNoInputAsyncDummy);

                actual.Should().BeOfType<PipelineNoInputBuilderAsync<double, string>>();
            }
        }

        #endregion

        #region Pipe - with no input, synchronous, return IPipelineBuilderAsync<TIn, TNextOut>

        public class Pipe_NoInput_Func_BuilderAsync : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = PipelineBuilderExtensions.Pipe<double, string>((IPipelineBuilderAsync<double>) null, _funcStep);
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
                    _ = PipelineBuilderExtensions.Pipe<double, string>(_builderNoInputAsyncDummy, (Func<double, string>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = PipelineBuilderExtensions.Pipe<double, string>(_builderNoInputAsyncDummy, _funcStep);

                actual.Should().BeOfType<PipelineNoInputBuilderAsync<double, string>>();
            }
        }

        public class Pipe_NoInput_Step_Instance_BuilderAsync : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = PipelineBuilderExtensions.Pipe<double, string>((IPipelineBuilderAsync<double>) null, _stepDummy);
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
                    _ = PipelineBuilderExtensions.Pipe<double, string>(_builderNoInputAsyncDummy, (IPipelineStep<double, string>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = PipelineBuilderExtensions.Pipe<double, string>(_builderNoInputAsyncDummy, _stepDummy);

                actual.Should().BeOfType<PipelineNoInputBuilderAsync<double, string>>();
            }
        }

        public class Pipe_NoInput_Step_Type_BuilderAsync : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = PipelineBuilderExtensions.Pipe<StepDummy, double, string>((IPipelineBuilderAsync<double>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("prevStep");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = PipelineBuilderExtensions.Pipe<StepDummy, double, string>(_builderNoInputAsyncDummy);

                actual.Should().BeOfType<PipelineNoInputBuilderAsync<double, string>>();
            }
        }

        #endregion
    }
}