using System;
using System.Collections.ObjectModel;
using System.Windows;
using MaterialDesignThemes.Wpf;
using Youtube_DL.Core;
using Youtube_DL.Model;
using Youtube_DL.View;

namespace Youtube_DL.ViewModel
{
    internal class MainViewModel : BaseViewModel
    {
        public MainViewModel()
        {
            MainVideoList = new ObservableCollection<YoutubeVideoModel>();

            /// Command
            _AddButton = new Command(AddViewShow);
            _AddToClipboard = new Command(AddToClipboard);
            DeleteIteam = new Command(DeleteIteamVoid);

            MainVideoList.CollectionChanged += (o, s) => OnPropertyChanged(nameof(ShowHsVideoText));
        }

        private async void AddToClipboard()
        {
            if (Clipboard.ContainsText())
            {
                string ClipBoardText = Clipboard.GetText();

                var videoInfo = await DialogHost.Show(new LoadingView(ClipBoardText));

                if (videoInfo == null) return;
                var Info = videoInfo as YoutubeVideoModel;
                Info.DeleteVideo = DeleteIteam;
                MainVideoList.Add(Info);
            }
        }

        private async void AddViewShow()
        {
            try
            {
                var videoInfo = await DialogHost.Show(new AddVideoPopup());

                if (videoInfo == null) return;

                var Info = videoInfo as YoutubeVideoModel;
                Info.DeleteVideo = DeleteIteam;
                MainVideoList.Add(Info);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void DeleteIteamVoid(object s)
        {
            if (s is YoutubeVideoModel) MainVideoList.Remove(s as YoutubeVideoModel);
        }

        #region Property

        public bool Isloading { get; set; }
        public bool ShowHsVideoText => MainVideoList.Count > 0;

        public ISnackbarMessageQueue Notifications { get; } = new SnackbarMessageQueue(TimeSpan.FromSeconds(5));

        public ObservableCollection<YoutubeVideoModel> MainVideoList { get; set; }

        #endregion Property

        #region Command

        public Command _AddButton { get; }
        public Command _AddToClipboard { get; }
        public Command DeleteIteam { get; }

        #endregion Command
    }
}