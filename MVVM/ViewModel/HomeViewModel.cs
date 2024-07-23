using Cerberus.Core;
using Cerberus.MVVM.Model;
using LiveCharts;
using System.Windows.Media;  // For SolidColorBrush and Colors
using LiveCharts.Wpf;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Linq;
using LiveCharts.Wpf;  // Add this directive for ColumnSeries
using Timer = System.Timers.Timer; // Explicitly specify System.Timers.Timer
using ElapsedEventArgs = System.Timers.ElapsedEventArgs; // Alias for clarity

namespace Cerberus.MVVM.ViewModel
{
    internal class HomeViewModel : ObservableObject
    {
        private readonly DataModelService _dataService;
        private List<Movie> _popularMovies;
        private int _currentMovieIndex;
        private Timer _movieTimer;


        private string _currentMoviePlot;
        public string CurrentMoviePlot
        {
            get { return _currentMoviePlot; }
            set { SetProperty(ref _currentMoviePlot, value); }
        }

        private string _currentMoviePoster;
        public string CurrentMoviePoster
        {
            get { return _currentMoviePoster; }
            set { SetProperty(ref _currentMoviePoster, value); }
        }

        private string _currentMovieGenre;
        public string CurrentMovieGenre
        {
            get { return _currentMovieGenre; }
            set { SetProperty(ref _currentMovieGenre, value); }
        }


        private double _currentIMDBRating;
        public double IMDBRating
        {
            get { return _currentIMDBRating; }
            set
            {
                SetProperty(ref _currentIMDBRating, value);
            }
        }

        private string _currentMovieDirector;
        public string CurrentMovieDirector
        {
            get { return  _currentMovieDirector; }
            set { SetProperty(ref _currentMovieDirector, value); }
        }

      

        private ObservableCollection<string> _timeEntities;
        public ObservableCollection<string> TimeEntities
        {
            get { return _timeEntities; }
            set { SetProperty(ref _timeEntities, value); }
        }

        private SeriesCollection _genreSeries;
        public SeriesCollection GenreSeries
        {
            get { return _genreSeries; }
            set { SetProperty(ref _genreSeries, value); }
        }

        // Define the LabelPoint property
        public Func<ChartPoint, string> LabelPoint { get; set; }

        public List<string> GenreLabels { get; set; }




        public HomeViewModel()
        {
            _dataService = new DataModelService();
            _popularMovies = new List<Movie>();
            _currentMovieIndex = 0;
            LoadGenreDistribution();





            _movieTimer = new Timer(10000); // Change movie every 10 seconds
            _movieTimer.Elapsed += MovieTimerElapsed;
            _movieTimer.Start();

            Task.Run(async () =>
            {
                await FetchPopularMoviesAndDetailsAsync();

            });

         
        }

        private async void LoadGenreDistribution()
        {
            var genreCounts = await GetGenreCountsAsync();

            GenreSeries = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Genres",
                    Values = new ChartValues<int>(genreCounts.Values),
                    DataLabels = true,
                    LabelPoint = point => $"{genreCounts.Keys.ElementAt((int)point.X)}: {point.Y}"
                   // LabelPoint = point => $"{genreCounts.Keys.ElementAt((int)point.X)}\n{point.Y}"
                }
            };

            GenreLabels = genreCounts.Keys.ToList();
        }




        private async Task FetchPopularMoviesAndDetailsAsync()
        {
            _popularMovies = await _dataService.GetPopularMoviesThisWeekAsync();

            foreach (var movie in _popularMovies)
            {
                var details = await _dataService.GetMovieDetailsAsync(movie.Title);
                if (details != null)
                {
                    movie.Poster = details.Poster;
                    movie.Plot = details.Plot;
                    movie.Genre = details.Genre; // Ensure you update the genre
                    movie.IMDbRating = details.IMDbRating;
                    movie.Director = details.Director;
                }
            }

            // Update UI with the first movie's plot, poster, and genre
            if (_popularMovies.Count > 0)
            {
                CurrentMoviePlot = _popularMovies[0].Plot;
                CurrentMoviePoster = _popularMovies[0].Poster;
                CurrentMovieGenre = _popularMovies[0].Genre;
                IMDBRating = _popularMovies[0].IMDbRating;
                CurrentMovieDirector = _popularMovies[0].Director;
                


            }
        }

        private async Task<Dictionary<string, int>> GetGenreCountsAsync()
        {
            var genreCounts = new Dictionary<string, int>();

            using (var connection = new SQLiteConnection("Data Source=C:\\Users\\DEICIDA\\source\\repos\\Cerberus\\MVVM\\Model\\AppServices.db"))
            {
                await connection.OpenAsync();
                var command = new SQLiteCommand("SELECT Genre FROM Shows", connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var genres = reader.GetString(0).Split(',');
                        foreach (var genre in genres)
                        {
                            var trimmedGenre = genre.Trim();
                            if (genreCounts.ContainsKey(trimmedGenre))
                            {
                                genreCounts[trimmedGenre]++;
                            }
                            else
                            {
                                genreCounts[trimmedGenre] = 1;
                            }
                        }
                    }
                }
            }

            return genreCounts;
        }





        private void MovieTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _currentMovieIndex++;
            if (_currentMovieIndex >= _popularMovies.Count)
            {
                _currentMovieIndex = 0;
            }

            CurrentMoviePlot = _popularMovies[_currentMovieIndex].Plot;
            CurrentMoviePoster = _popularMovies[_currentMovieIndex].Poster;
            CurrentMovieGenre = _popularMovies[_currentMovieIndex].Genre;
            CurrentMovieDirector = _popularMovies[_currentMovieIndex].Director;
            IMDBRating = _popularMovies[_currentMovieIndex].IMDbRating;
           
        }
    }
}
