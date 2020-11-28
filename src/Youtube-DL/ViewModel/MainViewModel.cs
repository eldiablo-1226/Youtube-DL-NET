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

            // Command
            AddButton = new Command(AddViewShow);
            AddToClipboard = new Command(AddClipboard);
            DeleteIteam = new Command(DeleteIteamVoid);

            MainVideoList.CollectionChanged += (o, s) => OnPropertyChanged(nameof(ShowHsVideoText));
        }

        private async void AddClipboard()
        {
            if (Clipboard.ContainsText())
            {
                string clipBoardText = Clipboard.GetText();

                var videoInfo = await DialogHost.Show(new LoadingView(clipBoardText));

                if (videoInfo == null) return;
                YoutubeVideoModel info = (YoutubeVideoModel)videoInfo;
                info.DeleteVideo = DeleteIteam;
                MainVideoList.Add(info);
            }
        }

        private async void AddViewShow()
        {
            try
            {
                var videoInfo = await DialogHost.Show(new AddVideoPopup());

                if (videoInfo == null) return;

                YoutubeVideoModel info = (YoutubeVideoModel)videoInfo;
                info.DeleteVideo = DeleteIteam;
                MainVideoList.Add(info);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void DeleteIteamVoid(object s)
        {
            if (s is YoutubeVideoModel) MainVideoList.Remove((YoutubeVideoModel)s);
        }

        #region Property

        public bool Isloading { get; set; }
        public bool ShowHsVideoText => MainVideoList.Count > 0;

        public ISnackbarMessageQueue Notifications { get; } = new SnackbarMessageQueue(TimeSpan.FromSeconds(5));

        public ObservableCollection<YoutubeVideoModel> MainVideoList { get; set; }

        #endregion Property

        #region Command

        public Command AddButton { get; }
        public Command AddToClipboard { get; }
        public Command DeleteIteam { get; }

        #endregion Command
    }
}