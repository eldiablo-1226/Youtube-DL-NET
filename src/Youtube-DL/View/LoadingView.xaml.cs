using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Youtube_DL.Core;
using Youtube_DL.Model;
using YoutubeExplode.Videos;

namespace Youtube_DL.View
{
    /// <summary>
    /// Логика взаимодействия для LoadingView.xaml
    /// </summary>
    public partial class LoadingView : UserControl
    {
        public YoutubeVideoService _VideoService = new YoutubeVideoService();
        private string _url;

        public LoadingView()
        {
            InitializeComponent();
        }

        public LoadingView(string url)
        {
            InitializeComponent();
            _url = url;
            Loaded += LoadingView_Loaded;
        }

        private async void LoadingView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var videos = await _VideoService.GetVideosAsync(_url);
                IReadOnlyList<VideoDownloadOption>? videoOptions = null;
                if (videos.Length == 1)
                {
                    videoOptions = await _VideoService.GetVideoDownloadOptionsAsync(_url);
                }
                else
                {
                    videoOptions = YoutubeVideoService.GetOptionPlaylist().Reverse().ToArray();
                }

                DialogHost.Close("MainDialog", new YoutubeVideoModel(videos, videoOptions));
            }
            catch (Exception)
            {
                DialogHost.Close("MainDialog");
            }
        }
    }
}