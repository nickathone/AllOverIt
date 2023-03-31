using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Tests.Extensions
{
    public class TaskExtensions : FixtureBase
    {
        public class FireAndForget : TaskExtensions
        {
            [Fact]
            public void Should_Throw_When_Handler_Null()
            {
                Invoking(() =>
                {
                    AllOverIt.Extensions.TaskExtensions.FireAndForget(Task.CompletedTask, null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("exceptionHandler");
            }


            [Fact]
            public void Should_Not_Call_Handler_When_Task_Completed()
            {
                var handlerCalled = false;

                AllOverIt.Extensions.TaskExtensions.FireAndForget(Task.CompletedTask, _ => handlerCalled = true);

                handlerCalled.Should().BeFalse();
            }
        }

        public class DoFireAndForget : TaskExtensions
        {
            [Fact]
            public async Task Should_Await_Non_Faulted_Exception()
            {
                Exception actual = null;

                await AllOverIt.Extensions.TaskExtensions.DoFireAndForget(Task.CompletedTask, dispatchInfo => actual = dispatchInfo.SourceException);

                actual.Should().BeNull();
            }

            [Fact]
            public async Task Should_Handle_Faulted_Exception()
            {
                var exception = new InvalidOperationException();

                var completionSource = new TaskCompletionSource<bool>();
                completionSource.SetException(exception);

                Exception actual = null;

                await AllOverIt.Extensions.TaskExtensions.DoFireAndForget(completionSource.Task, dispatchInfo => actual = dispatchInfo.SourceException);

                actual.Should().BeSameAs(exception);
            }
        }
    }
}
