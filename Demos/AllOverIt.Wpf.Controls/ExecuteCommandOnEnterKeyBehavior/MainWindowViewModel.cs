using AllOverIt.Reactive;
using AllOverIt.Wpf.Threading;
using System;
using System.Reactive.Linq;
using System.Windows.Input;

namespace ExecuteCommandOnEnterKeyBehavior
{
    public sealed class MainWindowViewModel : ObservableObject
    {
        private DateTime _currentDateTime;
        public DateTime CurrentDateTime
        {
            get => DateTime.Now;
            set => RaiseAndSetIfChanged(ref _currentDateTime, value);
        }

        public ICommand ShowMessageCommand { get; } = new ShowMessageCommand();

        public MainWindowViewModel()
        {
            Observable
                .Interval(TimeSpan.FromSeconds(1))
                .Subscribe(_ =>
                {
                    UIThread.Invoke(() =>
                    {
                        CurrentDateTime = DateTime.Now;
                    });
                });
        }
    }
}
