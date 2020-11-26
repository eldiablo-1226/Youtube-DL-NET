using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Youtube_DL.Core;
using YoutubeExplode.Videos;

namespace Youtube_DL.Model
{
    [Serializable]
    public class YoutubeVideoModel : BaseViewModel
    {
        private YoutubeVideoService _youtubeService = new YoutubeVideoService();
        private CancellationTokenSource? _cancellationToken = new CancellationTokenSource();
        public bool Isloading { get; set; }
        public bool Finished { get; set; }

        public bool IsIndeterminate { get; set; }
        public bool IsPlaylist { get; }

        public string Image { get; set; }
        public string Title { get; set; }
        public string PlaylistCount { get; set; } = "";

        public string? SavedPath { get; set; }

        public double DownloadPercentage { get; set; }

        private Progress<double> progress;

        public Command Download { get; set; }
        public Command CancelDownload { get; set; }

        public Command OpenFolder { get; set; }
        public Command OpenFile { get; set; }
        public Command DeleteVideo { get; set; }

        private readonly Video[] Videos;
        public Video CurrerntVideo;

        public IReadOnlyList<VideoDownloadOption> VideoOptions { get; set; }
        public VideoDownloadOption CurrerntVideoOption { get; set; }

        public YoutubeVideoModel(Video[] video, IReadOnlyList<VideoDownloadOption> options)
        {
            OpenFile = new Command(OpenFilePath);
            OpenFolder = new Command(OpenFolderPath);
            Download = new Command(StartDownload);
            CancelDownload = new Command(StopDownload);

            progress = new Progress<double>((d => DownloadPercentage = d * 100));
            IsPlaylist = video.Length > 1;

            Videos = video;
            CurrerntVideo = Videos.FirstOrDefault();

            Image = CurrerntVideo?.Thumbnails.MaxResUrl ?? "";
            Title = CurrerntVideo?.Title ?? "Unknow";

            VideoOptions = options;
            CurrerntVideoOption = VideoOptions.FirstOrDefault();
        }

        public async void StartDownload()
        {
            if (Isloading) return;
            _cancellationToken ??= new CancellationTokenSource();
            try
            {
                if (IsPlaylist)
                {
                    string? savePath = YoutubeVideoService.PromptDirectoryPath();
                    if (savePath == null) return;
                    SavedPath = savePath;

                    Isloading = true;
                    for (int i = 0; i < Videos.Length; i++)
                    {
                        try
                        {
                            PlaylistCount = $"{i + 1}/{Videos.Length}";
                            DownloadPercentage = 0;
                            IsIndeterminate = true;

                            var options = await _youtubeService.TryGetBestVideoDownloadOptionAsync(Videos[i].Id,
                                "mp4",
                                CurrerntVideoOption.QualityPreference);

                            IsIndeterminate = false;

                            await DownloadAsync(Videos[i], options,
                                Path.Combine(savePath, YoutubeVideoService.FixFileName(Videos[i].Title) + ".mp4"));
                            if (i + 1 < Videos.Length)
                            {
                                CurrerntVideo = Videos[i + 1];
                                Image = CurrerntVideo.Thumbnails.MaxResUrl;
                                Title = CurrerntVideo.Title;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }

                    Finished = true;
                }
                else
                {
                    var savePath =
                        YoutubeVideoService.PromptSaveFilePath(CurrerntVideo.Title, CurrerntVideoOption.Format);
                    SavedPath = savePath;
                    if (savePath == null) return;

                    Isloading = true;
                    await DownloadAsync(CurrerntVideo, CurrerntVideoOption, savePath);
                    Finished = true;
                }
            }
            catch (Exception e)
            {
                if (e.Message != "A task was canceled." && e.Message != "The operation was canceled.")
                    MessageBox.Show(e.Message);
            }
            finally
            {
                Isloading = false;
                _cancellationToken?.Dispose();
                _cancellationToken = null;
            }
            
        }

        public Task DownloadAsync(Video video, VideoDownloadOption option, string SaveFilePath) =>
            _youtubeService.DownloadAsync(option, video, progress, _cancellationToken.Token, SaveFilePath);

        public void StopDownload()
        {
            if (Isloading)
            {
                _cancellationToken?.Cancel();
            }
        }

        private void OpenFilePath()
        {
            if(!IsPlaylist)
                try
                {
                    new Process { StartInfo = new ProcessStartInfo(SavedPath) { UseShellExecute = true } }.Start();
                }
                catch (Exception)
                {
                    // ignored
                }
        }

        private void OpenFolderPath()
        {
            try
            {
                Process.Start("explorer.exe", IsPlaylist ? SavedPath : Path.GetDirectoryName(SavedPath));
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}