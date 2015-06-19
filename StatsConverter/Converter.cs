using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Stats;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hearthstone_Deck_Tracker.Enums;

namespace  AndBurn.HDT.Plugins.StatsConverter
{
    public class Converter
    {
        public static async Task Export(IStatsExporter export, StatsFilter filter, string filepath)
        {
            var controller = await Helper.MainWindow.ShowProgressAsync("Exporting stats", "Please Wait...");
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
                Failure(controller, "Import failed");
                Logger.WriteLine("Export Failed: " + e.Message, "StatsConverter");
            }
            await controller.CloseAsync();
        }

        public static async Task Import(IStatsImporter import, string filepath)
        {
			Logger.WriteLine("Import called", "plugin");
            var controller = await Helper.MainWindow.ShowProgressAsync("Importing stats", "Please Wait...");

			// make sure there is a backup for today
			//Hearthstone_Deck_Tracker.Utility.BackupManager.Run();

			Logger.WriteLine("Backups done", "plugin");
			try
            {
                Dictionary<string, List<GameStats>> stats = import.From(filepath);
				AddAll(stats);
            }
            catch (Exception e)
            {
                Failure(controller, "Import failed");
                Logger.WriteLine("Import Failed: " + e.Message, "StatsConverter");
            }
            await controller.CloseAsync();
        }

		private static void AddAll(Dictionary<string, List<GameStats>> stats)
		{
			foreach (var s in stats.Keys)
			{
				if (s.Equals(string.Empty))
				{
					AddDefaultStats(stats[s]);
				}
				else
				{
					AddDeckStats(s, stats[s]);
				}
			}
		}

		private static void AddDefaultStats(List<GameStats> games)
		{
			DefaultDeckStats.Load();
			var defStats = DefaultDeckStats.Instance;
			foreach (var g in games)
			{
				defStats.GetDeckStats(g.PlayerHero).AddGameResult(g);
			}
			DefaultDeckStats.Save();
		}

		private static void AddDeckStats(string name, List<GameStats> games)
		{
			var deck = AddDeck(name, games.FirstOrDefault<GameStats>());
			var stats = deck.DeckStats;

			foreach (var g in games)
			{
				// TODO: possible overlap of games, compare first
				stats.AddGameResult(g);
			}
			DeckStatsList.Save();
		}

		private static Deck AddDeck(string name, GameStats game)
		{
			DeckList.Load();
			var list = DeckList.Instance.Decks;
			var deck = list.SingleOrDefault<Deck>(d => d.Name.ToLower().Equals(name.ToLower()));
			if (String.IsNullOrEmpty(deck.Name))
			{
				deck = new Deck(); // not really necessary
				deck.Name = name;
				deck.Class = game.PlayerHero;
				deck.IsArenaDeck = game.GameMode == GameMode.Arena;
				list.Add(deck);
			}
			DeckList.Save();
			return deck;
		}		
        
        private static List<DeckStats> GetStats()
        {
            // use HDT to load the stats
            DeckStatsList.Load();
            return DeckStatsList.Instance.DeckStats;
        }

        private static async void Failure(ProgressDialogController c, string m)
        {
            c.SetMessage(m);
			// TODO: temporary, remove
            await Task.Delay(500);
        }

    }
}
