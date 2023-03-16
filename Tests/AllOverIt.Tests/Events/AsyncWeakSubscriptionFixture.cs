using AllOverIt.Events;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Tests.Events
{
    public class AsyncWeakSubscriptionFixture : FixtureBase
    {
        public class Constructor : AsyncWeakSubscriptionFixture
        {
            [Fact]
            public void Should_Throw_When_Delegate_Null()
            {
                Invoking(() => new AsyncWeakSubscription(null))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("handler");
            }
        }

        public class GetHandler : AsyncWeakSubscriptionFixture
        {
            private sealed class HandlerDummy
            {
                public static int ActualValue { get; set; }

                public Task Handler(int value)
                {
                    var cts = new TaskCompletionSource<int>();
                    cts.SetException(new Exception($"{value}"));

                    return cts.Task;
                }

                public static Task StaticHandler(int value)
                {
                    ActualValue = value;
                    return Task.CompletedTask;
                }

                public static HandlerDummy Create()
                {
                    return new HandlerDummy();
                }
            }

            [Fact]
            public async Task Should_Get_Handler()
            {
                var expected = Create<int>();
                var actual = -expected;

                Func<int, Task> handler = value =>
                {
                    actual = value;
                    return Task.CompletedTask;
                };

                var subscription = new AsyncWeakSubscription(handler);

                // unlike AsyncSubscription, AsyncWeakSubscription creates a delegate so we can't compare references
                var registeredHandler = subscription.GetHandler<int>();

                await registeredHandler.Invoke(expected);

                actual.Should().Be(expected);
            }

            [Fact]
            public async Task Should_Not_Handle_Disposed_Handler()
            {
                var handler = HandlerDummy.Create();

                var subscription = new AsyncWeakSubscription(handler.Handler);

                handler = null;

                await Task.Delay(100);

                GC.Collect();

                var registeredHandler = subscription.GetHandler<int>();

                // Would throw a faulted task if the handler was invoked
                var actual = registeredHandler.Invoke(0);

                actual.Should().Be(Task.CompletedTask);
            }

            [Fact]
            public async Task Should_Get_Static_Handler()
            {
                var expected = Create<int>();

                Func<int, Task> handler = HandlerDummy.StaticHandler;

                var subscription = new AsyncWeakSubscription(handler);

                // unlike AsyncSubscription, AsyncWeakSubscription creates a delegate so we can't compare references
                var registeredHandler = subscription.GetHandler<int>();

                HandlerDummy.ActualValue = -expected;
                await registeredHandler.Invoke(expected);

                HandlerDummy.ActualValue.Should().Be(expected);
            }
        }

        public class HandleAsync : AsyncWeakSubscriptionFixture
        {
            [Fact]
            public async Task Should_Invoke_Handler()
            {
                var expected = Create<int>();
                int actual = -expected;

                Func<int, Task> handler = value =>
                {
                    actual = value;
                    return Task.CompletedTask;
                };

                var subscription = new AsyncWeakSubscription(handler);

                await subscription.HandleAsync(expected);

                actual.Should().Be(expected);
            }
        }
    }
}
