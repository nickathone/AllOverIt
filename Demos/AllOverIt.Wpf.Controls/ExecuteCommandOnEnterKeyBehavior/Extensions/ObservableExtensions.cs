using AllOverIt.Wpf.Threading;
using System;
using System.Reactive.Linq;

namespace ExecuteCommandOnEnterKeyBehavior.Extensions
{
    public static class ObservableExtensions
    {
        public static IObservable<TType> ObserveOnUIThread<TType>(this IObservable<TType> observable)
        {
            return observable.SelectMany(async value =>
            {
                await UIThread.BindToAsync();

                return value;
            });
        }
    }
}
