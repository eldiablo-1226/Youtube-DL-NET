using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MaterialDesignThemes.Wpf;
using Youtube_DL.Core;
using Youtube_DL.Model;

namespace Youtube_DL.View
{
    public partial class LoadingView
    {
        private readonly string? _url;
        public YoutubeVideoService VideoService = new YoutubeVideoService();

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
                if (_url == null) throw new NullReferenceException(nameof(_url));
                var videos = await VideoService.GetVideosAsync(_url);
                IReadOnlyList<VideoDownloadOption>? videoOptions;
                if (videos.Length == 1)
                    videoOptions = await VideoService.GetVideoDownloadOptionsAsync(_url);
                else
                    videoOptions = YoutubeVideoService.GetOptionPlaylist().Reverse().ToArray();

                DialogHost.Close("MainDialog", new YoutubeVideoModel(videos, videoOptions));
            }
            catch (Exception)
            {
                DialogHost.Close("MainDialog");
            }
        }
    }
}