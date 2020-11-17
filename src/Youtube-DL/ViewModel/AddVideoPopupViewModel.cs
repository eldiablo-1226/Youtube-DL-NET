using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MaterialDesignThemes.Wpf;
using Youtube_DL.Core;
using Youtube_DL.Model;

namespace Youtube_DL.ViewModel
{
    class AddVideoPopupViewModel : BaseViewModel
    {
        public Command AddVideoCommand { get; set; }
        public bool IsLoading { get; set; }

        public AddVideoPopupViewModel()
        {
            AddVideoCommand = new Command(AddVideoToList);
        }

        private async void AddVideoToList(object s)
        {
            if (IsLoading || s == null) return;
            string url = s as string;
            if (string.IsNullOrEmpty(url))
                MessageBox.Show("Пожалуйста введите ссылку !!!");
            else
            {
                if (CheckURL(url))
                {
                    try
                    {
                        //IsLoading = true;
                        //var YI = await ydlClient.GetDownloadInfoAsync(url);
                        //IsLoading = false;
                        //DialogHost.Close("MainDialog", new YoutubeVideoModel(YI, url));

                    }
                    catch (Exception e)
                    {
                        if (IsLoading)
                            IsLoading = false;
                        MessageBox.Show(e.Message);
                    }
                }
                else
                    MessageBox.Show("Ссылку не валидна !!!");
            }
        }

        public static bool CheckURL(string videoUri)
        {
            videoUri = new StringBuilder(videoUri).Replace("youtu.be/", "youtube.com/watch?v=")
                .Replace("youtube.com/embed/", "youtube.com/watch?v=").Replace("/v/", "/watch?v=")
                .Replace("/watch#", "/watch?").ToString();

            if (videoUri.Contains("youtube.com/watch?v="))
                return true;
            return false;
        }
    }
}

