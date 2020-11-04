using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using VideoLibrary;

namespace Youtube_DL.Model
{
    internal class YoutubeVideoModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private const string ImageUrl = "https://img.youtube.com/vi/{VideoId}/hqdefault.jpg";

        public string urls { get; set; }

        public YoutubeVideoModel(IEnumerable<YouTubeVideo> VideoInfo)
        {

        }
    }
}