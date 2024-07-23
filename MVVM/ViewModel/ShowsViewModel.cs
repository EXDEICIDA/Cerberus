using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Cerberus.Core;
using Cerberus.MVVM.Model;

namespace Cerberus.MVVM.ViewModel
{
    internal class ShowsViewModel : INotifyPropertyChanged
    {
        private readonly ShowsModel _showsModel;
        private readonly DatabaseModel _databaseModel;

        private Show _selectedShow;
        private string _searchQuery;
        private ObservableCollection<Show> _searchResults;
        private ObservableCollection<Episode> _episodes;
        private Visibility _listBoxVisibility;
        private Visibility _infoVisibility;

        public ShowsViewModel()
        {
            _showsModel = new ShowsModel();
            _databaseModel = new DatabaseModel(); // Initialize DatabaseModel

            SearchCommand = new RelayCommand(async _ => await SearchShowsAsync());
            _searchResults = new ObservableCollection<Show>();
            _episodes = new ObservableCollection<Episode>();
            _listBoxVisibility = Visibility.Collapsed;
            SaveToDatabaseCommand = new RelayCommand(_ => SaveShowToDatabase());
            SaveToLikedTableCommand = new RelayCommand(_ => SaveShowToLiked());//usiing the method for saving series seperately if liked.
            _infoVisibility = Visibility.Collapsed;
            ToggleInfoVisibilityCommand = new RelayCommand(_ => ToggleInfoVisibility());

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

        public ObservableCollection<Show> SearchResults
        {
            get => _searchResults;
            set
            {
                _searchResults = value;
                OnPropertyChanged();
            }
        }

        public Show SelectedShow
        {
            get => _selectedShow;
            set
            {
                _selectedShow = value;
                OnPropertyChanged();
                if (_selectedShow != null)
                {
                    LoadEpisodesAsync(_selectedShow.Id);
                    ListBoxVisibility = Visibility.Collapsed;
                }
            }
        }

        public Visibility InfoVisibility
        {
            get => _infoVisibility;
            set
            {
                _infoVisibility = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Episode> Episodes
        {
            get => _episodes;
            set
            {
                _episodes = value;
                OnPropertyChanged();
            }
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

        public ICommand SearchCommand { get; }

        private async Task SearchShowsAsync()
        {
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                var results = await _showsModel.SearchShowsAsync(SearchQuery);
                SearchResults.Clear();
                foreach (var show in results)
                {
                    SearchResults.Add(show);
                }
                ListBoxVisibility = SearchResults.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public ICommand ToggleInfoVisibilityCommand { get; }
        private void ToggleInfoVisibility()
        {
            InfoVisibility = InfoVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }


        public ICommand SaveToDatabaseCommand { get; }
        private void SaveShowToDatabase()
        {
            if (SelectedShow != null)
            {
                _databaseModel.InsertShow(SelectedShow);
                MessageBox.Show("Show saved to watchlist successfully.");
            }
        }


        public ICommand SaveToLikedTableCommand { get; }
        private void SaveShowToLiked()
        {
            if (SelectedShow != null)
            {
                _databaseModel.InsertToLiked(SelectedShow);
                MessageBox.Show("Serie saved to liked series.");


            }
        }


        private async Task LoadEpisodesAsync(int showId)
        {
            var episodes = await _showsModel.GetEpisodesAsync(showId);
            Episodes.Clear();
            foreach (var episode in episodes)
            {
                Episodes.Add(episode);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
