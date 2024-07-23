using Cerberus.Core;
using Cerberus.MVVM.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Cerberus.MVVM.ViewModel
{
    public class MoviesViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseModel _databaseModel;
        private readonly MoviesModel _moviesModel;
        public ICommand SearchCommand { get; }
        private Visibility _listBoxVisibility;
        private Visibility _mainGridVisibility;
        private Movies _selectedMovie;
        private string _searchQuery;
        private ObservableCollection<Movies> _searchResults;

        public MoviesViewModel()
        {
            _moviesModel = new MoviesModel();
            _databaseModel = new DatabaseModel();

            _listBoxVisibility = Visibility.Collapsed;
            _mainGridVisibility = Visibility.Collapsed;
            SaveToDatabaseCommand = new RelayCommand(_ => SaveMovieToDatabase());
            SaveToLikedTableCommand = new RelayCommand(_ => SaveToLikedTable());

            SearchCommand = new RelayCommand(async _ => await SearchMoviesAsync());
            SearchResults = new ObservableCollection<Movies>();
        }

        public Visibility ListBoxVisibility
        {
            get => _listBoxVisibility;
            set
            {
                _listBoxVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility MainGridVisibility
        {
            get => _mainGridVisibility;
            set
            {
                _mainGridVisibility = value;
                OnPropertyChanged();
            }
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Movies> SearchResults
        {
            get => _searchResults;
            set
            {
                _searchResults = value;
                OnPropertyChanged();
            }
        }

        public Movies SelectedMovie
        {
            get => _selectedMovie;
            set
            {
                _selectedMovie = value;
                OnPropertyChanged();
                ListBoxVisibility = value == null ? Visibility.Visible : Visibility.Collapsed;
                MainGridVisibility = value == null ? Visibility.Collapsed : Visibility.Visible;


            }
        }


        public ICommand SaveToDatabaseCommand { get; }
        private void SaveMovieToDatabase()
        {
            if(SelectedMovie != null)
            {
                _databaseModel.InsertMovie(SelectedMovie);
                MessageBox.Show("Movie saved to watchlist successfully.");
            }
        }

        public ICommand SaveToLikedTableCommand {  get; }
        private void SaveToLikedTable()
        {
            if (SelectedMovie != null)
            {
                _databaseModel.InsertLiked(SelectedMovie);
                MessageBox.Show("Movie marked as liked.");
            }
        }

        private async Task SearchMoviesAsync()
        {
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                var results = await _moviesModel.SearchMoviesAsync(SearchQuery);
                SearchResults.Clear();
                foreach (var movie in results)
                {
                    SearchResults.Add(movie);
                }
                ListBoxVisibility = SearchResults.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
