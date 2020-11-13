using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using Youtube_DL.Core;
using Youtube_DL.Model;
using Youtube_DL.View;

namespace Youtube_DL.ViewModel
{
    internal class MainViewModel : BaseViewModel
    {
        private AddVideoPopup viewPopup;
        #region Property

        public bool Isloading { get; set; }
        public bool ShowHsVideoText => MainVideoList.Count > 0;

        public ObservableCollection<YoutubeVideoModel> MainVideoList { get; set; }

        #endregion Property

        #region Command

        public Command AddButton { get; set; }

        #endregion Command

        public MainViewModel()
        {
            MainVideoList = new ObservableCollection<YoutubeVideoModel>();
            viewPopup = new AddVideoPopup();
            MainVideoList.CollectionChanged += (o,s) => OnPropertyChanged(nameof(ShowHsVideoText));
            AddButton = new Command(AddViewShow);

        }

        private async void AddViewShow()
        {
            var videoInfo = await DialogHost.Show(viewPopup);

            if(videoInfo is null || videoInfo is not YoutubeVideoModel) return;

            MainVideoList.Add(videoInfo as YoutubeVideoModel);
            await YoutubeDownloader.ydlClient.DownloadAsync((videoInfo as YoutubeVideoModel).URL);
        }
    }
}