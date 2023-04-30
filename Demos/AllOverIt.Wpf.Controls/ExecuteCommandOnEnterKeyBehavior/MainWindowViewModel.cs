using AllOverIt.Assertion;
using AllOverIt.Reactive;
using AllOverIt.Wpf.Threading;
using System;
using System.Reactive.Linq;
using System.Windows.Input;

namespace ExecuteCommandOnEnterKeyBehavior
{
    public sealed class MainWindowViewModel : ObservableObject
    {
        private IDisposable _intervalSubscription;

        private DateTime _currentDateTime;
        public DateTime CurrentDateTime
        {
            get => DateTime.Now;
            set => RaiseAndSetIfChanged(ref _currentDateTime, value);
        }

        public ICommand ShowMessageCommand { get; } = new ShowMessageCommand();

        public MainWindowViewModel()
        {
            var uiThread = Environment.CurrentManagedThreadId;

            _intervalSubscription = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .Subscribe(_ =>
                {
                    UIThread.Invoke(() =>
                    {
                        Throw<InvalidOperationException>.When(uiThread != Environment.CurrentManagedThreadId, "Expected to be running on the UI thread.");

                        CurrentDateTime = DateTime.Now;
                    });
                });
        }

        // If this demo was written using ReactiveUI it would have used an ActivatableViewModel
        // so the subscription could automatically be cleaned up when the view is deactivated.
        // Doing it this way out of convenience - wouldn't do this in production.
        public void OnClosing()
        {
            // Need to make sure the subscription is disposed to prevent the ObserveOnUIThread()
            // method attempting to access the application's dispatcher when it is no longer available.
            _intervalSubscription?.Dispose();
            _intervalSubscription = null;
        }
    }
}
