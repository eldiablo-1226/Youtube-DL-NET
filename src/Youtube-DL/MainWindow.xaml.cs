using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Youtube_DL.Core;

namespace Youtube_DL
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SettingLocator.settings.Load();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            
            SettingLocator.settings.Save();
        }

        private void MouseMoveWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void MouseCloseClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.Close();
        }

        private void MouseHideClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
                this.WindowState = WindowState.Minimized;
        }
    }
}