using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.IO;

using Cerberus.Core;
using Cerberus.MVVM.Model;
using QuestPDF;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;



namespace Cerberus.MVVM.ViewModel
{
    public class WatchListViewModel : ObservableObject
    {
        private readonly DatabaseModel _databaseModel;

        public ObservableCollection<object> WatchList { get; }
        public ICommand ExportWatchlistCommand { get; }

        public WatchListViewModel()
        {
            _databaseModel = new DatabaseModel();
            WatchList = new ObservableCollection<object>();
            LoadWatchList();
            ExportWatchlistCommand = new RelayCommand(o => ExportWatchlist());


        }

        private void LoadWatchList()
        {
            var movies = _databaseModel.GetAllMovies();
            var shows = _databaseModel.GetAllShows();
            

            foreach (var movie in movies)
            {
                WatchList.Add(movie);
            }
            

            foreach (var show in shows)
            {
                WatchList.Add(show);
            }
        }


        private void ExportWatchlist()
        {
            try
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.DefaultTextStyle(x => x.FontSize(12));

                        // Header with application name and icon
                        page.Header()
                            .Row(row =>
                            {
                                row.RelativeItem().Text("TrackStar")
                                    .FontSize(20)
                                    .Bold()
                                    .AlignLeft();
                            });

                        // Content
                        page.Content()
                            .Column(column =>
                            {
                                foreach (var item in WatchList)
                                {
                                    if (item is Movies movie)
                                    {
                                        column.Item().Text($"Movie: {movie.Title}").Bold();
                                        column.Item().Text($"Plot: {movie.Plot}");
                                        column.Item().Text($"Genre: {movie.Genre}");
                                        column.Item().Text($"Release Year: {movie.ReleaseYear}");
                                        column.Item().Text($"IMDb Rating: {movie.ImdbRating}");
                                        column.Item().Text("");
                                    }
                                    else if (item is Show show)
                                    {
                                        column.Item().Text($"Show: {show.Title}").Bold();
                                        column.Item().Text($"Plot: {show.Plot}");
                                        column.Item().Text($"Genre: {show.Genre}");
                                        column.Item().Text($"Decade: {show.Decade}");
                                        column.Item().Text($"IMDb Rating: {show.ImdbRating}");
                                        column.Item().Text("");
                                    }
                                }
                            });

                        // Footer
                        page.Footer()
                            .AlignCenter()
                            .Text(x =>
                            {
                                x.Span("Page ");
                                x.CurrentPageNumber();
                                x.Span(" of ");
                                x.TotalPages();
                            });
                    });
                });

                var filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Watchlist.pdf");
                document.GeneratePdf(filename);
                Process.Start(new ProcessStartInfo(filename) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                // Log the exception or show an error message
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




    }
}
