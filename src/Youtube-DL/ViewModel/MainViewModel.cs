using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VideoLibrary;
using Youtube_DL.Core;
using Youtube_DL.Helper;
using Youtube_DL.Model;

namespace Youtube_DL.ViewModel
{
    internal class MainViewModel : INotifyPropertyChanged
    {

        #region ForTest

        public string Image { get; } = "https://img.youtube.com/vi/ujjSnIChB0g/hqdefault.jpg";
        public string Title { get; } = "iPhone 12 Pro vs 11 Pro: НА САМОМ ДЕЛЕ";


        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        #region Property
        private YouTube YouTubeClient = YouTube.Default;
        public SnackbarMessageQueue Snackbar { get; set; } = new SnackbarMessageQueue();

        ///AddButton
        public Visibility ButtonTextVisible { get; set; } = Visibility.Visible;
        public Visibility ButtonIsLoading { get; set; } = Visibility.Hidden;
        private bool Isloading
        {
            set
            {
                if (value)
                {
                    ButtonTextVisible = Visibility.Hidden;
                    ButtonIsLoading = Visibility.Visible;
                }
                else
                {
                    ButtonTextVisible = Visibility.Visible;
                    ButtonIsLoading = Visibility.Hidden;
                }
            }
        }

        public ObservableCollection<YoutubeVideoModel> MainVideoList { get; set; }

        #endregion Property

        #region Command

        public Command AddButton { get; set; }

        #endregion Command

        public MainViewModel()
        {
            MainVideoList = new ObservableCollection<YoutubeVideoModel>();
            AddButton = new Command((s) => AddToDownload(s as string));
        }

        private async void AddToDownload(string urls)
        {
            if (string.IsNullOrEmpty(urls))
            {
                if (Clipboard.ContainsText())
                    await AddVideo(Clipboard.GetText().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries));
                else
                    Snackbar.Enqueue("Пожалуйста введите ссылку !!!");
            }
            else
            {
                await AddVideo(urls.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries));
            }
        }

        private async Task AddVideo(string[] urls)
        {
            if (!VideoCheck(urls)) Snackbar.Enqueue("Ссылки не валидны !");
            else
            {
                Isloading = true;
                foreach (var url in urls)
                {
                    IEnumerable<YouTubeVideo> VideoInfoList = await YouTubeClient.GetAllVideosAsync(url);
                    MainVideoList.Add(new YoutubeVideoModel(VideoInfoList));
                }
                Isloading = false;
            }
        }

        private bool VideoCheck(string[] urls)
        {
            foreach (var url in urls)
            {
                if (!CheckURL(url))
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckURL(string videoUri)
        {
            videoUri = new StringBuilder(videoUri).Replace("youtu.be/", "youtube.com/watch?v=").Replace("youtube.com/embed/", "youtube.com/watch?v=").Replace("/v/", "/watch?v=").Replace("/watch#", "/watch?").ToString();
            if (!new Query(videoUri).TryGetValue("v"))
                return false;
            return true;
        }
    }
}