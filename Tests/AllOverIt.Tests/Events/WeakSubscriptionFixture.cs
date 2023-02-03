using AllOverIt.Events;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Tests.Events
{
    public class WeakSubscriptionFixture : FixtureBase
    {
        public class Constructor : WeakSubscriptionFixture
        {
            [Fact]
            public void Should_Throw_When_Delegate_Null()
            {
                Invoking(() => new WeakSubscription(null))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("handler");
            }
        }

        public class GetHandler : WeakSubscriptionFixture
        {
            private class DummyHandler
            {
                public static int ActualValue { get; set; }

                public static void StaticHandler(int value)
                {
                    ActualValue = value;
                }
            }

            [Fact]
            public void Should_Get_Handler()
            {
                var expected = Create<int>();
                var actual = -expected;

                Action<int> handler = value =>
                {
                    actual = value;
                };

                var subscription = new WeakSubscription(handler);

                // unlike AsyncSubscription, AsyncWeakSubscription creates a delegate so we can't compare references
                var registeredHandler = subscription.GetHandler<int>();

                registeredHandler.Invoke(expected);

                actual.Should().Be(expected);
            }

            // Weak reference related tests are not working
            //[Fact]
            //public void Should_Not_Handle_Disposed_Handler()
            //{
            //    var expected = Create<int>();
            //    var actual = -expected;

            //    Action<int> handler = value =>
            //    {
            //        actual = value;
            //    };

            //    var subscription = new WeakSubscription(handler);

            //    handler = null;
            //    GC.Collect();
            //    GC.Collect();

            //    var registeredHandler = subscription.GetHandler<int>();

            //    registeredHandler.Invoke(expected);

            //    actual.Should().Be(-expected);
            //}

            [Fact]
            public void Should_Get_Static_Handler()
            {
                var expected = Create<int>();

                Action<int> handler = DummyHandler.StaticHandler;

                var subscription = new WeakSubscription(handler);

                // unlike AsyncSubscription, WeakSubscription creates a delegate so we can't compare references
                var registeredHandler = subscription.GetHandler<int>();

                DummyHandler.ActualValue = -expected;
                registeredHandler.Invoke(expected);

                DummyHandler.ActualValue.Should().Be(expected);
            }
        }

        public class Handle : WeakSubscriptionFixture
        {
            [Fact]
            public void Should_Invoke_Handler()
            {
                var expected = Create<int>();
                int actual = -expected;

                Action<int> handler = value =>
                {
                    actual = value;
                };

                var subscription = new WeakSubscription(handler);

                subscription.Handle(expected);

                actual.Should().Be(expected);
            }
        }
    }
}
