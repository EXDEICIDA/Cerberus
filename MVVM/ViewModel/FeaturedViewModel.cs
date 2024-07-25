using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Cerberus.Core;
using Cerberus.MVVM.Model;

namespace Cerberus.MVVM.ViewModel
{
    public class FeaturedViewModel : ObservableObject
    {
        private readonly DataModelService _dataModelService;
        public ObservableCollection<Movie> Movies { get; set; }
        public ObservableCollection<Show> Shows { get; set; }

        public ICommand WatchTrailerCommand { get; }

        public FeaturedViewModel()
        {
            _dataModelService = new DataModelService();
            Movies = new ObservableCollection<Movie>();
            Shows = new ObservableCollection<Show>();
            WatchTrailerCommand = new RelayCommand(WatchTrailer);
            LoadMoviesAsync();
            LoadSeriesAsync();
        }

        private async Task LoadMoviesAsync()
        {
            var movies = await _dataModelService.GetPopularMoviesThisWeekAsync();
            foreach (var movie in movies)
            {
                var omdbMovie = await _dataModelService.GetMovieDetailsAsync(movie.Title);
                if (omdbMovie != null)
                {
                    omdbMovie.TrailerUrl = await _dataModelService.GetMovieTrailerAsync(movie.Id);
                    Movies.Add(omdbMovie);
                }
            }
        }

        private async Task LoadSeriesAsync()
        {
            var shows = await _dataModelService.GetPopularSeriesThisWeekAsync();
            foreach (var serie in shows)
            {
                var tmdbShow = await _dataModelService.GetSerieDetailsAsync(serie.Title);
                if (tmdbShow != null)
                {
                    tmdbShow.TrailerUrl = await _dataModelService.GetSerieTrailerAsync(serie.Id);
                    Shows.Add(tmdbShow);
                }
            }
        }



        private void WatchTrailer(object trailerUrlObj)
        {
            var trailerUrl = trailerUrlObj as string;
            if (!string.IsNullOrEmpty(trailerUrl))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = trailerUrl,
                    UseShellExecute = true
                });
            }
        }
    }
}
