using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MaterialDesignThemes.Wpf;
using Youtube_DL.Core;
using Youtube_DL.Model;

namespace Youtube_DL.ViewModel
{
    internal class AddVideoPopupViewModel : BaseViewModel
    {
        private readonly YoutubeVideoService _youtubeservise;

        public AddVideoPopupViewModel()
        {
            _youtubeservise = new YoutubeVideoService();
            AddVideoCommand = new Command(AddVideoToList);
        }

        public Command AddVideoCommand { get; }
        public bool IsLoading { get; private set; }

        private async void AddVideoToList(object s)
        {
            if (IsLoading || string.IsNullOrWhiteSpace(s as string)) return;
            string url = (string) s;

            try
            {
                IsLoading = true;
                var videos = await _youtubeservise.GetVideosAsync(url);
                IReadOnlyList<VideoDownloadOption>? videoOptions;
                string? title = default;

                if (videos.Length == 1)
                    videoOptions = await _youtubeservise.GetVideoDownloadOptionsAsync(url);
                else
                {
                    videoOptions = YoutubeVideoService.GetOptionPlaylist().Reverse().ToArray();
                    title = await _youtubeservise.GetPlaylistTitle(url);
                }
                IsLoading = false;
                DialogHost.Close("MainDialog", new YoutubeVideoModel(videos, videoOptions, title));
            }
            catch (ArgumentException e)
            {
                MessageBox.Show(e.Message);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            IsLoading = false;
        }
    }
}