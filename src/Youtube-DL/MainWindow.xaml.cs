using System.Windows;
using System.Windows.Input;

namespace Youtube_DL
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MouseMoveWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void MouseCloseClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                Close();
        }

        private void MouseHideClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
                WindowState = WindowState.Minimized;
        }
    }
}