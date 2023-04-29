using AllOverIt.Assertion;
using System;
using System.Reactive.Disposables;

namespace AllOverIt.Reactive
{
    public abstract class EventBusHandler<TEvent> : ObservableObject, IEventBusHandler<TEvent>, IDisposable
    {
        private readonly IEventBus _eventBus;
        private IDisposable _subscription;

        private bool _isActive;
        private bool _disposedValue;

        public bool IsActive
        {
            get => _isActive;
            set => RaiseAndSetIfChanged(ref _isActive, value, null,
                () =>
                {
                    if (_isActive)
                    {
                        _subscription = _eventBus.GetEvent<TEvent>().Subscribe(Handle);
                    }
                    else
                    {
                        DisposeSubscription();
                    }
                });
        }

        public EventBusHandler(IEventBus eventBus)
        {
            _eventBus = eventBus.WhenNotNull(nameof(eventBus));
        }

        public abstract void Handle(TEvent @event);

        public void DisposeUsing(CompositeDisposable disposables)
        {
            disposables.Add(this);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_subscription is not null)
            {
                if (disposing)
                {
                    DisposeSubscription();
                }
            }
        }

        private void DisposeSubscription()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}
