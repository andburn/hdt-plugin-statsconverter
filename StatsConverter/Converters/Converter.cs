using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HDT.Plugins.Common.Models;
using HDT.Plugins.Common.Services;
using HDT.Plugins.StatsConverter.Models;

namespace HDT.Plugins.StatsConverter.Converters
{
	public static class Converter
	{
		public static void Export(IDataRepository data, IStatsConverter converter, GameFilter filter, bool arenaExtras, string file)
		{
			var games = data.GetAllGames();
			var filtered = filter.Apply(games);
			
			try
			{
				// TODO don't like this exception
				if (filtered.Count <= 0)
					throw new Exception("No stats found");

				var stream = converter.To(filtered);
				using (var f = File.Create(file))
				{
					stream.Seek(0, SeekOrigin.Begin);
					stream.CopyTo(f);
				}

				if (arenaExtras)
				{
					var runs = data.GetAllDecks()
						.Where(x => x.IsArena && filtered.Any(s => s.Deck.Id == x.Id))
						.Select(x => new ArenaExtra(data, x, filtered))
						.OrderByDescending(x => x.LastPlayed).ToList();
					stream = converter.To(runs);
					using (var f = File.Create($"arena_{file}"))
					{
						stream.Seek(0, SeekOrigin.Begin);
						stream.CopyTo(f);
					}
				}
			}
			catch (Exception e)
			{
				StatsConverter.Logger.Error(e);
			}
		}

		public static void Import(IStatsConverter converter, string file)
		{
			try
			{
				using (var fs = new FileStream(file, FileMode.Open))
				{
					var games = converter.From(fs);
					StatsConverter.Data.UpdateGames(games);
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