namespace AllOverIt.Wpf.Threading
{
    public readonly struct UIThreadAwaitable
    {
        public UIThreadAwaiter GetAwaiter()
        {
            return new UIThreadAwaiter();
        }
    }
}