using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hearthstone_Deck_Tracker.Stats;
using Hearthstone_Deck_Tracker.Utility.Logging;
using MahApps.Metro.Controls.Dialogs;

namespace AndBurn.HDT.Plugins.StatsConverter
{
	public class Converter
	{
		public static async Task Export(IStatsExporter export, StatsFilter filter, string filepath)
		{
			var controller = await Hearthstone_Deck_Tracker.API.Core.MainWindow.ShowProgressAsync("Exporting stats", "Please Wait...");
			// filter stats
			List<GameStats> filtered = filter.Apply(GetStats());
			try
			{
				if (filtered.Count <= 0)
					throw new Exception("No stats found after applying the filter");
				export.To(filepath, filtered);
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