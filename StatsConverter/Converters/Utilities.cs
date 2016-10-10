using System;
using Microsoft.Win32;

namespace HDT.Plugins.StatsConverter.Converters
{
	public static class Utilities
	{
		public static string GetDefaultFileName()
		{
			string name = StatsConverter.Settings.Get("ExportFileName");
			if (StatsConverter.Settings.Get("UseExportFileTimestamp").Bool)
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