using System.Windows.Controls;
using Youtube_DL.Core;
using Youtube_DL.ViewModel;

namespace Youtube_DL.View
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
        }
    }
}