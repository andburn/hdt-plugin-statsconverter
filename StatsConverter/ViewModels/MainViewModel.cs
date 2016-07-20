using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Hearthstone_Deck_Tracker.Utility.Logging;

namespace HDT.Plugins.StatsConverter.ViewModels
{
	public class MainViewModel : ViewModelBase
	{
		private Dictionary<string, ViewModelBase> _viewModels = new Dictionary<string, ViewModelBase> {
			{ "settings", new SettingsViewModel() },
			{ "import", new ImportViewModel() },
			{ "export", new ExportViewModel() }
		};

		private string _contentTitle;

		public string ContentTitle
		{
			get { return _contentTitle; }
			set { Set(() => ContentTitle, ref _contentTitle, value); }
		}

		private ViewModelBase _contentViewModel;

		public ViewModelBase ContentViewModel
		{
			get { return _contentViewModel; }
			set { Set(() => ContentViewModel, ref _contentViewModel, value); }
		}

		public RelayCommand<string> NavigateCommand { get; private set; }

		public MainViewModel()
		{
			ContentTitle = "Settings";
			NavigateCommand = new RelayCommand<string>(x => OnNavigation(x));
		}

		private void OnNavigation(string location)
		{
			Log.Debug($"onNav({location})");
			// TODO do nothing if the same as current?
			if (_viewModels.ContainsKey(location.ToLower()))
			{
				Log.Debug("Found key");
				ContentViewModel = _viewModels[location.ToLower()];
			}
		}
	}
}