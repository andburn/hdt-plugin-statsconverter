using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using HDT.Plugins.Common.Models;
using HDT.Plugins.Common.Settings;
using HDT.Plugins.StatsConverter.Models;
using Microsoft.Win32;

namespace HDT.Plugins.StatsConverter.Converters
{
	public class Converter
	{
		public static List<Game> Filter(GameFilter filter)
		{
			StatsConverter.Logger.Info($"Filter: {filter.DeckId}, {filter.Mode}, {filter.Region}, {filter.TimeFrame}");
			var games = StatsConverter.Data.GetAllGames();
			StatsConverter.Logger.Info($"game count = {games.Count}");
			return filter.Apply(games);
		}

		public static void Export(IStatsConverter conveter, GameFilter filter, string filepath, List<Game> stats)
		{
			// TODO have loading spinner on view
			try
			{
				if (stats.Count <= 0)
					throw new Exception("No stats found");
				var stream = conveter.To(stats);
				using (var file = File.Create(@"E:\Dump\tmp.csv"))
				{
					stream.Seek(0, SeekOrigin.Begin);
					stream.CopyTo(file);
				}
			}
			catch (Exception e)
			{
				StatsConverter.Logger.Error(e);
			}
		}

		public static void Export(IStatsConverter conveter, GameFilter filter, string file)
		{
			var games = StatsConverter.Data.GetAllGames();
			var filtered = filter.Apply(games);
			StatsConverter.Logger.Info($"Filter: {filter.DeckId}, {filter.Mode}, {filter.Region}, {filter.TimeFrame}");
			StatsConverter.Logger.Info($"game count = {games.Count}");

			try
			{
				if (filtered.Count <= 0)
					throw new Exception("No stats found");

				// export and save document
				//await Converter.Export(exporter, filename, stats);
				// export arena extras
				//if (mode == GameMode.Arena && CheckBoxArenaExtras.IsChecked == true)
				//{
				//	await Task.Run(() => Converter.ArenaExtras(filename, stats, deck, decks));
				//}

				var stream = conveter.To(filtered);
				using (var f = File.Create(file))
				{
					stream.Seek(0, SeekOrigin.Begin);
					stream.CopyTo(f);
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

		public static void ArenaExtras(string filename, List<Game> stats, Guid? deck, List<Deck> decks)
		{
			List<ArenaExtra> arenaRuns = null;
			if (deck == null)
				arenaRuns = decks
					.Where(x => x.IsArena && stats.Any(s => s.Deck.Id == x.Id))
					.Select(x => new ArenaExtra(x, stats))
					.OrderByDescending(x => x.LastPlayed).ToList();
			else
				arenaRuns = decks
					.Where(x => x.Id == deck && x.IsArena)
					.Select(x => new ArenaExtra(x, stats))
					.OrderByDescending(x => x.LastPlayed).ToList();

			var fn = filename.Replace(".csv", "-extra.csv");
			using (var writer = new StreamWriter(fn))
			using (var csv = new CsvWriter(writer))
			{
				csv.Configuration.RegisterClassMap<ArenaExtraMap>();
				csv.WriteHeader<ArenaExtra>();
				csv.WriteRecords(arenaRuns);
			}
		}		
	}
}