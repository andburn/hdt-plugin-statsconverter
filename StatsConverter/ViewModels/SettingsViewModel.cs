using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ookii.Dialogs.Wpf;

namespace HDT.Plugins.StatsConverter.ViewModels
{
	public class SettingsViewModel : ViewModelBase
	{
		public string ExportFileName
		{
			get
			{
				return StatsConverter.Settings.Get("ExportFileName");
			}
			set
			{
				StatsConverter.Settings.Set("ExportFileName", value);
				RaisePropertyChanged("ExportFileName");
			}
		}

		public string DefaultExportPath
		{
			get
			{
				return StatsConverter.Settings.Get("DefaultExportPath");
			}
			set
			{
				StatsConverter.Settings.Set("DefaultExportPath", value);
				RaisePropertyChanged("DefaultExportPath");
			}
		}

		public bool UseExportFileTimestamp
		{
			get
			{
				return StatsConverter.Settings.Get("UseExportFileTimestamp").Bool;
			}
			set
			{
				StatsConverter.Settings.Set("UseExportFileTimestamp", value);
				RaisePropertyChanged("UseExportFileTimestamp");
			}
		}

		public RelayCommand SelectDirectoryCommand { get; private set; }

		public SettingsViewModel()
		{
			SelectDirectoryCommand = new RelayCommand(() => ChooseOuputDir());
		}

		private void ChooseOuputDir()
		{
			VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
			dialog.Description = "Select a folder";
			dialog.UseDescriptionForTitle = true;

			if ((bool)dialog.ShowDialog())
				DefaultExportPath = dialog.SelectedPath;
		}
	}
}