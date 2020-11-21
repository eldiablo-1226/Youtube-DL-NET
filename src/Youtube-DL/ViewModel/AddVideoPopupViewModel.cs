using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MaterialDesignThemes.Wpf;
using Youtube_DL.Core;
using Youtube_DL.Model;
using YoutubeExplode;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;

namespace Youtube_DL.ViewModel
{
    internal class AddVideoPopupViewModel : BaseViewModel
    {
        private YoutubeVideoService _youtubeservise;
        public Command AddVideoCommand { get; }
        public bool IsLoading { get; private set; }

        public AddVideoPopupViewModel()
        {
            _youtubeservise = new YoutubeVideoService();
            AddVideoCommand = new Command(AddVideoToList);
        }

        private async void AddVideoToList(object s)
        {
            if (IsLoading || string.IsNullOrWhiteSpace(s as string)) return;
            string url = (string)s;

            try
            {
                IsLoading = true;
                var videos = await _youtubeservise.GetVideosAsync(url);
                IReadOnlyList<VideoDownloadOption> videoOptions;
                if (videos.Length > 1)
                {
                    videoOptions = _youtubeservise.GetDownloadOptionPlaylist();
                }
                else
                {
                    videoOptions = await _youtubeservise.GetVideoDownloadOptionsAsync(url);
                }
                IsLoading = false;
                DialogHost.Close("MainDialog", new YoutubeVideoModel(videos.ToArray(), videoOptions));
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Ссылка не правильно");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            IsLoading = false;
        }


    }
}

