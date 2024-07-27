using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.IO;
using Cerberus.Core;
using Cerberus.MVVM.Model;
using QuestPDF;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Windows.Controls;

namespace Cerberus.MVVM.ViewModel
{
    public class WatchListViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseModel _databaseModel;
       
        private string _searchText;
        private ComboBoxItem _selectedGenre;
        private ComboBoxItem _selectedDecade;
        private ComboBoxItem _selectedRating;

        public ObservableCollection<object> WatchList { get; }
        public ObservableCollection<object> FilteredWatchList { get; }
        public ICommand ExportWatchlistCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ComboBoxSelectionChangedCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public WatchListViewModel()
        {
            _databaseModel = new DatabaseModel();
            WatchList = new ObservableCollection<object>();
            FilteredWatchList = new ObservableCollection<object>();
            LoadWatchList();
            ExportWatchlistCommand = new RelayCommand(o => ExportWatchlist());
            SearchCommand = new RelayCommand(o => Search());
            ComboBoxSelectionChangedCommand = new RelayCommand(o => ApplyFilters());
        }

        private void LoadWatchList()
        {
            var movies = _databaseModel.GetAllMovies();
            var shows = _databaseModel.GetAllShows();

            WatchList.Clear();
            foreach (var movie in movies)
            {
                WatchList.Add(movie);
            }

            foreach (var show in shows)
            {
                WatchList.Add(show);
            }

            ApplyFilters();
        }

      

        private void ApplyFilters()
        {
            var filtered = WatchList.AsEnumerable();

            if (_selectedGenre != null && _selectedGenre.Content.ToString() != "All")
            {
                string genre = _selectedGenre.Content.ToString();
                filtered = filtered.Where(item =>
                {
                    if (item is Movies movie)
                        return !string.IsNullOrEmpty(movie.Genre) && movie.Genre.Contains(genre, StringComparison.OrdinalIgnoreCase);
                    if (item is Show show)
                        return !string.IsNullOrEmpty(show.Genre) && show.Genre.Contains(genre, StringComparison.OrdinalIgnoreCase);
                    return false;
                });
            }

            if (_selectedDecade != null && _selectedDecade.Content.ToString() != "All")
            {
                var startYear = GetStartYearOfDecade(_selectedDecade.Content.ToString());
                filtered = filtered.Where(item =>
                {
                    if (item is Movies movie)
                    {
                        return movie.ReleaseDate?.Year >= startYear && movie.ReleaseDate?.Year < startYear + 10;
                    }
                    if (item is Show show)
                        return show.Decade >= startYear && show.Decade < startYear + 10;
                    return false;
                });
            }

            if (_selectedRating != null && _selectedRating.Content.ToString() != "All")
            {
                var ratingRange = _selectedRating.Content.ToString().Split('-')
                    .Select(r => double.TryParse(r, out var rating) ? rating : (double?)null)
                    .ToArray();

                if (ratingRange.Length == 2 && ratingRange[0] != null && ratingRange[1] != null)
                {
                    filtered = filtered.Where(item =>
                    {
                        if (item is Movies movie)
                            return movie.ImdbRating >= ratingRange[0] && movie.ImdbRating <= ratingRange[1];
                        if (item is Show show)
                            return show.ImdbRating >= ratingRange[0] && show.ImdbRating <= ratingRange[1];
                        return false;
                    });
                }
            }

            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                filtered = filtered.Where(item =>
                {
                    if (item is Movies movie)
                        return movie.Title.Contains(_searchText, StringComparison.OrdinalIgnoreCase);
                    if (item is Show show)
                        return show.Title.Contains(_searchText, StringComparison.OrdinalIgnoreCase);
                    return false;
                });
            }

            FilteredWatchList.Clear();
            foreach (var item in filtered)
            {
                FilteredWatchList.Add(item);
            }

            OnPropertyChanged(nameof(FilteredWatchList));
        }

        private int GetStartYearOfDecade(string decade)
        {
            return decade switch
            {
                "2020s" => 2020,
                "2010s" => 2010,
                "2000s" => 2000,
                "1990s" => 1990,
                "1980s" => 1980,
                "1970s" => 1970,
                "1960s" => 1960,
                "1950s" => 1950,
                _ => 0 // Default or invalid decade
            };
        }

        public void Search()
        {
            ApplyFilters();
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


        public ComboBoxItem SelectedGenre
        {
            get => _selectedGenre;
            set
            {
                if (_selectedGenre != value)
                {
                    _selectedGenre = value;
                    OnPropertyChanged(nameof(SelectedGenre));
                    ApplyFilters();
                }
            }
        }


        public ComboBoxItem SelectedDecade
        {
            get => _selectedDecade;
            set
            {
                if (_selectedDecade != value)
                {
                    _selectedDecade = value;
                    OnPropertyChanged(nameof(SelectedDecade));
                    ApplyFilters();
                }
            }
        }


        public ComboBoxItem SelectedRating
        {
            get => _selectedRating;
            set
            {
                if (_selectedRating != value)
                {
                    _selectedRating = value;
                    OnPropertyChanged(nameof(SelectedRating));
                    ApplyFilters();
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    ApplyFilters();
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}