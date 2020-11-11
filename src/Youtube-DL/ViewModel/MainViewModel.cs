using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NYoutubeDL;
using Youtube_DL.Core;
using Youtube_DL.Model;
using Youtube_DL.View;

namespace Youtube_DL.ViewModel
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        YoutubeDL ydlClient = new YoutubeDL(@"Resources\youtube-dl.exe");
        public event PropertyChangedEventHandler PropertyChanged;

        #region Property
        public SnackbarMessageQueue Snackbar { get; set; } = new SnackbarMessageQueue();

        public bool Isloading { get; set; }

        public ObservableCollection<YoutubeVideoModel> MainVideoList { get; set; }

        #endregion Property

        #region Command

        public Command AddButton { get; set; }

        #endregion Command

        public MainViewModel()
        {
            MainVideoList = new ObservableCollection<YoutubeVideoModel>();
            //AddButton = new Command((s) => AddToDownload(s as string));
            AddButton = new Command(Test);
        }

        private async void Test()
        {
            var valeu = await DialogHost.Show(new AddVideoPopup());
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
                        if(MainVideoList.Any(x=> x.URL == urls))
                            throw new ArgumentException(message:"Это ссылка уже добавлена");

                        await AddVideo(urls);
                    }
                    catch (Exception e)
                    {
                        if(Isloading)
                            Isloading = false;
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
            MainVideoList.Add(new YoutubeVideoModel(YI, urls));

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