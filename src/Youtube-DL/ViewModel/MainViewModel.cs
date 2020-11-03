using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using VideoLibrary;
using VideoLibrary.Helpers;
using Youtube_DL.Core;
using Youtube_DL.Helper;

namespace Youtube_DL.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        private YouTube YouTubeClient = YouTube.Default;

        public event PropertyChangedEventHandler PropertyChanged;

        #region Property

        public SnackbarMessageQueue Snackbar { get; set; }

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

        public ObservableCollection<string> MainVideoList { get; set; }

        #endregion

        #region Command

        public Command AddButton { get; set; }


        #endregion

        public MainViewModel()
        {
            Snackbar = new SnackbarMessageQueue();
            MainVideoList = new ObservableCollection<string>();
            AddButton = new Command((s) => AddToDownload(s as string));
        }

        public async void AddToDownload(string urls)
        {

            if (string.IsNullOrEmpty(urls))
            {
                if (Clipboard.ContainsText())
                    await VideoCheck(Clipboard.GetText());
                else
                {
                    Snackbar.Enqueue("Пожалуйста введите ссылку");
                }
            }
            else
                await VideoCheck(urls);
        }

        private async Task VideoCheck(string urls)
        {
            string[] ArrayUrls = urls.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            foreach (var I in ArrayUrls)
            {
                if (!CheckURL(I))
                    Snackbar.Enqueue("Ссылки не правильные");
            }
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
