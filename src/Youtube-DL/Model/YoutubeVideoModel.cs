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
using ByteConverter = Youtube_DL.Core.ByteConverter;

namespace Youtube_DL.Model
{
    internal class YoutubeVideoModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private YoutubeDL Downloader;

        public Visibility IsNotVisible { get; set; } = Visibility.Visible;
        public Visibility IsVisible { get; set; } = Visibility.Hidden;
        private bool _isloading = false;
        public bool Isloading
        {
            set
            {
                if (value)
                {
                    IsNotVisible = Visibility.Hidden;
                    IsVisible = Visibility.Visible;
                    _isloading = true;
                }
                else
                {
                    IsNotVisible = Visibility.Visible;
                    IsVisible = Visibility.Hidden;
                    _isloading = false;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Isloading)));
            }
            get => _isloading;
        }

        public string Image { get; set; }
        public string Title { get; set; }
        public string URL { get; set; }
        public double DownloadSize { get; set; } = 0;

        public List<string> Formats { get; set; }

        public string SelectedIteam { get; set; }

        public Command Download { get; set; }
        public Command CancelDownload { get; set; }

        public PackIcon IconClose { get; set; } = new PackIcon(){ Kind = PackIconKind.CloseOutline };

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
                .OrderByDescending(x=> x.FormatId)
                .Select(x =>
                    {
                        string size = x.Filesize == null ? "~ byte" : ByteConverter.SizeSuffix((long)x.Filesize);
                        return $"{x.FormatNote} - {size} - {x.Ext}";
                    })
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