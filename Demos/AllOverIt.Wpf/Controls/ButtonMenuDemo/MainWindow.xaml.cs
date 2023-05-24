using System.Windows;

namespace ButtonMenuDemo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnMenu1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Menu Item 1");
        }

        private void OnMenu2(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Menu Item 2");
        }

        private void OnMenu3(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Menu Item 3");
        }
    }
}
