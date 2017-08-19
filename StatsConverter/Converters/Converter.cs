using System;
using System.IO;
using HDT.Plugins.Common.Services;
using HDT.Plugins.Common.Utils;
using System.Threading.Tasks;

namespace HDT.Plugins.StatsConverter.Converters
{
	public static class Converter
	{
		public static async Task<bool> Export(IDataRepository data, IStatsConverter converter, GameFilter filter, string file)
		{
			return await Task.Run(() =>
			{
				var games = data.GetAllGames();
				var filtered = filter.Apply(games);

				try
				{
					if (filtered.Count <= 0)
						throw new ConverterException("No stats found");

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
			});
		}

		public static async Task<bool> Import(IStatsConverter converter, string file)
		{
			try
			{
				return await Task.Run(() =>
				{
					using (var fs = new FileStream(file, FileMode.Open))
					{
						var games = converter.ConvertFromStream(fs);
						StatsConverter.Data.AddGames(games);
						return true;
					}
				});
			}
			catch (Exception e)
			{
				StatsConverter.Logger.Error(e);
				return false;
			}
		}
	}
}