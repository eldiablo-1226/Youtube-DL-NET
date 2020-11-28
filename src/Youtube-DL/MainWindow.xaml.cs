﻿using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Hardcodet.Wpf.TaskbarNotification;
using Color = System.Windows.Media.Color;

namespace Youtube_DL
{
    public partial class MainWindow : Window
    {
        private bool _firstShowBol = true; 
        public MainWindow()
        {
            InitializeComponent();
            Loaded += (o, s) => NotifyIcon.Icon = ToBitmapSource();
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
            {
                this.WindowState = WindowState.Minimized;
                this.ShowInTaskbar = false;
                NotifyIcon.Visibility = Visibility.Visible;
                if (_firstShowBol)
                {
                    NotifyIcon.ShowBalloonTip("Youtube Downloader", "Приложения свернута в трей", BalloonIcon.Info);
                    _firstShowBol = false;
                }
            }
        }

        private void OpenWindow(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
            this.ShowInTaskbar = true;
            NotifyIcon.Visibility = Visibility.Hidden;
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public Icon ToBitmapSource(DrawingImage? source = null)
        {
            source ??= (DrawingImage)FindResource("YoutubeLogo");
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            drawingContext.DrawImage(source, new Rect(new System.Windows.Point(0, 0), new System.Windows.Size(source.Width, source.Height)));
            drawingContext.Close();

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)source.Width, (int)source.Height, 96, 75, PixelFormats.Pbgra32);
            rtb.Render(drawingVisual);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            MemoryStream stream = new MemoryStream();
            encoder.Save(stream);
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(stream);
            return System.Drawing.Icon.FromHandle(bmp.GetHicon());
        }
    }
}