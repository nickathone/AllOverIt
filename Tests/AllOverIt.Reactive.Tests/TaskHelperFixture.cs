using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Reactive.Tests
{
    public class TaskHelperFixture : FixtureBase
    {
        public class ExecuteAsyncAndWait : TaskHelperFixture
        {
            [Fact]
            public void Should_Throw_When_Action_Null()
            {
                Invoking(() =>
                {
                    TaskHelper.ExecuteAsyncAndWait(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("action");
            }

            [Fact]
            public void Should_Invoke_Action()
            {
                var executed = false;

                Task RunAction()
                {
                    executed = true;

                    return Task.CompletedTask;
                }

                TaskHelper.ExecuteAsyncAndWait(RunAction);

                executed.Should().BeTrue();
            }

            [Fact]
            public void Should_Throw_Action_Exception()
            {
                var exception = new Exception(Create<string>());

                Task RunAction()
                {
                    throw exception;
                }

                Invoking(() =>
                {
                    TaskHelper.ExecuteAsyncAndWait(RunAction);
                })
                .Should()
                .Throw<Exception>()
                .WithMessage(exception.Message);
            }
        }

        public class ExecuteAsyncAndWait_Result : TaskHelperFixture
        {
            [Fact]
            public void Should_Throw_When_Action_Null()
            {
                Invoking(() =>
                {
                    _ = TaskHelper.ExecuteAsyncAndWait<string>(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("action");
            }

            [Fact]
            public void Should_Invoke_Action()
            {
                var expected = Create<bool>();

                Task<bool> RunAction()
                {
                    return Task.FromResult(expected);
                }

                var actual = TaskHelper.ExecuteAsyncAndWait<bool>(RunAction);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Throw_Action_Exception()
            {
                var exception = new Exception(Create<string>());

                Task<bool> RunAction()
                {
                    throw exception;
                }

                Invoking(() =>
                {
                    _ = TaskHelper.ExecuteAsyncAndWait(RunAction);
                })
                .Should()
                .Throw<Exception>()
                .WithMessage(exception.Message);
            }
        }
    }
}