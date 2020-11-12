using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Youtube_DL.Core;
using Youtube_DL.Model;
using Youtube_DL.View;

namespace Youtube_DL.ViewModel
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Property

        public bool Isloading { get; set; }

        public ObservableCollection<YoutubeVideoModel> MainVideoList { get; set; }

        #endregion Property

        #region Command

        public Command AddButton { get; set; }

        #endregion Command

        public MainViewModel()
        {
            MainVideoList = new ObservableCollection<YoutubeVideoModel>();
            AddButton = new Command(AddViewShow);
        }

        private async void AddViewShow()
        {
            var VideoInfo = await DialogHost.Show(new AddVideoPopup());

            if(VideoInfo is null) return;

            MessageBox.Show(VideoInfo.GetType().FullName);
        }
    }
}