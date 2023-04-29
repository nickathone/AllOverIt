namespace AllOverIt.Reactive
{
    public interface IEventBusHandler<TEvent>
    {
        void Handle(TEvent @event);
    }
}
