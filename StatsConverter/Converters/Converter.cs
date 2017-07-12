using System;
using System.IO;
using HDT.Plugins.Common.Services;

namespace HDT.Plugins.StatsConverter.Converters
{
	public static class Converter
	{
		public static bool Export(IDataRepository data, IStatsConverter converter, GameFilter filter, string file)
		{
			var games = data.GetAllGames();
			var filtered = filter.Apply(games);

			try
			{
				// TODO don't like this exception
				if (filtered.Count <= 0)
					throw new Exception("No stats found");

				var stream = converter.ConvertToStream(filtered);
				using (var f = File.Create(file))
				{
					stream.Seek(0, SeekOrigin.Begin);
					stream.CopyTo(f);
				}

				return true;
			}
			catch (Exception e)
			{
				StatsConverter.Logger.Error(e);
				return false;
			}
		}

		public static void Import(IStatsConverter converter, string file)
		{
			try
			{
				using (var fs = new FileStream(file, FileMode.Open))
				{
					var games = converter.ConvertFromStream(fs);
					StatsConverter.Data.AddGames(games);
				}
			}
			catch (Exception e)
			{
				// TODO error message
				StatsConverter.Logger.Error(e);
			}
		}
	}
}