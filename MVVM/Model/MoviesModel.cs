using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Cerberus.MVVM.Model
{
    public class MoviesModel
    {
        private readonly string _apiKey = "b32d5524";
        private readonly RestClient _client;

        public MoviesModel()
        {
            _client = new RestClient("http://www.omdbapi.com/");
        }

        // Method to search for movies
        public async Task<List<Movies>> SearchMoviesAsync(string query)
        {
            var request = new RestRequest($"?apikey={_apiKey}&s={query}", Method.Get);
            var response = await _client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                var searchResults = JsonConvert.DeserializeObject<MovieSearchResult>(response.Content);
                if (searchResults.Response == "True")
                {
                    var tasks = searchResults.Search.Select(result => GetMovieDetailsAsync(result.ImdbID)).ToArray();
                    var movies = await Task.WhenAll(tasks);
                    return movies.Where(movie => movie != null).ToList();
                }
            }
            return new List<Movies>();
        }

        private async Task<Movies> GetMovieDetailsAsync(string imdbID)
        {
            var request = new RestRequest($"?apikey={_apiKey}&i={imdbID}", Method.Get);
            var response = await _client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                var movie = JsonConvert.DeserializeObject<Movie>(response.Content);
                var releaseDate = DateTime.TryParse(movie.Released, out var parsedDate) ? parsedDate : (DateTime?)null;
                return new Movies
                {
                    Id = movie.ImdbID,
                    Title = movie.Title,
                    Poster = movie.Poster,
                    Plot = movie.Plot,
                    Genre = movie.Genre,
                    Runtime = movie.Runtime,
                    Director = movie.Director,
                    Language = movie.Language,
                    Country = movie.Country,
                    Rated = movie.Rated,
                    ReleaseDate = releaseDate,
                    ReleaseYear = releaseDate?.Year ?? 0,
                    ImdbRating = double.TryParse(movie.ImdbRating, out var rating) ? rating : 0
                };
            }
            return null;
        }
        public class MovieSearchResult
        {
            [JsonProperty("Search")]
            public List<Movie> Search { get; set; }

            [JsonProperty("Response")]
            public string Response { get; set; }
        }

        public class Movie
        {
            [JsonProperty("Title")]
            public string Title { get; set; }

            [JsonProperty("Poster")]
            public string Poster { get; set; }

            [JsonProperty("Plot")]
            public string Plot { get; set; }

            [JsonProperty("Genre")]
            public string Genre { get; set; }

            [JsonProperty("Released")]
            public string Released { get; set; }

            [JsonProperty("imdbID")]
            public string ImdbID { get; set; }

            [JsonProperty("imdbRating")]
            public string ImdbRating { get; set; }

            [JsonProperty("Director")]
            public string Director { get; set; }

            [JsonProperty("Runtime")]
            public string Runtime { get; set; }

            [JsonProperty("Language")]
            public string Language { get; set; }

            [JsonProperty("Country")]
            public string Country { get; set; }

            [JsonProperty("Rated")]
            public string Rated { get; set; }
        }
    }

    public class Movies
    {
        public string Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Poster { get; set; } = string.Empty;
        public string Plot { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public double ImdbRating { get; set; }
        public string Director { get; set; }
        public string Language { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Rated { get; set; } = string.Empty;
        public int ReleaseYear { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string Runtime { get; set; } = string.Empty;
    }

    public class LikedItem : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Poster { get; set; }
        public string Plot { get; set; }
        public string Genre { get; set; }
        public string Decade { get; set; }
        public double ImdbRating { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


}
