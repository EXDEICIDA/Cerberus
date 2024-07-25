using System.Windows.Input;

namespace Cerberus.MVVM.ViewModel
{
    internal class SettingsViewModel
    {
        private readonly WatchListViewModel _watchListViewModel;

        public ICommand ExportWatchlistCommand => _watchListViewModel.ExportWatchlistCommand;

        public SettingsViewModel()
        {
            _watchListViewModel = new WatchListViewModel();
        }
    }
}
