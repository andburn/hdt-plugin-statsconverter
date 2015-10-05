using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Stats;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace  AndBurn.HDT.Plugins.StatsConverter
{
    public class Converter
    {
        public static async Task Export(IStatsExporter export, StatsFilter filter, string filepath)
        {
			var controller = await Hearthstone_Deck_Tracker.API.Core.MainWindow.ShowProgressAsync("Exporting stats", "Please Wait...");
            // load stats
            List<DeckStats> stats = GetStats();
            // filter stats
            List<GameStats> filtered = filter.Apply(stats);            
            try
            {
                // export stats
                // TODO: shouldn't really be exporting decks with no stats?
                export.To(filepath, filtered);
            }
            catch (Exception e)
            {
                // TODO: show dialog?
                Logger.WriteLine("Export Failed: " + e.Message, "StatsConverter");
            }
            await controller.CloseAsync();
        }

		public static void Import(IStatsImporter import, string filename)
		{			
			var games = import.From(filename);			
			// for current import options, do nothing with result		
		}

        private static List<DeckStats> GetStats()
        {
            // use HDT to load the stats
            DeckStatsList.Load();
            return DeckStatsList.Instance.DeckStats;
        }
	}
}
