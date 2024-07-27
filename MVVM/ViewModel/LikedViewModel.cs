using Cerberus.MVVM.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;



namespace Cerberus.MVVM.ViewModel
{
    public class LikedViewModel : INotifyPropertyChanged
  {
    private readonly DatabaseModel _databaseModel;
    private ObservableCollection<LikedItem> _likedItems;

    public ObservableCollection<LikedItem> LikedItems
    {
        get => _likedItems;
        set
        {
            _likedItems = value;
            OnPropertyChanged(nameof(LikedItems));
        }
    }

    public LikedViewModel()
    {
        _databaseModel = new DatabaseModel();
        LoadLikedItems();
    }

    private void LoadLikedItems()
    {
        LikedItems = _databaseModel.GetAllLikedItems();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

}