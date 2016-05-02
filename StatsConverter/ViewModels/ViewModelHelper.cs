using System;
using HDT.Plugins.StatsConverter.Utils;
using Ookii.Dialogs.Wpf;

namespace HDT.Plugins.StatsConverter.ViewModels
{
	public class ViewModelHelper
	{
		public static string GetDefaultFileName()
		{
			string name = StatsConverter.Settings.Get(Strings.ExportFileName);
			if (StatsConverter.Settings.Get(Strings.UseExportFileTimestamp).Bool)
			{
				name += "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
			}
			return name;
		}

		public static string SaveFileDialog(string name, string ext, string path)
		{
			var dlg = new VistaSaveFileDialog();
			dlg.FileName = GetDefaultFileName();
			dlg.DefaultExt = "." + ext;
			dlg.InitialDirectory = path;
			dlg.Filter = name + " Files | *." + ext;
			bool? result = dlg.ShowDialog();

			if (result != true)
			{
				StatsConverter.Logger.Debug("SaveFileDialog returned false");
				return string.Empty;
			}

			return dlg.FileName;
		}

		public static string OpenFileDialog(string name, string ext, string path)
		{
			var dlg = new VistaOpenFileDialog();
			dlg.DefaultExt = "." + ext;
			dlg.InitialDirectory = path;
			dlg.Filter = name + " Files | *." + ext;
			bool? result = dlg.ShowDialog();

			if (result != true)
			{
				StatsConverter.Logger.Debug("OpenFileDialog dialog returned false");
				return string.Empty;
			}

			return dlg.FileName;
		}
	}
}