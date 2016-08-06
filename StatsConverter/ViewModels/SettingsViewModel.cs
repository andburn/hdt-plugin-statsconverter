using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HDT.Plugins.StatsConverter.Properties;
using Ookii.Dialogs.Wpf;

namespace HDT.Plugins.StatsConverter.ViewModels
{
	public class SettingsViewModel : ViewModelBase
	{
		private Settings _settings;

		public Settings Settings
		{
			get { return _settings; }
			set { Set(() => Settings, ref _settings, value); }
		}

		public RelayCommand SelectDirectoryCommand { get; private set; }

		public SettingsViewModel()
		{
			Settings = Settings.Default;
			SelectDirectoryCommand = new RelayCommand(() => ChooseOuputDir());
		}

		private void ChooseOuputDir()
		{
			VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
			dialog.Description = "Select a folder";
			dialog.UseDescriptionForTitle = true;

			if ((bool)dialog.ShowDialog())
				Settings.DefaultExportPath = dialog.SelectedPath;
		}
	}
}