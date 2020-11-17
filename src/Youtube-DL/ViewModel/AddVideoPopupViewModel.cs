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
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;

namespace Youtube_DL.ViewModel
{
    internal class AddVideoPopupViewModel : BaseViewModel
    {
        public Command AddVideoCommand { get; set; }
        public bool IsLoading { get; set; }

        public AddVideoPopupViewModel()
        {
            AddVideoCommand = new Command(AddVideoToList);
        }

        private async void AddVideoToList(object s)
        {
            if (IsLoading || string.IsNullOrWhiteSpace(s as string)) return;
            string url = s as string;
            
            
        }

        public static bool tryParce(string videoUri)
        {
            if()
        }
        
        
    }

    public static class VideoChecker
    {
        private static VideoType TryParseVideoId(string query)
        {
            VideoId? Id = null;
            try
            {
                Id = new VideoId(query);
            }
            catch (ArgumentException)
            {
            }

            return Id is not null;
        }

        private static bool TryParsePlaylistId(string query)
        {
            PlaylistId? Id = null;
            try
            {
                Id = new PlaylistId(query);
            }
            catch (ArgumentException)
            {
            }
            return Id is not null;
        }
    }

    public enum VideoType
    {
        Video,
        PlayList,
        none
    }
        
}

