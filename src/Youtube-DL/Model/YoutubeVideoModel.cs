using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MaterialDesignThemes.Wpf;
using NYoutubeDL;
using NYoutubeDL.Helpers;
using NYoutubeDL.Models;
using NYoutubeDL.Options;
using Youtube_DL.Core;
using Youtube_DL.Helps;
using ByteConverter = Youtube_DL.Helps.ByteConverter;

namespace Youtube_DL.Model
{
    internal class YoutubeVideoModel : BaseViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private YoutubeDL Downloader;

        public bool Isloading { get; set; }
        public bool Finished { get; set; }
        public bool ErrorStatus { get; set; }

        public string Image { get; set; }
        public string Title { get; set; }
        public string URL { get; set; }
        public double DownloadSize { get; set; } = 0;

        public List<string> Formats { get; set; }

        public string SelectedIteam { get; set; }

        public Command Download { get; set; }
        public Command CancelDownload { get; set; }


        public YoutubeVideoModel([NotNull]DownloadInfo Info, string url)
        {
            // Commands
            Download = new Command(StartDownload);
            CancelDownload = new Command(StopDownload);
            //Fild
            var AllInfo = Info as VideoDownloadInfo;

            Image = AllInfo.Thumbnail;
            Title = AllInfo.Title;
            URL = url;
            Formats = AllInfo.Formats
                .Where(x=>x.FormatNote != "tiny" || x.Height != null)
                .OrderByDescending(x=> x.Height)
                .Select(x => $"{x.FormatNote}-{x.Filesize.SizeSuffix()}-{x.Ext}")
                .ToList();
            SelectedIteam = Formats.FirstOrDefault();

            Downloader = new YoutubeDL(@"Resources\youtube-dl.exe");
            Downloader.InfoChangedEvent += Downloader_InfoChangedEvent;
        }

        private void Downloader_InfoChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            DownloadInfo info = (DownloadInfo)sender;
            var propertyValue = info.GetType().GetProperty(e.PropertyName).GetValue(info);

            switch (e.PropertyName)
            {
                case "VideoProgress":
                    Debug.WriteLine($" > Video Progress: {propertyValue}%");
                    break;
                case "Status":
                    Debug.WriteLine($" > Status: {propertyValue}");
                    break;
                case "DownloadRate":
                    Debug.WriteLine($" > Download Rate: {propertyValue}");
                    break;
                default:
                    break;
            }
        }

        public void StartDownload()
        {
            Downloader.DownloadAsync(URL);
        }

        public void StopDownload()
        {

        }
    }
}