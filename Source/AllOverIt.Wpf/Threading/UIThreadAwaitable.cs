namespace AllOverIt.Wpf.Threading
{
    public struct UIThreadAwaitable
    {
        public UIThreadAwaiter GetAwaiter()
        {
            return new UIThreadAwaiter();
        }
    }
}