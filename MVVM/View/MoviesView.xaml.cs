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
    /// Interaction logic for MoviesView.xaml
    /// </summary>
    public partial class MoviesView : UserControl
    {
        public MoviesView()
        {
            InitializeComponent();
            DataContext = new MoviesViewModel();

        }

        private void MoviesSearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var viewModel = DataContext as MoviesViewModel;
                if (viewModel?.SearchCommand.CanExecute(null) == true)
                {
                    viewModel.SearchCommand.Execute(null);
                }
            }
        }

        

       

       
    }
}
