using Cerberus.Core;
using Cerberus.MVVM.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;



namespace Cerberus.MVVM.ViewModel
{
    public class LikedViewModel : INotifyPropertyChanged
  {
    private readonly DatabaseModel _databaseModel;
    private ObservableCollection<LikedItem> _likedItems;
        private ObservableCollection<LikedItem> _allLikedItems;
        private ObservableCollection<LikedItem> _filteredLikedItems;
    private string _searchText;


    //Commands 
    public ICommand SearchCommand { get; }

        public ObservableCollection<LikedItem> LikedItems
        {
            get => _filteredLikedItems;
            set
            {
                _filteredLikedItems = value;
                OnPropertyChanged(nameof(LikedItems));
            }
        }

        public LikedViewModel()
    {
        _databaseModel = new DatabaseModel();
        LoadLikedItems();
        SearchCommand = new RelayCommand(o => Search());

    }


        public void Search()
        {
            if (string.IsNullOrWhiteSpace(_searchText))
            {
                LikedItems = new ObservableCollection<LikedItem>(_allLikedItems);
            }
            else
            {
                var filtered = _allLikedItems.Where(item =>
                    item.Title.Contains(_searchText, StringComparison.OrdinalIgnoreCase));
                LikedItems = new ObservableCollection<LikedItem>(filtered);
            }
        }

        private void LoadLikedItems()
        {
            _allLikedItems = _databaseModel.GetAllLikedItems();
            LikedItems = new ObservableCollection<LikedItem>(_allLikedItems);
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
                    Search();
                }
            }
        }


      


        public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

}