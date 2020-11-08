using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NYoutubeDL.Models;

namespace Youtube_DL.Model
{
    internal class YoutubeVideoModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Image { get; set; }
        public string Title { get; set; }
        public string URL { get; set; }
        public List<FormatDownloadInfo> Formats { get; set; }

        public YoutubeVideoModel([NotNull]DownloadInfo Info)
        {
            var AllInfo = Info as VideoDownloadInfo;

            Image = AllInfo.Thumbnail;
            Title = AllInfo.Title;
            Formats = AllInfo.Formats;
            URL = AllInfo.WebpageUrl;
        }
    }
}