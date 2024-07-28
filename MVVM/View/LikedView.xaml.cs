using Cerberus.MVVM.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cerberus.MVVM.View
{
    public partial class LikedView : UserControl
    {
        public LikedView()
        {
            InitializeComponent();
            DataContext = new LikedViewModel();
           

        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var viewModel = DataContext as WatchListViewModel;
                if (viewModel != null && !string.IsNullOrWhiteSpace(viewModel.SearchText))
                {
                    viewModel.Search();
                }
                else
                {
                    // Optionally show a message or handle the case where no text is entered
                    MessageBox.Show("Please enter a value to search.", "Input Required", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
       

      
    }
}
