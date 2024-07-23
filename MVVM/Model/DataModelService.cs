using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestSharp;

namespace Cerberus.MVVM.Model
{
    public class DataModelService
    {
        private readonly RestClient _client;//Omdb
        private readonly RestClient _tmdbClient;//TMDB
        private readonly string _apiKey = "b32d5524";
        private readonly string _tmdbApiKey = "a66f68330c434dbbef711799a337a258";

        public DataModelService()
        {
            _client = new RestClient("http://www.omdbapi.com/");
            _tmdbClient = new RestClient("https://api.themoviedb.org/3/");
        }

        public async Task<List<Movie>> GetPopularMoviesThisWeekAsync()
        {
            var request = new RestRequest("trending/movie/week", Method.Get);
            request.AddParameter("api_key", _tmdbApiKey);

            var response = await _tmdbClient.ExecuteAsync<TmdbSearchResult>(request);
            var movies = response.Data?.Results ?? new List<Movie>();

            return movies;
        }

        public async Task<Movie> GetMovieDetailsAsync(string title)
        {
            var request = new RestRequest("", Method.Get);
            request.AddParameter("apikey", _apiKey);
            request.AddParameter("t", title);

            var response = await _client.ExecuteAsync<Movie>(request);
            return response.Data;
        }

        public async Task<List<Movie>> SearchMoviesAsync(string query)
        {
            var request = new RestRequest("", Method.Get);
            request.AddParameter("apikey", _apiKey);
            request.AddParameter("s", query);

            var response = await _client.ExecuteAsync<OmdbSearchResult>(request);
            var movies = response.Data?.Search ?? new List<Movie>();

            // Ensure each movie's Poster is fully qualified
            foreach (var movie in movies)
            {
                if (!string.IsNullOrEmpty(movie.Poster))
                {
                    movie.Poster = movie.Poster; // OMDB already provides full path
                }
            }

            return movies;
        }

        public async Task<List<Movie>> SearchMoviesByGenreAsync(string genre)
        {
            // Step 1: Get the list of movies by genre from TMDb
            var tmdbRequest = new RestRequest("discover/movie", Method.Get);
            tmdbRequest.AddParameter("api_key", _tmdbApiKey);
            tmdbRequest.AddParameter("with_genres", genre);

            var tmdbResponse = await _tmdbClient.ExecuteAsync<TmdbSearchResult>(tmdbRequest);
            var tmdbMovies = tmdbResponse.Data?.Results ?? new List<Movie>();

            // Step 2: Get additional details from OMDb for each movie
            var omdbMovies = new List<Movie>();
            foreach (var tmdbMovie in tmdbMovies)
            {
                var omdbRequest = new RestRequest("", Method.Get);
                omdbRequest.AddParameter("apikey", _apiKey);
                omdbRequest.AddParameter("t", tmdbMovie.Title);

                var omdbResponse = await _client.ExecuteAsync<Movie>(omdbRequest);
                var omdbMovie = omdbResponse.Data;

                if (omdbMovie != null)
                {
                    omdbMovies.Add(omdbMovie);
                }
            }

            return omdbMovies;
        }


    }

    public class TmdbShowSearchResult
    {
        public List<Show> Results { get; set; }
    }

    public class OmdbSearchResult
    {
        public List<Movie> Search { get; set; }
    }

    public class TmdbSearchResult
    {
        public List<Movie> Results { get; set; }
    }

 
    public class Movie
    {
        public string Title { get; set; } = string.Empty;
        public string Poster { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string Plot { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public double IMDbRating { get; set; }
    }

   
}
