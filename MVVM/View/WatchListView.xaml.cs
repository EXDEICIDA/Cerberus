using Cerberus.MVVM.ViewModel;
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

namespace Cerberus.MVVM.View
{
    /// <summary>
    /// Interaction logic for WatchListView.xaml
    /// </summary>
    public partial class WatchListView : UserControl
    {
        public WatchListView()
        {
            InitializeComponent();
            DataContext = new WatchListViewModel();
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
