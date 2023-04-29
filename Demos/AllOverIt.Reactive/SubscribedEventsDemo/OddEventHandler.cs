using AllOverIt.Reactive;
using System;

namespace SubscribedEventsDemo
{
    internal sealed class OddEventHandler : EventBusHandler<OddEvent>
    {
        public OddEventHandler(IEventBus eventBus)
            : base(eventBus)
        {
        }

        public override void Handle(OddEvent @event)
        {
            Console.WriteLine($"Received an odd number event: {@event.Value}");
        }
    }
}
