using Cerberus.Core;
using Cerberus.MVVM.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Cerberus.MVVM.ViewModel 
{
    internal class MainViewModel : ObservableObject
    {

        /*This class will help us to create relay commands which 
         * pla a role as for coordinating and moving between pages
         * binding this relay commands to the button in our MainWindow
         */

        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand FeaturedViewCommand { get; set; }
        public RelayCommand MoviesViewCommand { get; set; }
        public RelayCommand WatchListViewCommand { get; set; }
        public RelayCommand SettingsViewCommand { get; set; }
        public RelayCommand ShowsViewCommand { get; set; }






        public FeaturedViewModel FeaturedVm {  get; set; }
        public HomeViewModel HomeVm { get; set; }
        public MoviesViewModel MoviesVm { get; set; }
        public WatchListViewModel WatchListVm { get; set; }
        public SettingsViewModel SettingsVm { get; set; }
        public ShowsViewModel ShowsVm { get; set; }


        private object _currentView;


        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

       
        

        public MainViewModel()
        {
            HomeVm = new HomeViewModel();
            CurrentView = HomeVm;
            FeaturedVm = new  FeaturedViewModel();
            MoviesVm = new MoviesViewModel();
            WatchListVm = new WatchListViewModel();
            SettingsVm = new SettingsViewModel();
            ShowsVm = new ShowsViewModel();

          




            //initializations for other commands or properties
            HomeViewCommand = new RelayCommand(o =>
            {
                CurrentView = HomeVm;
            });

            FeaturedViewCommand = new RelayCommand(o =>
            {
                CurrentView = FeaturedVm;
            });

            MoviesViewCommand = new RelayCommand(o =>
            {
                CurrentView = MoviesVm;
            });

           
            WatchListViewCommand = new RelayCommand(o =>
            {
                CurrentView = WatchListVm;
            });


            SettingsViewCommand = new RelayCommand(o =>
            {
                CurrentView = SettingsVm;
            });

            ShowsViewCommand = new RelayCommand(o => 
            {
                CurrentView = ShowsVm;
            
            });



        }

       



    }
}
