using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Youtube_DL.Core;
using Youtube_DL.Model;
using YoutubeExplode.Videos;

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
                this.Close();
        }

        private void MouseHideClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
                this.WindowState = WindowState.Minimized;
        }
    }
}