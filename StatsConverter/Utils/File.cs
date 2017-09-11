using System;
using System.IO;

namespace HDT.Plugins.StatsConverter.Utils
{
	public static class File
	{
		/// <summary>
		/// Get the default export path from the settings, fallback to desktop if empty
		/// </summary>
		public static string GetDefaultOutputPath()
		{
			var path = StatsConverter.Settings.Get(Strings.DefaultExportPath);
			if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
				return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);			
			return path;
		}
	}
}