using System.Collections.ObjectModel;
using Cerberus.Core;
using Cerberus.MVVM.Model;

namespace Cerberus.MVVM.ViewModel
{
    public class WatchListViewModel : ObservableObject
    {
        private readonly DatabaseModel _databaseModel;

        public ObservableCollection<object> WatchList { get; }

        public WatchListViewModel()
        {
            _databaseModel = new DatabaseModel();
            WatchList = new ObservableCollection<object>();
            LoadWatchList();
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
    }
}
