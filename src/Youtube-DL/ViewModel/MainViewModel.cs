using MaterialDesignThemes.Wpf;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VideoLibrary;
using Youtube_DL.Core;
using Youtube_DL.Helper;

namespace Youtube_DL.ViewModel
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        private YouTube YouTubeClient = YouTube.Default;

        public event PropertyChangedEventHandler PropertyChanged;

        #region Property

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

        public ObservableCollection<string> MainVideoList { get; set; }

        #endregion Property

        #region Command

        public Command AddButton { get; set; }

        #endregion Command

        public MainViewModel()
        {
            MainVideoList = new ObservableCollection<string>();
            AddButton = new Command((s) => AddToDownload(s as string));
        }
        
        private async void AddToDownload(string urls)
        {
            if (string.IsNullOrEmpty(urls) || !Clipboard.ContainsText())
                Snackbar.Enqueue("Пожалуйста введите ссылку !!!");
            else if (VideoCheck(urls))
            {
                
            }
        }

        private async Task AddVideo(string[] urls)
        {

        }

        private bool VideoCheck(string urls)
        {
            string[] ArrayUrls = urls.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            foreach (var I in ArrayUrls)
            {
                if (!CheckURL(I))
                {
                    Snackbar.Enqueue("Ссылки не валидны !");
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