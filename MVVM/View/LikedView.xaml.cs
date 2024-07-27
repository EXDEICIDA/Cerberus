using Cerberus.MVVM.ViewModel;
using System.Windows.Controls;

namespace Cerberus.MVVM.View
{
    public partial class LikedView : UserControl
    {
        public LikedView()
        {
            InitializeComponent();
            DataContext = new LikedViewModel();

        }
    }
}
