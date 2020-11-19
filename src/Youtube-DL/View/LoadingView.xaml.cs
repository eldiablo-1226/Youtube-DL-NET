using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;
using Youtube_DL.Core;
using Youtube_DL.Model;
using Youtube_DL.ViewModel;
using YoutubeExplode.Videos;

namespace Youtube_DL.View
{
    /// <summary>
    /// Логика взаимодействия для LoadingView.xaml
    /// </summary>
    public partial class LoadingView : UserControl
    {
        public YoutubeVideoService _VideoService = new YoutubeVideoService();
        private string _url;

        public LoadingView()
        {
            InitializeComponent();
        }
        
        public LoadingView(string url)
        {
            InitializeComponent();
            _url = url;
            Loaded += LoadingView_Loaded;
        }

        private async void LoadingView_Loaded(object sender, RoutedEventArgs e)
        {
            Video[] videos = null;

            try
            {
                videos = await _VideoService.GetVideosAsync(_url);
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Ссылка не правильно");
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка сети");
            }

            DialogHost.Close("MainView", videos);
        }

    }
}
