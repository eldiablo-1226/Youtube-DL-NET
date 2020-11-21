using System.Collections.ObjectModel;
using Tyrrrz.Settings;
using Youtube_DL.Helps;
using Youtube_DL.Model;

namespace Youtube_DL.Core
{
    public class Settings : SettingsManager
    {
        private string _savePath = FolderManager.GetPath(FolderEnum.Downloads);
        private ObservableCollection<YoutubeVideoModel> _collection;

        public string SavePath
        {
            get => _savePath;
            set => Set(ref _savePath, value);
        }

        public ObservableCollection<YoutubeVideoModel> Collection
        {
            get
            {
                if (_collection == null)
                {
                    _collection = new ObservableCollection<YoutubeVideoModel>();
                }
                return _collection;
            }
            set => Set(ref _collection, value);
        }

        public Settings()
        {
            Configuration.FileName = "Settings.dat";
            Configuration.StorageSpace = StorageSpace.Instance;
            Configuration.SubDirectoryPath = "Data";
        }
    }
}