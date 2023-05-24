using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace AllOverIt.Wpf.Controls.ButtonMenu
{
    /// <summary>Displays a list of menu items by utilising the built-in ContextMenu.</summary>
    public partial class ButtonMenu : Button
    {
        public static readonly DependencyProperty MenuItemsProperty =
            DependencyProperty.Register(nameof(MenuItems), typeof(ObservableCollection<MenuItem>), typeof(ButtonMenu), new PropertyMetadata(null));

        public ObservableCollection<MenuItem> MenuItems
        {
            get { return (ObservableCollection<MenuItem>) GetValue(MenuItemsProperty); }
            set { SetValue(MenuItemsProperty, value); }
        }

        public ButtonMenu()
        {
            InitializeComponent();

            MenuItems = new ObservableCollection<MenuItem>();
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if (ContextMenu is not null)
            {
                ContextMenu.IsOpen = true;
            }
        }

        private void OnContextMenuOpened(object sender, RoutedEventArgs e)
        {
            if (sender is ContextMenu contextMenu)
            {
                contextMenu.PlacementTarget = this;

                // Schedule the positioning code to run after the current layout pass is complete
                // otherwise the positioning of the menu will not be correct the first time it is
                // displayed (because the ActualWidth and ActualHeight have not been calculated)
                Dispatcher.BeginInvoke(() =>
                {
                    contextMenu.HorizontalOffset = contextMenu.ActualWidth + 16;
                    contextMenu.VerticalOffset = -contextMenu.ActualHeight;
                }, DispatcherPriority.Render);
            }
        }
    }
}
