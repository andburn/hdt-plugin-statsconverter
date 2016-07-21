using System.IO;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HDT.Plugins.StatsConverter.Properties;
using Microsoft.WindowsAPICodePack.Dialogs;

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
			var dialog = new CommonOpenFileDialog();
			dialog.IsFolderPicker = true;
			if (Directory.Exists(Settings.DefaultExportPath))
				dialog.DefaultDirectory = Settings.DefaultExportPath;

			CommonFileDialogResult result = dialog.ShowDialog();
			if (result == CommonFileDialogResult.Ok)
				Settings.DefaultExportPath = dialog.FileName;
		}
	}
}