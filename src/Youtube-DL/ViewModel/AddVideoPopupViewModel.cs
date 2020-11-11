using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;

namespace Youtube_DL.ViewModel
{
    class AddVideoPopupViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsLoading { get; set; }

        public AddVideoPopupViewModel()
        {
        }
    }
}
