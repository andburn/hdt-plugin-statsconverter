using System;
using HDT.Plugins.StatsConverter.Utils;
using Microsoft.Win32;

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

		public static string SelectFile(string name, string ext, string path, bool save = false)
		{
			FileDialog dlg;
			if (save)
			{
				dlg = new SaveFileDialog();
				dlg.FileName = GetDefaultFileName();
			}
			else
			{
				dlg = new OpenFileDialog();
			}

			dlg.DefaultExt = "." + ext;
			dlg.InitialDirectory = path;
			dlg.Filter = name + " Files | *." + ext;
			bool? result = dlg.ShowDialog();

			// TODO add error message
			if (result != true)
				return string.Empty;

			return dlg.FileName;
		}
	}
}