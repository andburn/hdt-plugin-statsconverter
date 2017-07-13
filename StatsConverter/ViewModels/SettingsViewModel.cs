using System.IO;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HDT.Plugins.StatsConverter.Utils;
using Ookii.Dialogs.Wpf;

namespace HDT.Plugins.StatsConverter.ViewModels
{
	public class SettingsViewModel : ViewModelBase
	{
		public string ExportFileName
		{
			get
			{
				return StatsConverter.Settings.Get(Strings.ExportFileName);
			}
			set
			{
				StatsConverter.Settings.Set(Strings.ExportFileName, value);
				RaisePropertyChanged("ExportFileName");
			}
		}

		public string DefaultExportPath
		{
			get
			{
				return StatsConverter.Settings.Get(Strings.DefaultExportPath);
			}
			set
			{
				StatsConverter.Settings.Set(Strings.DefaultExportPath, value);
				RaisePropertyChanged("DefaultExportPath");
			}
		}

		public bool ExportWithoutDialog
		{
			get
			{
				return StatsConverter.Settings.Get(Strings.ExportWithoutDialog).Bool;
			}
			set
			{
				StatsConverter.Settings.Set(Strings.ExportWithoutDialog, value);
				RaisePropertyChanged("ExportWithoutDialog");
			}
		}

		public bool UseExportFileTimestamp
		{
			get
			{
				return StatsConverter.Settings.Get(Strings.UseExportFileTimestamp).Bool;
			}
			set
			{
				StatsConverter.Settings.Set(Strings.UseExportFileTimestamp, value);
				RaisePropertyChanged("UseExportFileTimestamp");
			}
		}

		public bool IsInDevMode
		{
			get
			{
				return StatsConverter.Settings.Get(Strings.DeveloperMode).Bool;
			}
			set
			{
				StatsConverter.Settings.Set(Strings.DeveloperMode, value);
				RaisePropertyChanged("IsInDevMode");
			}
		}

		public bool EnableDebugLog
		{
			get
			{
				return StatsConverter.Settings.Get(Strings.DebugLog).Bool;
			}
			set
			{
				StatsConverter.Settings.Set(Strings.DebugLog, value);
				RaisePropertyChanged("EnableDebugLog");
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

			// set initial directory to setting if exists
			var current = StatsConverter.Settings.Get(Strings.DefaultExportPath).Value;
			if (Directory.Exists(current))
				dialog.SelectedPath = current;

			if ((bool)dialog.ShowDialog())
			{
				DefaultExportPath = dialog.SelectedPath;
				StatsConverter.Settings.Set(Strings.DefaultExportPath, dialog.SelectedPath);
				RaisePropertyChanged(Strings.DefaultExportPath);
			}
		}
	}
}