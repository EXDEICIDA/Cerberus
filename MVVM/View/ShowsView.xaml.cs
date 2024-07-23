using Cerberus.MVVM.ViewModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cerberus.MVVM.View
{
    public partial class ShowsView : UserControl
    {

        public ShowsView()
        {
            InitializeComponent();

        }

        private void ShowSearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var viewModel = DataContext as ShowsViewModel;
                if (viewModel?.SearchCommand.CanExecute(null) == true)
                {
                    viewModel.SearchCommand.Execute(null);
                }
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
