using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MaterialDesignThemes.Wpf;
using Youtube_DL.Core;
using Youtube_DL.Helps;
using ByteConverter = Youtube_DL.Helps.ByteConverter;

namespace Youtube_DL.Model
{
    [Serializable]
    public class YoutubeVideoModel : BaseViewModel
    {

        public bool Isloading { get; set; }
        public bool Finished { get; set; }

        public string Image { get; set; }
        public string Title { get; set; }
        public string URL { get; set; }

        public double DownloadPercentage { get; set; }
        public string Speed { get; set; }

        public List<string> Formats { get; set; }
        public string SelectedIteam { get; set; }

        public Command Download { get; set; }
        public Command CancelDownload { get; set; }


        public YoutubeVideoModel(string)
        {
            
        }

        public async void StartDownload()
        {
            
        }

        public void StopDownload()
        {
            
        }
    }
}