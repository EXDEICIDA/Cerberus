using Cerberus.Core;
using Cerberus.MVVM.Model;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Cerberus.MVVM.ViewModel
{
    public class FeaturedViewModel : ObservableObject
    {
        private readonly DataModelService _dataModelService;
        

        private ObservableCollection<Movie> _movies;
        private string _searchQuery;
        private bool _isLoading;
        private string _errorMessage;

        public ICommand LoadMoviesCommand { get; }
        

        public FeaturedViewModel()
        {
            _dataModelService = new DataModelService(); // Your existing service for fetching movie data

            Movies = new ObservableCollection<Movie>();
            LoadMoviesCommand = new RelayCommand(async _ => await LoadMovies());
        }

        public ObservableCollection<Movie> Movies
        {
            get => _movies;
            set => SetProperty(ref _movies, value);
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set => SetProperty(ref _searchQuery, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public async Task LoadMovies()
        {
            try
            {
                IsLoading = true;
                var movies = await _dataModelService.SearchMoviesAsync(SearchQuery);

                Movies.Clear();
                foreach (var movie in movies)
                {
                    Movies.Add(movie);
                }
                ErrorMessage = "";
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to load movies: " + ex.Message;
            }
            finally
            {
                IsLoading = false;
            }
        }

       

    }
}
