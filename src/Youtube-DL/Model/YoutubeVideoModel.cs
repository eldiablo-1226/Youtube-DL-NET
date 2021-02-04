using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Youtube_DL.Core;
using Youtube_DL.Helps;
using YoutubeExplode.Videos;

namespace Youtube_DL.Model
{
    [Serializable]
    public class YoutubeVideoModel : BaseViewModel
    {
        private readonly Video[] _videos;
        private CancellationTokenSource? _cancellationToken = new CancellationTokenSource();
        private YoutubeVideoService _youtubeService = new YoutubeVideoService();
        public Video? CurrerntVideo;

        private Progress<double> _progress;


        public bool Isloading { get; set; }
        public bool Finished { get; set; }

        public bool IsIndeterminate { get; set; }
        public bool IsPlaylist { get; }

        public string Image { get; set; }
        public string Title { get; set; }
        public string PlaylistCount { get; set; } = "";
        public string? PlaylistTitle { get; set; }

        public string? SavedPath { get; set; }

        public double DownloadPercentage { get; set; }

        public Command Download { get; set; }
        public Command CancelDownload { get; set; }

        public Command OpenFolder { get; set; }
        public Command OpenFile { get; set; }
        public Command? DeleteVideo { get; set; }

        public IReadOnlyList<VideoDownloadOption> VideoOptions { get; set; }
        public VideoDownloadOption? CurrerntVideoOption { get; set; }

        public YoutubeVideoModel(Video[] video, IReadOnlyList<VideoDownloadOption> options, string? playlistTitle = null)
        {
            OpenFile = new Command(OpenFilePath);
            OpenFolder = new Command(OpenFolderPath);
            Download = new Command(StartDownload);
            CancelDownload = new Command(StopDownload);

            _progress = new Progress<double>(d => DownloadPercentage = d * 100);
            IsPlaylist = video.Length > 1;

            _videos = video;
            CurrerntVideo = _videos.FirstOrDefault();

            Image = CurrerntVideo?.Thumbnails.MaxResUrl ?? "";
            Title = CurrerntVideo?.Title ?? "Unknow";
            PlaylistTitle = playlistTitle;

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
                    var savePath = YoutubeVideoService.PromptDirectoryPath();
                    if (savePath == null) return;
                    string fullpath = Path.Combine(savePath, YoutubeVideoService.FixFileName(PlaylistTitle!));
                    Directory.CreateDirectory(fullpath);
                    SavedPath = fullpath;
                    Isloading = true;
                    for (var i = 0; i < _videos.Length; i++)
                        try
                        {
                            PlaylistCount = $"{i + 1}/{_videos.Length}";
                            DownloadPercentage = 0;
                            IsIndeterminate = true;

                            var options = await _youtubeService.TryGetBestVideoDownloadOptionAsync(_videos[i].Id,
                                "mp4",
                                CurrerntVideoOption?.QualityPreference ?? VideoQualityPreference.Maximum);

                            IsIndeterminate = false;

                            await DownloadAsync(_videos[i], options ?? throw new NullReferenceException(),
                                Path.Combine(fullpath, YoutubeVideoService.FixFileName(_videos[i].Title) + ".mp4"));
                            if (i + 1 < _videos.Length)
                            {
                                CurrerntVideo = _videos[i + 1];
                                Image = CurrerntVideo.Thumbnails.MaxResUrl;
                                Title = CurrerntVideo.Title;
                            }
                        }
                        catch (Exception e)
                        {
                            if (e.Message != "A task was canceled." && e.Message != "The operation was canceled.")
                                Console.WriteLine(e.Message);
                        }

                    Finished = true;
                }
                else
                {
                    var savePath =
                        YoutubeVideoService.PromptSaveFilePath(CurrerntVideo?.Title ?? "Unknow", CurrerntVideoOption?.Format ?? "Unknow");
                    SavedPath = savePath;
                    if (savePath == null) return;

                    Isloading = true;
                    await DownloadAsync(CurrerntVideo ?? throw new NullReferenceException(), CurrerntVideoOption ?? throw new NullReferenceException(), savePath);
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

        public Task DownloadAsync(Video video, VideoDownloadOption option, string saveFilePath)
        {
            return _youtubeService.DownloadAsync(option, video, _progress, _cancellationToken?.Token ?? throw new NullReferenceException(), saveFilePath);
        }

        public void StopDownload()
        {
            if (Isloading) _cancellationToken?.Cancel();
        }

        private void OpenFilePath()
        {
            if (!IsPlaylist)
                try
                {
                    new Process {StartInfo = new ProcessStartInfo(SavedPath) {UseShellExecute = true}}.Start();
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