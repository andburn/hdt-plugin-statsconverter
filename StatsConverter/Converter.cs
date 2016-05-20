using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hearthstone_Deck_Tracker.Stats;
using Hearthstone_Deck_Tracker.Utility.Logging;
using MahApps.Metro.Controls.Dialogs;

using APICore = Hearthstone_Deck_Tracker.API.Core;

namespace AndBurn.HDT.Plugins.StatsConverter
{
	public class Converter
	{
		public static List<GameStats> Filter(StatsFilter filter)
		{
			return filter.Apply(GetStats());
		}

		public static async Task Export(IStatsExporter export, string filepath, List<GameStats> stats)
		{
			var controller = await APICore.MainWindow.ShowProgressAsync("Exporting stats", "Please Wait...");
			try
			{
				if (stats.Count <= 0)
					throw new Exception("No stats found");
				export.To(filepath, stats);
			}
			catch (Exception e)
			{
				Log.Error("Export Failed: " + e.Message, "StatsConverter");
			}
			finally
			{
				await controller.CloseAsync();
			}
		}

		public static void Import(IStatsImporter import, string filename)
		{
			var games = import.From(filename);
			// for current import options, do nothing with result
		}

		private static List<DeckStats> GetStats()
		{
			// use HDT to load the stats
			Facade.LoadDeckStatsList();
			Facade.LoadDefaultDeckStats();
			var ds = new List<DeckStats>(DeckStatsList.Instance.DeckStats);
			ds.AddRange(DefaultDeckStats.Instance.DeckStats);
			return ds;
		}
	}
}