using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NYoutubeDL;
using Youtube_DL.Core;
using Youtube_DL.Model;

namespace Youtube_DL.ViewModel
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        YoutubeDL ydlClient = new YoutubeDL(@"Resources\youtube-dl.exe");
        public event PropertyChangedEventHandler PropertyChanged;

        #region Property
        public SnackbarMessageQueue Snackbar { get; set; } = new SnackbarMessageQueue();

        ///AddButton
        public Visibility ButtonTextVisible { get; set; } = Visibility.Visible;
        public Visibility ButtonIsLoading { get; set; } = Visibility.Hidden;

        private bool _isloading = false;
        public bool Isloading
        {
            set
            {
                if (value)
                {
                    ButtonTextVisible = Visibility.Hidden;
                    ButtonIsLoading = Visibility.Visible;
                    _isloading = true;
                }
                else
                {
                    ButtonTextVisible = Visibility.Visible;
                    ButtonIsLoading = Visibility.Hidden;
                    _isloading = false;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Isloading)));
            }
            get => _isloading;
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
                Snackbar.Enqueue("Пожалуйста введите ссылку !!!");
            else
            {
                if (CheckURL(urls))
                {
                    try
                    {
                        await AddVideo(urls);
                    }
                    catch (Exception e)
                    {
                        Snackbar.Enqueue(e.Message);
                    }
                }
                else
                    Snackbar.Enqueue("Ссылку не валидна !!!");
            }
        }

        private async Task AddVideo(string urls)
        {
            Isloading = true;
            var YI = await ydlClient.GetDownloadInfoAsync(urls);
            MainVideoList.Add(new YoutubeVideoModel(YI));
            Isloading = false;
        }

        private bool CheckURL(string videoUri)
        {
            videoUri = new StringBuilder(videoUri).Replace("youtu.be/", "youtube.com/watch?v=").Replace("youtube.com/embed/", "youtube.com/watch?v=").Replace("/v/", "/watch?v=").Replace("/watch#", "/watch?").ToString();

            if (videoUri.Contains("youtube.com/watch?v="))
                return true;
            return false;
        }
    }
}