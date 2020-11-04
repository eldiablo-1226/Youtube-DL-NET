using System.ComponentModel;

namespace Youtube_DL.Model
{
    internal class YoutubeVideoModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public YoutubeVideoModel()
        {
        }
    }
}