using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace Cerberus.MVVM.Model
{
    public class ShowsModel
    {
        private readonly RestClient _tvmazeClient;

        // Constructor
        public ShowsModel()
        {
            _tvmazeClient = new RestClient("https://api.tvmaze.com/");
        }

        // Method to search for TV shows
        public async Task<List<Show>> SearchShowsAsync(string query)
        {
            var request = new RestRequest($"search/shows?q={query}", Method.Get);
            var response = await _tvmazeClient.ExecuteAsync(request);
            if (response.IsSuccessful)
            {
                var searchResults = JsonConvert.DeserializeObject<List<TvMazeShowSearchResult>>(response.Content);
                return searchResults.ConvertAll(result => new Show
                {
                    Id = result.Show.Id,
                    Title = result.Show.Name,
                    Poster = result.Show.Image?.Medium,
                    Plot = StripHtmlTags(result.Show.Summary),
                    Genre = result.Show.Genres != null ? string.Join(", ", result.Show.Genres) : string.Empty,
                    Decade = GetDecade(result.Show.Premiered),
                    ImdbRating = result.Show.Rating?.Average ?? 0
                });
            }
            return new List<Show>();
        }

        // Method to get TV series details by ID (including episodes)
        /*
        public async Task<Show> GetShowDetailsAsync(int showId)
        {
            var request = new RestRequest($"shows/{showId}", Method.Get);
            var response = await _tvmazeClient.ExecuteAsync(request);
            if (response.IsSuccessful)
            {
                var show = JsonConvert.DeserializeObject<TvMazeShow>(response.Content);
                return new Show
                {
                    Id = show.Id,
                    Title = show.Name,
                    Poster = show.Image?.Medium,
                    Plot = StripHtmlTags(show.Summary),
                    Genre = show.Genres != null ? string.Join(", ", show.Genres) : string.Empty,
                    Decade = GetDecade(show.Premiered),
                    ImdbRating = show.Rating?.Average ?? 0
                };
            }
            return null;
        }
        */

        // Method to get episodes of a TV series by ID
        public async Task<List<Episode>> GetEpisodesAsync(int showId)
        {
            var request = new RestRequest($"shows/{showId}/episodes", Method.Get);
            var response = await _tvmazeClient.ExecuteAsync(request);
            if (response.IsSuccessful)
            {
                var episodes = JsonConvert.DeserializeObject<List<TvMazeEpisode>>(response.Content);
                return episodes.ConvertAll(e => new Episode
                {
                    Title = e.Name,
                    Season = e.Season,
                    Number = e.Number,
                    Summary = FormatSummary(StripHtmlTags(e.Summary)),
                    ImdbRating = e.Rating?.Average ?? 0
                });
            }
            return new List<Episode>();
        }

        // Method to strip HTML tags from a string
        private string StripHtmlTags(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            return System.Text.RegularExpressions.Regex.Replace(input, "<.*?>", string.Empty);
        }

        private string FormatSummary(string summary)
        {
            if (string.IsNullOrWhiteSpace(summary))
            {
                return string.Empty;
            }

            // Split the summary into words
            var words = summary.Split(' ');

            var formattedSummary = new StringBuilder();
            for (int i = 0; i < words.Length; i++)
            {
                formattedSummary.Append(words[i]);
                if ((i + 1) % 15 == 0 && i != words.Length - 1)
                {
                    formattedSummary.AppendLine(); // Add a line break
                }
                else
                {
                    formattedSummary.Append(" ");
                }
            }

            return formattedSummary.ToString().Trim();
        }

        private int GetDecade(string premiered)
        {
            if (DateTime.TryParse(premiered, out DateTime date))
            {
                return (date.Year / 10) * 10;
            }
            return 0;
        }

        public class TvMazeShowSearchResult
        {
            public TvMazeShow Show { get; set; }
        }

        public class TvMazeShow
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public TvMazeShowImage Image { get; set; }
            public string Summary { get; set; }
            public List<string> Genres { get; set; }
            public string Premiered { get; set; }
            public TvMazeShowRating Rating { get; set; }
        }

        public class TvMazeShowImage
        {
            public string Medium { get; set; } = string.Empty;
        }

        public class TvMazeShowRating
        {
            public double? Average { get; set; }
        }

        public class TvMazeEpisode
        {
            public string Name { get; set; } = string.Empty;
            public int Season { get; set; }
            public int Number { get; set; }
            public string Summary { get; set; } = string.Empty;
            public TvMazeEpisodeRating Rating { get; set; }
        }

        public class TvMazeEpisodeRating
        {
            public double? Average { get; set; }
        }
    }

    public class Show
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Poster { get; set; } = string.Empty;
        public string PosterPath { get; set; } = string.Empty; // Ensure this matches the binding in XAML

        public string Plot { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public int Decade { get; set; }
        public double ImdbRating { get; set; }
        public string TrailerUrl { get; set; }
    }

    public class Episode
    {
        public string Title { get; set; } = string.Empty;
        public int Season { get; set; }
        public int Number { get; set; }
        public string Summary { get; set; } = string.Empty;
        public double ImdbRating { get; set; }
    }
}
