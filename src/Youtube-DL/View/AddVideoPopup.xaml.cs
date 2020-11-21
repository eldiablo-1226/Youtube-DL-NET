using System.Windows.Controls;
using Youtube_DL.ViewModel;

namespace Youtube_DL.View
{
    public partial class AddVideoPopup : UserControl
    {
        public AddVideoPopup()
        {
            InitializeComponent();

            DataContext = new AddVideoPopupViewModel();
        }
    }
}