using System.Collections.ObjectModel;
using Tyrrrz.Settings;
using Youtube_DL.Model;

namespace Youtube_DL.Core
{
    public class Settings : SettingsManager
    {

        public string SavePath { get; set; }
        
        public ObservableCollection<YoutubeVideoModel> Collection { get; set; } = new ObservableCollection<YoutubeVideoModel>();
        
        public Settings()
        {
            Configuration.FileName = "Settings.dat";
            Configuration.StorageSpace = StorageSpace.Instance;
            Configuration.SubDirectoryPath = "";
        }
    }
}