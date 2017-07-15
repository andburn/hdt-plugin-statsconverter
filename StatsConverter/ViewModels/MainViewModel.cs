using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HDT.Plugins.StatsConverter.Utils;

namespace HDT.Plugins.StatsConverter.ViewModels
{
	public class MainViewModel : ViewModelBase
	{
		private ViewModelBase _settingsVM;

		public ViewModelBase SettingsVM
		{
			get
			{
				if (_settingsVM == null)
					_settingsVM = new SettingsViewModel();
				return _settingsVM;
			}
		}

		private ViewModelBase _importVM;

		public ViewModelBase ImportVM
		{
			get
			{
				if (_importVM == null)
					_importVM = new ImportViewModel();
				return _importVM;
			}
		}

		private ViewModelBase _exportVM;

		public ViewModelBase ExportVM
		{
			get
			{
				if (_exportVM == null)
					_exportVM = new ExportViewModel();
				return _exportVM;
			}
		}

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
			ContentViewModel = SettingsVM;
			ContentTitle = "Settings";
			NavigateCommand = new RelayCommand<string>(x => OnNavigation(x));
		}

		private void OnNavigation(string location)
		{
			var loc = location.ToLower();
			if (loc == Strings.NavSettings)
			{
				ContentViewModel = SettingsVM;
			}
			else if (loc == Strings.NavExport)
			{
				ContentViewModel = ExportVM;
			}
			else if (loc == Strings.NavImport)
			{
				ContentViewModel = ImportVM;
			}
			else
			{
				StatsConverter.Logger.Error($"Unknown Main navigation '{location}'");
			}

			if (loc.Length > 2)
				ContentTitle = loc.Substring(0, 1).ToUpper() + loc.Substring(1);
		}
	}
}