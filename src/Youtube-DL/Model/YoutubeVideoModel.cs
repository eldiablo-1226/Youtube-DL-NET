using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using MaterialDesignThemes.Wpf;
using Youtube_DL.Core;
using Youtube_DL.Helps;
using YoutubeExplode;
using YoutubeExplode.Videos;
using ByteConverter = Youtube_DL.Helps.ByteConverter;

namespace Youtube_DL.Model
{
    [Serializable]
    public class YoutubeVideoModel : BaseViewModel
    {
        private YoutubeVideoService _youtubeService = new YoutubeVideoService();
        private CancellationTokenSource _cancellationToken = new CancellationTokenSource();
        public bool Isloading { get; set; }
        public bool Finished { get; set; }

        public bool IsPlaylist { get; }

        public string Image { get; set; }
        public  string Title { get; set; }

        public double DownloadPercentage { get; set; }

        private Progress<double> progress;

        public Command Download { get; set; }
        public Command CancelDownload { get; set; }

        private readonly Video[] Videos;
        public Video CurrerntVideo;

        public IReadOnlyList<VideoDownloadOption> VideoOptions { get; set; }
        public VideoDownloadOption CurrerntVideoOption { get; set; }

        public YoutubeVideoModel(Video[] video, IReadOnlyList<VideoDownloadOption> options)
        {
            Download = new Command(StartDownload);
            CancelDownload = new Command(StopDownload);

            progress = new Progress<double>((d => DownloadPercentage = d));
            IsPlaylist = video.Length > 1;

            Videos = video;
            CurrerntVideo = Videos.FirstOrDefault();

            Image = CurrerntVideo.Thumbnails.MaxResUrl;
            Title = CurrerntVideo.Title;

            VideoOptions = options;
            CurrerntVideoOption = VideoOptions.FirstOrDefault();
        }

        public async void StartDownload()
        {
            if (Isloading || CurrerntVideo == null) return;
            if (IsPlaylist)
            {
                try
                {
                    Isloading = true;
                    for (int i = 0; i < Videos.Length; i++)
                    {
                        await DownloadAsync(Videos[i], CurrerntVideoOption);
                        CurrerntVideo = Videos[i + 1];
                        Image = CurrerntVideo.Thumbnails.MaxResUrl;
                        Title = CurrerntVideo.Title;
                    }
                    Finished = true;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                finally
                {
                    Isloading = false;
                }
            }
            else
            {
                try
                {
                    Isloading = true;
                    await DownloadAsync(CurrerntVideo, CurrerntVideoOption);
                    Finished = true;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                finally
                {
                    Isloading = false;
                }
            }
        }

        public Task DownloadAsync(Video video, VideoDownloadOption option) => _youtubeService.DownloadAsync(option, video, progress, _cancellationToken.Token);

        public void StopDownload()
        {
            if (Isloading)
            {
                _cancellationToken.Cancel();
            }
        }
    }
}