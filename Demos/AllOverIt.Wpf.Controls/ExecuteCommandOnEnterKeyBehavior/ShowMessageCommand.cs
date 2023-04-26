using System;
using System.Windows;
using System.Windows.Input;

namespace ExecuteCommandOnEnterKeyBehavior
{
    public class ShowMessageCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var dateTime = (DateTime) parameter;

            MessageBox.Show(dateTime.ToString("o"));
        }
    }
}
