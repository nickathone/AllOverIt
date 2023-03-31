using AllOverIt.Fixture;
using FluentAssertions;
using System;
using System.Reactive.Linq;
using Xunit;

namespace AllOverIt.Reactive.Tests
{
    public class EventBusFixture : FixtureBase
    {
        private readonly IEventBus _eventBus = new EventBus();

        private class EventDummy
        {
        }

        public class Publish_GetEvent : EventBusFixture
        {
            [Fact]
            public void Should_Publish_Receive_Event()
            {
                var received = false;

                _eventBus.GetEvent<EventDummy>().Subscribe(_ => { received = true; });

                _eventBus.Publish<EventDummy>();

                received.Should().BeTrue();
            }
        }

        public class Publish_Arg_GetEvent : EventBusFixture
        {
            [Fact]
            public void Should_Publish_Receive_Event()
            {
                EventDummy expected = new EventDummy();
                EventDummy actual = null;

                _eventBus.GetEvent<EventDummy>().Subscribe(@event => { actual = @event; });

                _eventBus.Publish(expected);

                actual.Should().BeSameAs(expected);
            }
        }
    }
}